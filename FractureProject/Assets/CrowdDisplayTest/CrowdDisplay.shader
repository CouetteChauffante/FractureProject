Shader "Custom/CrowdDisplay"
{
    Properties
    {
        _MainTex ("Sprite Atlas Texture", 2D) = "white" {}
        _PointA ("Point A", Vector) = (-10, 0, 0, 0)
        _PointB ("Point B", Vector) = (10, 0, 0, 0)
        _Scale ("Taille (X=Largeur, Y=Hauteur)", Vector) = (1, 1, 0, 0)
        _Width ("Largeur de la foule", Float) = 2.0
        _BounceSpeed ("Bounce Speed", Float) = 5
        _BounceAmp ("Bounce Amplitude", Float) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZWrite On

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct CharacterData {
                float3 randomOffset;
                float initialProgress;
                float4 uvRect;
            };

            StructuredBuffer<CharacterData> _CrowdBuffer;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float alpha : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _PointA;
                float4 _PointB;
                float4 _Scale;
                float _Width;
                float _GlobalOffset;
                float _BounceSpeed;
                float _BounceAmp;
            CBUFFER_END

            Varyings vert(Attributes input, uint instanceID : SV_InstanceID)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                CharacterData data = _CrowdBuffer[instanceID];

                float progress = frac(data.initialProgress + _GlobalOffset);
                float3 basePos = lerp(_PointA.xyz, _PointB.xyz, progress);
                
                float3 dir = normalize(_PointB.xyz - _PointA.xyz);
                float3 up = float3(0, 1, 0);
                float3 sideDir = normalize(cross(dir, up));
                
                if(length(sideDir) < 0.01) sideDir = float3(1, 0, 0);

                float3 sideOffset = sideDir * (data.randomOffset.x * _Width);
                float3 worldPos = basePos + sideOffset;

                float bounce = abs(sin(_Time.y * _BounceSpeed + data.randomOffset.y)) * _BounceAmp;
                worldPos.y += bounce;

                float3 scaledPositionOS = input.positionOS.xyz * _Scale.xyz;
                float3 finalWorldPos = worldPos + scaledPositionOS; 
                output.positionCS = TransformWorldToHClip(finalWorldPos);

                float4 uvRect = data.uvRect;
                output.uv = input.uv * uvRect.zw + uvRect.xy;
                
                output.alpha = smoothstep(0.0, 0.05, progress) * (1.0 - smoothstep(0.95, 1.0, progress));

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                col.a *= input.alpha;
                clip(col.a - 0.1); 
                return col;
            }
            ENDHLSL
        }
    }
}