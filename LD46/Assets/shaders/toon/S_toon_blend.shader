Shader "Toon/ramp_angle_blend" {
	Properties {
		_Color ("Color (RGB)", Color) = (1,1,1,1) 
		_MainTex ("Texture", 2D) = "white" {}
		_RampTex ("Ramp", 2D) = "white" {}

		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
     	_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Toon

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		half4 _Color;
		sampler2D _MainTex;
		float4 _RimColor;
      	float _RimPower;

		void surf (Input IN, inout SurfaceOutput o) {
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
			half3 rimc =  _RimColor.rgb * pow (rim, _RimPower);
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color * (1-rim) + rim * rimc;
			//o.Emission = _RimColor.rgb * pow (rim, _RimPower);
		}

		sampler2D _RampTex;
		
		fixed4 LightingToon (SurfaceOutput s, fixed3 lightDir, fixed atten)	{

			half NdotL = dot(s.Normal, lightDir); 
			NdotL = tex2D(_RampTex, fixed2(NdotL, 0.5));
			
			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * NdotL * atten * 2;
			c.a = s.Alpha;

			return c;
		}

		ENDCG
	} 
	Fallback "Diffuse"
}