// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Toon/flower draw" {

	Properties {
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}
	}

	SubShader {

	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 100

    Lighting Off

		Pass {
			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;

			uniform float _RushHour;

			v2f vert (float4 vertex : POSITION, float2 uv : TEXCOORD0) {
				vertex.y += abs(sin(_RushHour*10))*0.25;
				vertex.z += _RushHour * 1;
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uv = TRANSFORM_TEX(uv, _MainTex);
				return o;
			}

			float4 frag (v2f i) : SV_Target {
				fixed4 col = _Color;
				col.a *= tex2D(_MainTex, i.uv).a;
                clip(col.a - 0.5);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
			}
			ENDCG
		}
	}
}