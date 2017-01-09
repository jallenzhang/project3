Shader "Custom/Scrolling" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ScrollU ("Scroll U", Float) = 1
	_ScrollV ("Scroll V", Float) = 1
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200

CGPROGRAM
#pragma surface surf Lambert alpha

sampler2D _MainTex;
fixed4 _Color;
float _ScrollU;
float _ScrollV;

struct Input {
	float2 uv_MainTex;
	float4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex + (_Time * float2(_ScrollU, _ScrollV))) * _Color * IN.color;
	o.Emission = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}


}