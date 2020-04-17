Shader "Toon/waves" {

	Properties {
		_WaveA ("Wave A (dir, steepness, wavelength)", Vector) = (1,0,50,10)
		_WaveB ("Wave B (dir, steepness, wavelength)", Vector) = (0,1,25,20)
		_WaveC ("Wave C (dir, steepness, wavelength)", Vector) = (1,1,15,10)

		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}
		_BumpTex("Normal Texture", 2D) = "bump" {}
		_BumpStrength("Normal Strength", Range(0, 1)) = 1
		_Glossiness("Glossiness", Float) = 15

		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
	}

	SubShader {

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

			float4 _WaveA;
			float4 _WaveB;
			float4 _WaveC;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpTex;
			float4 _BumpTex_ST;
			float _BumpStrength;
			sampler2D _LightingRamp;
			float _Glossiness;
			float4 _SpecularColor;

			half4 _AmbientColor;

			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;

			float3 GerstnerWave (float4 wave, float3 p, inout float3 tangent, inout float3 binormal) {
				float steepness = wave.z;
				float wavelength = wave.w;
				float k = 2 * UNITY_PI / wavelength;
				float c = sqrt(9.8 / k);
				float2 d = normalize(wave.xy);
				float f = k * (dot(d, p.xz) - c * _Time.y);
				float a = steepness / k;
			
				//p.x += d.x * (a * cos(f));
				//p.y = a * sin(f);
				//p.z += d.y * (a * cos(f));

				tangent += float3(
					-d.x * d.x * (steepness * sin(f)),
					d.x * (steepness * cos(f)),
					-d.x * d.y * (steepness * sin(f))
				);
				binormal += float3(
					-d.x * d.y * (steepness * sin(f)),
					d.y * (steepness * cos(f)),
					-d.y * d.y * (steepness * sin(f))
				);
				return float3(
					d.x * (a * cos(f)),
					a * sin(f),
					d.y * (a * cos(f))
				);
			}

			v2f vert (float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0) {
				v2f o;

				float3 gridPoint = vertex.xyz;
				tangent.xyz = float3(1, 0, 0);
				float3 binormal = float3(0, 0, 1);
				float3 p = gridPoint;
				p += GerstnerWave(_WaveA, gridPoint, tangent.xyz, binormal);
				p += GerstnerWave(_WaveB, gridPoint, tangent.xyz, binormal);
				p += GerstnerWave(_WaveC, gridPoint, tangent.xyz, binormal);
				normal = normalize(cross(binormal, tangent.xyz));
				vertex.xyz = p;

				o.pos = UnityObjectToClipPos(vertex);
				half3 wNormal = UnityObjectToWorldNormal(normal);
				o.nor =  wNormal;
				o.viewDir = WorldSpaceViewDir(vertex);
				o.uv = uv;

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
				half3 tnormal = UnpackNormal(tex2D(_BumpTex, i.uv * _BumpTex_ST.xy + _BumpTex_ST.zw));
				half3 wnormal = half3(i.tspace0.z, i.tspace1.z, i.tspace2.z);
				half3 normal;
                normal.x = dot(i.tspace0, tnormal);
                normal.y = dot(i.tspace1, tnormal);
                normal.z = dot(i.tspace2, tnormal);
				normal = normalize(lerp(wnormal, normal, _BumpStrength));
				
				float4 color = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);

				float NdotL = dot(_WorldSpaceLightPos0, normal);
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);

				float4 rimDot = 1 - dot(viewDir, normal);

				float shadow = SHADOW_ATTENUATION(i);

				// --- Lighting ---
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);

				// --- Specular ---
				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);

				// --- Rim ---
				float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity) * shadow;

				// --- Light ---
				float4 light = lightIntensity * _LightColor0;
				float4 specular = specularIntensitySmooth * _SpecularColor * lightIntensity * _LightColor0;
				float4 rim = rimIntensity * _RimColor * _LightColor0;
				
				//return NdotH;
				return color * _Color * (_AmbientColor + light + specular + rim);
			}
			ENDCG
		}

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
           
			float4 _WaveA;
			float4 _WaveB;
			float4 _WaveC;

			float3 GerstnerWave (float4 wave, float3 p, inout float3 tangent, inout float3 binormal) {
				float steepness = wave.z;
				float wavelength = wave.w;
				float k = 2 * UNITY_PI / wavelength;
				float c = sqrt(9.8 / k);
				float2 d = normalize(wave.xy);
				float f = k * (dot(d, p.xz) - c * _Time.y);
				float a = steepness / k;
			
				//p.x += d.x * (a * cos(f));
				//p.y = a * sin(f);
				//p.z += d.y * (a * cos(f));

				tangent += float3(
					-d.x * d.x * (steepness * sin(f)),
					d.x * (steepness * cos(f)),
					-d.x * d.y * (steepness * sin(f))
				);
				binormal += float3(
					-d.x * d.y * (steepness * sin(f)),
					d.y * (steepness * cos(f)),
					-d.y * d.y * (steepness * sin(f))
				);
				return float3(
					d.x * (a * cos(f)),
					a * sin(f),
					d.y * (a * cos(f))
				);
			}

            v2f vert(appdata_base v) {
            v2f o;
			float3 gridPoint = v.vertex.xyz;
			float3 tangent = float3(1, 0, 0);
			float3 binormal = float3(0, 0, 1);
			float3 p = gridPoint;
			p += GerstnerWave(_WaveA, gridPoint, tangent.xyz, binormal);
			p += GerstnerWave(_WaveB, gridPoint, tangent.xyz, binormal);
			p += GerstnerWave(_WaveC, gridPoint, tangent.xyz, binormal);
			v.vertex.xyz = p;
            TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
            return o;
            }
           
            float4 frag(v2f i) : COLOR {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack Off
}