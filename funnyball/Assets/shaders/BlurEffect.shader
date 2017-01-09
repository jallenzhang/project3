Shader "Custom/BlurEffectConeTap" {
	Properties { 
		_MainTex ("MainTex", 2D) = "white" {} 
		_BlurOffsets("_BlurOffsets", float) = 0
		_Color("Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags{"RenderType"="Transparent" "Queue"="Transparent"}
		ZWrite Off
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				half2 taps[8] : TEXCOORD2; 
				UNITY_FOG_COORDS(1)
			};
			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			half4 _MainTex_TexelSize;
			float _BlurOffsets;
			half4 _Color;
			v2f vert( appdata_full v ) {
				v2f o; 
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				//o.uv = v.texcoord - float2(_BlurOffsets,_BlurOffsets) * _MainTex_TexelSize.xy; // hack, see BlurEffect.cs for the reason for this. let's make a new blur effect soon
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.taps[0] = o.uv + _MainTex_TexelSize * float2(_BlurOffsets,_BlurOffsets);
				o.taps[1] = o.uv - _MainTex_TexelSize * float2(_BlurOffsets,_BlurOffsets);
				o.taps[2] = o.uv + _MainTex_TexelSize * float2(_BlurOffsets,_BlurOffsets) * half2(1,-1);
				o.taps[3] = o.uv - _MainTex_TexelSize * float2(_BlurOffsets,_BlurOffsets) * half2(1,-1);
				o.taps[4] = o.uv + _MainTex_TexelSize * float2(_BlurOffsets,_BlurOffsets) * half2(1, 0);
				o.taps[5] = o.uv - _MainTex_TexelSize * float2(_BlurOffsets,_BlurOffsets) * half2(1, 0);
				o.taps[6] = o.uv + _MainTex_TexelSize * float2(_BlurOffsets,_BlurOffsets) * half2(0, 1);
				o.taps[7] = o.uv - _MainTex_TexelSize * float2(_BlurOffsets,_BlurOffsets) * half2(0, 1);

				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			float4 frag(v2f i) : COLOR {
				float4 col = tex2D(_MainTex, i.taps[0]);
					
				col += tex2D(_MainTex, i.taps[1]);
				col += tex2D(_MainTex, i.taps[2]);
				col += tex2D(_MainTex, i.taps[3]); 
				col += tex2D(_MainTex, i.taps[4]); 
				col += tex2D(_MainTex, i.taps[5]); 
				col += tex2D(_MainTex, i.taps[6]); 
				col += tex2D(_MainTex, i.taps[7]);
				col *= 0.125;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				col *= _Color;
				return col;
			}
			ENDCG
		}
	}
	Fallback off
}
