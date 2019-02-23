Shader "Custom/OutLinePostEfectX2" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurTex ("Blur", 2D) = "white" {}
		_OriTex ("Ori", 2D) = "white" {}
	}
	
	SubShader{

	pass{
	ZTest OFF
	Cull OFF
	ZWrite OFF 
	Fog {Mode OFF}



	CGPROGRAM
	#pragma vertex vert_blur
	#pragma fragment frag_blur
	#include "UnityCG.cginc"

	
	 struct v2f_blur
	{
	    float4 pos: SV_POSITION;
		float2 uv: TEXCOORD0;
		float4 uv01: TEXCOORD1;
		float4 uv23: TEXCOORD2;
		float4 uv45: TEXCOORD3;
	};

	   sampler2D _MainTex;
	 float4 _MainTex_TexelSize;
	 sampler2D _BlurTex;
	 float4 _BlurTex_TexelSize;
	 sampler2D _OriTex;
	 float4 _OriTex_TexelSize;
	 float4 _offsets;
	 fixed4 _OutlineColor;


	//高斯模糊 vert shader
	v2f_blur vert_blur(appdata_img v)
	{
	    v2f_blur o;
		_offsets *= _MainTex_TexelSize.xyxy;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;

		o.uv01 = v.texcoord.xyxy + _offsets.xyxy * float4(1,1,-1,-1);
		o.uv23 = v.texcoord.xyxy + _offsets.xyxy * float4(1,1,-1,-1) * 2.0;
		o.uv45 = v.texcoord.xyxy + _offsets.xyxy * float4(1,1,-1,-1) * 3.0;

		return o;
	}

	fixed4 frag_blur(v2f_blur i):SV_Target
	{
	     fixed4 c = fixed4(0,0,0,0);
		 c += 0.40 * tex2D(_MainTex, i.uv);
		 c += 0.15 * tex2D(_MainTex, i.uv01.xy);
		 c += 0.15 * tex2D(_MainTex, i.uv01.zw);
		 c += 0.10 * tex2D(_MainTex, i.uv23.xy);
		 c += 0.10 * tex2D(_MainTex, i.uv23.zw);
		 c += 0.05 * tex2D(_MainTex, i.uv45.xy);
		 c += 0.05 * tex2D(_MainTex, i.uv45.zw);
		 return c;
	}

	ENDCG
	
	
	}
	

	pass{
	ZTest OFF
	Cull OFF
	ZWrite OFF 
	Fog{Mode OFF}

	
	CGPROGRAM
	#pragma vertex vert_add
	#pragma fragment frag_add
	#include "UnityCG.cginc"

	struct v2f_add
	{
	    float4 pos : SV_POSITION;
		float2 uv: TEXCOORD0;
		float4 uv1: TEXCOORD1;
		float4 uv2: TEXCOORD2;
	};

     sampler2D _MainTex;
	 float4 _MainTex_TexelSize;
	 sampler2D _BlurTex;
	 float4 _BlurTex_TexelSize;
	 sampler2D _OriTex;
	 float4 _OriTex_TexelSize;
	 float4 _offsets;
	 fixed4 _OutlineColor;


    v2f_add vert_add(appdata_img v)
	{
	  

	v2f_add o;
	UNITY_INITIALIZE_OUTPUT(v2f_add,o);

	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv.xy = v.texcoord.xy;
	o.uv1.xy = o.uv.xy;
	o.uv2.xy = o.uv.xy;
	return o;
	}

	fixed4 frag_add(v2f_add i):SV_Target
	{
	     fixed4 scene = tex2D(_MainTex, i.uv1);
		 fixed4 blur = tex2D(_BlurTex, i.uv);
		 fixed4 ori = tex2D(_OriTex, i.uv);

		 fixed4 outline = blur - ori;

		 return ori;
		// _OutlineColor.a = outline.r;

		//return scene + outline;

		//if(outline.r < 0.0001)
		//   return scene;

		  // return fixed4(0,0,1,1);

		 fixed4 final =  scene * (1 - all(outline)) + _OutlineColor * any(outline);
		 final.a = 1;

		 return final;
	}

	ENDCG

	}
	
	}
	FallBack "Diffuse"
}

