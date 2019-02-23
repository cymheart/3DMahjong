// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TexRoundRectMaskShader" {
	Properties
	{	
	    _MainTex("Base (RGB)", 2D) = "white" {}
	    _RADIUSBUCE("RADIUS",Range(0,0.5)) = 0.2
		_SoomthRange("SoomthRange",Range(0,0.1)) = 0.008
		_Color ("Tint", Color) = (1.000000,1.000000,1.000000,1.000000)
		
		_StencilComp("Stencil Comparison", Float) = 8
			_Stencil("Stencil ID", Float) = 0
			_StencilOp("Stencil Operation", Float) = 0
			_StencilWriteMask("Stencil Write Mask", Float) = 255
			_StencilReadMask("Stencil Read Mask", Float) = 255

			_ColorMask("Color Mask", Float) = 15
			[Toggle(UNITY_UI_ALPHACLIP)]  _UseUIAlphaClip ("Use Alpha Clip", Float) = 0.000000
	}
		
	SubShader
	{
	
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}


				Stencil {
   Ref [_Stencil]
   ReadMask [_StencilReadMask]
   WriteMask [_StencilWriteMask]
   Comp [_StencilComp]
   Pass [_StencilOp]
  }

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
	
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask [_ColorMask]

	pass
	{	
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "unitycg.cginc"

        float4 _Color;
		float _RADIUSBUCE;
		sampler2D _MainTex;
		float _SoomthRange;


		    struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
 
			struct v2f
			{
				float4 pos : POSITION;
				float4 color : COLOR;
				float2 ModeUV: TEXCOORD0;
			    float2 RadiusBuceVU : TEXCOORD1;
			};


		v2f vert(appdata v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex); //v.vertex;
			o.color = v.color * _Color;
			o.ModeUV = v.texcoord;
			o.RadiusBuceVU = v.texcoord - float2(0.5,0.5);       //将模型UV坐标原点置为中心原点,为了方便计算

			return o;
		}

		

		fixed4 frag(v2f i) :COLOR
		{
			fixed4 col;
		    float m;

	    	col = float4(0,1,1,0);

			if (abs(i.RadiusBuceVU.x) < 0.5 - _RADIUSBUCE)    //|x|<(0.5-r)或|y|<(0.5-r)	
			{	
				col = tex2D(_MainTex,i.ModeUV) * i.color;
				m = smoothstep(0.5 - _SoomthRange, 0.5 + _SoomthRange, abs(i.RadiusBuceVU.y));
				col.a = col.a * (1 - m);	
			}
			else if (abs(i.RadiusBuceVU.y)<0.5 - _RADIUSBUCE)
			{
				col = tex2D(_MainTex, i.ModeUV) * i.color;
				m = smoothstep(0.5 - _SoomthRange, 0.5 + _SoomthRange, abs(i.RadiusBuceVU.x));
				col.a = col.a * (1 - m);
			}
			else
			{
				col = tex2D(_MainTex, i.ModeUV)* i.color;
				m = smoothstep(_RADIUSBUCE - _SoomthRange, _RADIUSBUCE + _SoomthRange, length(abs(i.RadiusBuceVU) - float2(0.5 - _RADIUSBUCE, 0.5 - _RADIUSBUCE)));
				col.a = col.a * (1 - m);
			}
			return col;
		}

	
		float linearstep(float edge0, float edge1, float x) 
		{
			float t = (x - edge0) / (edge1 - edge0);
			return clamp(t, 0.0, 1.0);
		}

			
		ENDCG
	
		}
	}
}



