Shader "Geometry/stencil_mask" {

    Properties {
		_Mask ("Stencil Mask (I)", int) = 32
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
            Cull Off
			//ZTest Greater makes it so we are only writing when the geometry is behind other geometry
            ZTest Always

			//Stencil Block:
            Stencil {
				//Value that we are testing against
                Ref [_Mask]
                Comp Always
                Pass Replace
				ZFail Replace }
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			int _Mask;

            half4 frag(v2f i) : SV_Target {
                return half4(1,1,1,1);
            }
        ENDCG
        }
	} 
}  