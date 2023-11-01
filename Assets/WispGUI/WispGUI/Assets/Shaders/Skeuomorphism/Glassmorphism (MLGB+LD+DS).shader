// MLGB : Multi level gaussian blur.
// LD : Luminosity Driven (Dark for borders and Light for blur).
// DS : Drop shadow.
Shader "Nukode/Skeuomorphism/Glassmorphism (MLGB+LD+DS)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [KeywordEnum(Low, Medium, High)] _Level("Blur Level", Float) = 1
        _BorderColor ("Border Color", Color) = (1,1,1,1)
        
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

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _Level;
            sampler2D _MainTex;

            fixed4 SampleGaussianBlur_Low(float4 ParamGrabPosition)
            {
                // Center
                fixed4 result = tex2Dproj(_GrabTexture, ParamGrabPosition) * 0.1531703;

                // Positive Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 1, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.1448929;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 2, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.1226492;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 3, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0929025;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 4, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0629702;

                // Negative Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -1, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.1448929;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -2, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.1226492;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -3, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0929025;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -4, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0629702;

                return result;
            }

            fixed4 SampleGaussianBlur_Medium(float4 ParamGrabPosition)
            {
                // Center
                fixed4 result = tex2Dproj(_GrabTexture, ParamGrabPosition) * 0.1031526;

                // Positive Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 1, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.09997895;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 2, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.09103187;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 3, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.07786368;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 4, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.06256523;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 5, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04722671;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 6, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03348875;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 7, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.02230832;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 8, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.01396019;

                // Negative Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -1, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.09997895;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -2, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.09103187;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -3, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.07786368;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -4, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.06256523;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -5, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04722671;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -6, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03348875;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -7, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.02230832;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -8, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.01396019;

                return result;
            }

            fixed4 SampleGaussianBlur_High(float4 ParamGrabPosition)
            {
                // Center
                fixed4 result = tex2Dproj(_GrabTexture, ParamGrabPosition) * 0.06149404;

                // Positive Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 1, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.06101549;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 2, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05960207;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 3, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05731875;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 4, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0542683;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 5, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05058362;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 6, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04641814;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 7, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04193529;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 8, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03729803;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 9, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03265924;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * 10, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.02815403;

                // Negative Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -1, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.06101549;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -2, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05960207;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -3, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05731875;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -4, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0542683;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -5, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05058362;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -6, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04641814;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -7, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04193529;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -8, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03729803;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -9, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03265924;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x, ParamGrabPosition.y + _GrabTexture_TexelSize.y * -10, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.02815403;

                return result;
            }

            struct appdata_t
			{
				float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD1;
                float4 color : COLOR;
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
                fixed4 original = tex2Dproj(_GrabTexture, i.grabPos);
                fixed4 blurred = float4(0,0,0,0);
                
                float lumi = LinearRgbToLuminance(tex2D(_MainTex, i.uv));
                float alpha = tex2D(_MainTex, i.uv).a;
                
                if (_Level == 2)
                    blurred = SampleGaussianBlur_High(i.grabPos);
                else if (_Level == 1)
                    blurred = SampleGaussianBlur_Medium(i.grabPos);
                else
                    blurred = SampleGaussianBlur_Low(i.grabPos);

                return lerp(original, blurred, i.color.a * lumi * alpha);
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
            float _Level;
            sampler2D _MainTex;
            float4 _BorderColor;

            fixed4 SampleGaussianBlur_Low(float4 ParamGrabPosition)
            {
                // Center
                fixed4 result = tex2Dproj(_GrabTexture, ParamGrabPosition) * 0.1531703;

                // Positive Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 1, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.1448929;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 2, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.1226492;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 3, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0929025;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 4, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0629702;

                // Negative Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -1, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.1448929;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -2, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.1226492;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -3, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0929025;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -4, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0629702;

                return result;
            }

            fixed4 SampleGaussianBlur_Medium(float4 ParamGrabPosition)
            {
                // Center
                fixed4 result = tex2Dproj(_GrabTexture, ParamGrabPosition) * 0.1031526;

                // Positive Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 1, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.09997895;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 2, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.09103187;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 3, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.07786368;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 4, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.06256523;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 5, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04722671;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 6, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03348875;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 7, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.02230832;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 8, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.01396019;

                // Negative Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -1, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.09997895;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -2, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.09103187;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -3, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.07786368;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -4, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.06256523;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -5, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04722671;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -6, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03348875;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -7, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.02230832;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -8, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.01396019;

                return result;
            }

            fixed4 SampleGaussianBlur_High(float4 ParamGrabPosition)
            {
                // Center
                fixed4 result = tex2Dproj(_GrabTexture, ParamGrabPosition) * 0.06149404;

                // Positive Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 1, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.06101549;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 2, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05960207;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 3, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05731875;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 4, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0542683;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 5, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05058362;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 6, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04641814;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 7, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04193529;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 8, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03729803;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 9, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03265924;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * 10, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.02815403;

                // Negative Offset
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -1, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.06101549;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -2, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05960207;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -3, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05731875;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -4, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.0542683;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -5, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.05058362;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -6, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04641814;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -7, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.04193529;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -8, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03729803;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -9, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.03265924;
                result += tex2Dproj(_GrabTexture, float4(ParamGrabPosition.x + _GrabTexture_TexelSize.x * -10, ParamGrabPosition.y, ParamGrabPosition.z, ParamGrabPosition.w)) * 0.02815403;

                return result;
            }

            struct appdata_t
			{
				float4 vertex   : POSITION;
                float2 uv : TEXCOORD0;
				float4 color    : COLOR;
			};

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD1;
                float4 color : COLOR;
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
                /*
                fixed4 blurred = fixed4(0,0,0,0);
                float lumi = LinearRgbToLuminance(tex2D(_MainTex, i.uv));
                
                if (_Level == 2)
                    blurred = SampleGaussianBlur_High(i.grabPos) * i.color;
                if (_Level == 1)
                    blurred = SampleGaussianBlur_Medium(i.grabPos) * i.color;
                else
                    blurred = SampleGaussianBlur_Low(i.grabPos) * i.color;

                return lerp(_BorderColor, blurred, lumi);
                */

                fixed4 original = tex2Dproj(_GrabTexture, i.grabPos);
                fixed4 blurred = float4(0,0,0,0);
                
                float lumi = LinearRgbToLuminance(tex2D(_MainTex, i.uv));
                float TexAlpha = tex2D(_MainTex, i.uv).a;
                float VertexAlpha = i.color.a;
                
                if (_Level == 2)
                    blurred = SampleGaussianBlur_High(i.grabPos);
                else if (_Level == 1)
                    blurred = SampleGaussianBlur_Medium(i.grabPos);
                else
                    blurred = SampleGaussianBlur_Low(i.grabPos);

                // return lerp(original, blurred * i.color, lumi * alpha);
                fixed4 resultOne = lerp(_BorderColor * float4(1,1,1,VertexAlpha), blurred * i.color, lumi);
                fixed4 resultTwo = lerp(original, resultOne, TexAlpha);

                return resultTwo;
            }
            ENDCG
        }
    }
}