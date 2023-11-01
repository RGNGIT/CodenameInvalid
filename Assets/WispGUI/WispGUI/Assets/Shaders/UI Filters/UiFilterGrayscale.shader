Shader "Nukode/UI Filters/Grayscale"
{
    Properties
    {
        _Blending ("Blending", Range(0, 1)) = 1
        [KeywordEnum(RgbAverage, RgbMax, Linear)] _CalcMethod("Calculation method", Float) = 0

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

        GrabPass{}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            float _Blending;
            float _CalcMethod;

            struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
			};

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 color    : COLOR;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 ColorToGrayScale(fixed4 ParamColor)
            {
                if (_CalcMethod == 0) // Average
                {
                    float average = (ParamColor.r + ParamColor.g + ParamColor.b)/3;
                    return fixed4(average, average, average, ParamColor.a);
                }
                else if (_CalcMethod == 1) // RgbMax
                {
                    float maxrgb = 0;

                    if (ParamColor.r > maxrgb)
                        maxrgb = ParamColor.r;

                    if (ParamColor.g > maxrgb)
                        maxrgb = ParamColor.g;

                    if (ParamColor.b > maxrgb)
                        maxrgb = ParamColor.b;

                    return fixed4(maxrgb, maxrgb, maxrgb, ParamColor.a);
                }
                else // Linear
                {
                    return LinearRgbToLuminance(ParamColor);
                }
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2Dproj(_GrabTexture, i.grabPos);

                return lerp(col, ColorToGrayScale(col) * i.color, _Blending * i.color.a);
            }
            ENDCG
        }
    }
}