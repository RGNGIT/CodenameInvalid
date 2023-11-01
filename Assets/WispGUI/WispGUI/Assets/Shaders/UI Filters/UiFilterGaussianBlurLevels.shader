Shader "Nukode/UI Filters/Gaussian Blur (Levels)"
{
    Properties
    {
        [KeywordEnum(Low, Medium, High)] _Level("Level", Float) = 1
        
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
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
				float4 vertex   : POSITION;
				float4 color    : COLOR;
			};

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_Level == 2)
                    return SampleGaussianBlur_High(i.grabPos) * i.color;
                if (_Level == 1)
                    return SampleGaussianBlur_Medium(i.grabPos) * i.color;
                else
                    return SampleGaussianBlur_Low(i.grabPos) * i.color;
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
				float4 color    : COLOR;
			};

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_Level == 2)
                    return SampleGaussianBlur_High(i.grabPos) * i.color;
                if (_Level == 1)
                    return SampleGaussianBlur_Medium(i.grabPos) * i.color;
                else
                    return SampleGaussianBlur_Low(i.grabPos) * i.color;
            }
            ENDCG
        }
    }
}