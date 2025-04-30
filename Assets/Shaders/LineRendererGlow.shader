Shader "Custom/LineRendererGlow"
{
Properties
{
_Color ("Color", Color) = (1,1,1,1)
_EmissionColor ("Emission Color", Color) = (1,1,1,1)
_GlowIntensity ("Glow Intensity", Range(0, 10)) = 1.0
_MainTex ("Texture", 2D) = "white" {}
_Alpha ("Alpha", Range(0, 1)) = 1.0
}
SubShader
{
Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
LOD 100
Blend SrcAlpha OneMinusSrcAlpha
ZWrite Off

Pass
{
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
float4 positionOS : POSITION;
float2 uv : TEXCOORD0;
};

struct Varyings
{
float4 positionCS : SV_POSITION;
float2 uv : TEXCOORD0;
float fogCoord : TEXCOORD1;
};

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);
CBUFFER_START(UnityPerMaterial)
float4 _Color;
float4 _EmissionColor;
float _GlowIntensity;
float _Alpha;
float4 _MainTex_ST;
CBUFFER_END

Varyings vert (Attributes input)
{
Varyings output;
output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
output.uv = TRANSFORM_TEX(input.uv, _MainTex);
output.fogCoord = ComputeFogFactor(output.positionCS.z);
return output;
}

half4 frag (Varyings input) : SV_Target
{
half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
half4 color = texColor * _Color;

// Emission
half3 emission = _EmissionColor.rgb * _GlowIntensity * color.rgb;

// Combine color and emission
half3 finalColor = color.rgb + emission;

// Alpha control: transparent when not glowing
half alpha = _Alpha * color.a * (_GlowIntensity > 0 ? 1 : 0.1);

half4 output = half4(finalColor, alpha);
output.rgb = MixFog(output.rgb, input.fogCoord);
return output;
}
ENDHLSL
}
}
FallBack "Universal Render Pipeline/Lit"
}