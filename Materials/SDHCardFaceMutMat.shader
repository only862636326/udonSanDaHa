Shader "Custom/SDH_Face" {
    Properties {
        _Time_Count("计时", Vector) = (0,0,0,0)

        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Image Sequence", 2D) = "white" {}

        _MaskTex ("Mask Texture", 2D) = "white" {}
        
        // 新增：背面纹理属性
        _BackTex ("Back Image", 2D) = "white" {}
        _BackColor ("Back Color", Color) = (1,1,1,1)
        
        // 合并主图参数（四维向量：Row, Col, Rows, Columns）
        _MainParams ("Main Params (Row, Col, Rows, Columns)", Vector) = (0, 0, 6, 6)
        
        // 图标大图纹理及行列参数
        _IconAtlasTex ("Icon Atlas Texture", 2D) = "white" {}
        _IconGrid ("Icon Grid (Rows, Columns)", Vector) = (5, 5, 0,0)

        // 图标1参数（合并后：Row, Col, Alpha, 0）
        _Icon1Meta ("Icon 1 Meta (Row, Col, Alpha, 0)", Vector) = (0, 0, 1.0, 0)
        // 图标1的位置和缩放（合并后：PosX, PosY, ScaleX, ScaleY）
        _Icon1Transform ("Icon 1 Transform (PosX, PosY, ScaleX, ScaleY)", Vector) = (0.07, 0.99,0.2, 0.2)
        
        // 图标2参数（合并后：Row, Col, Alpha, 0）
        _Icon2Meta ("Icon 2 Meta (Row, Col, Alpha, 0)", Vector) = (4, 0, 1.0, 0)
        // 图标2的位置和缩放（合并后：PosX, PosY, ScaleX, ScaleY）
        _Icon2Transform ("Icon 2 Transform (PosX, PosY, ScaleX, ScaleY)", Vector) = (0.11, 0.85,0.1, 0.1)

        _MaskIntensity ("Mask Intensity", Range(0, 1)) = 0.5
        _ColorBlend ("Color Blend", Range(0, 1)) = 0.2
        
        // 背面专用参数
        _BackMaskIntensity ("Back Mask Intensity", Range(0, 1)) = 0.5
        _BackColorBlend ("Back Color Blend", Range(0, 1)) = 0.2
    }
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
        // 正面渲染通道
        Pass {
            Tags { "LightMode"="ForwardBase" "Face"="Front" }

            ZWrite On
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            // 图标参数结构体（优化后）
            struct IconParams {
                float row;      // 图标行索引
                float col;      // 图标列索引
                float2 pos;     // 位置（x:X位置, y:Y位置）
                float2 scale;   // 缩放（x:水平缩放, y:垂直缩放）
                float alpha;    // 整体透明度
            };

            struct a2v {  
                float4 vertex : POSITION; 
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL; // 添加法线信息

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };  
            
            struct v2f {  
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD2; // 传递世界空间法线
                float3 worldPos : TEXCOORD3;    // 传递世界空间位置

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };  

            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _IconAtlasTex;
            float4 _MainTex_ST;

            sampler2D _BackTex; // 新增：背面纹理采样器
            float4 _BackTex_ST;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Time_Count)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(float4, _MainParams)    // 合并后的主图参数
            UNITY_DEFINE_INSTANCED_PROP(float4, _IconGrid)     // 图标行列数
            
            UNITY_DEFINE_INSTANCED_PROP(float4, _Icon1Meta)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Icon1Transform)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Icon2Meta)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Icon2Transform)
            
            UNITY_DEFINE_INSTANCED_PROP(float, _MaskIntensity)
            UNITY_DEFINE_INSTANCED_PROP(float, _ColorBlend)
            
            // 新增：背面纹理相关属性
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _BackColor)
            UNITY_DEFINE_INSTANCED_PROP(float, _BackMaskIntensity)
            UNITY_DEFINE_INSTANCED_PROP(float, _BackColorBlend)
            UNITY_INSTANCING_BUFFER_END(Props)

            // 解析主图参数
            float4 ParseMainParams(float4 params) {
                return params; // x:行索引, y:列索引, z:总行数, w:总列数
            }

            // 解析图标网格参数
            float2 ParseIconGrid(float4 grid) {
                return float2(grid.x, grid.y); // x:行, y:列
            }
            // 计算RGB颜色的灰度值
            float CalculateGrayscale(fixed3 color) {
                return dot(color, float3(0.299, 0.587, 0.114));
            }
            // 解析图标参数（优化后）
            IconParams ParseIconParams(float4 meta, float4 transform) {
                IconParams p;
                p.row = meta.x;         // 行索引
                p.col = meta.y;         // 列索引
                p.pos = transform.xy;   // 位置（xy分量）
                p.scale = transform.zw; // 缩放（zw分量）
                p.alpha = meta.z;       // 透明度（z分量）
                return p;
            }

            v2f vert (a2v v) {  
                v2f o;  
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.pos = UnityObjectToClipPos(v.vertex);  
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);  

                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }  

            fixed4 frag (v2f i) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(i);
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float facing = dot(normalize(i.worldNormal), viewDir);                
                // 根据面朝向选择纹理

                // 获取实例属性
                fixed4 time_count = UNITY_ACCESS_INSTANCED_PROP(Props, _Time_Count);
                fixed4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);	
                float4 mainParams = ParseMainParams(UNITY_ACCESS_INSTANCED_PROP(Props, _MainParams));
                float2 iconGrid = ParseIconGrid(UNITY_ACCESS_INSTANCED_PROP(Props, _IconGrid));
                float maskIntensity = UNITY_ACCESS_INSTANCED_PROP(Props, _MaskIntensity);
                float colorBlend = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorBlend);
                 
                if(facing >= 0) // 正面
                {
                
                    // 提取主图参数
                    float cardRow = mainParams.x;    // 主图行索引
                    float cardCol = mainParams.y;    // 主图列索引
                    float mainRows = mainParams.z;   // 主图总行数
                    float mainCols = mainParams.w;   // 主图总列数

                    // 主图UV计算（使用合并后的参数）
                    half2 mainUV = TRANSFORM_TEX(i.uv, _MainTex);
                    mainUV.x = (mainUV.x + cardCol) / mainCols;
                    mainUV.y = (mainUV.y + cardRow) / mainRows;
                    fixed4 mainTex = tex2D(_MainTex, mainUV);
                

                    // ------------------- 处理图标1 -------------------
                    float4 icon1Meta = UNITY_ACCESS_INSTANCED_PROP(Props, _Icon1Meta);
                    float4 icon1Transform = UNITY_ACCESS_INSTANCED_PROP(Props, _Icon1Transform);
                    IconParams icon1 = ParseIconParams(icon1Meta, icon1Transform);
                
                    // 图标在主图上的位置和缩放
                    half2 icoPos = icon1.pos;
                    half2 icoScaleFactor = icon1.scale;
                    half2 icoLocalUV = i.uv - icoPos;
                    half2 icoScaledUV = icoLocalUV / icoScaleFactor;
                    half2 finalIcoUV = icoScaledUV + icoPos;
                
                    // 图标可见性检查
                    float icoVisible = step(0, finalIcoUV.x) * step(finalIcoUV.x, 1) * 
                                      step(0, finalIcoUV.y) * step(finalIcoUV.y, 1);
                
                    // 图标在大图中的UV计算
                    half2 icoAtlasUV = float2(
                        (icon1.col + finalIcoUV.x) / iconGrid.y,  // 使用iconGrid.y(列数)
                        (icon1.row + finalIcoUV.y) / iconGrid.x   // 使用iconGrid.x(行数)
                    );
                    fixed4 icoTex = tex2D(_IconAtlasTex, icoAtlasUV);
                    icoTex *= 2;
                    icoTex.a *= icoTex.a;
                    // 混合图标1
                    mainTex.rgb = lerp(mainTex.rgb, icoTex.rgb, icoTex.a * icon1.alpha * icoVisible);

                    // ------------------- 处理图标2 -------------------
                    float4 icon2Meta = UNITY_ACCESS_INSTANCED_PROP(Props, _Icon2Meta);
                    float4 icon2Transform = UNITY_ACCESS_INSTANCED_PROP(Props, _Icon2Transform);
                    IconParams icon2 = ParseIconParams(icon2Meta, icon2Transform);
                
                    half2 icoPos2 = icon2.pos;
                    half2 icoScaleFactor2 = icon2.scale;
                    half2 icoLocalUV2 = i.uv - icoPos2;
                    half2 icoScaledUV2 = icoLocalUV2 / icoScaleFactor2;
                    half2 finalIcoUV2 = icoScaledUV2 + icoPos2;
                
                    float icoVisible2 = step(0, finalIcoUV2.x) * step(finalIcoUV2.x, 1) * 
                                       step(0, finalIcoUV2.y) * step(finalIcoUV2.y, 1);
                
                    half2 icoAtlasUV2 = float2(
                        (icon2.col + finalIcoUV2.x) / iconGrid.y,
                        (icon2.row + finalIcoUV2.y) / iconGrid.x
                    );
                    fixed4 icoTex2 = tex2D(_IconAtlasTex, icoAtlasUV2);                
                    // 混合图标2
                    mainTex.rgb = lerp(mainTex.rgb, icoTex2.rgb, icoTex2.a * icon2.alpha * icoVisible2);

                    // 应用掩码和颜色混合
                    fixed4 mask = tex2D(_MaskTex, i.uv);
                    float m_c = 1 - step(mask.a, 0.5);
                    m_c *= maskIntensity;
                    mainTex.rgb = lerp(mainTex.rgb, mainTex.rgb * color.rgb, colorBlend);
                    mainTex.rgb = lerp(mainTex.rgb, color.rgb, m_c);
                    mainTex.a *= color.a;
                    return mainTex;
                }
                else
                {
                        //        // 直接采样背面纹理并返回
                    fixed4 backTex = tex2D(_BackTex, i.uv);
                    fixed4 backColor = UNITY_ACCESS_INSTANCED_PROP(Props, _BackColor);
                    //fixed4 t_count = UNITY_ACCESS_INSTANCED_PROP(Props, _Time_Count);
                
                    // 仅应用颜色倍增，不进行其他处理
                    backTex.rgb *= backColor.rgb;
                    backTex.a = backColor.a;    
                    return backTex;
                }
            }            
            ENDCG
        }          
    }
    FallBack "Transparent/VertexLit"
}