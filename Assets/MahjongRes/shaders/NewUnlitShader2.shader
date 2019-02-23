Shader "Unlit/NewUnlitShader2"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	    _LightLevel("亮度", Range(0,1)) = 0
	    _Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _LightLevel;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float4 col = tex2D(_MainTex, i.uv);
			    float4 maxlight = float4(1, 1, 1, col.a);

				col = col + (maxlight - col) * _LightLevel;

				//col = col + float4(0,0,0,0) * 0;
			    
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
