// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Yogi/ImageEffect/Blur"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Offset("Offset", Vector) = (0, 0, 0, 0)
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
			fixed2 _Offset;
 
			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
			};
 
			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed2 offsets[4] : TEXCOORD1;
			};
		
			v2f vert(a2v v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				o.uv = v.texcoord.xy;
				o.offsets[0] = fixed2(_Offset.x, _Offset.y);
				o.offsets[1] = fixed2(-_Offset.x, _Offset.y);
				o.offsets[2] = fixed2(_Offset.x, -_Offset.y);
				o.offsets[3] = fixed2(-_Offset.x, -_Offset.y);
 
				return o;
			}
 
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 o = tex2D(_MainTex, i.uv);
				o += tex2D(_MainTex, i.uv + i.offsets[0]);
				o += tex2D(_MainTex, i.uv + i.offsets[1]);
				o += tex2D(_MainTex, i.uv + i.offsets[2]);
				o += tex2D(_MainTex, i.uv + i.offsets[3]);
				o *= 0.2f;
 
				return o;
			}
 
			ENDCG
		}
	}
 
	Fallback off
}
