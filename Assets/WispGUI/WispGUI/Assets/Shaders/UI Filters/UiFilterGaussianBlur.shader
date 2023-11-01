// Based on : https://www.ronja-tutorials.com/post/023-postprocessing-blur/
Shader "Nukode/UI Filters/Gaussian Blur (Parametrable)"
{
    Properties
    {
        _Blending ("Blending", Range(0, 1)) = 1
        _BlurSize("Blur Size", Range(0,0.5)) = 0.5
        _Samples ("Samples", Range(2, 100)) = 20
		[PowerSlider(3)] _StandardDeviation("Standard Deviation", Range(0.00, 0.3)) = 0.02

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
            #define SampleGrabTexture(posX, posY) tex2Dproj( _GrabTexture, float4(i.grabPos.x + _GrabTexture_TexelSize.x * posX, i.grabPos.y + _GrabTexture_TexelSize.y * posY, i.grabPos.z, i.grabPos.w))
            #define PI 3.14159265359
            #define E 2.71828182846
        ENDCG

        // Vertical Blur.
        GrabPass{}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _Blending;
            float _Samples;
            float _BlurSize;
            float _StandardDeviation;

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

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = 0;
				float sum = 0;
                fixed4 sampled;
                fixed4 original = SampleGrabTexture(0, 0);

				for(float index = 0; index < _Samples; index++)
				{
					float offset = (index/(_Samples-1) - 0.5) * _BlurSize;
					
					float stDevSquared = _StandardDeviation*_StandardDeviation;
					float gauss = (1 / sqrt(2*PI*stDevSquared)) * pow(E, -((offset*offset)/(2*stDevSquared)));
					
					sum += gauss;
					
                    sampled = SampleGrabTexture(0, offset*100);
					col += (sampled * gauss);
				}
				
                return lerp(original, col/sum, _Blending);
            }
            ENDCG
        }

        // Horizontal Blur.
        GrabPass{}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _Blending;
            float _Samples;
            float _BlurSize;
            float _StandardDeviation;

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

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = 0;
				float sum = 0;
                fixed4 sampled;
                fixed4 original = SampleGrabTexture(0, 0);

				for(float index = 0; index < _Samples; index++)
				{
					float offset = (index/(_Samples-1) - 0.5) * _BlurSize;
					
					float stDevSquared = _StandardDeviation*_StandardDeviation;
					float gauss = (1 / sqrt(2*PI*stDevSquared)) * pow(E, -((offset*offset)/(2*stDevSquared)));
					
					sum += gauss;
					
                    sampled = SampleGrabTexture(offset*100, 0);
					col += (sampled * gauss);
				}
				
				return lerp(original, col/sum, _Blending);
            }
            ENDCG
        }
    }
}