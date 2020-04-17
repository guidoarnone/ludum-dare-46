// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Flat/Landscape" {
	Properties{
		_SkyColour("Sky Colour", Color) = (1, 1, 1, 1)
		_MountainColour("Mountain Colour", Color) = (1, 1, 1, 1)
		_GrassColour("Grass Colour", Color) = (1, 1, 1, 1)

		_Speed("Speed", Range(0, 10)) = 1
		_MountainHeight("Mountain Height", Range(-100, 100)) = 10
		_GrassHeight("Grass Height", Range(-10, 10)) = 1
		_MountainFrequency("Mountain Frequency", Range(0, 100)) = 10
		_GrassFrequency("Grass Frequency", Range(0, 10)) = 1
		_MountainAmplitude ("Mountain Amplitude", Range(0, 100)) = 10
		_GrassAmplitude ("Grass Amplitude", Range(0, 10)) = 1
		_MountainDistance("Mountain Distance", Range(0, 100)) = 10
		_GrassDistance("Grass Distance", Range(0, 10)) = 1
	}

	SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		fixed4 _SkyColour;
		fixed4 _MountainColour;
		fixed4 _GrassColour;

		uniform half _Speed;
		uniform half _MountainHeight;
		uniform half _GrassHeight;
		uniform half _MountainFrequency;
		uniform half _GrassFrequency;
		uniform half _MountainAmplitude;
		uniform half _GrassAmplitude;
		uniform half _MountainDistance;
		uniform half _GrassDistance;

		struct 	vertInput {
			float4 pos : POSITION;
		};

		struct vertOutput {
			float4 pos : SV_POSITION;
			float3 wPos : TEXCOORD1;
		};

		vertOutput vert(vertInput input) {
			vertOutput o;
			o.pos = UnityObjectToClipPos(input.pos);
			o.wPos = mul(unity_ObjectToWorld, input.pos).xyz;
			return o;
		}

		half4 frag(vertOutput o) : COLOR {
			half3 pos = o.wPos;
			half4 c = _SkyColour;

			half x = frac((pos.z + pos.x + _Time[1] * _Speed) * 0.007) * 100;

			half xm = x * _MountainFrequency;
			half xg = x * pow(_GrassFrequency, 2);

			half m = _MountainHeight + (sin(xm) * .1 + sin(xm * 21.1 + 3.) * .03 + cos(xm * 49.8 + 1.3) * .01) * _MountainAmplitude; //Mountain
			half g = _GrassHeight + (frac(sin(xg) * 8.5) * 0.05) * _GrassAmplitude; //Grass

			if (pos.y < m)
				c = _MountainColour; // color the mountains
			if (pos.y < g)
				c = _GrassColour; // color the grass
			return c;
		}

		ENDCG
	}
	}
}