Shader "Shader Graphs/NeonSign" {
	Properties {
		Color_E5106B50 ("BaseColor", Vector) = (0.5717649,0.7830189,0.5207815,0)
		[NoScaleOffset] Texture2D_F2C6710C ("Texture", 2D) = "white" {}
		Vector2_ED39139F ("Texture Tiling", Vector) = (1,0.12,0,0)
		Vector1_4FB1BB09 ("Scroll Speed", Float) = -0.0075
		Vector1_9E5E967E ("AlphaClip", Float) = 0
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