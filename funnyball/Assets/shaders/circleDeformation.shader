Shader "Custom/circleDeformation"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CircleColor("CircleColor", Color) = (1, 1, 1, 1)
		_Radians("Radians", Range(0, 0.5)) = 0.5
		_PixelSize("PixelSize", float) = 100
		_Antialias("Anti", Range(0, 1)) = 1
		_MousePosition("MousePosition", vector) = (0.5, 0.5, 0, 0)
		_ShapeHead("Shape Head", Range(0.3, 0.5)) = 0.4

	}

	CGINCLUDE
	#define STEPS 12
	// make fog work
	#pragma multi_compile_fog
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
		float4 vertex : SV_POSITION;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _MousePosition;
	float4 _CircleColor;
	float _PixelSize;
	float _Antialias;
	float _Radians;
	float _ShapeHead;
			
	v2f vert (appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}
	
	float4 main(float2 fragCord);
	fixed4 frag (v2f i) : COLOR
	{
		// sample the texture
		fixed4 col = tex2D(_MainTex, i.uv);
		col = main(i.uv);
		// apply fog
		UNITY_APPLY_FOG(i.fogCoord, col);
		return col;
	}

	float4 circle(float2 pos, float2 center, float3 color, float r) {  
		
        float d = length(pos - center) - r;
		d *= _PixelSize;
        float t = smoothstep(0, _Antialias, d);  
        return float4(color, 1.0 - t);  
    }  

	float4 deformPart(float2 pos, float2 center)
	{
		float p = length(pos -center) - _Radians;
		
		float2 p0 = pos;
		float2 r0 = _Radians;
		float r1 = _Radians * 0.25;
		float s0 = 0.01;
		for(int i = 1; i <= STEPS; i++)
		{
			p0 = lerp(p0, _MousePosition.xy, _ShapeHead);
			r0 = lerp(r0, r1, _ShapeHead);
			r0 = lerp(r0, r1, _ShapeHead);
			r0 = lerp(r0, r1, _ShapeHead);
			s0 = lerp(s0, 1.0 / float(STEPS), _ShapeHead);
			p -= s0 * (1.0 - smoothstep(0, 1.0, length(pos - p0)/r0));
		}

		p *= _PixelSize;

		float t = smoothstep(0, _Antialias, p);  

		float4 col = float4(_CircleColor.rgb, 1- t);

		return col;
	}

	float4 main(float2 fragCord)
	{
		//float4 col = circle(fragCord, float2(0.5, 0.5), _CircleColor.rgb, _Radians);  
		
		float4 col = deformPart(fragCord, float2(0.5, 0.5));
		return col;
	}
	ENDCG

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			ENDCG
		}
	}
}
