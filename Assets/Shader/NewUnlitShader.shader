// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//
//Shader "LT/AlphaBlend"
//{
//	Properties
//	{
//		_MainTex("Texture", 2D) = "white" {}
//	_Color("Color",Color) = (1,1,1,1)
//		_AlphaScale("Alpha Scale",Range(0,1)) = 1
//
//	}
//		SubShader
//	{
//		Tags{
//		"Queue" = "Transparent"
//		"IgnoreProjector" = "True"
//		"RenderType" = "Transparent"
//		"PreviewType" = "Plane"
//		"CanUseSpriteAtlas" = "True" 
//	//	"LightMode" = "ForwardBase"
//	}//这一行千万要注意空格，不然模型会出现一会儿部分透明一会儿不透明的问题
//
//		Cull Off
//		Lighting Off
//		ZWrite Off
//		
//		//ZTest[unity_GUIZTestMode]
//		//Blend SrcAlpha OneMinusSrcAlpha
//		//ColorMask[_ColorMask]
//
//		Pass
//	{
//		//Blend SrcAlpha OneMinusSrcAlpha
//		//Tags{ "LightMode" = "ForwardBase" }
//	//	ZWrite off   //如果不关闭深度写入，半透明物体离相机更近的话，进行深度测试后会将其后的物体表面剔除。导致没有半透效果了，因此关闭深度写入。
//
//		Blend SrcAlpha OneMinusSrcAlpha     //透明度混合
//
//											//           Blend OneMinusDstColor One     //柔和相加
//
//											//          Blend DstColor Zero       //正片叠底，目标颜色*源颜色
//
//											//            BlendOp Min            //取颜色最小值
//											//            Blend Zero Zero     
//
//											//            BlendOp Max        ////取颜色最大值
//											//            Blend One One 
//
//											 //           Blend OneMinusDstColor One  //滤色，与下面这句等价
//											//            Blend  One OneMinusSrcColor
//
//		CGPROGRAM
//#pragma vertex vert
//#pragma fragment frag
//
//#include "UnityCG.cginc"
//#include "Lighting.cginc"
//
//		sampler2D _MainTex;
//	float4 _MainTex_ST;
//	float4 _Color;
//	fixed _AlphaScale;
//
//	struct appdata
//	{
//		float4 vertex : POSITION;
//		float2 uv : TEXCOORD0;
//		float3 normal :NORMAL;
//	};
//
//	struct v2f
//	{
//		float2 uv : TEXCOORD0;
//		float4 vertex : SV_POSITION;
//		float3 worldNormal :TEXCOORD1;
//		float3 worldPos :TEXCOORD2;
//	};
//
//	//坐标系转换，模型空间->世界空间
//	v2f vert(appdata v)
//	{
//		v2f o;
//		o.vertex = UnityObjectToClipPos(v.vertex);
//		o.worldNormal = UnityObjectToWorldNormal(v.normal);
//		o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
//		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//		return o;
//	}
//
//	fixed4 frag(v2f i) : SV_Target
//	{
//		fixed3 worldNormal = normalize(i.worldNormal);
//	fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
//	fixed4 col = tex2D(_MainTex, i.uv);
//
//	//alpha test
//	//                clip(col.a - _Cutoff);
//	//上面这句等同与下面这个
//	//if((col.a - _Cutoff)<0.0)
//	//                {
//	//                    disscard;
//	//                }
//	fixed3 albedo = col.rgb;
//	fixed3 diffuse = albedo * max(0,dot(worldNormal,worldLightDir));
//
//	return fixed4(col.rgb,col.a);
//	}
//		ENDCG
//	}
//	}
//		FallBack "Transparent/VertexLit"
//}


//Shader "Custom/BlendMode_Effect" {
//
//	Properties{
//		_MainTex("Albedo (RGB)", 2D) = "white" {}
//	_Blendtex("Blend Texture",2D) = "white"{}
//	_Opacity("Blend Opacity",Range(0,1)) = 1
//	}
//		SubShader{
//		Pass
//	{
//		CGPROGRAM
//#pragma vertex vert_img
//#pragma fragment frag
//#pragma fragmentoption ARB_precision_hint_fastest
//#include "UnityCG.cginc"
//		uniform sampler2D _MainTex;
//	uniform sampler2D _BlendTex;
//	fixed _Opacity;
//
//
//	fixed4 frag(v2f_img i) : COLOR
//	{
//		fixed4 renderTex = tex2D(_MainTex,i.uv);
//	fixed4 blendTex = tex2D(_BlendTex,i.uv);
//
//	//fixed4 blendedMultiply = renderTex * blendTex;
//	fixed4 blendedScreen = (1.0 - ((1.0 - renderTex) * (1.0 - blendTex)));//这里是颜色计算核心
//	renderTex = lerp(renderTex,blendedScreen,_Opacity);
//	return renderTex;
//	}
//		ENDCG
//	}
//
//
//	}
//}
//
//

//Shader "Unlit/NewUnlitShader"
//{
//	Properties
//	{
//		_MainTex("Texture", 2D) = "white" {}
//	    _StencilComp("Stencil Comparison", Float) = 8
//		_Stencil("Stencil ID", Float) = 0
//		_StencilOp("Stencil Operation", Float) = 0
//		_StencilWriteMask("Stencil Write Mask", Float) = 255
//		_StencilReadMask("Stencil Read Mask", Float) = 255
//
//		_ColorMask("Color Mask", Float) = 15
//
//		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
//	}
//		SubShader
//	{
//		//Tags { "RenderType"="Opaque" }
//
//												Tags{
//												"QUEUE" = "Transparent" 
//												"IGNOREPROJECTOR" = "True"
//												"RenderType" = "Transparent"
//												"PreviewType" = "Plane"
//												"CanUseSpriteAtlas" = "True" }//这一行千万要注意空格，不然模型会出现一会儿部分透明一会儿不透明的问题
//										
//												Cull Off
//												Lighting Off
//												ZWrite Off
//												ZTest[unity_GUIZTestMode]
//												Blend SrcAlpha OneMinusSrcAlpha
//												ColorMask[_ColorMask]
//
//		LOD 100
//
//		GrabPass{} // 绘制当前的屏幕图像到 _GrabTexture 里
//
//		Pass
//	{
//		CGPROGRAM
//#pragma vertex vert
//#pragma fragment frag
//		// make fog work
//#pragma multi_compile_fog
//
//#include "UnityCG.cginc"
//
//		struct appdata
//	{
//		float4 vertex : POSITION;
//		float2 uv : TEXCOORD0;
//	};
//
//	struct v2f
//	{
//		float2 uv : TEXCOORD0;
//		UNITY_FOG_COORDS(1)
//			float4 vertex : SV_POSITION;
//		half4 screenuv : TEXCOORD2; // 当前屏幕图像的UV坐标
//	};
//
//	sampler2D _MainTex;
//	float4 _MainTex_ST;
//
//	sampler2D _GrabTexture; // 当前屏幕图像
//
//	v2f vert(appdata v)
//	{
//		v2f o;
//		o.vertex = UnityObjectToClipPos(v.vertex);
//		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//		UNITY_TRANSFER_FOG(o,o.vertex);
//		o.screenuv = ComputeGrabScreenPos(o.vertex); // 获取当前屏幕图像的UV坐标
//		return o;
//	}
//
//	fixed4 frag(v2f i) : SV_Target
//	{
//
//		fixed4 colour = tex2D(_GrabTexture, float2(i.screenuv.x, i.screenuv.y)); // 获取当前屏幕图像的颜色
//
//																				 // sample the texture
//	fixed4 col = tex2D(_MainTex, i.uv);
//
//	fixed4 endd = col + colour; // ADD 混合模式
//
//								// apply fog
//	UNITY_APPLY_FOG(i.fogCoord, endd);
//	return endd;
//	}
//		ENDCG
//	}
//	}
//}



//Shader "Unity Shaders/BlendShader/Blend Operations" {
//	Properties{
//		_Color("Color Tint", Color) = (1, 1, 1, 1)
//		_MainTex("Main Tex", 2D) = "white" {}
//	_AlphaScale("Alpha Scale", Range(0, 1)) = 1
//	}
//		SubShader{
//		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
//
//
//		GrabPass{} // 获取当前的屏幕图像到 _GrabTexture 里
//
//		Pass{
//		Tags{ "LightMode" = "ForwardBase" }
//
//		ZWrite Off
//		//          正常，透明度混合
//		//          // Normal
//		         Blend SrcAlpha OneMinusSrcAlpha
//
//		//          柔和叠加
//		//          // Soft Additive
//		//          Blend OneMinusDstColor One
//
//		//          正片叠底 相乘         
//		//          // Multiply
//		//Blend DstColor Zero
//
//		//          两倍叠加 相加
//		//          // 2x Multiply
//		//Blend SrcFactor DstFactor, SrcFactorA DstFactorA
//		//          Blend DstColor SrcColor
//
//		//          变暗
//		//          // Darken
//		//          BlendOp Min
//		//          Blend One One   // When using Min operation, these factors are ignored
//
//		//          变亮
//		//          //  Lighten
//		//          BlendOp Max
//		//           Blend One One // When using Max operation, these factors are ignored
//
//		//          滤色
//		//          // Screen
//		//          Blend OneMinusDstColor One
//		// Or
//		//          Blend One OneMinusSrcColor
//
//		//          线性减淡
//		//          // Linear Dodge
//		//          Blend One One
//
//		
//
//		CGPROGRAM
//
//#pragma vertex vert
//#pragma fragment frag
//
//#include "Lighting.cginc"
//
//		fixed4 _Color;
//	sampler2D _MainTex;
//	float4 _MainTex_ST;
//	fixed _AlphaScale;
//	sampler2D _GrabTexture; // 当前屏幕图像
//
//	struct a2v {
//		float4 vertex : POSITION;
//		float3 normal : NORMAL;
//		float4 texcoord : TEXCOORD0;
//	};
//
//
//		struct v2f
//		{
//			float2 uv : TEXCOORD0;
//			float4 pos : SV_POSITION;
//			half4 screenuv : TEXCOORD2; // 当前屏幕图像的UV坐标
//		};
//
//
//
//	v2f vert(a2v v) {
//		v2f o;
//		o.pos = UnityObjectToClipPos(v.vertex);
//		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
//
//		return o;
//	}
//
//
//	fixed4 frag(v2f i) : SV_Target{
//		fixed4 blend = tex2D(_MainTex, i.uv);
//	    fixed4 base = tex2D(_GrabTexture, float2(i.screenuv.x, i.screenuv.y)); // 获取当前屏幕图像的颜色
//
//	fixed4 result;
//
//	if (base.a < 0.5) {
//		result.a = 2.0 * base.a * blend.a;
//	}
//	else {
//		result.a = 1.0 - 2.0 * (1.0 - blend.a) * (1.0 - base.a);
//	}
//
//	if (base.r < 0.5) {
//		result.r = 2.0 * blend.r * base.r;
//	}
//	else {
//		result.r = 1.0 - 2.0 * (1.0 - blend.r) * (1.0 - base.r);
//	}
//
//	if (base.g < 0.5) {
//		result.g = 2.0 * blend.g * base.g;
//	}
//	else {
//		result.g = 1.0 - 2.0 * (1.0 - blend.g) * (1.0 - base.g);
//	}
//
//	if (base.b < 0.5) {
//		result.b = 2.0 * blend.b * base.b;
//	}
//	else {
//		result.b = 1.0 - 2.0 * (1.0 - blend.b) * (1.0 - base.b);
//	}
//	   
//	result.a = 1.0;
//	//base.a = 0.5;
//	return base;
//
//
//	}
//
//		ENDCG
//	}
//	}
//		FallBack "Transparent/VertexLit"
//}
//
Shader "Unlit/NewUnlitShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		//Tags { "RenderType"="Opaque" }
		Tags{ "Queue" = "Transparent" } // 不使用这个会导致出现背景透明部分不显示的情况
		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		GrabPass{} // 绘制当前的屏幕图像到 _GrabTexture 里

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
			float4 vertex : SV_POSITION;
		half4 screenuv : TEXCOORD2; // 当前屏幕图像的UV坐标
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;

	sampler2D _GrabTexture; // 当前屏幕图像

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		o.screenuv = ComputeGrabScreenPos(o.vertex); // 获取当前屏幕图像的UV坐标
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{

		fixed4 base = tex2D(_GrabTexture, float2(i.screenuv.x, i.screenuv.y)); // 获取当前屏幕图像的颜色

																				 // sample the texture
	fixed4 blend = tex2D(_MainTex, i.uv);

	fixed4 result;
	  
						  	if (base.r < 0.5) {
								result.r = 2.0 * blend.r * base.r;
						  	}
						  	else {
								result.r = 1.0 - 2.0 * (1.0 - blend.r) * (1.0 - base.r);
						  	}
						  
						  	if (base.g < 0.5) {
								result.g = 2.0 * blend.g * base.g;
						  	}
						  	else {
								result.g = 1.0 - 2.0 * (1.0 - blend.g) * (1.0 - base.g);
						  	}
						  
						  	if (base.b < 0.5) {
								result.b = 2.0 * blend.b * base.b;
						  	}
						  	else {
								result.b = 1.0 - 2.0 * (1.0 - blend.b) * (1.0 - base.b);
						  	}

							float4 a = float4(blend.a, blend.a, blend.a, blend.a);
							float4 m = float4(1, 1, 1, 1);

							result = (m - a) * result + a * blend;
							result.a = clamp(blend.a * 3.5, 0, 1);

						/*	result.r = (1 - a) * result.r + a * blend.r;
							result.g = (1 - a) * result.g + a * blend.g;
							result.b = (1 - a) * result.b + a * blend.b;*/


	return result;
	}
		ENDCG
	}
	}
}