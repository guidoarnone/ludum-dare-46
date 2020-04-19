// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Toon/flower" {
	Properties {
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}

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
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 view : TEXCOORD1;
				SHADOW_COORDS(3)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _AmbientColor;
			
			float4 _SpecularColor;
			float _Glossiness;

			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;

			float4 rotate_around_y (float4 vertex, float degrees) {
				float alpha = degrees;
				float sin, cos;
				sincos(alpha, sin, cos);
				float4x4 m = float4x4(cos, 0, sin, 0,
									  0, 1, 0, 0,
									  -sin, 0, cos, 0,
									  0, 0, 0, 1);
				return mul(m, vertex);
			}

			float4 rotate_around_z (float4 vertex, float degrees) {
				float alpha = degrees;
				float sin, cos;
				sincos(alpha, sin, cos);
				float4x4 m = float4x4(cos, -sin, 0, 0,
									  sin, cos, 0, 0,
									  0, 0, 1, 0,
									  0, 0, 0, 1);
				return mul(m, vertex);
			}

			v2f vert (float4 vertex : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD0) {
				float3 world = unity_ObjectToWorld._m03_m13_m23;
				float seed = (world.x*1024 + world.z*1024) % 5769761;
				vertex = rotate_around_y(vertex, _Time[2]+seed);
				vertex = rotate_around_z(vertex, sin(_Time[1]+seed) / 10);
				normal = rotate_around_y(float4(normal.xyz, 0), _Time[2]+seed);
				normal = rotate_around_z(float4(normal.xyz, 0), sin(_Time[1]+seed) / 10);
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.normal = UnityObjectToWorldNormal(normal);
				o.view = WorldSpaceViewDir(vertex);
				o.uv = TRANSFORM_TEX(uv, _MainTex);

				TRANSFER_SHADOW(o)
				return o;
			}

			float4 frag (v2f i) : SV_Target {

				float3 viewDir = normalize(i.view);
				float3 normal = normalize(i.normal);

				float4 color = tex2D(_MainTex, i.uv);
				float NdotL = dot(_WorldSpaceLightPos0, normal);

				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);

				float4 rimDot = 1 - dot(viewDir, normal);

				float shadow = SHADOW_ATTENUATION(i);

				// --- Lighting ---
				// - Step lighting -
				float lightIntensity = smoothstep(0, 1, NdotL * shadow);

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

				return color * (_AmbientColor + light + specular + rim) * _Color;
			}
			ENDCG
		}

		Pass {
            Tags {"LightMode"="ShadowCaster"}
     
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
     
            struct v2f {
                V2F_SHADOW_CASTER;
            };
     
			float4 rotate_around_y (float4 vertex, float degrees) {
				float alpha = degrees;
				float sin, cos;
				sincos(alpha, sin, cos);
				float4x4 m = float4x4(cos, 0, sin, 0,
									  0, 1, 0, 0,
									  -sin, 0, cos, 0,
									  0, 0, 0, 1);
				return mul(m, vertex);
			}

			float4 rotate_around_z (float4 vertex, float degrees) {
				float alpha = degrees;
				float sin, cos;
				sincos(alpha, sin, cos);
				float4x4 m = float4x4(cos, -sin, 0, 0,
									  sin, cos, 0, 0,
									  0, 0, 1, 0,
									  0, 0, 0, 1);
				return mul(m, vertex);
			}

            v2f vert(appdata_base v) {
                v2f o;
				float3 world = unity_ObjectToWorld._m03_m13_m23;
				float seed = (world.x*1024 + world.z*1024) % 5769761;
				v.vertex = rotate_around_y(v.vertex, _Time[2]+seed);
				v.vertex = rotate_around_z(v.vertex, sin(_Time[1]+seed) / 10);
				v.normal = rotate_around_y(float4(v.normal.xyz, 0), _Time[2]+seed);
				v.normal = rotate_around_z(float4(v.normal.xyz, 0), sin(_Time[1]+seed) / 10);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }
     
            float4 frag(v2f i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
	}
}