Shader "Shader Graphs/Water" {
	Properties {
		Vector1_98139E9E ("Metallic", Float) = 0
		Vector1_56039FC6 ("Smoothness", Float) = 0
		[HDR] Color_875C0508 ("BaseColor", Vector) = (0,0.3803514,0.6792453,0)
		[HDR] Color_BAB18E18 ("RippleColor", Vector) = (0,0.7253118,1,0)
		Vector1_824CE7A3 ("RippleSpeed", Float) = 0.75
		Vector1_E6FF0CE3 ("RippleScale", Float) = 5
		Vector1_9199F095 ("RippleDissolve", Float) = 5
		Vector1_45FE81DD ("Alpha", Float) = 1
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