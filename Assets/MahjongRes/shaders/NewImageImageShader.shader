Shader "Custom/NewImageEffectShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_DiffusePower("Diffuse Power", Float) = 1.0
	}
		SubShader
	{

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float3 normalDir : TEXCOORD1;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;

		// 将物体法线从物体坐标系转换到世界坐标系
		o.normalDir = UnityObjectToWorldNormal(v.normal);
		return o;
	}

	sampler2D _MainTex;
	float _DiffusePower;

	fixed4 frag(v2f i) : SV_Target
	{
		// 法线方向
		float3 normalDirection = normalize(i.normalDir);
		// 灯光方向
		float lightDirection = normalize(_WorldSpaceLightPos0.xyz);
		// 灯光颜色
		float3 lightColor = _LightColor0.rgb;

		// 计算灯光衰减
		float attenuation = LIGHT_ATTENUATION(i);
		float3 attenColor = attenuation * _LightColor0.xyz;

		// 基于兰伯特模型计算灯光
		float NdotL = max(0,dot(normalDirection,lightDirection));
		// 方向光
		float3 directionDiffuse = pow(NdotL, _DiffusePower) * attenColor;
		// 环境光  
		float3 inDirectionDiffuse = float3(0,0,0) + UNITY_LIGHTMODEL_AMBIENT.rgb;

		// 灯光与材质球表面颜色进行作用
		float3 texColor = tex2D(_MainTex, i.uv).rgb;
		float3 diffuseColor = texColor *(directionDiffuse + inDirectionDiffuse);
		float4 finalColor = float4(diffuseColor,1);

		return finalColor;
	}
		ENDCG
	}
	}
}