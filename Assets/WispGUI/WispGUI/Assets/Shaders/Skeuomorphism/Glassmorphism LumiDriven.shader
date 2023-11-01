Shader "Nukode/Glassmorphism (Luminosity Driven)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        // _Blending ("Blending", Range(0, 1)) = 1
        _DarkColor ("Dark Color", Color) = (1,1,1,1)
		_LightColor ("Light Color", Color) = (1,1,1,1)

        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
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

        // Vertical Blur.
        GrabPass{}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD1;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // _BlurSize = 0.5
                // _Samples  = 20
                // _StandardDeviation = 0.07
                
                fixed4 col = 0;
				float sum = 0;
                fixed4 sampled;
                float4 vertexColor = i.color;
                float lumi = LinearRgbToLuminance(tex2D(_MainTex, i.uv));

                float gpx = i.grabPos.x;
                float gpy = i.grabPos.y;
                float gpz = i.grabPos.z;
                float gpw = i.grabPos.w;
                fixed4 original = tex2Dproj( _GrabTexture, float4(gpx, gpy, gpz, gpw));

                for(float i = 0; i < 20; i++)
                {
                    float offset = ((i/19) - 0.5) * 0.5;
                    float gauss = 5.699175434311 * pow(2.71828182846, -( (offset*offset)/0.0098 ) );
                    sum += gauss;
                    sampled = tex2Dproj( _GrabTexture, float4(gpx, gpy + _GrabTexture_TexelSize.y * offset*50, gpz, gpw));
                    col += (sampled * gauss);
                }

                return lerp(original, col/sum, vertexColor.a * lumi);
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

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            // float _Blending;
            float4 _DarkColor;
            float4 _LightColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD1;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				// _BlurSize = 0.5
                // _Samples  = 20
                // _StandardDeviation = 0.07
                
                fixed4 col = 0;
				float sum = 0;
                fixed4 sampled;
                float4 vertexColor = i.color;
                float4 texColor = tex2D(_MainTex, i.uv);
                float lumi = LinearRgbToLuminance(texColor);
                
                float gpx = i.grabPos.x;
                float gpy = i.grabPos.y;
                float gpz = i.grabPos.z;
                float gpw = i.grabPos.w;
                fixed4 original = tex2Dproj( _GrabTexture, float4(gpx, gpy, gpz, gpw));

                for(float i = 0; i < 20; i++)
                {
                    float offset = ((i/19) - 0.5) * 0.5;
                    float gauss = 5.699175434311 * pow(2.71828182846, -( (offset*offset)/0.0098 ) );
                    sum += gauss;
                    sampled = tex2Dproj( _GrabTexture, float4(gpx + _GrabTexture_TexelSize.x * offset*50, gpy, gpz, gpw));
                    col += (sampled * gauss);
                }

                // fixed4 result = (col/sum) * vertexColor;
                float4 darkColor = lerp(float4(0,0,0,0), _DarkColor * vertexColor, texColor.a);
                fixed4 result = lerp(darkColor, (col/sum) * _LightColor * vertexColor, lumi);
                return lerp(original, result, vertexColor.a);
            }
            ENDCG
        }
    }
}