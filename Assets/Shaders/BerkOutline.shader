// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
Shader "Unlit/BerkOutline"{

Properties 
    {
        _Outline ("_Outline", Range(0,0.1)) = 0
        _OutlineColor ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader 
    {

        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}
        
        Pass {
            Tags { "LightMode"="UniversalForward" }
            Cull Front
 
            HLSLPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 
            struct Attributes
			{
				float4 POSITIONOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct Varyings
			{
				float4 vertex : SV_POSITION;
            };


            CBUFFER_START(UnityPerMaterial)
            float _Outline;
            float4 _OutlineColor;
            CBUFFER_END


                Varyings vert(Attributes v) 
            {
                Varyings o;
                o.vertex = TransformObjectToHClip(v.POSITIONOS.xyz);
                float3 normal = mul((float3x3) UNITY_MATRIX_MV, v.normal);
                normal.x *= UNITY_MATRIX_P[0][0];
                normal.y *= UNITY_MATRIX_P[1][1];
                o.vertex.xy += normal.xy * _Outline;
                return o;
            }
 
            half4 frag(Varyings i) : COLOR 
            {
                return _OutlineColor;
            }
 
            ENDHLSL
        }
    }
 
}