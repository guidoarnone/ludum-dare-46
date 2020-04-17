Shader "Toon/VColor Lit" {
	Properties {
		_Color ("Main Color (RGB)", Color) = (0.5,0.5,0.5,1)
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}

		_SColor ("Specular Color (RGB)", Color) = (0.5,0.5,0.5,1)
		_SRamp ("Specular Ramp (RGB)", 2D) = "gray" {}
		_SSize ("Specular Size (F)", Range(0.0, 0.999)) = 0.9
		_SOffset ("Specular Offset (F)", Range(0.0, 1.0)) = 0.5
		
		_RimColor ("Rim Color (RGB)", Color) = (0.5,0.5,0.5,1)
		_RimStrength ("Rim Strength (F)", Range(0, 5.0)) = 1.0
		_RimSharp ("Rim sharpness (F)", Range(0.25, 10.0)) = 2.0
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

CGPROGRAM
#pragma surface surf ToonRamp vertex:vert

sampler2D _Ramp;
sampler2D _SRamp;

// custom lighting function that uses a texture ramp based
// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
{
	#ifndef USING_DIRECTIONAL_LIGHT
	lightDir = normalize(lightDir);
	#endif
	
	half d = dot (s.Normal, lightDir)*0.5 + 0.5;
	half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
	
	half4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
	c.a = 0;
	return c;
}

float4 _Color;

sampler2D _BumpMap;

float4 _SColor;
float _SSize;
float _SOffset;

float4 _RimColor;
float _RimStrength;
float _RimSharp;

struct Input {
	float4 vColor : COLOR;
	float2 uv_MainTex : TEXCOORD0;
    float3 lightDir;
    float3 worldPos; // world position
    float3 viewDir; // view direction from camera
};

void vert(inout appdata_full v, out Input o) {
    UNITY_INITIALIZE_OUTPUT(Input, o);
    o.lightDir = WorldSpaceLightDir(v.vertex); // get the worldspace lighting direction
}

void surf (Input IN, inout SurfaceOutput o) {

	float3 localPos = (IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz);// local position of the object, with an offset
    half4 c = _Color * IN.vColor;
    half d = dot(o.Normal, IN.lightDir)*0.5 + _SOffset; // basing on normal and light direction
    half3 specular = tex2D(_SRamp, float2(d, d)).rgb * IN.vColor; // specular ramp
 
    float rim = 1 - saturate(dot(IN.viewDir, o.Normal)); // calculate fresnel rim
    o.Emission = _RimColor.rgb * pow(rim, _RimSharp) * _RimStrength; // fresnel rim
    o.Albedo = (step(_SSize, specular.r)) * specular * d * _SColor; // specular
    o.Albedo += c.rgb; // multiply color by gradient lerp
    o.Alpha = c.a;
}
ENDCG
	} 
	Fallback "Diffuse"
}
