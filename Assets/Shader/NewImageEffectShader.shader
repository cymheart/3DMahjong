// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NewShader" {
	Properties{	
		_MainTex("Base (RGB)", 2D) = "white" {}
	    _RADIUSBUCE("_RADIUSBUCE",Range(0,0.5)) = 0.2

			_StencilComp("Stencil Comparison", Float) = 8
			_Stencil("Stencil ID", Float) = 0
			_StencilOp("Stencil Operation", Float) = 0
			_StencilWriteMask("Stencil Write Mask", Float) = 255
			_StencilReadMask("Stencil Read Mask", Float) = 255

			_ColorMask("Color Mask", Float) = 15

			_SoomthRange("SoomthRange",Range(0,0.1)) = 0.008

			[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
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

			Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]

	pass
	{	
		CGPROGRAM

#pragma exclude_renderers gles
#pragma vertex vert
#pragma fragment frag
#include "unitycg.cginc"
		float _RADIUSBUCE;
		sampler2D _MainTex;
		float _SoomthRange;

		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 ModeUV: TEXCOORD0;
			float2 RadiusBuceVU : TEXCOORD1;
		};


		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex); //v.vertex;
			o.ModeUV = v.texcoord;
			o.RadiusBuceVU = v.texcoord - float2(0.5,0.5);       //将模型UV坐标原点置为中心原点,为了方便计算

			return o;
		}

		

		fixed4 frag(v2f i) :COLOR
		{
			fixed4 col;
		    float m;

	    	col = float4(0,1,1,0);

			if (abs(i.RadiusBuceVU.x)<0.5 - _RADIUSBUCE )    //即上面说的|x|<(0.5-r)或|y|<(0.5-r)	
			{	
				col = tex2D(_MainTex,i.ModeUV);

				m = smoothstep(0.5 - _SoomthRange, 0.5 + _SoomthRange, abs(i.RadiusBuceVU.y));
				col.a = 1 - m;
		
			}
			else if (abs(i.RadiusBuceVU.y)<0.5 - _RADIUSBUCE)
			{
				col = tex2D(_MainTex, i.ModeUV);

				m = smoothstep(0.5 - _SoomthRange, 0.5 + _SoomthRange, abs(i.RadiusBuceVU.x));
				col.a = 1 - m;
			}
			else
			{
				col = tex2D(_MainTex, i.ModeUV);
				m = smoothstep(_RADIUSBUCE - _SoomthRange, _RADIUSBUCE + _SoomthRange, length(abs(i.RadiusBuceVU) - float2(0.5 - _RADIUSBUCE, 0.5 - _RADIUSBUCE)));
				col.a = 1 - m;

				//if (length(abs(i.RadiusBuceVU) - float2(0.5 - _RADIUSBUCE, 0.5 - _RADIUSBUCE)) <_RADIUSBUCE)
				//{
				//	col = tex2D(_MainTex, i.ModeUV);
				//	m = smoothstep(_RADIUSBUCE - 0.008, _RADIUSBUCE + 0.008, length(abs(i.RadiusBuceVU) - float2(0.5 - _RADIUSBUCE, 0.5 - _RADIUSBUCE)));
				//	col.a = 1 - m;
				//}
				//else
				//{
				//	col = tex2D(_MainTex, i.ModeUV);
				//	m = smoothstep(_RADIUSBUCE - 0.008, _RADIUSBUCE + 0.008, length(abs(i.RadiusBuceVU) - float2(0.5 - _RADIUSBUCE, 0.5 - _RADIUSBUCE)));
				//	col.a = 1 - m;

				//	//discard;
				//}
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


