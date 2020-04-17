Shader "Geometry/stencil_shadow" {

    Properties {
		[HDR]
        _Color ("Main Color", Color) = (1,1,1,0)
    }

    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Transparent"}

        ZWrite off
       
        CGINCLUDE
        #include "UnityCG.cginc"
            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
				float3 worldpos : TEXCOORD0;
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos =  UnityObjectToClipPos(v.vertex);
				o.worldpos = mul (unity_ObjectToWorld, v.vertex);
                return o;
            }
			v2f half_hull(appdata v) {
                v2f o;
                o.pos =  UnityObjectToClipPos(v.vertex * 0.5);
				o.worldpos = mul (unity_ObjectToWorld, v.vertex);
                return o;
            }
        ENDCG


		//Write back faces 
        Pass {
			ZWrite off
			//Colormask 0, since we jsut want to modify teh stencil buffer
			colormask 0
			//Cull front faces, this gets us the inverted normals of the sphere
            Cull Front
			//ZTest Greater makes it so we are only writing when the geometry is behind other geometry
            ZTest Greater

			//Stencil Block:
            Stencil {
				//Value that we are testing against
                Ref 1
                Comp Always
                Pass Replace
				ZFail Zero }
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(v2f i) : SV_Target {
                return half4(1,1,1,1);
            }
        ENDCG
        }

		//Check intersection with forward facing normals, effect outside
        Pass {
			ZWrite off
			//Colormask RGB since we are outputting color this time
			ColorMask RGB
			//Cull Back since we want the front faces this time
			Cull Back
			//Faces in front (Since we turned ZWriting off our light spheres wont interfere)
			ZTest Less

			Stencil {
				Ref 1
				Comp Equal
				Pass IncrSat
				Fail Zero
				ZFail DecrSat
			}

			BlendOP Add
			Blend DstColor SrcColor
        
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			half4 _Color;
            
			fixed4 frag (v2f i) : SV_Target {
				return _Color;
			}
        ENDCG
        }

		//Check intersection with forward facing normals, effect inside
		Pass {
			ColorMask RGB
			Cull Front
			ZTest Always

			Stencil {
				Ref 1
				Comp Equal
				Pass IncrSat
				Fail Zero
				ZFail Zero
			}

			BlendOP Add
			Blend DstColor SrcColor
        
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			half4 _Color;
            
			fixed4 frag (v2f i) : SV_Target {
				return _Color;
			}
        ENDCG
        }
    } 
}  