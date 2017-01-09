Shader "Custom/Gaosi Blur Effect" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
	_BlurOffsets("_BlurOffsets", float) = 0
}
SubShader {
	Tags{"RenderType" = "Transparent" "Queue" = "Transparent"}
	Blend SrcAlpha OneMinusSrcAlpha
    Pass {
	
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma fragmentoption ARB_precision_hint_fastest 
    #include "UnityCG.cginc"
    
    uniform sampler2D _MainTex;
    uniform float4 _MainTex_TexelSize;
    float _BlurOffsets;

    
    struct v2f {
        float4 pos : POSITION;
        float2 uv : TEXCOORD0;
    };
    
    v2f vert( appdata_img v )
    {
        v2f o;
        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
        float2 uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord );
        o.uv = uv;
        
        return o;
    }
    
    
    
    half4 frag (v2f i) : COLOR
    {
        float2 uv = i.uv;
        half4 original = 0;
        //original = tex2D(_MainTex, i.uv[0]);
        //return original;
        float3x3 _smooth_fil = float3x3 (1/16.0 ,2/16.0,1/16.0 ,
                              2/16.0 ,4/16.0,2/16.0 ,
                              1/16.0 ,2/16.0,1/16.0 );
        float3x3 _filter_pos_delta_x = float3x3 ( -1 , 0, 1 ,
                                                   0 , 0, 1 ,
                                                   1 , 0, 1 );
        float3x3 _filter_pos_delta_y = float3x3 ( -1 ,-1,-1 ,
                                                  -1 , 0, 0 ,
                                                  -1 , 1, 1 );
       _filter_pos_delta_x *= _BlurOffsets;
	   _filter_pos_delta_y *= _BlurOffsets;
	   float tmpX;
	   float tmpY;
	   float smoothValue;
       for(int i = 0 ; i < 3 ; i ++ )
       {
           for(int j = 0 ; j < 3 ; j ++)
           {
				tmpX = uv.x + _filter_pos_delta_x[j]* _MainTex_TexelSize.x;
				tmpY = uv.y + _filter_pos_delta_y[j]*_MainTex_TexelSize.y;
				float2 _xy_new = float2(tmpX, tmpY);
				smoothValue = _smooth_fil[j];
                original += tex2D(_MainTex, _xy_new) * smoothValue;
           } 
       }
       return original;
    }
    ENDCG
    }
}
Fallback off
}