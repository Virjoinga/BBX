Shader "Example Island/Shader_IslandWater" {
	Properties {
		Vector1_F976D8BD ("Smoothness", Range(0, 1)) = 0.5
		Vector1_7FB5584F ("Metalness", Range(0, 1)) = 0.5
		Color_3CACD0BA ("Bottom Dark Colour", Vector) = (0.2196079,0.3529412,0.509804,1)
		Color_E63B4CEF ("Bottom Light Colour", Vector) = (0.145098,0.9490197,1,1)
		Color_936A8730 ("Top Dark Colour", Vector) = (0.1450979,0.9490196,1,1)
		Color_D70B9788 ("Top Light Colour", Vector) = (0.145098,0.9490197,1,1)
		Vector1_AD7DFD35 ("Top Colour Contrast", Float) = 0
		Vector1_4B305923 ("Smaller Wave/Scale", Float) = 5.65
		Vector1_BF07B4C4 ("Smaller Wave/Speed", Float) = 1.9
		Vector1_1AE1D4CB ("Smaller Wave/Height", Float) = 0.133
		Vector1_8BF7F1BF ("Smaller Wave/Power", Float) = 1
		Vector1_39222957 ("Smaller Wave/Rotate X", Range(0, 1)) = 0.76
		Vector1_CC89A15C ("Smaller Wave/Rotate Z", Range(0, 1)) = 1
		Vector1_5EB91206 ("Medium Wave/Colour Blend", Float) = 0.56
		Vector1_BD72615D ("Medium Wave/Scale", Float) = 2.6
		Vector1_BC8BA90A ("Medium Wave/Speed", Float) = 2.12
		Vector1_24DAD7F ("Medium Wave/Height", Float) = 0.1
		Vector1_C6051FE9 ("Medium Wave/Power", Float) = 1.67
		Vector1_905986FC ("Medium Wave/Rotate X", Range(0, 1)) = 1
		Vector1_51075417 ("Medium Wave/Rotate Z", Range(0, 1)) = 0.63
		Vector1_5232C9B8 ("Bigger Wave/Scale", Float) = 1
		Vector1_3D495A1F ("Bigger Wave/Speed", Float) = 0.9
		Vector1_1E58CA3C ("Bigger Wave/Height", Float) = 0.3
		Vector1_B42C4654 ("Bigger Wave/Power", Float) = 0.97
		Vector1_5190E028 ("Bigger Wave/Rotate X", Range(0, 1)) = 0.37
		Vector1_F4A73BA ("Bigger Wave/Rotate Z", Range(0, 1)) = 0.77
		Vector1_722CB011 ("Refraction Strength", Range(0, 0.3)) = 0
		Vector1_6DA97D7 ("Refraction Scale", Float) = 35
		Vector1_FABC47E4 ("Refraction Blend", Range(0, 1)) = 0.5
		Vector1_39047021 ("Gradient Scale", Float) = 0.5
		Vector2_B86025B3 ("Gradient Min Max", Vector) = (0.5,1,0,0)
		Vector2_D494475D ("Gradient Speed", Vector) = (0.2,0.2,0,0)
		Vector1_489F9EEF ("Fresnel Power", Float) = 3
		Vector1_9E5F763C ("Froth Blend", Range(0, 1)) = 1
		Vector1_B98A4C60 ("Froth Speed", Float) = 0
		Vector1_F0FA9185 ("Froth Scale Big", Float) = 6
		Vector1_986CF1D2 ("Froth Scale Small", Float) = 14
		Vector1_A38692A6 ("Froth Step", Range(0, 1)) = 0.6
		Vector1_D42CC29E ("Froth Falloff", Float) = 2
		Vector1_F2029A8E ("Froth Flatness", Range(0, 1)) = 1
		Vector1_4F92311A ("Froth Flatness Falloff", Float) = 1
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