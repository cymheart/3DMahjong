Shader "Custom/Diffuse Texture" {
	Properties {
	     _Color ("Color", Color) = (1,1,1,1)
		 _MainTex ("Albedo (RGB)", 2D) = "white" {}//A通道存储高光
		 _GlossTex ("Albedo (RGB)", 2D) = "white" {}//A通道存储高光
		 _specularColor ("高光颜色", Color) = (1.0, 1.0, 1.0, 1.0)
         _NormalMap ("Normal Map", 2D) = "bump" {}
        _SpecIntensity ("Specular Width", Range(0.01,1)) = 0.5
         }

       SubShader 
	   {
          Tags { "RenderType"="Opaque" }
          LOD 200

		  CGPROGRAM
		  // Physically based Standard lighting model, and enable shadows on all light types
		  #pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview
		 // #pragma exclude_renderers flash d3d11

          // Use shader model 3.0 target, to get nicer looking lighting
		  #pragma target 2.0

		struct Input {
			half2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D _GlossTex;
		sampler2D _NormalMap;
		half _SpecIntensity;
		fixed4 _Color;
		fixed4 _specularColor;

		void surf (Input IN, inout SurfaceOutput o)
		{
             // Albedo comes from a texture tinted by color
			 fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 gs = tex2D (_GlossTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Gloss = gs.r;
			o.Alpha = 0;
			o.Specular = _SpecIntensity;
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			o.Alpha = c.a;
        }
		
		inline fixed4 LightingMobileBlinnPhong(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir,fixed atten)
       {
	   fixed diff= max(0,dot(s.Normal,lightDir));
	   fixed nh= max(0,dot(s.Normal,halfDir));
	   fixed spec= pow(nh,s.Specular*128) * s.Gloss;
	   fixed4 c;
	   c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb *spec *_specularColor) * (atten * 2);
	   c.a = 0;
	   return c;
     }
	 ENDCG
    }
   FallBack off
}