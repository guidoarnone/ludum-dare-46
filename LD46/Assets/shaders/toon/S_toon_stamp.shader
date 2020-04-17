Shader "Toon/Stamp" {

	Properties {
		_RotationSpeed ("Rotation Speed(F)", Range(-50.0, 50.0)) = 25

		_Color ("Main Color (RGB)", Color) = (0.5,0.5,0.5,1)
		_FrontTex ("Front (RGB)", 2D) = "white" {}
		_BackTex ("Back (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}

	SubShader {
		Tags { "Queue" = "AlphaTest" "RenderType"="AlphaTest" }
		LOD 200
		Cull Off
		
CGPROGRAM
#pragma surface surf ToonRamp vertex:vert addshadow
#pragma target 3.0
//#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

sampler2D _Ramp;

#pragma lighting ToonRamp exclude_path:prepass
inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten) {
	#ifndef USING_DIRECTIONAL_LIGHT
	lightDir = normalize(lightDir);
	#endif
	
	half d = dot (s.Normal, lightDir) * 0.5 + 0.5;
	half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
	
	half4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
	c.a = s.Alpha;
	return c;
}

float _RotationSpeed;
float4 _Color;
sampler2D _FrontTex;
sampler2D _BackTex;

struct Input {
	float2 uv_FrontTex : TEXCOORD0;
    float3 lightDir;
	float facing : VFACE;
};

float4 RotateAroundYInDegrees (float4 vertex, float degrees) {
	float alpha = degrees * UNITY_PI / 180.0;
	float sin, cos;
	sincos(alpha, sin, cos);
	float2x2 m = float2x2(cos, -sin, sin, cos);
	return float4(mul(m, vertex.xz), vertex.yw).xzyw;
}

void vert(inout appdata_full v, out Input o) {
    UNITY_INITIALIZE_OUTPUT(Input, o);
    o.lightDir =  WorldSpaceLightDir(v.vertex); // get the worldspace lighting direction
	v.vertex = RotateAroundYInDegrees(v.vertex, _RotationSpeed * _Time[3]);
}

void surf (Input IN, inout SurfaceOutput o) {
    half4 f = tex2D(_FrontTex, IN.uv_FrontTex) * _Color;
	clip(f.a - 0.5);
	half4 b = tex2D(_BackTex, IN.uv_FrontTex) * _Color;
    o.Albedo = IN.facing > 0 ? f.rgb : b.rgb; // multiply color by gradient lerp
	//o.Albedo = float4(1,0,0,1);
    o.Alpha = f.a;
}

ENDCG

	} 
	Fallback "Diffuse"
}
