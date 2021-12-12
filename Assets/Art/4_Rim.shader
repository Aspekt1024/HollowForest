Shader "Holistic/4_Rim" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _StripeWidth ("Stripe Width", Range(0.00001, 1)) = 1
        _RimColor ("Rim Color", Color) = (0, 0.5, 0.5)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
    }
    
    SubShader {
        CGPROGRAM
        #pragma surface surf Lambert
        
        struct Input {
            float3 viewDir;
            float3 worldPos;
            float2 uv_Diffuse;
        };

        float4 _RimColor;
        float _RimPower;
        float _StripeWidth;
        sampler2D _Diffuse;
        
        void surf(Input IN, inout SurfaceOutput o) {
            half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
            o.Albedo = rim * _RimColor;//tex2D(_Diffuse, IN.uv_Diffuse).rgb;
            //o.Emission = (frac(IN.worldPos.y / _StripeWidth * 0.5) > 0.5 ? _RimColor : 0) * pow(rim, _RimPower);
        }
        ENDCG
    }
    Fallback "Diffuse"
}