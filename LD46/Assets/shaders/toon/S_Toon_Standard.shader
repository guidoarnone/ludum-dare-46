Shader "Toon/standard" {

	Properties {
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_LightingRamp("Lighting ramp texture", 2D) = "white" {}
		_MainTex("Main Texture", 2D) = "white" {}
		_BumpTex("Normal Texture", 2D) = "bump" {}
		_BumpStrength("Normal Strength", Range(0, 1)) = 1
		_RMATex("RMA Texture", 2D) = "bump" {}

		_SpecularRamp("Specular ramp texture", 2D) = "black" {}
		_SpecularHigh("Specular Upper Bound", float) = 1
		_SpecularLow("Specular Lower Bound", float) = 0
		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		_Glossiness("Glossiness", Float) = 15

		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
	}

	SubShader {

		Pass {
             Name "ShadowCaster"
             Tags { "LightMode" = "ShadowCaster" }
           
             Fog { Mode Off }
             ZWrite On ZTest LEqual Cull Back
             Offset 1, 1
             
             CGPROGRAM
 
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_shadowcaster
             #pragma fragmentoption ARB_precision_hint_fastest
             
             #include "UnityCG.cginc"
 
             struct v2f { 
                 V2F_SHADOW_CASTER;
             };
           
             v2f vert(appdata_base v) {
                 v2f o;
                 TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                 return o;
             }
           
             float4 frag(v2f i) : COLOR {
                 SHADOW_CASTER_FRAGMENT(i)
             }
 
             ENDCG
        }

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float3 nor : NORMAL;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
				float3 tspace0 : TEXCOORD2; // tangent.x, bitangent.x, normal.x
                float3 tspace1 : TEXCOORD3; // tangent.y, bitangent.y, normal.y
                float3 tspace2 : TEXCOORD4; // tangent.z, bitangent.z, normal.z
				float3 worldRefl : TEXCOORD5;
				SHADOW_COORDS(6)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpTex;
			float4 _BumpTex_ST;
			float _BumpStrength;
			sampler2D _RMATex;
			float4 _RMATex_ST;
			sampler2D _LightingRamp;
			float4 _AmbientColor;
			
			sampler2D _SpecularRamp;
			float4 _SpecularColor;
			float _Glossiness;
			float _SpecularHigh;
			float _SpecularLow;

			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;

			v2f vert (float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0) {
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				half3 wNormal = UnityObjectToWorldNormal(normal);
				o.nor =  wNormal;
				o.viewDir = WorldSpaceViewDir(vertex);
				o.uv = TRANSFORM_TEX(uv, _MainTex);

                half3 wTangent = UnityObjectToWorldDir(tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

				o.worldRefl = reflect(-o.viewDir, wNormal);
				
				TRANSFER_SHADOW(o)
				return o;
			}
			
			float4 _Color;

			float4 frag (v2f i) : SV_Target {

				float3 viewDir = normalize(i.viewDir);
				half3 tnormal = UnpackNormal(tex2D(_BumpTex, i.uv));
				half3 wnormal = half3(i.tspace0.z, i.tspace1.z, i.tspace2.z);
				half3 normal;
                normal.x = dot(i.tspace0, tnormal);
                normal.y = dot(i.tspace1, tnormal);
                normal.z = dot(i.tspace2, tnormal);
				normal = normalize(lerp(wnormal, normal, _BumpStrength));
				
				float4 color = tex2D(_MainTex, i.uv);
				float4 rma = tex2D(_RMATex, i.uv);

				float NdotL = dot(_WorldSpaceLightPos0, normal);
				float2 diff_uv = float2(1 - (NdotL * 0.5 + 0.5), 0.5);

				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);
				float2 spec_uv = float2((NdotH * 0.5 + 0.5), 0.5);

				float4 rimDot = 1 - dot(viewDir, normal);

				float shadow = SHADOW_ATTENUATION(i);

				half4 probeData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.worldRefl);
                half3 probeColor = DecodeHDR (probeData, unity_SpecCube0_HDR);

				// --- Lighting ---
				// - Ramp lighting -
				float2 ramp_uv = float2((NdotL * 0.5 + 0.5), 0.5);
				float lightIntensity = tex2D(_LightingRamp, ramp_uv) * shadow;
				// - Reflections -
				half3 reflection = max(probeColor, tex2D(_SpecularRamp, spec_uv) * lightIntensity) * NdotH;
				lightIntensity = lerp(lightIntensity + reflection * pow(1-rma.r, 3), reflection, rma.g * (1-rma.r));
				// - AO -
				lightIntensity *= rma.b;

				// --- Specular ---
				float specularIntensity = (pow(NdotH * lightIntensity, _Glossiness * _Glossiness));
				float specularIntensitySmooth = smoothstep(_SpecularLow / 100, _SpecularHigh / 100, specularIntensity);
				// - Metal/Highlights -

				// --- Rim ---
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity) * shadow;

				// --- Light ---
				float4 light = lightIntensity * _LightColor0;
				float4 specular = specularIntensitySmooth * _SpecularColor * lightIntensity * _LightColor0;
				float4 rim = rimIntensity * _RimColor * _LightColor0;
				
				//return shadow;
				return color * (_AmbientColor + light + specular + rim) * _Color;
			}
			ENDCG
		}
		//UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    FallBack Off
}