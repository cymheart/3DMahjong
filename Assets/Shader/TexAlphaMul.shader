// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Test/AlphaTexMul"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }	
        _AlphaTex("AlphaTex",2D) = "white"{}
		 _GlowTex ("GlowTex", 2D) = "white" { }
		  _MaskTex("MaskTex",2D) = "white"{}

		 _GlowAlpha("GlowAlpha", Range(0, 1)) = 1
		 _MaskOffset("MaskOffset", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags
        {
           "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" 
        }
        Pass
        {
            Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
            Cull Off
            ColorMask RGB


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            sampler2D _AlphaTex;
			 sampler2D _GlowTex;
			sampler2D _MaskTex;
 
            float _GlowAlpha;
			float _MaskOffset;
 
            struct v2f
            {
                    float4  pos : SV_POSITION;
                    float2  uv : TEXCOORD0;
					//float2 uv1:TEXCOORD1;
                    float4 color :COLOR0;
            };
 
            //half4 _MaskTex_ST;

		float solve1(float aColor, float aAlpha, float bColor, float bAlpha)
	      {
            float n = aColor * (1 - bAlpha) * aAlpha + bColor * (1 - aAlpha) * bAlpha;
            float color = 2 * aColor * bColor * aAlpha * bAlpha + n;
            return clamp(color, 0, 1);
         }

        float solve2(float aColor, float aAlpha, float bColor, float bAlpha)
        {
            float n = aColor * (1 - bAlpha) * aAlpha + bColor * (1 - aAlpha) * bAlpha;
            float color = (1 - 2 * (1 - aColor) * (1- bColor)) * aAlpha * bAlpha + n;
            return clamp(color, 0, 1);
        }

            v2f vert (appdata_full v)
            {
                    v2f o;
                    o.pos = UnityObjectToClipPos (v.vertex);
                    o.uv =  v.texcoord;
                    o.color = v.color;
                    return o;
            }


             float4  frag (v2f i) : SV_Target 
            {
			   float2 mask_uv;

			   _MaskOffset = 2 * _MaskOffset - 1;
			    mask_uv.x = -0.5 * _MaskOffset + i.uv.x ;
				mask_uv.y = 0.5 * _MaskOffset + i.uv.y ;

			    float maskAlpha = tex2D (_MaskTex, mask_uv).a;


			    float4 aColor = tex2D (_GlowTex, i.uv);
			    float4 bColor = tex2D (_MainTex, i.uv);
				bColor.a = tex2D (_AlphaTex, i.uv).r;

				bColor = bColor * i.color;

				float4 result;
				float aAlpha = aColor.a * _GlowAlpha * maskAlpha;

				float a = 1 - (1 - aAlpha)* (1 - bColor.a);

				if(bColor.r <= 0.5)
				{
				    result.r = solve1(aColor.r, aAlpha, bColor.r, bColor.a);
				}
				else{
				    result.r = solve2(aColor.r, aAlpha, bColor.r, bColor.a);
				}

				if(bColor.g <= 0.5)
				{
				    result.g = solve1(aColor.g, aAlpha, bColor.g, bColor.a);
				}
				else{
				    result.g = solve2(aColor.g, aAlpha, bColor.g, bColor.a);
				}

				if(bColor.b <= 0.5)
				{
				    result.b = solve1(aColor.b, aAlpha, bColor.b, bColor.a);
				}
				else{
				    result.b = solve2(aColor.b, aAlpha, bColor.b, bColor.a);
				}

				result = result / a;
				result.a = a;
       
                return result;
            }

		

            ENDCG
        }
    }
}


