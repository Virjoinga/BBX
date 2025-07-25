Shader "Shader Graphs/TriPlanar_noNormal" {
	Properties {
		Vector1_D1B7A17F ("Top Tiling", Float) = 1
		[NoScaleOffset] Texture2D_245BB685 ("TopDefuse", 2D) = "white" {}
		Vector1_1F06ECEA ("Front Tiling", Float) = 1
		[NoScaleOffset] Texture2D_6C0CACAB ("FrontDefuse", 2D) = "white" {}
		Vector1_ECD19232 ("Side Tiling", Float) = 1
		[NoScaleOffset] Texture2D_B35F504 ("SideDefuse", 2D) = "white" {}
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