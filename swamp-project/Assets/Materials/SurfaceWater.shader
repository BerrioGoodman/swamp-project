Shader "Custom/SurfaceWater"
{
    Properties {
        _Color ("Color", Color) = (0, 0.5, 1, 0.3)
        _MainTex ("Normal Map", 2D) = "bump" {}
        _WaveSpeed ("Wave Speed", Range(0, 2)) = 1
        _WaveHeight ("Wave Height", Range(0, 1)) = 0.1
        _Glossiness ("Smoothness", Range(0, 1)) = 0.8
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert

        struct Input {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        fixed4 _Color;
        float _WaveSpeed;
        float _WaveHeight;
        half _Glossiness;

        // Animación de vértices para ondas
        void vert(inout appdata_full v) {
            float time = _Time.y * _WaveSpeed;
            v.vertex.y += sin(time + v.vertex.x * 10) * _WaveHeight;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Normal map animada
            float2 uv = IN.uv_MainTex + _Time.x * 0.1;
            o.Normal = UnpackNormal(tex2D(_MainTex, uv));
            
            // Color y transparencia
            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
