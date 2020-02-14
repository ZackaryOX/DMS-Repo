Shader "Hidden/Shader/PP_Spread"

{

	HLSLINCLUDE

#pragma target 4.5

#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

		struct Attributes

	{

		uint vertexID : SV_VertexID;

		UNITY_VERTEX_INPUT_INSTANCE_ID

	};

	struct Varyings

	{

		float4 positionCS : SV_POSITION;

		float2 texcoord   : TEXCOORD0;

		UNITY_VERTEX_OUTPUT_STEREO

	};

	Varyings Vert(Attributes input)

	{

		Varyings output;

		UNITY_SETUP_INSTANCE_ID(input);

		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);

		output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);

		return output;

	}

	// List of properties to control your post process effect

	float _Intensity;
	float2 _Center;
	float startTime = 0;
	float dis = 0.0f;
	float _maxSize = 0.4;

	float3 worldPos;

	TEXTURE2D_X(_InputTexture);

	TEXTURE2D(_BloodTexture);

	TEXTURE2D(_Noise);
	
	float4x4 _InverseProjection;

	float4x4 _ViewToWorld;

	float2 tile(float2 _uv, float _zoom) { //Function taken from the book of shaders
		_uv *= _zoom;
		return frac(_uv);
	}

	float random(float2 _uv) { //Function taken from the book of shaders
		return frac(sin(dot(_uv.xy,
			float2(12.9898, 78.233)))*
			43758.5453123);
	}


	float4 CustomPostProcess(Varyings input) : SV_Target

	{

		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);




		float depth = LoadCameraDepth(input.positionCS.xy);

		
	

		uint2 positionSS = input.texcoord * _ScreenSize.xy;
		
		
		float4 clipSpace = float4((2 * input.texcoord) - 1.0, depth, 1.0); //clipSpace with depth buffer z
		float4 viewSpace = mul(_InverseProjection, clipSpace); //convert clipSpace to viewSpace
		

	worldPos = mul(_ViewToWorld, viewSpace); //Convert viewSpace to World Space

	float2 _worldUV = worldPos.xy; //using to sample noise texture in world space so it will crawl over the objects.

	_worldUV *= 250; //Controls the scale of our UV. Since our noise texture is small I need to scale it up

	float3 _noiseTex = LOAD_TEXTURE2D(_Noise, _worldUV).xyz;

		

		dis = distance(input.texcoord, float2(0.5, 0.5));

		float3 _tex = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;

		

		float3 _texOrig = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;


		float vignette = smoothstep(0.7, 0.5 - 0.7, dis);

		 
		 

		_tex = saturate(vignette * (vignette));

		_tex.xy = random(sin(_tex.xy * _Time.y) + 2 - 1) * _tex;

		_tex = saturate(_tex * (1 - vignette));

		float3 finalTex = _tex * _texOrig;


		
		//input.texcoord = tile(input.texcoord, 20/11);

		

	

		//positionSS.xy /= (tile((dis) + 2 - 1, 20));

		//positionSS.xy /= (dis) + 2 - 1;

		float radius = lerp(0.1f, 2.0f, _Intensity); //Swap this to time or whatever I want to trigger the effect

		radius -= _noiseTex * 0.5; // += applies the noise texture to the end of the effect so -= applies it to the start of the effect 0.5 determins how big it will be or scale
		
		float worldDis = distance(float2(0.2, 0.2), worldPos); //world dis floats determins where it will be positioned on screen place ghost position and convert it to screenspace

		float mix = worldDis <= radius ? 0 : 1; //if >= then 0 else 1 this lets us mix our textures based on our effects distance from our camera and the center of our camera screen in world space.



		float3 outColor = (1 - mix) * (_tex * _texOrig) + (mix * _texOrig); //Depending on if it returns 0 or 1 we draw our specific texture. eg 1 - 0 = 1 this means we use our mixed texture and avoid using the other one due to it zeroing
																		 //and vise versa

		return float4(outColor, 1.0f);

	}

		ENDHLSL

		SubShader

	{

		Pass

		{

			Name "PP_Spread"

			ZWrite Off

			ZTest Always

			Blend Off

			Cull Off

			HLSLPROGRAM

				#pragma fragment CustomPostProcess

				#pragma vertex Vert

			ENDHLSL

		}

	}

	Fallback Off

}