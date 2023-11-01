Shader "Nukode/Procedural/Directional Gradient"
{
	Properties
	{
		[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		
		_StartColor ("Start Color", Color) = (1,0,0,1)
		_EndColor ("End Color", Color) = (1,0,1,1)
		_Direction("Direction", Range(0,90)) = 0
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
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					float2 uv : TEXCOORD0;
				};
				
				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.uv = IN.uv;
					return OUT;
				}

				float4 _StartColor;
				float4 _EndColor;
				float _Direction;

				fixed4 frag(v2f IN) : SV_Target
				{
					float XAmount = IN.uv.x * cos(_Direction * 0.0174533);
					float YAmount = IN.uv.y * sin(_Direction * 0.0174533);

					return lerp(_StartColor, _EndColor, XAmount + YAmount);
				}
			ENDCG
		}
	}
}