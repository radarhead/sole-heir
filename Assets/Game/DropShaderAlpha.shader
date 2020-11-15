// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/DropShaderAlpha" {
    Properties {
        _Color ("Color", Color) = (1.000000,1.000000,1.000000,1.000000)
        _MainTex ("Texture", 2D) = "white" {}
        _Amount ("Extrusion Amount", Range(0,1.0)) = 0
    }

    SubShader {
        Pass {
            ZWrite On
            ColorMask 0
        }

        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType"="Fade" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert alpha:fade
        struct Input {
            float2 uv_MainTex;
        };
        float _Amount;
        void vert (inout appdata_full v) {
            v.vertex.xyz += mul(unity_WorldToObject, float3(0.0,-4.0,0.0) * _Amount).xyz;
        }
        float4 _Color;
        sampler2D _MainTex;
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = _Color.a;
        }
        ENDCG
    } 
    Fallback "Diffuse"
}