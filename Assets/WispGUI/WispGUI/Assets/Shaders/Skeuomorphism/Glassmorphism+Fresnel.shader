Shader "Nukode/Glassmorphism (Fresnel)"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        [Header(Surface Settings)]
        _SurfaceBlurRadius ("Blur Radius", Range(0, 20)) = 3
        _Blending ("Blur Blending", Range(0, 1)) = 1
        _MinimumAlpha ("Minimum Alpha", Range(0, 1)) = 1
        _Brightness ("Brightness", Range(0, 1)) = 0
        _FresnelBrightness ("Fresnel Brightness", Range(0, 2)) = 0
        _GradientCoords("Gradient Coordinates", Vector) = (0.5, 0.5, 1, 1)
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _GradientFalloff ("Gradient Falloff", Range(0,1)) = 0.8
        
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
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
		Blend One OneMinusSrcAlpha

        Stencil
        {
            Ref [_Stencil]
            Comp LEqual
        }

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

        ENDCG

        // Vertical surface blur
        GrabPass {}
        Pass
        {
            // Stencil
            // {
            //     Ref 1
            //     Comp Always
            //     Pass Replace
            // }
            
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
                    
                    half4 result = SampleGrabTexture(0, 0);
                    for (int range = 1; range <= _SurfaceBlurRadius; range++)
                    {
                        result += SampleGrabTexture(0, range);
                        result += SampleGrabTexture(0, -range);
                    }

                    result /= _SurfaceBlurRadius * 2 + 1;

                    return result * (1+_Brightness);
                }

            ENDCG
        }

        // Horizontal surface blur
        GrabPass {}
        Pass
        {
            // Stencil
            // {
            //     Ref 1
            //     Comp Always
            //     Pass Replace
            // }
            
            CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                
                sampler2D _MainTex;
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _SurfaceBlurRadius;
                float _MinimumAlpha;
                float4 _GradientCoords;
                float4 _FresnelColor;
                float _GradientFalloff;
                float _FresnelBrightness;

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
                    
                    half4 result = SampleGrabTexture(0, 0);
                    for (int range = 1; range <= _SurfaceBlurRadius; range++)
                    {
                        result += SampleGrabTexture(range, 0);
                        result += SampleGrabTexture(-range, 0);
                    }

                    result /= _SurfaceBlurRadius * 2 + 1;

                    float xDistance = pow(i.uv.x - _GradientCoords.x, 2) / _GradientCoords.z;
                    float yDistance = pow(i.uv.y - _GradientCoords.y, 2) / _GradientCoords.w;
                    float distance = clamp(sqrt(xDistance + yDistance), 0, 1);

                    float newDist;
                    float light;

                    if (distance < _GradientFalloff)
                    {
                        newDist = 0;
                        light = 0;
                    }
                    else
                    {
                        newDist = (distance - _GradientFalloff) / (1 - _GradientFalloff);
                        light = _FresnelBrightness * newDist;
                    }

                    float4 fresnel = lerp(float4(0,0,0,0), _FresnelColor, newDist);
                    fresnel = light + fresnel;

                    return (result * i.color) + fresnel;
                }
            ENDCG
        }
	}
}