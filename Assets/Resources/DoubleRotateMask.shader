// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
 
Shader "Custom/DoubleRotateMask"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		 _AlphaTex("AlphaTex",2D) = "white"{}
		   _Speed("Speed", FLOAT) = 1
	}
	
	SubShader
	{
		tags{"Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True"}
		Lighting Off
		Blend SrcAlpha One
       // Blend SrcAlpha OneMinusSrcAlpha
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
			
			half _Speed;
			half4 _Color;
			sampler2D _MainTex;	
			sampler2D _AlphaTex;
	
			struct v2f
			{
				float4 pos:POSITION;
				float2 uv:TEXCOORD0;
			//	float2 uv1:TEXCOORD1;
			    float4 color :COLOR;
			};
			
			v2f vert(appdata_full v)
			{
				v2f o;
				//将物体坐标转化为剪裁坐标（顶点坐标转换：物体坐标->世界坐标->观察坐标->剪裁坐标）
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
			//	o.uv1 = TRANSFORM_TEX(v.texcoord, _AlphaTex);
			    o.color = v.color;
				return o;
			}
			
			half4 frag(v2f i):COLOR
			{    
			    half2 uv = i.uv.xy - half2(0.5, 0.5);
			    float uvDeg = degrees(atan2(uv.y, uv.x));
				float tm = _Time.y * _Speed;
				tm = tm % (3.14159);
				float limitDeg = degrees(tm);
				float limitDeg2 = limitDeg + 180;
			    half4 c;
				half alpha = tex2D(_AlphaTex,i.uv).r;		
				
				c = tex2D(_MainTex, i.uv);        
				uvDeg = saturate(uvDeg) * uvDeg + saturate(-uvDeg) * (360 + uvDeg);
				alpha = (1 - saturate(uvDeg - limitDeg)) * alpha + saturate(uvDeg - 180) * (1 - saturate(uvDeg - limitDeg2)) * alpha;
				c.a = alpha;
				c = c * _Color * i.color;
		
				return c;

				  
				// half2 uv = i.uv.xy - half2(0.5, 0.5);
			 //   float uvDeg = degrees(atan2(uv.y, uv.x));
				//float tm = _Time.y * _Speed;
				//tm = tm % (3.14159);
				//float limitDeg = degrees(tm);
				//float limitDeg2 = limitDeg + 180;
			 //   half4 c = half4(0,0,0,0);
	
				//if(uvDeg < 0)
				//  uvDeg = 360 + uvDeg;
	
				//if((uvDeg <= limitDeg && uvDeg>=0) ||
				//    (uvDeg > 180 && uvDeg <= limitDeg2))
				//{
				//   c = tex2D(_MainTex, i.uv);        
				//   c.a = tex2D(_AlphaTex,i.uv).r;		
				//   c = c * _Color;
				//   return c;
				//}
		
				//return c;

			}


			ENDCG
		}
	}
}
