Shader "Toon/FX/foam" {
	Properties {

		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MaskTex ("Mask (RGB)", 2D) = "white" {}
		_RSpeed ("Speed (R)", Range(0, 1)) = 1
		_GSpeed ("Speed (G)", Range(0, 1)) = 0.5
		_Cutoff("Cutoff (F)", Range(0, 1)) = 0.1

		[HDR]
		_Color ("Base Color(RGB)", Color) = (1,1,1,1)
	}
	
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MaskTex;
		float4 _Mask_ST;

		struct Input {
			float2 uv_MaskTex;
			float2 uv_MainTex;
			float3 color : COLOR;
		};

		fixed4 _Color;

		fixed _RSpeed;
		fixed _GSpeed;
		fixed _BSpeed;
		fixed _Cutoff;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed2 uv_r = fixed2(0, _Time[3]*-_RSpeed * 0.1);
			fixed2 uv_g = fixed2(0, _Time[3]*-_GSpeed * 0.1);

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex + (uv_r + uv_g)) * _Color;
			fixed r = tex2D (_MaskTex, IN.uv_MaskTex + uv_r).r;
			fixed b = tex2D (_MaskTex, IN.uv_MaskTex + uv_g).g;

			o.Albedo = float4(1, 1, 1, 1);
			clip(saturate(r + b)*(IN.color.r)-_Cutoff);
		}
		ENDCG
	}
}
