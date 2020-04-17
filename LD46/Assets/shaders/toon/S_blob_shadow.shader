Shader "Unlit/blob_shadow"
{
	Properties
	{
		_Color ("Color (RGB)", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "DisableBatching" = "True" }

		ZWrite Off
		BlendOP Add
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			ZTest Equal

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			half4 _Color;
			sampler2D _MainTex;
			half4 _MainTex_ST;
			
			v2f vert (appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv.xy;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target	{
				// sample the texture
				return tex2D(_MainTex, i.uv) * _Color;;
			}
			ENDCG
		}
	}
}