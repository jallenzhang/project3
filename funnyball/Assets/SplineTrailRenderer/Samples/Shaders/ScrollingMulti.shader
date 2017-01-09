Shader "Custom/ScrollingMulti" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ScrollU1 ("Scroll U1", Float) = 1
	_ScrollV1 ("Scroll V1", Float) = 1
	_ScrollU2 ("Scroll U2", Float) = 1
	_ScrollV2 ("Scroll V2", Float) = 1
	_Boost ("Boost", Float) = 1
	_Tex1 ("Base (RGB) Trans (A) 1", 2D) = "white" {}
	_Tex2 ("Base (RGB) Trans (A) 2", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200

CGPROGRAM
#pragma surface surf Lambert alpha

sampler2D _Tex1;
sampler2D _Tex2;
fixed4 _Color;
float _ScrollU1;
float _ScrollV1;
float _ScrollU2;
float _ScrollV2;
float _Boost;

struct Input {
	float2 uv_Tex1;
	float4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c1 = tex2D(_Tex1, IN.uv_Tex1 + (_Time * float2(_ScrollU1, _ScrollV1)));
	fixed4 c2 = tex2D(_Tex2, IN.uv_Tex1 + (_Time * float2(_ScrollU2, _ScrollV2)));
	fixed4 c = c1 * c2 * _Color * IN.color * _Boost;
	o.Emission = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}


}