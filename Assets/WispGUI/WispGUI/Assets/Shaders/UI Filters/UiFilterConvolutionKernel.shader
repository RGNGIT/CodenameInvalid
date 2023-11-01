Shader "Nukode/UI Filters/Convolution Kernel 3x3"
{
    Properties
    {
        _Blending ("Blending", Range(0, 1)) = 1
        
        _Row_1 ("Row 1", Vector) = (1, 1, 1, 1)
        _Row_2 ("Row 2", Vector) = (1, 1, 1, 1)
        _Row_3 ("Row 3", Vector) = (1, 1, 1, 1)

        [Toggle] _PreLuminance("Colors to luminance ?", Float) = 0
        [Toggle] _Average("Average weights ?", Float) = 1
        [Toggle] _PostLuminance("Result to luminance ?", Float) = 0

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
            float4 _GrabTexture_TexelSize;

            Vector _Row_1;
            Vector _Row_2;
            Vector _Row_3;

            float _Blending;
            float _PreLuminance;
            float _Average;
            float _PostLuminance;

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            float4 SampleGrabTexture(float ParamX, float ParamY, v2f i)
            {
                float4 color = tex2Dproj( _GrabTexture, float4(i.grabPos.x + _GrabTexture_TexelSize.x * ParamX, i.grabPos.y + _GrabTexture_TexelSize.y * ParamY, i.grabPos.z, i.grabPos.w));

                if (_PreLuminance == 1)
                    return LinearRgbToLuminance(color);
                else
                    return color;
            }

            float4 RawSampleGrabTexture(float ParamX, float ParamY, v2f i)
            {
                return tex2Dproj( _GrabTexture, float4(i.grabPos.x + _GrabTexture_TexelSize.x * ParamX, i.grabPos.y + _GrabTexture_TexelSize.y * ParamY, i.grabPos.z, i.grabPos.w));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = RawSampleGrabTexture(0,0,i);
                
                // Center
                fixed4 result = SampleGrabTexture(0, 0, i) * _Row_2.y;
                
                // Row 1
                result += SampleGrabTexture(-1, 1, i) * _Row_1.x;
                result += SampleGrabTexture(0, 1, i) * _Row_1.y;
                result += SampleGrabTexture(1, 1, i) * _Row_1.z;

                // Row 2
                result += SampleGrabTexture(-1, 0, i) * _Row_2.x;
                result += SampleGrabTexture(1, 0, i) * _Row_2.z;

                // Row 3
                result += SampleGrabTexture(-1, -1, i) * _Row_3.x;
                result += SampleGrabTexture(0, -1, i) * _Row_3.y;
                result += SampleGrabTexture(1, -1, i) * _Row_3.z;

                float sum = 1;
                
                if (_Average == 1)
                    sum = _Row_1.x + _Row_1.y + _Row_1.z + _Row_2.x + _Row_2.y + _Row_2.z + _Row_3.x + _Row_3.y + _Row_3.z;

                if (_PostLuminance == 1)
                    result = LinearRgbToLuminance(result);

                return lerp(color, saturate(result / sum), _Blending);
            }
            ENDCG
        }
    }
}