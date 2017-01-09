Shader "Unlit/OrthographicWave"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OffsetX("Offset X", Range(0,1)) = 0.5
		_OffsetY("Offset Y", Range(0,1)) = 0.5
		_Radius("Radians", Range(0,1)) = 0.5
		_WaveHeight("Wave Height", float) = 0.01
		_WaveWidth("Wave Width", float) = 1
		_Speed("Speed", float) = 2
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _OffsetX;
			float _OffsetY;
			float _Radius;
			float _WaveHeight;
			float _Speed;
			float _WaveWidth;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float dis = distance(uv, float2(_OffsetX, _OffsetY));
				_WaveHeight *= saturate(1-dis/_Radius);
				float scale = _WaveHeight * sin(-dis * 3.14 * _WaveWidth + _Time.y* _Speed);
				uv = uv + uv * scale;
				// sample the texture
				fixed4 col = tex2D(_MainTex, uv) + fixed4(1,1,1,0)*saturate(scale)*10;
				return col;
			}
			ENDCG
		}
	}
}
