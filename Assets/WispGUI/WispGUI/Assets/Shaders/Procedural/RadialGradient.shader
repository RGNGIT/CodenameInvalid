Shader "Nukode/Procedural/Radial Gradient"
{
	Properties
	{
		[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		
		_StartColor ("Start Color", Color) = (1,0,0,1)
		_EndColor ("End Color", Color) = (1,0,1,1)
		_GradientCoords("Gradient Coordinates", Vector) = (0.5, 0.5, 1, 1)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 uv : TEXCOORD0;
				float4 col : COLOR;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 col : COLOR;
			};
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;
				OUT.col = IN.col;
				return OUT;
			}

			float4 _GradientCoords;
			float4 _StartColor;
			float4 _EndColor;

			fixed4 frag(v2f IN) : SV_Target
			{
				float xDistance = pow(IN.uv.x - _GradientCoords.x, 2) / _GradientCoords.z;
				float yDistance = pow(IN.uv.y - _GradientCoords.y, 2) / _GradientCoords.w;
				float distance = clamp(sqrt(xDistance + yDistance), 0, 1);
				return lerp(_StartColor, _EndColor, distance) * IN.col;
			}
		ENDCG
		}
	}
}