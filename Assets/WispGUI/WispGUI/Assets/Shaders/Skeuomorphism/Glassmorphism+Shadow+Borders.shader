Shader "Nukode/Glassmorphism (Shadow & Borders)"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        [Header(Surface Settings)]
        _SurfaceBlurRadius ("Blur Radius", Range(0, 20)) = 3
        _Blending ("Blur Blending", Range(0, 1)) = 1
        _MinimumAlpha ("Minimum Alpha", Range(0, 1)) = 1
        _Brightness ("Brightness", Range(0, 1)) = 0
        
        [Header(Outline Settings)]
        _OutlineWidth ("Outline Width", Range(0, 64)) = 2
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)

        [Header(Shahow Settings)]
        [Space(8)]
        _ShadowDistance ("Shadow Distance", Float) = 32
        _ShadowAngle ("Shadow Angle", Range(0, 360)) = 315
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)

        [Header(Shadow Blur Settings)]
        [Space(8)]
        [KeywordEnum(None, Box Blur, Gaussian Blur)] _BlurMode ("Blur Mode", Float) = 0
        _Radius ("Blur Radius", Range(0, 20)) = 3

        [Header(Shadow Box Blur Parameters)]
        [Space(8)]
        _BlurPixelSkip ("Blur Pixel Skip", Range(1, 5)) = 1

        _Stencil ("Stencil ID", Float) = 64
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        CGINCLUDE

            #include "UnityCG.cginc"
            
            #define SampleGrabTexture(posX, posY) tex2Dproj( _GrabTexture, float4(i.grabPos.x + _GrabTexture_TexelSize.x * posX, i.grabPos.y + _GrabTexture_TexelSize.y * posY, i.grabPos.z, i.grabPos.w))

            bool IsBetween(float ParamMin, float ParamMax, float ParamValue)
            {
                if (ParamValue >= ParamMin && ParamValue <= ParamMax)
                {
                    return true;
                }

                return false;
            }

            float invLerp(float from, float to, float value)
            {
                return (value - from) / (to - from);
            }

            float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value){
                float rel = invLerp(origFrom, origTo, value);
                return lerp(targetFrom, targetTo, rel);
            }

        ENDCG

        // Vertical surface blur
        GrabPass {}
        Pass
        {
            Stencil
            {
                Ref [_Stencil]
                Comp Always
                Pass Replace
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            
            CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                
                sampler2D _MainTex;
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _SurfaceBlurRadius;
                float _MinimumAlpha;
                float _Brightness;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float4 grabPos : TEXCOORD0;
                    float2 uv : TEXCOORD1;
                    fixed4 color : COLOR;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.grabPos = ComputeGrabScreenPos(o.vertex);
                    o.uv = v.uv;
                    o.color = v.color;
                    return o;
                }

                half4 frag(v2f i) : SV_TARGET
                {
                    if (tex2D(_MainTex, i.uv).a < _MinimumAlpha)
                        discard;

                    half4 source = SampleGrabTexture(0, 0);
                    
                    half4 result = SampleGrabTexture(0, 0);
                    for (int range = 1; range <= _SurfaceBlurRadius; range++)
                    {
                        result += SampleGrabTexture(0, range);
                        result += SampleGrabTexture(0, -range);
                    }

                    result /= _SurfaceBlurRadius * 2 + 1;

                    return lerp(source, result * (1+_Brightness), i.color.a);
                }

            ENDCG
        }

        // Horizontal surface blur
        GrabPass {}
        Pass
        {
            Stencil
            {
                Ref [_Stencil]
                Comp Equal
                Pass Keep
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }

            CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                
                sampler2D _MainTex;
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _SurfaceBlurRadius;
                float _MinimumAlpha;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float4 grabPos : TEXCOORD0;
                    float2 uv : TEXCOORD1;
                    fixed4 color : COLOR;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.grabPos = ComputeGrabScreenPos(o.vertex);
                    o.uv = v.uv;
                    o.color = v.color;
                    return o;
                }

                half4 frag(v2f i) : SV_TARGET
                {
                    if (tex2D(_MainTex, i.uv).a < _MinimumAlpha)
                        discard;

                    half4 source = SampleGrabTexture(0, 0);
                    
                    half4 result = SampleGrabTexture(0, 0);
                    for (int range = 1; range <= _SurfaceBlurRadius; range++)
                    {
                        result += SampleGrabTexture(range, 0);
                        result += SampleGrabTexture(-range, 0);
                    }

                    result /= _SurfaceBlurRadius * 2 + 1;

                    return lerp(source, result * i.color, i.color.a);
                }

            ENDCG
        }

        // Draw Shadow.
        Pass
		{
            Stencil
            {
                Ref [_Stencil]
                Comp NotEqual
                Pass Keep
                Fail Keep
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                
                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color    : COLOR;
                    float2 texcoord  : TEXCOORD0;
                    float2 gradiator  : TEXCOORD1;
                };

                float _ShadowDistance;
                float _ShadowAngle;
                
                v2f vert(appdata_t IN)
                {
                    v2f OUT;

                    float x = IN.texcoord.x;
                    float y = IN.texcoord.y;

                    if (_ShadowDistance < 0) _ShadowDistance = 0;
                    float _BorderWidth = _ShadowDistance;

                    // Bottom Left Corner.
                    if (IsBetween(0,0.5,x) && IsBetween(0,0.5,y))
                    {
                        IN.vertex.x -= _BorderWidth;
                        IN.vertex.y -= _BorderWidth;
                        OUT.gradiator = float2(-1,-1);
                    }
                    // Bottom Right Corner.
                    else if (IsBetween(0.5,1,x) && IsBetween(0,0.5,y))
                    {
                        IN.vertex.x += _BorderWidth;
                        IN.vertex.y -= _BorderWidth;
                        OUT.gradiator = float2(1,-1);
                    }
                    // Top Right Corner.
                    else if (IsBetween(0.5,1,x) && IsBetween(0.5,1,y))
                    {
                        IN.vertex.x += _BorderWidth;
                        IN.vertex.y += _BorderWidth;
                        OUT.gradiator = float2(1,1);
                    }
                    // Top Left Corner.
                    else if (IsBetween(0,0.5,x) && IsBetween(0.5,1,y))
                    {
                        IN.vertex.x -= _BorderWidth;
                        IN.vertex.y += _BorderWidth;
                        OUT.gradiator = float2(-1,1);
                    }

                    float shadowX = _ShadowDistance * cos(_ShadowAngle * 0.0174533);
                    float shadowY = _ShadowDistance * sin(_ShadowAngle * 0.0174533);
                    float4 shadowOffset = float4(shadowX, shadowY, 0, 0);

                    OUT.vertex = UnityObjectToClipPos(IN.vertex + shadowOffset);
                    OUT.texcoord = IN.texcoord;
                    OUT.color = IN.color;

                    return OUT;
                }

                sampler2D _MainTex;
                float4 _MainTex_TexelSize;
                float _Radius;
                float _BlurPixelSkip;
                fixed4 _ShadowColor;
                float _BlurMode;

                fixed4 SampleTextureWithOffset(float2 ParamUV, float ParamXOffset, float ParamYOffset)
                {
                    float u = ParamUV.x + _MainTex_TexelSize.x * ParamXOffset;
                    float v = ParamUV.y + _MainTex_TexelSize.y * ParamYOffset;
                    float2 uv = float2(u,v);

                    return tex2D(_MainTex, uv);
                }

                fixed4 BoxBlurSampler(float2 ParamUV)
                {
                    fixed4 result = SampleTextureWithOffset(ParamUV, 0, 0);
                    int c = 1;

                    for (int rangeX = -_Radius; rangeX <= _Radius; rangeX+=int(_BlurPixelSkip))
                    {
                        for (int rangeY = -_Radius; rangeY <= _Radius; rangeY+=int(_BlurPixelSkip))
                        {
                            result += SampleTextureWithOffset(ParamUV, rangeX, rangeY);
                            c++;
                        }
                    }
                    result = result / c;

                    return result;
                }

                fixed4 GaussianSampler(float2 ParamUV)
                {
                    float4 color = SampleTextureWithOffset(ParamUV, 0, 0);
                    float4 result = SampleTextureWithOffset(ParamUV, 0, 0) * 36;
                    
                    // Row 1
                    result += SampleTextureWithOffset(ParamUV, -2, 2) * 1;
                    result += SampleTextureWithOffset(ParamUV, -1, 2) * 4;
                    result += SampleTextureWithOffset(ParamUV, 0, 2) * 6;
                    result += SampleTextureWithOffset(ParamUV, 1, 2) * 4;
                    result += SampleTextureWithOffset(ParamUV, 2, 2) * 1;

                    // Row 2
                    result += SampleTextureWithOffset(ParamUV, -2, 1) * 4;
                    result += SampleTextureWithOffset(ParamUV, -1, 1) * 16;
                    result += SampleTextureWithOffset(ParamUV, 0, 1) * 24;
                    result += SampleTextureWithOffset(ParamUV, 1, 1) * 16;
                    result += SampleTextureWithOffset(ParamUV, 2, 1) * 4;

                    // Row 3
                    result += SampleTextureWithOffset(ParamUV, -2, 0) * 6;
                    result += SampleTextureWithOffset(ParamUV, -1, 0) * 24;
                    // result += SampleTexture(0, 0) * 24; // Center already done.
                    result += SampleTextureWithOffset(ParamUV, 1, 0) * 24;
                    result += SampleTextureWithOffset(ParamUV, 2, 0) * 6;

                    // Row 4
                    result += SampleTextureWithOffset(ParamUV, -2, -1) * 4;
                    result += SampleTextureWithOffset(ParamUV, -1, -1) * 16;
                    result += SampleTextureWithOffset(ParamUV, 0, -1) * 24;
                    result += SampleTextureWithOffset(ParamUV, 1, -1) * 16;
                    result += SampleTextureWithOffset(ParamUV, 2, -1) * 4;

                    // Row 5
                    result += SampleTextureWithOffset(ParamUV, -2, -2) * 1;
                    result += SampleTextureWithOffset(ParamUV, -1, -2) * 4;
                    result += SampleTextureWithOffset(ParamUV, 0, -2) * 6;
                    result += SampleTextureWithOffset(ParamUV, 1, -2) * 4;
                    result += SampleTextureWithOffset(ParamUV, 2, -2) * 1;


                    result = result/256;

                    return result;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    if (_ShadowDistance < 0) 
                        _ShadowDistance = 0;
                    
                    // float _BorderWidth = _ShadowDistance;

                    float uvOffsetX = _ShadowDistance * _MainTex_TexelSize.x * -1 * abs(IN.gradiator.x);
                    float uvOffsetY = _ShadowDistance * _MainTex_TexelSize.y * -1 * abs(IN.gradiator.y);

                    float x = IN.texcoord.x;
                    float y = IN.texcoord.y;
                    
                    float2 newCoord;

                    // Bottom Left Corner.
                    if (IsBetween(0,0.5,x) && IsBetween(0,0.5,y))
                    {
                        newCoord = float2(IN.texcoord.x + uvOffsetX, IN.texcoord.y + uvOffsetY);
                    }
                    // Bottom Right Corner.
                    else if (IsBetween(0.5,1,x) && IsBetween(0,0.5,y))
                    {
                        newCoord = float2(IN.texcoord.x - uvOffsetX, IN.texcoord.y + uvOffsetY);
                    }
                    // Top Right Corner.
                    else if (IsBetween(0.5,1,x) && IsBetween(0.5,1,y))
                    {
                        newCoord = float2(IN.texcoord.x - uvOffsetX, IN.texcoord.y - uvOffsetY);
                    }
                    // Top Left Corner.
                    else if (IsBetween(0,0.5,x) && IsBetween(0.5,1,y))
                    {
                        newCoord = float2(IN.texcoord.x + uvOffsetX, IN.texcoord.y - uvOffsetY);
                    }

                    fixed4 c;
                    
                    if (_BlurMode == 1)
                        c = BoxBlurSampler(newCoord);
                    else if (_BlurMode == 2)
                        c = GaussianSampler(newCoord);
                    else
                        c = tex2D (_MainTex, newCoord);

                    c = c * _ShadowColor * float4(1,1,1,IN.color.a);
                    
                    if (c.a > 0)
                    {
                        c.rgb *= c.a;
                        return c;
                    }
                    else
                    {
                        return float4(0,0,0,0);
                    }
                }
            ENDCG
		}

        // Draw outline.
        Pass
		{
            Stencil
            {
                Ref [_Stencil]
                Comp NotEqual
                Pass Zero
                Fail Zero
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                
                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float2 texcoord : TEXCOORD0;
                    fixed4 color    : COLOR;
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color    : COLOR;
                    float2 texcoord  : TEXCOORD0;
                };

                float _OutlineWidth;
                fixed4 _OutlineColor;
                
                v2f vert(appdata_t IN)
                {
                    v2f OUT;

                    float x = IN.texcoord.x;
                    float y = IN.texcoord.y;

                    // Bottom Left Corner.
                    if (IsBetween(0,0.5,x) && IsBetween(0,0.5,y))
                    {
                        IN.vertex.x -= _OutlineWidth;
                        IN.vertex.y -= _OutlineWidth;
                    }
                    // Bottom Right Corner.
                    else if (IsBetween(0.5,1,x) && IsBetween(0,0.5,y))
                    {
                        IN.vertex.x += _OutlineWidth;
                        IN.vertex.y -= _OutlineWidth;
                    }
                    // Top Right Corner.
                    else if (IsBetween(0.5,1,x) && IsBetween(0.5,1,y))
                    {
                        IN.vertex.x += _OutlineWidth;
                        IN.vertex.y += _OutlineWidth;
                    }
                    // Top Left Corner.
                    else if (IsBetween(0,0.5,x) && IsBetween(0.5,1,y))
                    {
                        IN.vertex.x -= _OutlineWidth;
                        IN.vertex.y += _OutlineWidth;
                    }

                    OUT.vertex = UnityObjectToClipPos(IN.vertex);
                    OUT.texcoord = IN.texcoord;
                    OUT.color = _OutlineColor * float4(1,1,1,IN.color.a);

                    return OUT;
                }

                sampler2D _MainTex;

                fixed4 frag(v2f IN) : SV_Target
                {
                    fixed4 c = tex2D (_MainTex, IN.texcoord) * IN.color;
                    c.rgb *= c.a;
                    return c;
                }
            ENDCG
		}
	}
}