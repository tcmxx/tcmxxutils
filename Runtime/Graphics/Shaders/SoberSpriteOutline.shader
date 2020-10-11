// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/SoberSpriteOutline"
{
	Properties{ 
		[PerRendererData] _MainTex("Texture", 2D) = "white" {}		
		_Delta("Width", Range(0, 0.001)) = 0.0001		
		_EdgeColor("EdgeColor", Color) = (1, 0, 0, 1)
		_Color ("Tint", Color) = (1,1,1,1)
	}	
	SubShader{ 
			Tags{ 			
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}		
	Blend SrcAlpha OneMinusSrcAlpha 
	Cull Off
	ZWrite Off
	Lighting Off

	Pass{ 
	CGPROGRAM			
	#pragma vertex vert			
	#pragma fragment frag			
	#include "UnityCG.cginc" 			
	struct appdata { 
		float4 vertex : POSITION;				
		fixed2 uv : TEXCOORD0;
		float4 color    : COLOR;
	}; 			
	struct v2f 
	{ 
		float4 vertex : SV_POSITION;				
		fixed2 uv : TEXCOORD0; 
		fixed4 color    : COLOR;
	}; 	

	sampler2D _MainTex;			
	float4 _MainTex_ST;			
	fixed _Delta;			
	fixed4 _EdgeColor;
	fixed4 _Color;
	sampler2D _AlphaTex;
	float _AlphaSplitEnabled;

	float sampeAlpha(float2 uv){
		//if(uv.x > 1 || uv.y > 1 || uv.x < 0 || uv.y < 0)
		//	return 0;
		return tex2D(_MainTex,uv).a;	
	}

    float sobel (float2 uv) 
    {
        float2 delta = float2(_Delta, _Delta);
        
        float hr = 0;
        float vt = 0;
        
        hr += sampeAlpha(uv + float2(-1.0, -1.0) * delta) *  1.0;
        hr += sampeAlpha(uv + float2( 1.0, -1.0) * delta) * -1.0;
        hr += sampeAlpha(uv + float2(-1.0,  0.0) * delta) *  2.0;
        hr += sampeAlpha(uv + float2( 1.0,  0.0) * delta) * -2.0;
        hr += sampeAlpha(uv + float2(-1.0,  1.0) * delta) *  1.0;
        hr += sampeAlpha(uv + float2( 1.0,  1.0) * delta) * -1.0;
        
        vt += sampeAlpha(uv + float2(-1.0, -1.0) * delta) *  1.0;
        vt += sampeAlpha(uv + float2( 0.0, -1.0) * delta) *  2.0;
        vt += sampeAlpha(uv + float2( 1.0, -1.0) * delta) *  1.0;
        vt += sampeAlpha(uv + float2(-1.0,  1.0) * delta) * -1.0;
        vt += sampeAlpha(uv + float2( 0.0,  1.0) * delta) * -2.0;
        vt += sampeAlpha(uv + float2( 1.0,  1.0) * delta) * -1.0;
        
        return sqrt(hr * hr + vt * vt);
    }


	v2f vert(appdata v) {
		v2f o;				
		o.vertex = UnityObjectToClipPos(v.vertex);								
		o.uv = v.uv; 
		o.color = v.color * _Color;				
		return o;		
		}						
	fixed4 frag (v2f i) : SV_Target			
	{				
		fixed4 original = tex2D(_MainTex, i.uv);
#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					original.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED
		original *= i.color;                  
		fixed alpha = original.a;          
		float soberResult = saturate(sobel(i.uv));        
		float s = pow(1 - saturate(soberResult), 1);
		return lerp(_EdgeColor,original,s); 			
	}		
		ENDCG
	
        }
    }
}
