Shader "Universal Render Pipeline/NiloCat Extension/Screen Space Decal/Unlit" {
	Properties {
		[Header(Basic)] _MainTex ("Texture", 2D) = "white" {}
		[HDR] _Color ("_Color (default = 1,1,1,1)", Vector) = (1,1,1,1)
		[Header(Blending)] [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("_SrcBlend (default = SrcAlpha)", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("_DstBlend (default = OneMinusSrcAlpha)", Float) = 10
		[Header(Alpha remap(extra alpha control))] _AlphaRemap ("_AlphaRemap (default = 1,0,0,0) _____alpha will first mul x, then add y    (zw unused)", Vector) = (1,0,0,0)
		[Header(Prevent Side Stretching(Compare projection direction with scene normal and Discard if needed))] [Toggle(_ProjectionAngleDiscardEnable)] _ProjectionAngleDiscardEnable ("_ProjectionAngleDiscardEnable (default = off)", Float) = 0
		_ProjectionAngleDiscardThreshold ("_ProjectionAngleDiscardThreshold (default = 0)", Range(-1, 1)) = 0
		[Header(Mul alpha to rgb)] [Toggle] _MulAlphaToRGB ("_MulAlphaToRGB (default = off)", Float) = 0
		[Header(Ignore texture wrap mode setting)] [Toggle(_FracUVEnable)] _FracUVEnable ("_FracUVEnable (default = off)", Float) = 0
		[Header(Stencil Masking)] _StencilRef ("_StencilRef", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("_StencilComp (default = Disable) _____Set to NotEqual if you want to mask by specific _StencilRef value, else set to Disable", Float) = 0
		[Header(ZTest)] [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("_ZTest (default = Disable) _____to improve GPU performance, Set to LessEqual if camera never goes into cube volume, else set to Disable", Float) = 0
		[Header(Cull)] [Enum(UnityEngine.Rendering.CullMode)] _Cull ("_Cull (default = Front) _____to improve GPU performance, Set to Back if camera never goes into cube volume, else set to Front", Float) = 1
		[Header(Unity Fog)] [Toggle(_UnityFogEnable)] _UnityFogEnable ("_UnityFogEnable (default = on)", Float) = 1
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
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy) * _Color;
			}

			ENDHLSL
		}
	}
}