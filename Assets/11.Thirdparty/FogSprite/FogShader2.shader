Shader "Custom/FogShader2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _AlternativeTex ("Alternative Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // 스텐실 값이 1인 경우 렌더링
        Pass
        {
            Name "RenderWithStencilValue1"
            Stencil
            {
                Ref 0
                Comp equal
                Pass replace
            }
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag1
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _Color;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float4 frag1 (v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv) * _Color;
                return texColor;
            }
            ENDCG
        }

        // 스텐실 값이 0인 경우 렌더링
        Pass
        {
            Name "RenderWithStencilValue0"
            Stencil
            {
                Ref 1
                Comp notEqual
                Pass keep
            }
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag2
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _AlternativeTex;
            float4 _Color;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float4 frag2 (v2f i) : SV_Target
            {
                float4 texColor = tex2D(_AlternativeTex, i.uv) * _Color;
                texColor.a = 0.0; // 투명하게 설정
                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
