Shader "Custom/Twist"
{
	Properties
	{
		_Color("COLOR", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_Twist("Twist", Range(0, 20)) = 0
		_Offset("_Offset", Range(0, 0.5)) = 0.1
	}
	SubShader
	{
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 modelUV : TEXCOORD1;
				float2 uv2: TEXCOORD2;
			};

			float _Twist;
			float _Offset;
			sampler2D _MainTex;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				float2 modelUV = v.uv - float2(0.5, 0.5);

				float angle = _Twist * length(modelUV);
				float cosLength;
				float sinLength;
				sincos(angle, sinLength, cosLength);
				o.uv[0] = cosLength * modelUV[0] - sinLength * modelUV[1];
				o.uv[1] = sinLength * modelUV[0] + cosLength * modelUV[1];
				o.uv += float2(0.5, 0.5);
				o.uv2 = o.uv - float2(_Offset, 0);
				o.uv += float2(_Offset, 0);
				return o;
			}
			

			fixed4 frag (v2f i) : COLOR
			{

				fixed4 col1 = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_MainTex, i.uv2);
				fixed4 col = lerp(col1, col2, 0.5);
				return col;
			}
			ENDCG
		}
	}
}
