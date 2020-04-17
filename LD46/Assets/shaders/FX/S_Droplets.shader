Shader "FX/droplets"
{
    Properties
    {
	    _MainTex ("Texture", 2D) = "white" {}
		_Size("Size", float) = 1
		_Speed("Speed", float) = 1
		_Distortion("Distortion", float) = 1
		_Blur("Blur", float) = 1
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
                UNITY_FOG_COORDS(2)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			sampler2D _GrabTex;
            float4 _MainTex_ST;
			float _Size;
			float _Speed;
			float _Distortion;
			float _Blur;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.grab_uv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float N21(float2 p) {
				p = frac(p*float2(123.34, 345.45));
				p += dot (p, p + 34.345);
				return frac(p.x * p.y);
			}

			float3 layer (float2 in_uv, float t) {
				float2 aspect = float2(2, 1);
				float2 uv = in_uv * _Size * aspect;
				uv.y += t * 0.25;
				float2 gv = frac(uv) - 0.5;
				float2 id = floor(uv);

				float n = N21(id);
				t += n * 6.28;
				float w = in_uv.y * 10;
				float x = (n-0.5) * 0.8;
				x += (0.4-abs(x)) * sin(3*w)*pow(sin(w), 6) * 0.45;
				float y = -sin(t+sin(t+sin(t)*0.5))*0.45;
				y -= (gv.x-x)*(gv.x-x);

				float2 drop_position = (gv - float2(x, y)) / aspect;
				float drop = smoothstep(0.05, 0.03, length(drop_position));

				float2 trail_position = (gv - float2(x, t * 0.25)) / aspect;
				trail_position.y = (frac(trail_position.y * 8)-0.5)/8;
				float trail = smoothstep(0.03, 0.01, length(trail_position));
				float fog_trail = smoothstep(-0.05, 0.05, drop_position.y);
				fog_trail *= smoothstep(0.5, y, gv.y);
				trail *= fog_trail;
				fog_trail *= smoothstep(0.05, 0.04, abs(drop_position.x));

				float2 offset = drop * drop_position + trail * trail_position;

				return float3(offset, fog_trail);
			}

            fixed4 frag (v2f i) : SV_Target {

				float t = fmod(_Time.y * _Speed * tex2D(_MainTex, i.uv), 7200);

                fixed4 col = 0;
				
				float3 drops = layer(i.uv, t);
				drops += layer(i.uv*1.23+7.54, t);
				drops += layer(i.uv*1.51+12.39, t);
				drops += layer(i.uv*1.13+2.86, t);
				drops += layer(i.uv*1.42+23.59, t);

				float fade = 1-saturate(fwidth(i.uv));
				float blur = _Blur * 7 * (1-drops.z * fade);

				float2 proj_uv =  i.grab_uv.xy / i.grab_uv.w;
				proj_uv += drops.xy * _Distortion * fade;

				const int samples = 16;
				float a = N21(i.uv) * 6.28;
				for (int i = 0; i < samples; i++) {
					float2 offset = float2(sin(a), cos(a)) * blur;
					offset *= sqrt(frac(sin(i+1)*5462));
					col += tex2D(_GrabTex, proj_uv+offset * 0.001);
					a++;
				}
				col /= samples;
				
				//col  *= 0; col.rg = drop_position;
                return col;
            }
            ENDCG
        }
    }
}
