Shader "Custom/masked" {
	Properties {

		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex ("Normal (RGB)", 2D) = "white" {}
		_RMA ("RMA (RGB)", 2D) = "white" {}
		_Occlusion ("Ambient Occlusion(F)", Range(0, 1)) = 0.5
		_Mask ("Mask (RGB)", 2D) = "white" {}

		[HDR]
		_Color ("Base Color(RGB)", Color) = (1,1,1,1)
		[HDR]
		_RColor ("Red Color(RGB)", Color) = (1,1,1,1)
		[HDR]
		_GColor ("Green Color(RGB)", Color) = (1,1,1,1)
		[HDR]
		_BColor ("Blue Color(RGB)", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalTex;
		sampler2D _RMA;
		sampler2D _Mask;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		fixed4 _RColor;
		fixed4 _GColor;
		fixed4 _BColor;

		fixed _Occlusion;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed3 n = UnpackNormal (tex2D (_NormalTex, IN.uv_MainTex));
			fixed3 rma = tex2D (_RMA, IN.uv_MainTex);
			fixed3 m = tex2D (_Mask, IN.uv_MainTex);
			fixed4 ao = (fixed4(1,1,1,1) - ((1 - rma.b) * _Occlusion));

			float mr = m.r;
			float mg = (1 - mr) * m.g ;
			float mb = (1 - mg) * m.b;
			float base = 1 - (mr + mg + mb);

			o.Albedo = c * (base * _Color + mr * _RColor + mg * _GColor + mb * _BColor) * ao;
			o.Normal = n;
			o.Smoothness = 1 - rma.r;
			o.Metallic = rma.g;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
