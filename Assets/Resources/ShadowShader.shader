// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ShadowShader" {
	Properties {
			_Color ("Color", Color) = (1,1,1,1)
			_MainTex ("shadow0", 2D) = "black" {}		
			_Angle("RotateAngle", Range(0, 360)) = 0	//角度

			_MainTex1 ("shadow1", 2D) = "black" {}		
			_Angle1("RotateAngle1", Range(0, 360)) = 0	//角度

			_MainTex2 ("shadow2", 2D) = "black" {}		
			_Angle2("RotateAngle2", Range(0, 360)) = 0	//角度
 
	}
 
	
	SubShader {
		
		Tags{ "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" }

        //Blend选值为：SrcAlpha  和OneMinusSrcAlpha（即1-SrcAlpha）
		Blend SrcAlpha OneMinusSrcAlpha	
		Lighting Off		
	    ZWrite Off
        Cull Off
        ColorMask RGB


		Pass{
			Name "RotateShader"
			Cull off //双面都显示
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
		
	    	#define PI 3.142

			sampler2D _MainTex; //变量使用前声明
			sampler2D _MainTex1; 
			sampler2D _MainTex2; 

			float4 _MainTex_ST;
			float4 _MainTex1_ST;
			float4 _MainTex2_ST;

			float4 _Color;
			float _Angle;
			float _Angle1;
			float _Angle2;
			
			struct v2f{		
				float4 pos:POSITION; 
				float2 uv:TEXCOORD0;
				float2 uv1:TEXCOORD1;
				float2 uv2:TEXCOORD2;
			};
		
			v2f vert(appdata_base v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.texcoord, _MainTex1);
				o.uv2 = TRANSFORM_TEX(v.texcoord, _MainTex2);
				return o;
			}
		
			half4 frag(v2f i):COLOR{
 
				float2 uv = i.uv.xy - float2(0.5, 0.5);//UV原点移动到UV中心点
				float arc = _Angle * PI / 180;
				
				//float2 rotate = float2(cos(_RSpeed *_Time.x), sin(_RSpeed *_Time.x));
				//uv = float2(uv.x * rotate.x - uv.y * rotate.y, uv.x * rotate.y + uv.y * rotate.x)
                //θ旋转角度 UV旋转 (xcosθ - ysinθ,xsinθ+ycosθ)
				uv = float2(uv.x *cos(-arc) - uv.y * sin(-arc),uv.x *sin(-arc) + uv.y*cos(-arc));
				uv += float2(0.5, 0.5);//UV中心转移回原来原点位置
				half4 c1 = tex2D(_MainTex, uv) * _Color;			


				uv = i.uv1.xy - float2(0.5, 0.5);//UV原点移动到UV中心点
				arc = _Angle1 * PI / 180;	
				uv = float2(uv.x *cos(-arc) - uv.y * sin(-arc),uv.x *sin(-arc) + uv.y*cos(-arc));
				uv += float2(0.5, 0.5);//UV中心转移回原来原点位置
				half4 c2 = tex2D(_MainTex1, uv) * _Color;	

				uv = i.uv2.xy - float2(0.5, 0.5);//UV原点移动到UV中心点
				arc = _Angle2 * PI / 180;	
				uv = float2(uv.x *cos(-arc) - uv.y * sin(-arc),uv.x *sin(-arc) + uv.y*cos(-arc));
				uv += float2(0.5, 0.5);//UV中心转移回原来原点位置
				half4 c3 = tex2D(_MainTex2, uv) * _Color;	
		
				half4 c;
				c.a = c1.a + c2.a + c3.a;
				c.r = c1.r + c2.r + c3.r;
				c.g = c1.g + c2.g + c3.g;
				c.b = c1.b + c2.b + c3.b;

				//c3.r = (c.r * c.a * (1 - c2.a) + c2.r * c2.a) / c3.a;
				//c3.g = (c.g * c.a * (1 - c2.a) + c2.g * c2.a) / c3.a;
				//c3.b = (c.b * c.a * (1 - c2.a) + c2.b * c2.a) / c3.a;

				return c;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
