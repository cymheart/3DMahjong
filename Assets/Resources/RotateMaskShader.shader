// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
 
Shader "Custom/RotateMaskShader"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_Angle("ang", Range(0, 360)) = 360
		 _AlphaTex("AlphaTex",2D) = "white"{}

		 _CenterX("CenterX", FLOAT) = 1
		  _CenterY("CenterY", FLOAT) = 1

		   _Alpha("Alpha",FLOAT) = 1
		   _StartAng("StartAng", FLOAT) = -50
	}
	
	SubShader
	{
		tags{"Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True"}
		Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
		ZWrite Off
        ColorMask RGB

		
		Pass
		{
			Name "Simple"
			Cull off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			half _CenterX;
			half _CenterY;
			half _StartAng;
			half _Angle;
			half _Alpha;
			half4 _Color;
			sampler2D _MainTex;	
			sampler2D _AlphaTex;
	
			struct v2f
			{
				float4 pos:POSITION;
				float2 uv:TEXCOORD0;
			//	float2 uv1:TEXCOORD1;
			};
			
			v2f vert(appdata_base v)
			{
				v2f o;
				//将物体坐标转化为剪裁坐标（顶点坐标转换：物体坐标->世界坐标->观察坐标->剪裁坐标）
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
			//	o.uv1 = TRANSFORM_TEX(v.texcoord, _AlphaTex);
				return o;
			}
			
			half4 frag(v2f i):COLOR
			{
				half limitAngle = 360 + _StartAng;

				//half real_Angle = saturate(limitAngle - _Angle) * _Angle + saturate(_Angle - limitAngle) * (_Angle - 360);
				half real_Angle =  _Angle + _StartAng;

				half2 uv = i.uv.xy - half2(_CenterX, _CenterY);
				half ang = round(degrees(atan2(uv.y, -uv.x))) + 180;
				half real_ang = saturate(limitAngle - ang) * ang + saturate(ang - limitAngle) * (ang - 360);
				
				//real_ang = (1 - saturate(abs(ang - limitAngle))) * (ang - 360) + saturate(abs(ang - limitAngle)) * real_ang;
				real_ang = ang - 360 - saturate(abs(ang - limitAngle)) * (ang - 360 - real_ang);

				half  n = real_Angle - real_ang;
				half4 c = tex2D(_MainTex , i.uv) * _Color;
				half subAlpha = saturate((real_ang - _StartAng) / 15);
				half subAlpha2 = saturate(n / 5);	 	
				c.a = tex2D(_AlphaTex,i.uv).a * saturate(n);
			    c.a = saturate(c.a * subAlpha * subAlpha2 * _Alpha);
		
				return c;	
			}


			ENDCG
		}
	}
}
