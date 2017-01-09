Shader "Custom/XRay" {
	Properties {
		_MainTex("Main Texture", 2D) = "white" {}
		_Mask ("Mask Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;

				float2 maskUV : TEXCOORD1;

			};

			sampler2D _MainTex;
			sampler2D _Mask;
			fixed3 _Color;

			uniform float4 _Mask_ST;
			uniform float4 _MainTex_ST;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.maskUV = TRANSFORM_TEX(v.texcoord, _Mask);


				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				float4 diffMain = tex2D(_MainTex, i.uv);
				float4 diffMask = tex2D(_Mask, i.maskUV);

				diffMain.a = diffMain.a - diffMask.a;

				diffMain.rgb *= _Color.rgb;

				return diffMain;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
