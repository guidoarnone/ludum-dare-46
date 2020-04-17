Shader "FX/proximity_hex"
{
    Properties {
		[HDR]
		_Color ("Color (RGB)", Color) = (1,1,1,1) 
        _MainTex ("Texture", 2D) = "black" {}
		_Size("Size", float) = 1
		_Force("Force", float) = 2
		_Blur("Blur", float) = 1
		_Distortion("Distortion", float) = 1
		_Position("Position", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

		GrabPass { "_GrabTex" }
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
				float4 grab_uv : TEXCOORD1;
				float3 world_pos : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

			float4 _Color;
            sampler2D _MainTex;
			sampler2D _GrabTex;
            float4 _MainTex_ST;
			float4 _Position;
			float _Size;
			float _Force;
			float _Blur;
			float _Distortion;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.grab_uv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));
				o.world_pos = mul (unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float hexagon (float2 uv) {
				uv = abs(uv);
                fixed c = dot(uv, normalize(float2(1, 1.73)));
				c = max(c, uv.x);
				return c;
			}

			fixed4 hexcoord(float2 uv) {
				float2 aspect = float2(1, 1.73);
				float2 a = fmod(uv + aspect, aspect) - aspect * 0.5;
				float2 b = fmod((uv + aspect * 0.5), aspect) - aspect * 0.5;
				float2 gv = dot(a, a) < dot(b, b) ? a : b;

				float x = (atan2(gv.x, gv.y)+3.14)/(3.14*2);
				float y = 0.5 - hexagon(gv);
				float2 id = uv-gv;
				return float4(x, y, id);
			}

			float N21(float2 p) {
				p = frac(p*float2(123.34, 345.45));
				p += dot (p, p + 34.345);
				return frac(p.x * p.y);
			}

            fixed4 frag (v2f i) : SV_Target {
				
				fixed3 hex = 0;
				fixed3 color = 0;
				float2 uv = i.uv;
				i.uv *= _Size;

				float4 hc = hexcoord(abs(i.world_pos.xz * _Size));

				float dist = 1-length(float3(hc.z, i.world_pos.y, hc.w) - float3(_Position.x * _Size, _Position.y, _Position.z * _Size)) / _Force / _Size;
				float c = smoothstep(.005 / dist, .01 / dist, hc.y);
				hex = c;

				clip(hex-1);

				float2 proj_uv =  i.grab_uv.xy / i.grab_uv.w;
				float a = hc.x * 6.28;
				proj_uv += float2(cos(a), sin(a)) / 100 * _Distortion * fmod((hc.z+12.76) * (hc.w+4.35), 2);

				float4 tint = tex2D(_MainTex, frac(sin(hc.z*21.76) * cos(hc.w*4.35) * 18.56));

				const int samples = 16;
				a = N21(i.uv) * 6.28;
				for (int i = 0; i < samples; i++) {
					float2 offset = float2(sin(a), cos(a)) * _Blur;
					offset *= sqrt(frac(sin(i+1)*5462));
					color.r += tex2D(_GrabTex, proj_uv + offset * tint.r/150).r;
					color.g += tex2D(_GrabTex, proj_uv + offset * tint.g/200).g;
					color.b += tex2D(_GrabTex, proj_uv + offset * tint.b/250).b;
					a++;
				}
				color /= samples;

                return lerp(float4(color, 1) * tint, float4(color, 1) * tint * _Color, _Color.a);
            }
            ENDCG
        }
    }
}
