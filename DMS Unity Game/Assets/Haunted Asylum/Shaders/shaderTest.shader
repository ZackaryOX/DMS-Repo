// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/shaderTest"
{
	//Variables
	Properties{
		_MainTexture("Main Color (RGB)", 2D) = "white"{}
		_Color2("ColorPicker", Color) = (1,1,1,1)	
	}
		SubShader{
		Pass{
		CGPROGRAM
		#pragma vertex vertexFunction
		#pragma fragment fragmentFunction
		#include "UnityCG.cginc"
		//Vertex
		//Normal
		//Color
		//uv

		//Vec2 == float2
		//vec3 == float3
		//Vec4 == float4
		struct appdata {
		float4 vertex : POSITION;
		float2 uv :	TEXCOORD0;
		};

	struct v2f {
		float4 position : SV_POSITION; //SV_POSITON for directX
		float2 uv : TEXCOORD0;
	};
	//v2f vertex to fragment function sending it to screen

	//Bring in shader lab values
	sampler2D _MainTexture;


	//Use uniforms to edit values outside of shader (eg scripts)
	uniform float4 _Color2;

	//build object
	v2f vertexFunction(appdata IN) {
		v2f OUT;

		OUT.position = UnityObjectToClipPos(IN.vertex);
		OUT.uv = IN.uv;

		return OUT;
	}

	//draw pixels to screen 
	fixed4 fragmentFunction(v2f IN) : SV_Target{ //SV_Target is to target it to screenspace
		float4 textureColor = tex2D(_MainTexture, IN.uv);
		return textureColor * _Color2;
	}

		//Fragment

		ENDCG
	}
		}
}