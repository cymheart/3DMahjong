
Shader "Custom/DrawOutline1"
{
    Properties
    {
        _MainTex("Main Texture",2D)="black"{}			// 绘制完物体面积后纹理
        _SceneTex("Scene Texture",2D)="black"{}			// 原场景纹理
		_Color("Outline Color",Color) = (0,1,0,1)		// 描边颜色
		_Width("Outline Width",int) = 4					// 描边宽度（像素级别）
		_Iterations("Iterations",int) = 3				// 描边迭代次数（越高越平滑，消耗越高，复杂度O(n^2)）
    }
    SubShader 
    {
		Tags { "Queue" = "Transparent" }
        Pass 
        {
			ZTest Always Cull Off ZWrite Off

            CGPROGRAM
     
            sampler2D _MainTex;
            float2 _MainTex_TexelSize;
			sampler2D _SceneTex;
			fixed4 _Color;
			float _Width;
			int _Iterations;
 
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
             
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
             
            v2f vert (appdata_base v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;  
                return o;
            }
             
            half4 frag(v2f i) : COLOR 
            {
				// 迭代为奇数，保证对称
                int iterations = _Iterations * 2 + 1;
                float ColorIntensityInRadius;
                float TX_x = _MainTex_TexelSize.x * _Width;
                float TX_y = _MainTex_TexelSize.y * _Width;

				float r = iterations/2;

				// 类似高斯模糊，只是这里的核权重不重要，只要最后计算大于0，说明该像素属于外边范围内。
                for(int k = 0;k < iterations;k += 1)
                    for(int j = 0;j < iterations;j += 1)
						if((k-r)*(k-r) + (j-r)*(j-r) <= r * r)
							ColorIntensityInRadius += tex2D(_MainTex, i.uv.xy + float2((k - r) * TX_x,(j - r) * TX_y));

				// 如果该像素有颜色（原来所占面积），或者该像素不在外边范围内，直接渲染原场景。否则就渲染为外边颜色。
				if(tex2D(_MainTex,i.uv.xy).r > 0 || ColorIntensityInRadius == 0)
					return tex2D(_SceneTex, i.uv);
				else
					return _Color.a * _Color + (1 - _Color.a) * tex2D(_SceneTex, i.uv);
            }
            ENDCG
        }

    }
}
