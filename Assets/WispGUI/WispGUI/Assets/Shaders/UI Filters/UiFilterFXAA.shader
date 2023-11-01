// Thanks to Jasper Flick / Catlike Coding for the tutorial on implementing FXAA.
// Link to the tutorial : https://catlikecoding.com/jasper-flick/

// TODO : Add branches to SampleLuminance function.

Shader "Nukode/UI Filters/Fast Approximate Anti-aliasing"
{
    Properties
    {
        _Blending ("Helper Blending", Range(0, 1)) = 1 // Used to locate where is the filter on the screen.
        _ContrastThreshold ("Contrast Threshold", Range(0, 1)) = 0.03
        _RelativeThreshold ("Relative Threshold", Range(0, 1)) = 0.06
        _SubpixelBlending ("Sub-pixel Blending", Range(0, 1)) = 1

        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" }

        Stencil
        {
            Ref [_Stencil]
            Comp LEqual
        }

        CGINCLUDE
                #include "UnityCG.cginc"

                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _Blending;

                float _ContrastThreshold;
                float _RelativeThreshold;
                float _SubpixelBlending;

                struct VertexData {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct Interpolators {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                Interpolators VertexProgram (VertexData v) {
                    Interpolators i;
                    i.pos = UnityObjectToClipPos(v.vertex);
                    i.uv = ComputeGrabScreenPos(i.pos);
                    return i;
                }

                float4 Sample (float2 uv) {
                    return tex2Dlod(_GrabTexture, float4(uv, 0, 0));
                }

                float SampleLuminance (float2 uv) {
                    // #if defined(LUMINANCE_GREEN)
                    //     return Sample(uv).g;
                    // #else
                    //     return Sample(uv).a;
                    // #endif
                    float4 color = Sample(uv);
                    float average = (color.r + color.g + color.b) / 3;
                    return float4(average, average, average, 1);
                }

                float SampleLuminance (float2 uv, float uOffset, float vOffset) {
                    uv += _GrabTexture_TexelSize * float2(uOffset, vOffset);
                    // uv += 1 * float2(uOffset, vOffset);
                    return SampleLuminance(uv);
                }

                struct LuminanceData {
                    float m, n, e, s, w;
                    float ne, nw, se, sw;
                    float highest, lowest, contrast;
                };

                LuminanceData SampleLuminanceNeighborhood (float2 uv) {
                    LuminanceData l;
                    l.m = SampleLuminance(uv);
                    l.n = SampleLuminance(uv,  0,  1);
                    l.e = SampleLuminance(uv,  1,  0);
                    l.s = SampleLuminance(uv,  0, -1);
                    l.w = SampleLuminance(uv, -1,  0);

                    l.ne = SampleLuminance(uv,  1,  1);
                    l.nw = SampleLuminance(uv, -1,  1);
                    l.se = SampleLuminance(uv,  1, -1);
                    l.sw = SampleLuminance(uv, -1, -1);

                    l.highest = max(max(max(max(l.n, l.e), l.s), l.w), l.m);
                    l.lowest = min(min(min(min(l.n, l.e), l.s), l.w), l.m);
                    l.contrast = l.highest - l.lowest;
                    return l;
                }

                bool ShouldSkipPixel (LuminanceData l) {
                    float threshold =
                        max(_ContrastThreshold, _RelativeThreshold * l.highest);
                    return l.contrast < threshold;
                }

                float DeterminePixelBlendFactor (LuminanceData l) {
                    float filter = 2 * (l.n + l.e + l.s + l.w);
                    filter += l.ne + l.nw + l.se + l.sw;
                    filter *= 1.0 / 12;
                    filter = abs(filter - l.m);
                    filter = saturate(filter / l.contrast);

                    float blendFactor = smoothstep(0, 1, filter);
                    return blendFactor * blendFactor * _SubpixelBlending;
                }

                struct EdgeData {
                    bool isHorizontal;
                    float pixelStep;
                    float oppositeLuminance, gradient;
                };

                EdgeData DetermineEdge (LuminanceData l) {
                    EdgeData e;
                    float horizontal =
                        abs(l.n + l.s - 2 * l.m) * 2 +
                        abs(l.ne + l.se - 2 * l.e) +
                        abs(l.nw + l.sw - 2 * l.w);
                    float vertical =
                        abs(l.e + l.w - 2 * l.m) * 2 +
                        abs(l.ne + l.nw - 2 * l.n) +
                        abs(l.se + l.sw - 2 * l.s);
                    e.isHorizontal = horizontal >= vertical;

                    float pLuminance = e.isHorizontal ? l.n : l.e;
                    float nLuminance = e.isHorizontal ? l.s : l.w;
                    float pGradient = abs(pLuminance - l.m);
                    float nGradient = abs(nLuminance - l.m);

                    e.pixelStep =
                        e.isHorizontal ? _GrabTexture_TexelSize.y : _GrabTexture_TexelSize.x;
                        // e.isHorizontal ? 1 : 1;
                    
                    if (pGradient < nGradient) {
                        e.pixelStep = -e.pixelStep;
                        e.oppositeLuminance = nLuminance;
                        e.gradient = nGradient;
                    }
                    else {
                        e.oppositeLuminance = pLuminance;
                        e.gradient = pGradient;
                    }

                    return e;
                }

                #if defined(LOW_QUALITY)
                    #define EDGE_STEP_COUNT 4
                    #define EDGE_STEPS 1, 1.5, 2, 4
                    #define EDGE_GUESS 12
                #else
                    #define EDGE_STEP_COUNT 10
                    #define EDGE_STEPS 1, 1.5, 2, 2, 2, 2, 2, 2, 2, 4
                    #define EDGE_GUESS 8
                #endif

                static const float edgeSteps[EDGE_STEP_COUNT] = { EDGE_STEPS };

                float DetermineEdgeBlendFactor (LuminanceData l, EdgeData e, float2 uv) {
                    float2 uvEdge = uv;
                    float2 edgeStep;
                    if (e.isHorizontal) {
                        uvEdge.y += e.pixelStep * 0.5;
                        edgeStep = float2(_GrabTexture_TexelSize.x, 0);
                        // edgeStep = float2(1, 0);
                    }
                    else {
                        uvEdge.x += e.pixelStep * 0.5;
                        edgeStep = float2(0, _GrabTexture_TexelSize.y);
                        // edgeStep = float2(0, 1);
                    }

                    float edgeLuminance = (l.m + e.oppositeLuminance) * 0.5;
                    float gradientThreshold = e.gradient * 0.25;

                    float2 puv = uvEdge + edgeStep * edgeSteps[0];
                    float pLuminanceDelta = SampleLuminance(puv) - edgeLuminance;
                    bool pAtEnd = abs(pLuminanceDelta) >= gradientThreshold;

                    UNITY_UNROLL
                    for (int i = 1; i < EDGE_STEP_COUNT && !pAtEnd; i++) {
                        puv += edgeStep * edgeSteps[i];
                        pLuminanceDelta = SampleLuminance(puv) - edgeLuminance;
                        pAtEnd = abs(pLuminanceDelta) >= gradientThreshold;
                    }
                    if (!pAtEnd) {
                        puv += edgeStep * EDGE_GUESS;
                    }

                    float2 nuv = uvEdge - edgeStep * edgeSteps[0];
                    float nLuminanceDelta = SampleLuminance(nuv) - edgeLuminance;
                    bool nAtEnd = abs(nLuminanceDelta) >= gradientThreshold;

                    UNITY_UNROLL
                    for (int j = 1; j < EDGE_STEP_COUNT && !nAtEnd; j++) {
                        nuv -= edgeStep * edgeSteps[j];
                        nLuminanceDelta = SampleLuminance(nuv) - edgeLuminance;
                        nAtEnd = abs(nLuminanceDelta) >= gradientThreshold;
                    }
                    if (!nAtEnd) {
                        nuv -= edgeStep * EDGE_GUESS;
                    }

                    float pDistance, nDistance;
                    if (e.isHorizontal) {
                        pDistance = puv.x - uv.x;
                        nDistance = uv.x - nuv.x;
                    }
                    else {
                        pDistance = puv.y - uv.y;
                        nDistance = uv.y - nuv.y;
                    }

                    float shortestDistance;
                    bool deltaSign;
                    if (pDistance <= nDistance) {
                        shortestDistance = pDistance;
                        deltaSign = pLuminanceDelta >= 0;
                    }
                    else {
                        shortestDistance = nDistance;
                        deltaSign = nLuminanceDelta >= 0;
                    }

                    if (deltaSign == (l.m - edgeLuminance >= 0)) {
                        return 0;
                    }
                    return 0.5 - shortestDistance / (pDistance + nDistance);
                }

                float4 ApplyFXAA (float2 uv) {
                    LuminanceData l = SampleLuminanceNeighborhood(uv);
                    if (ShouldSkipPixel(l)) {
                        return Sample(uv);
                    }

                    float pixelBlend = DeterminePixelBlendFactor(l);
                    EdgeData e = DetermineEdge(l);
                    float edgeBlend = DetermineEdgeBlendFactor(l, e, uv);
                    float finalBlend = max(pixelBlend, edgeBlend);

                    if (e.isHorizontal) {
                        uv.y += e.pixelStep * finalBlend;
                    }
                    else {
                        uv.x += e.pixelStep * finalBlend;
                    }
                    return float4(Sample(uv).rgb, l.m);
                }
        ENDCG

        GrabPass{}
        Pass { // 0 luminancePass
			CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram

				// #pragma multi_compile _ GAMMA_BLENDING

				float4 FragmentProgram (Interpolators i) : SV_Target {
					float4 result = tex2D(_GrabTexture, i.uv);
					result.rgb = saturate(result.rgb);
					result.a = LinearRgbToLuminance(result.rgb);
					// #if defined(GAMMA_BLENDING)
					// 	result.rgb = LinearToGammaSpace(result.rgb);
					// #endif
					return result;
				}
			ENDCG
		}

        GrabPass{}
		Pass { // 1 fxaaPass
			CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram

				// #pragma multi_compile _ LUMINANCE_GREEN
				// #pragma multi_compile _ LOW_QUALITY
				// #pragma multi_compile _ GAMMA_BLENDING

				float4 FragmentProgram (Interpolators i) : SV_Target {
					float4 result = ApplyFXAA(i.uv);
					// #if defined(GAMMA_BLENDING)
					// 	result.rgb = GammaToLinearSpace(result.rgb);
					// #endif
					// return result;

                    return lerp(float4(1,0,0,1), result, _Blending);
				}
			ENDCG
		}
    }
}