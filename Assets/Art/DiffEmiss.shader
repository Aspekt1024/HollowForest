Shader "Holistic/DiffEmiss" {
  
  Properties {
       _mainTex ("Main Texture", 2D) = "white" {}
       _emissive ("Emission", 2D) = "black" {}
       _normal ("Normal", 2D) = "white" {}
  }
  
  SubShader {
    
    CGPROGRAM
      #pragma surface surf Lambert

      sampler2D _mainTex;
      sampler2D _emissive;
      sampler2D _normal;

      struct Input {
        float2 uv_mainTex;
        float2 uv_emissive;
        float2 uv_normal;
        float3 worldRefl;
      };      

      void surf (Input IN, inout SurfaceOutput o){
        o.Albedo = (tex2D(_mainTex, IN.uv_mainTex)).rgb;
        o.Emission = (tex2D(_emissive, IN.uv_emissive)).rgb;
      }
    
    ENDCG
  }
  
  FallBack "Diffuse"
}

