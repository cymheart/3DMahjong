Shader "Custom/UIAlphaShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _AlphaTex("AlphaTex",2D) = "white"{}

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
		"QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" 
        }
        Pass
        {
	Stencil {
   Ref [_Stencil]
   ReadMask [_StencilReadMask]
   WriteMask [_StencilWriteMask]
   Comp [_StencilComp]
   Pass [_StencilOp]
  }

            Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
           	ColorMask [_ColorMask]


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            sampler2D _AlphaTex;
			float4 _Color;
 
			 struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
 
            struct v2f
            {
                    float4  pos : SV_POSITION;
                    float2  uv : TEXCOORD0;
                    float4 color :COLOR;
            };
 
            v2f vert (appdata v)
            {
                    v2f o;
                    o.pos = UnityObjectToClipPos (v.vertex);
                    o.uv =  v.texcoord;
                    o.color = v.color * _Color;
                    return o;
            }
 
            half4 frag (v2f i) : COLOR
            {
                half4 texcol = tex2D (_MainTex, i.uv);
                half4 result = texcol;
			    result.a = tex2D(_AlphaTex,i.uv).a;
				result = result * i.color;

                return result;
            }
            ENDCG
        }
    }
}
