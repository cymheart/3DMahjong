// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Yogi/ImageEffect/Outline"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_StencilMap("StencilMap (RGB)", 2D) = "white" {}
		_BlurMap("BlurMap (RGB)", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
		_Intensity("Intensity", Float) = 1
	}
 
	SubShader
	{
		Pass
		{
			ZTest Always
			ZWrite Off
			Cull Off
			Fog{ Mode Off }
 
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
 
			sampler2D _MainTex;
			sampler2D _StencilMap;
			sampler2D _BlurMap;
			fixed3 _OutlineColor;
			fixed _Intensity;
 
			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
			};
 
			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
			};
 
			v2f vert(a2v v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
 
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed4 s = tex2D(_StencilMap, i.uv);
				fixed4 b = tex2D(_BlurMap, i.uv);

				//o.rgba = fixed3(c.a,c.a,c.a,c.a);
 
				fixed4 o = b - s;
				o.rgb = saturate(o.a) * _Intensity * _OutlineColor +c.rgb * (1 -saturate(o.a) ) ;
				o.a = c.a;

			  //   if(saturate(o.a) < 1)
				 //{
				 //o.rgb = saturate(o.a) * _Intensity * _OutlineColor +c.rgb * (1 -saturate(o.a) ) ;
				 //}
				 //else
			  //     o.rgb = c.rgb + saturate(o.a) * _Intensity * _OutlineColor;
				//o.a = c.a;
 
				return o;
			}
 
			ENDCG
		}
	}
 
	Fallback off
}
