Shader "Nukode/UI Filters/Monochrome"
{
    Properties
    {
        _Blending ("Blending", Range(0, 1)) = 1
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

        GrabPass{}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            float _Blending;
            fixed4 _Color;

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
                fixed4 col = tex2Dproj(_GrabTexture, i.grabPos);
                float average = (col.r + col.g + col.b)/3;
                fixed4 result = fixed4(average, average, average, col.a) * _Color;

                return lerp(col, result, _Blending);
            }
            ENDCG
        }
    }
}