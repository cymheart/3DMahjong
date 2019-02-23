// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/13-Rock NormalMap" { 
    Properties{
        _MainTex("Main Tex", 2D) = "white"{} // 纹理贴图
        _Color("Color", Color) = (1,1,1,1)   // 控制纹理贴图的颜色
        _NormalMap("Normal Map", 2D) = "bump"{} // 表示当该位置没有指定任何法线贴图时，就使用模型顶点自带的法线
        _BumpScale("Bump Scale", Float) = 1  // 法线贴图的凹凸参数。为0表示使用模型原来的发现，为1表示使用法线贴图中的值。大于1则凹凸程度更大。
    }
    SubShader{
        Pass {
            // 只有定义了正确的LightMode才能得到一些Unity的内置光照变量
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM

// 包含unity的内置的文件，才可以使用Unity内置的一些变量
#include "Lighting.cginc" // 取得第一个直射光的颜色_LightColor0 第一个直射光的位置_WorldSpaceLightPos0（即方向）
#pragma vertex vert
#pragma fragment frag
 
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST; // 命名是固定的贴图名+后缀"_ST"，4个值前两个xy表示缩放，后两个zw表示偏移
            sampler2D _NormalMap;
            float4 _NormalMap_ST; // 命名是固定的贴图名+后缀"_ST"，4个值前两个xy表示缩放，后两个zw表示偏移
            float _BumpScale;    

            struct a2v
            {
                float4 vertex : POSITION;    // 告诉Unity把模型空间下的顶点坐标填充给vertex属性
                float3 normal : NORMAL;        // 不再使用模型自带的法线。保留该变量是因为切线空间是通过（模型里的）法线和（模型里的）切线确定的。
                float4 tangent : TANGENT;    // tangent.w用来确定切线空间中坐标轴的方向的。
                float4 texcoord : TEXCOORD0; 
            };

            struct v2f
            {
                float4 position : SV_POSITION; // 声明用来存储顶点在裁剪空间下的坐标
                //float3 worldNormal : TEXCOORD0;  // 不再使用世界空间下的法线方向
                float3 lightDir : TEXCOORD0;   // 切线空间下，平行光的方向
                float3 worldVertex : TEXCOORD1;
                float4 uv : TEXCOORD2; // xy存储MainTex的纹理坐标，zw存储NormalMap的纹理坐标
            };

            // 计算顶点坐标从模型坐标系转换到裁剪面坐标系
            v2f vert(a2v v)
            {
                v2f f;
                f.position = UnityObjectToClipPos(v.vertex); // UNITY_MATRIX_MVP是内置矩阵。该步骤用来把一个坐标从模型空间转换到剪裁空间
                
                // 法线方向。把法线方向从模型空间转换到世界空间
                //f.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject); // 反过来相乘就是从模型到世界，否则是从世界到模型
                f.worldVertex = mul(v.vertex, unity_WorldToObject).xyz;
                
                //f.uv = v.texcoord.xy; // 不使用缩放和偏移
                f.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw; // 贴图的纹理坐标
                f.uv.zw = v.texcoord.xy * _NormalMap_ST.xy + _NormalMap_ST.zw; // 法线贴图的纹理坐标

                TANGENT_SPACE_ROTATION; // 调用这个宏会得到一个矩阵rotation，该矩阵用来把模型空间下的方向转换为切线空间下

                //ObjSpaceLightDir(v.vertex); // 得到模型空间下的平行光方向
                f.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)); // 切线空间下，平行光的方向

                return f;
            }

            // 要把所有跟法线方向有关的运算，都放到切线空间下。因为从法线贴图中取得的法线方向是在切线空间下的。
            fixed4 frag(v2f f) : SV_Target 
            {
                // 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
                
                // 法线方向。从法线贴图中获取。法线贴图的颜色值 --> 法线方向
                //fixed3 normalDir = normalize(f.worldNormal);   // 不再使用模型自带的法线
                fixed4 normalColor = tex2D(_NormalMap, f.uv.zw); // 在法线贴图中的颜色值
                //fixed3 tangentNormal = normalize(normalColor.xyz * 2 - 1); // 切线空间下的法线方向，发现计算得到的法线不正确！
                fixed3 tangentNormal = UnpackNormal(normalColor); // 使用Unity内置的方法，从颜色值得到法线在切线空间的方向
                tangentNormal.xy = tangentNormal.xy * _BumpScale; // 控制凹凸程度
                tangentNormal = normalize(tangentNormal);

                // 光照方向。
                fixed3 lightDir = normalize(f.lightDir); // 切线空间下的光照方向
                
                // 纹理坐标对应的纹理图片上的点的颜色
                fixed3 texColor = tex2D(_MainTex, f.uv.xy) * _Color.rgb;
                
                // 漫反射Diffuse颜色 = 直射光颜色 * max(0, cos(光源方向和法线方向夹角)) * 材质自身色彩（纹理对应位置的点的颜色）
                fixed3 diffuse = _LightColor0 * max(0, dot(tangentNormal, lightDir)) * texColor; // 颜色融合用乘法
            
                // 最终颜色 = 漫反射 + 环境光 
                fixed3 tempColor = diffuse + ambient * texColor; // 让环境光也跟纹理颜色做融合，防止环境光使得纹理效果看起来朦胧

                return fixed4(tempColor, 1); // tempColor是float3已经包含了三个数值
            }

            ENDCG
        }
        
    }
    FallBack "Diffuse"
}