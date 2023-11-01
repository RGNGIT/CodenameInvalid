Shader "Nukode/UI Filters/SpriteBlur"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _AlphaThreshold ("Alpha THreshold", Range(0,1)) = 0.01
        _Radius ("Blur Radius", Range(0, 12)) = 3

        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        // [HideInInspector] _ColorMask ("Color Mask", Float) = 121
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

		Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

		Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

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
            int _Radius;
            sampler2D _MainTex;
            float _AlphaThreshold;

            struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
                float2 uv : TEXCOORD0;
			};

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.color = v.color;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 result = SampleGrabTexture(0, 0);
				for (int range = 1; range <= _Radius; range++)
				{
					result += SampleGrabTexture(0, range);
					result += SampleGrabTexture(0, -range);
				}
                result /= _Radius * 2 + 1;

                fixed4 col = tex2Dproj(_GrabTexture, i.grabPos);
                fixed4 mainTexCol = tex2D(_MainTex, i.uv);
                fixed4 mainTexColOpaque = fixed4(mainTexCol.r, mainTexCol.g, mainTexCol.b, 1);

                if (mainTexCol.a > _AlphaThreshold)
                {
                    return lerp(col, result*mainTexColOpaque*i.color, mainTexCol.a*i.color.a);
                }
                else
                {
                    return col;
                }
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
            int _Radius;
            sampler2D _MainTex;
            float _AlphaThreshold;

            struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
                float2 uv : TEXCOORD0;
			};

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.color = v.color;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 result = SampleGrabTexture(0, 0);
				for (int range = 1; range <= _Radius; range++)
				{
					result += SampleGrabTexture(range, 0);
					result += SampleGrabTexture(-range, 0);
				}
                result /= _Radius * 2 + 1;

                fixed4 col = tex2Dproj(_GrabTexture, i.grabPos);
                fixed4 mainTexCol = tex2D(_MainTex, i.uv);
                fixed4 mainTexColOpaque = fixed4(mainTexCol.r, mainTexCol.g, mainTexCol.b, 1);

                if (mainTexCol.a > _AlphaThreshold)
                {
                    return lerp(col, result*mainTexColOpaque*i.color, mainTexCol.a*i.color.a);
                }
                else
                {
                    return col;
                }
            }
            ENDCG
        }
	}
}