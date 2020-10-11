// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Highlights intersections with other objects
//addapted from Chris Flynn's Blog and Such 
//https://chrismflynn.wordpress.com/2012/09/06/fun-with-shaders-and-the-depth-buffer/
//Fixed some issues in the original code
Shader "TCShaders/IntersectionHighlights"
{
	Properties
	{
		_RegularColor("Main Color", Color) = (1, 1, 1, .5) //Color when not intersecting
		[PerRendererData] _MainTex("Texture", 2D) = "white" {}
	_HighlightColor("Highlight Color", Color) = (1, 1, 1, .5) //Color when intersecting
		_HighlightThresholdMax("Highlight Threshold Max", Float) = 1 //Max difference for intersections
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Pass
	{
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off

		CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		uniform sampler2D _CameraDepthTexture; //Depth Texture
	uniform float4 _RegularColor;
	uniform float4 _HighlightColor;
	uniform float _HighlightThresholdMax;

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 texcoord : TEXCOORD0;
		float4 projPos : TEXCOORD1; //Screen position of pos
		float eyeDepth : TEXCOORD2; //
	};

	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.projPos = ComputeScreenPos(o.pos);
		o.texcoord = v.texcoord;
		o.eyeDepth = COMPUTE_EYEDEPTH(v.vertex);
		return o;
	}


	sampler2D _MainTex;

	half4 frag(v2f i) : COLOR
	{
		float4 finalColor = _RegularColor;

		//Get the distance to the camera from the depth buffer for this point
		float sceneZ = LinearEyeDepth(tex2Dproj(_CameraDepthTexture,
			UNITY_PROJ_COORD(i.projPos)).r);

		//Actual distance to the camera
		float partZ = i.eyeDepth;

		//If the two are similar, then there is an object intersecting with our object
		float diff = (abs(sceneZ - partZ)) /
			_HighlightThresholdMax;

		if (diff <= 1)
		{
			finalColor = lerp(_HighlightColor,
				_RegularColor*tex2D(_MainTex, i.texcoord),
				float4(diff, diff, diff, diff));
		}

		half4 c;
		c.r = finalColor.r;
		c.g = finalColor.g;
		c.b = finalColor.b;
		c.a = finalColor.a;

		return c;
	}

		ENDCG
	}
	}
		FallBack "VertexLit"
}