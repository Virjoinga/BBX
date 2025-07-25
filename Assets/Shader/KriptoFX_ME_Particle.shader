Shader "KriptoFX/ME/Particle" {
	Properties {
		[Header(Main Settings)] [Space] [PerRendererData] [HDR] _TintColor ("Tint Color", Vector) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		[Header(Fading)] [Space] [Toggle(_FADING_ON)] _UseSoft ("Use Soft Particles", Float) = 0
		_InvFade ("Soft Particles Factor", Float) = 1
		_SoftInverted ("Inverted Soft Particles", Range(0, 1)) = 0
		[Toggle(USE_FRESNEL_FADING)] _UseFresnelFading ("Use Fresnel Fading", Float) = 0
		_FresnelFadeFactor ("Fresnel Fade Factor", Float) = 3
		[Space] [Header(Light)] [Toggle(USE_VERTEX_LIGHT)] _UseVertexLight ("Use Vertex Lighting", Float) = 0
		[Space] [Header(Noise Distortion)] [Toggle(USE_NOISE_DISTORTION)] _UseNoiseDistortion ("Use Noise Distortion", Float) = 0
		_NoiseTex ("Noise Texture (RG)", 2D) = "black" {}
		_DistortionSpeedScale ("Speed (XY) Scale(XY)", Vector) = (1,-1,0.15,0.15)
		_UseVertexStreamRandom ("Use Vertex Stream Random", Float) = 0
		[Space] [Header(Fresnel)] [Toggle(USE_FRESNEL)] _UseFresnel ("Use Fresnel", Float) = 0
		[HDR] _FresnelColor ("Fresnel Color", Vector) = (1,1,1,1)
		_FresnelPow ("Fresnel Pow", Float) = 5
		_FresnelR0 ("Fresnel R0", Float) = 0.04
		[Space] [Header(Cutout)] [Toggle(USE_CUTOUT)] _UseCutout ("Use Cutout", Float) = 0
		[PerRendererData] _Cutout ("Cutout", Range(0, 1)) = 1
		_UseSoftCutout ("Use Soft Cutout", Float) = 0
		_UseParticlesAlphaCutout ("Use Particles Alpha", Float) = 0
		[Toggle(USE_CUTOUT_TEX)] _UseCutoutTex ("Use Cutout Texture", Float) = 0
		_CutoutTex ("Cutout Tex", 2D) = "white" {}
		[Toggle(USE_CUTOUT_THRESHOLD)] _UseCutoutThreshold ("Use Cutout Threshold", Float) = 0
		[HDR] _CutoutColor ("Cutout Color", Vector) = (1,1,1,1)
		_CutoutThreshold ("Cutout Threshold", Range(0, 1)) = 0.015
		[Space] [Header(Rendering)] [Toggle(_FLIPBOOK_BLENDING)] _UseFrameBlending ("Use Frame Blending", Float) = 0
		[Toggle] _ZWriteMode ("ZWrite On?", Float) = 0
		[Enum(Cull Off,0, Cull Front,1, Cull Back,2)] _CullMode ("Culling", Float) = 0
		[KeywordEnum(Add, Blend, Mul)] _BlendMode ("Blend Mode", Float) = 1
		_SrcMode ("SrcMode", Float) = 5
		_DstMode ("DstMode", Float) = 10
		_FogColorMultiplier ("Fog Color Multiplier", Vector) = (0,0,0,0)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy);
			}

			ENDHLSL
		}
	}
	//CustomEditor "ME_UberParticleGUI"
}