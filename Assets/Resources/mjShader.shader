// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Yogi/Character"
{
	Properties
	{
		_MainTex("Base(RGB) Trans(A)", 2D) = "white" {}
		_BlendMap("Gloss(R) Illum(G) Mask(B)", 2D) = "black" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_FlowMap("Flowmap", 2D) = "black" {}
		_MaskColor("Mask Color", Color) = (1, 1, 1, 1)
		_Alpha("Alpha", Range(0, 1)) = 1
		_Specular("Specular", Range(0, 10)) = 1
		_Shininess("Shininess", Range(0.01, 1)) = 0.5
		_Emission("Emission", Range(0, 10)) = 1
		_FlowSpeed("Flow Speed", Range(0, 10)) = 1
		_RimColor("Rim Color", Color) = (0, 0, 0, 1)
		_RimPower("Rim Power", Range(0, 10)) = 1
	}
 
	SubShader
	{
		Tags
		{
			"Queue" = "Geometry+1"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater 0
 
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase nolightmap nodirlightmap
 
			sampler2D _MainTex;
			sampler2D _BlendMap;
			sampler2D _FlowMap;
			sampler2D _BumpMap;
			fixed4 _MainTex_ST;
			fixed3 _MaskColor;
			fixed3 _RimColor;
			fixed _Alpha;
			fixed _Specular;
			fixed _Shininess;
			fixed _Emission;
			fixed _FlowSpeed;
			fixed _RimPower;
 
			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed4 tangent : TANGENT;
				fixed4 texcoord : TEXCOORD0;
				fixed4 texcoord1 : TEXCOORD1;
			};
 
			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed4 uv : TEXCOORD0;
				fixed3 lightDir : TEXCOORD1;
				fixed3 vlight : TEXCOORD2;
				fixed3 viewDir : TEXCOORD3;
				LIGHTING_COORDS(4,5)
			};
 
			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = v.texcoord1.xy;
 
				TANGENT_SPACE_ROTATION;
				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
 
#ifdef LIGHTMAP_OFF
				fixed3 worldNormal = mul((fixed3x3)unity_ObjectToWorld, SCALED_NORMAL);
				o.vlight = ShadeSH9(fixed4(worldNormal, 1.0));
#ifdef VERTEXLIGHT_ON
				fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.vlight += Shade4PointLights(
					unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
					unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
					unity_4LightAtten0, worldPos, worldNormal);
#endif
#endif
 
				TRANSFER_VERTEX_TO_FRAGMENT(o);
 
				return o;
			}
 
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv.xy);
				fixed3 b = tex2D(_BlendMap, i.uv.xy);
				fixed3 f = tex2D(_FlowMap, i.uv.zw + _Time.xx * _FlowSpeed);
				fixed3 n = UnpackNormal(tex2D(_BumpMap, i.uv.xy));
				fixed atten = LIGHT_ATTENUATION(i);
				fixed3 h = normalize(i.lightDir + i.viewDir);
				fixed diff = saturate(dot(n, i.lightDir));
				fixed nh = saturate(dot(n, h));
				fixed spec = pow(nh, _Shininess * 128.0) * b.r * _Specular;
				fixed nv = pow(1 - saturate(dot(n, i.viewDir)), _RimPower);
				fixed4 o = 0;
 
				c.rgb = lerp(c.rgb, _MaskColor.rgb, b.b);
				o.rgb = (c.rgb * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten * 2);
				o.rgb += nv * _RimColor;
				o.rgb += c.rgb * i.vlight;
				o.rgb += c.rgb * b.g * _Emission + f;
				o.a = c.a * _Alpha;
 
				return o;
			}
 
			ENDCG
		}
	}
 
	FallBack "Mobile/Diffuse"
}
