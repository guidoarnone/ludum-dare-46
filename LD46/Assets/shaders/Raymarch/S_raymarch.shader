// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Flat/Raymarch" {
	Properties{
		_VoidColour("Void Colour", Color) = (1, 1, 1, 1)
		_ObjectColour("Object Colour", Color) = (1, 1, 1, 1)

		_Centre("Center", vector) = (0,0,0,0)
		_Radius("Radius", Range(0, 10)) = 1
	}

	SubShader{
		Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		fixed4 _VoidColour;
		fixed4 _ObjectColour;

		fixed3 _Centre;

		uniform half _Radius;

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

		#define STEPS 512
		#define STEP_SIZE 0.1

		bool test(float3 p)
		{
			return (p.z > 5);
		}

		bool sphereHit(float3 p)
		{
			return (distance(p, _Centre) < _Radius );
		}

		bool raymarchHit(float3 position, float3 direction)
		{
			for (int i = 0; i < STEPS; i++)
			{
				if ( sphereHit(position) )
					return true;

				position += direction * STEP_SIZE;
			}

			return false;
		}

		fixed4 frag(vertOutput o) : SV_Target
		{
			float3 worldPosition = o.wPos;
			float3 viewDirection = normalize(o.wPos - _WorldSpaceCameraPos);
			if (raymarchHit(worldPosition, viewDirection))
				return _ObjectColour; // Red if hit the ball
			else
				return _VoidColour; // White otherwise
		}
		ENDCG
	}
	}
}