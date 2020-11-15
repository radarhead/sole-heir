// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/DropShader" {
    Properties {
        _Color ("Color", Color) = (1.000000,1.000000,1.000000,1.000000)
        _MainTex ("Texture", 2D) = "white" {}
        _Amount ("Extrusion Amount", Range(0,1.0)) = 0
    }

    SubShader {
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert
        struct Input {
            float2 uv_MainTex;
        };
        float _Amount;
        float4 _Color;
        void vert (inout appdata_full v) {
            v.vertex.xyz;// += mul(unity_WorldToObject, float3(0.0,-4.0,0.0) * _Amount).xyz;
        }
        sampler2D _MainTex;
        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * max(0.1, 1 - _Amount) * _Color;
        }
        ENDCG
    } 
    Fallback "Diffuse"
}