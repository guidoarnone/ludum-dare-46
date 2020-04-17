Shader "Geometric/hex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Size("Size", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float _Size;

			float hexagon (float2 uv) {
				uv = abs(uv);
                fixed c = dot(uv, normalize(float2(1, 1.73)));
				c = max(c, uv.x);
				return c;
			}

			fixed4 hexcoord(float2 uv) {
				float2 aspect = float2(1, 1.73);
				float2 a = fmod(uv + aspect, aspect) - aspect * 0.5;
				float2 b = fmod((uv + aspect * 0.5), aspect) -aspect * 0.5;
				float2 gv = dot(a, a) < dot(b, b) ? a : b;

				float x = (atan2(gv.x, gv.y)+3) / 3;
				float y = 0.5 - hexagon(gv);
				float2 id = uv-gv;
				return float4(x, y, id);
			}

            fixed4 frag (v2f i) : SV_Target {
				
				fixed3 col = 0;
				i.uv *= _Size;

				float4 hc = hexcoord(i.uv);

				float state = (sin(hc.z * hc.w +_Time.z)+1)/2;
				float c = smoothstep(.05, .1, hc.y * state);
				col = tex2D(_MainTex, c);

                return float4(col, 1);
            }
            ENDCG
        }
    }
}
