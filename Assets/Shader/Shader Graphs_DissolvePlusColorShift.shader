Shader "Shader Graphs/DissolvePlusColorShift" {
	Properties {
		_EdgeColor ("Edge Color", Vector) = (1.584314,0.2980392,1.145098,1)
		_EdgeWidth ("Edge Width", Float) = 0.01
		_NoiseScale ("Noise Scale", Float) = 30
		[NoScaleOffset] _Albedo ("Albedo", 2D) = "white" {}
		_DissolveValue ("DissolveValue", Range(0, 1)) = 1
		_Metallic ("Metallic", Range(0, 1)) = 0.5
		_Smoothness ("Smoothness", Range(0, 1)) = 0
		[ToggleUI] _EnableGlow ("Enable Glow", Float) = 0
		_GlowEffect ("GlowEffect", Float) = 1
		_TeamColor ("TeamColor", Vector) = (1,0.01827145,0,1)
		[ToggleUI] _EnableBehindEffect ("EnableBehindEffect", Float) = 0
		Vector1_194CD650 ("Frequency", Float) = 0.5
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
}