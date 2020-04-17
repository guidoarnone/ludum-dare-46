Shader "Geometry/Sphere_Light" {

    Properties {
        _Color ("Main Color", Color) = (1,1,1,0)
    }

    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Transparent"}

        CGINCLUDE
        #include "UnityCG.cginc"
            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos =  UnityObjectToClipPos(v.vertex);
                return o;
            }
        ENDCG

		// writes back faces to a stencil buffer
        Pass {
			ZWrite OFF
			colormask RGB
            Cull Back
            ZTest LEqual

			Stencil {     
                Ref 8
                Comp always
                Pass Replace
				ZFail Replace }

			BlendOP Add
			Blend SrcAlpha DstAlpha
        
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _Color;
            
			fixed4 frag (v2f i) : SV_Target {

				return _Color;
			}
            ENDCG
        }

		Pass {
			ZWrite Off
            colormask RGB
            Cull Front
            ZTest Always
        
			Stencil {     
                Ref 8
                Comp Greater
                Pass DecrSat
				Fail DecrSat }

			BlendOP Add
			Blend SrcAlpha DstAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			float4 _Color;

            half4 frag(v2f i) : SV_Target {
                return _Color;
            }
            ENDCG
        }
    } 
}  