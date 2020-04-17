Shader "Toon/VColor" {
	Properties {
		_Color("Color", Color) = (0.5, 0.65, 1, 1)

		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)

		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		_Glossiness("Glossiness", Float) = 32

		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
	}
	SubShader {
		Pass {
			Tags {
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float3 normal : NORMAL;
				float3 view : TEXCOORD0;
				SHADOW_COORDS(2)
			};

			float4 _Color;
			float4 _AmbientColor;
			
			float4 _SpecularColor;
			float _Glossiness;

			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;

			v2f vert (float4 vertex : POSITION, float4 color : COLOR, float3 normal : NORMAL) {
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.color = color;
				o.normal = UnityObjectToWorldNormal(normal);
				o.view = WorldSpaceViewDir(vertex);

				TRANSFER_SHADOW(o)
				return o;
			}

			float4 frag (v2f i) : SV_Target {

				float3 viewDir = normalize(i.view);
				float3 normal = normalize(i.normal);

				float NdotL = dot(_WorldSpaceLightPos0, normal);

				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);

				float4 rimDot = 1 - dot(viewDir, normal);

				float shadow = SHADOW_ATTENUATION(i);

				// --- Lighting ---
				// - Step lighting -
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);

				// --- Specular ---
				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);

				// --- Rim ---
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);

				// --- Light ---
				float4 light = lightIntensity * _LightColor0;
				float4 specular = specularIntensitySmooth * _SpecularColor * lightIntensity * _LightColor0;
				float4 rim = rimIntensity * _RimColor;

				return i.color * (_AmbientColor + light + specular + rim) * _Color;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}