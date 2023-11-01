Shader "Nukode/UI Filters/Box Blur (Alpha Masked)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        
        _Blending ("Blending", Range(0, 1)) = 1
        _Radius ("Blur Radius", Range(0, 12)) = 3
        _Color ("Color", Color) = (1,1,1,1)

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
            int _Radius;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.texcoord = v.texcoord;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 result = SampleGrabTexture(0, 0);
				for (int range = 1; range <= _Radius; range++)
				{
					result += SampleGrabTexture(0, range);
					result += SampleGrabTexture(0, -range);
				}
				result /= _Radius * 2 + 1;

                fixed4 col = SampleGrabTexture(0, 0);

                return lerp(col, result, tex2D(_MainTex, i.texcoord).a * _Blending);
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
            int _Radius;
            fixed4 _Color;

            struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.texcoord = v.texcoord;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 result = SampleGrabTexture(0, 0);
				for (int range = 1; range <= _Radius; range++)
				{
					result += SampleGrabTexture(range, 0);
					result += SampleGrabTexture(-range, 0);
				}
				result /= _Radius * 2 + 1;

                fixed4 col = SampleGrabTexture(0, 0);

                return lerp(col, result * _Color, tex2D(_MainTex, i.texcoord).a * _Blending);
            }
            ENDCG
        }
    }
}