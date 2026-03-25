Shader "Custom/XRaySpecialCoin"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Normal Color", Color) = (0, 0, 1, 1)
        
        [HDR] _XRayColor ("X-Ray Color", Color) = (0, 0.5, 1, 1) 
        // --- 新增：手动亮度控制，范围 1 到 20 ---
        _XRayIntensity ("X-Ray Brightness", Range(1, 20)) = 5.0

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" }

        // --- 第一步：渲染被遮挡的部分 ---
        Pass
        {
            Name "XRAY"
            ZTest Greater      
            ZWrite Off         
            Blend SrcAlpha One 
            Lighting Off       

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
            };

            fixed4 _XRayColor;
            float _XRayIntensity; // 声明新增的变量

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 baseColor = _XRayColor;

                // 呼吸效果逻辑保持不变
                float pulse = sin(_Time.y * 3.0) * 0.15 + 0.85; 

                // --- 使用新增的 _XRayIntensity 变量来暴力加亮 ---
                fixed4 finalColor = baseColor * pulse * _XRayIntensity; 

                finalColor.a = baseColor.a;
                return finalColor;
            }
            ENDCG
        }

        // --- 第二步：正常渲染 ---
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}