Shader "Toon/FX/fire" {
	Properties {
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_OuterColor("Outer Color", Color) = (0.5, 0.65, 1, 1)
		_MainTex("Firemap", 2D) = "white" {}
		_DistortTex("Distortion map", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.75
		_Distortion("Distortion", float) = 1
		_Speed("Speed", float) = 1
	}

	SubShader {
		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}
			ZWrite On ZTest LEqual Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DistortTex;
			float4 _DistortTex_ST;
			float _Distortion;
			float4 _Color;
			float4 _OuterColor;
			float _Cutoff;
			float _Speed;

			v2f vert (float4 vertex : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD0) {
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uv = TRANSFORM_TEX(uv, _MainTex);
				return o;
			}

			float4 frag (v2f i) : SV_Target {
				float red = tex2D(_DistortTex, i.uv * _DistortTex_ST.xy + _DistortTex_ST.zw - float2(0, _Time[0] * 5) * _Speed).x * 0.125 * _Distortion;
				float green = tex2D(_DistortTex,  i.uv * _DistortTex_ST.xy + _DistortTex_ST.zw - float2(0, _Time[0]) * _Speed).y * 0.25 * _Distortion;
				float alpha = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw).a;
				float distort = (red + green) * i.uv.y;
				i.uv.y -= distort;
				float4 color = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);

				clip(_Cutoff-(color.b) - (1-alpha));
				//return alpha.a;
				return (color.r + color.b) * _OuterColor + color.g * _Color;
			}
			ENDCG
		}
	}
}