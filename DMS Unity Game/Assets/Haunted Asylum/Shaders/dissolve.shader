Shader "Custom/dissolve"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NoiseMap("DissolveMap.", 2D) = "white" {}
		_Amount("Amount", Range(0,1)) = 1
		_Ramp("TextureRamp", 2D) = "white" {}
		_disColor("dissolveColor", Color) = (1,1,1,1)
			//_colorSize("Color Amount", float) = 10
		_glowAmount("Glow Amount", Range(0,2)) = 0
		_vertAmount("vertAmount", Range(-1,1)) = -1
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 0.5
		_Radius("Radius", Range(0,10)) = 10.0
			//_VertexPosition("VP", 2D) = "white"{};
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
			//LOD 200
			//Cull OFF
			//Zwrite OFF

		CGPROGRAM
		#pragma surface surf Standard vertex:vert fullforwardshadows alpha:fade

			// Physically based Standard lighting model, and enable shadows on all light types
			//#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0



			sampler2D _MainTex;

		sampler2D _NoiseMap;
		sampler2D _Ramp;
		float _Amount;
		float _colorSize = 0.0f;
		float _glowAmount;
		float _vertAmount;
		float4 _disColor;
		float _RimPower;
		float4 _RimColor;
		float _Radius;
		sampler2D _VertexPosition;
		uniform float _vertsXYZ[3];




			struct Input
			{
				float2 uv_MainTex;
				float3 viewDir;
			};

			float random(float2 st) {
				return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
			}


			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void vert(inout appdata_full v) {
				//float t = pow(min(cos(3.14 * v.vertex.x / 2.0), 1.0 - abs(v.vertex.x)), 3.0);
				//float3 vertNoise = tex2D(_NoiseMap, IN.uv_MainTex).rgb

				for (int i = 0; i <= 1; i++) {
					_vertsXYZ[0] = v.vertex.x;
					_vertsXYZ[1] = v.vertex.y;
					_vertsXYZ[2] = v.vertex.z;


				}
				v.vertex.xyz = lerp(-normalize(v.normal), float3(_vertsXYZ[0], _vertsXYZ[1], _vertsXYZ[2]), _Amount + 2 - 1);

				// uncomment this if above doesn't work
				//v.vertex.xyz = lerp(-normalize(v.normal) * random(v.vertex.xy), v.vertex.xyz, _Amount + 2 - 1);




				//v.vertex.xyz = lerp(-normalize(v.normal) * tex2Dlod(_VertexPosition, float4(IN.uv_MainTex, 0.0, 0.0)), v.vertex.xyz, _Amount + 2 - 1);



				//v.vertex.xyz += v.normal * _vertAmount;
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				float dissolve = tex2D(_NoiseMap, IN.uv_MainTex).rgb;
				clip(dissolve - _Amount);
				float4 rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
				float _newAmount = (((_Amount - 0) * (2 - 0)) / (1 - 0));


				if (_Amount > 0.0f) {
					o.Emission += lerp(dissolve,tex2D(_Ramp, distance(IN.uv_MainTex, float2(_Amount + 2 - 1, _Amount + 2 - 1))).rgb * _newAmount, _Amount + 2 - 1);

				}
				o.Emission += (_RimColor.rgb * pow(rim, _RimPower + (_Amount * _RimPower)));

				//o.Alpha = tex2D(_Ramp, float2(_Radius * cos(1.5 * _Amount), _Radius * sin(1.5 * _Amount))).rgb;

			//o.Alpha += _Amount;
				//o.Emission = 
			//}
			o.Albedo = c.rgb;
			o.Alpha += c.a;

			//if (_Amount >= 0.5) {


			//}
		}
		ENDCG


				Tags{ "RenderType" = "Transparent" }
			//LOD 200
			//Cull OFF
			//Zwrite OFF

			CGPROGRAM
			#pragma surface surf Standard vertex:vert fullforwardshadows

			// Physically based Standard lighting model, and enable shadows on all light types
			//#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0



			sampler2D _MainTex;

		sampler2D _NoiseMap;
		sampler2D _Ramp;
		float _Amount;
		float _colorSize = 0.0f;
		float _glowAmount;
		float _vertAmount;
		float4 _disColor;
		float _RimPower;
		float4 _RimColor;
		float _Radius;
		sampler2D _VertexPosition;

		float random(float2 st) {
			return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
		}


		struct Input
		{
			float2 uv_MainTex;
			float3 viewDir;
		};


		void vert(inout appdata_full v) {
			//float t = pow(min(cos(3.14 * v.vertex.x / 2.0), 1.0 - abs(v.vertex.x)), 3.0);
			//float3 vertNoise = tex2D(_NoiseMap, IN.uv_MainTex).rgb


			v.vertex.xyz = lerp(normalize(v.normal) * -distance(v.vertex.xyz, float3(0.1, 0.1, 0.1)), v.vertex.xyz, _Amount + 2 - 1);
			//v.vertex.xyz = lerp(normalize(v.normal) * random(v.vertex.xy), v.vertex.xyz, -_Amount + 2 - 1);

			//v.vertex.xyz = lerp(-normalize(v.normal) * tex2Dlod(_VertexPosition, float4(IN.uv_MainTex, 0.0, 0.0)), v.vertex.xyz, _Amount + 2 - 1);



			//v.vertex.xyz += v.normal * _vertAmount;
		}

		fixed4 _Color;


		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color

				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				float dissolve = tex2D(_NoiseMap, distance(IN.uv_MainTex, float2(cos(_Amount + 2 - 1), sin(_Amount + 2 - 1)))).rgb;
				clip(dissolve - _Amount);
				float4 rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));

				float _newAmount = (((_Amount - 0) * (2 - 0)) / (1 - 0));
				if (_Amount > 0.0f) {
					o.Emission += tex2D(_Ramp, distance(IN.uv_MainTex, float2(_Amount + 2 - 1, _Amount + 2 - 1))).rgb * _newAmount;
				}
				o.Emission += (_RimColor.rgb * pow(rim, _RimPower + (_Amount * _RimPower)));

				//o.Alpha = tex2D(_Ramp, float2(_Radius * cos(1.5 * _Amount), _Radius * sin(1.5 * _Amount))).rgb;

			//o.Alpha += _Amount;
				//o.Emission = 
			//}
				o.Albedo = c.rgb;
				o.Alpha += c.a;
			}
		ENDCG
		}
			FallBack "Diffuse"
}

