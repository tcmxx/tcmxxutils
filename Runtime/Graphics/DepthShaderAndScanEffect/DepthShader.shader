//Shows the grayscale of the depth from the camera.

Shader "Custom/DepthShader"
{
	Properties
	{
		_MaxDepth("MaxDepth", Float) = 100 //Max depth for pure white
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		Pass{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct v2f {
		float4 pos : SV_POSITION;
		float4 projPos : TEXCOORD1; //Screen position of pos
		float eyeDepth : TEXCOORD2; //
	};

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.projPos = ComputeScreenPos(o.pos);
		o.eyeDepth = COMPUTE_EYEDEPTH(v.vertex);
		return o;
	}

	sampler2D _CameraDepthTexture;
	float _MaxDepth;
	half4 frag(v2f i) : SV_Target{
		float sceneZ = tex2Dproj(_CameraDepthTexture,
		UNITY_PROJ_COORD(i.projPos)).r;
	sceneZ = LinearEyeDepth(sceneZ) / _MaxDepth;
	//sceneZ = i.eyeDepth/ _MaxDepth;
	//sceneZ = UNITY_Z_0_FAR_FROM_CLIPSPACE(i.pos.z);
#if defined(UNITY_REVERSED_Z)
#else
	//sceneZ = 1.0f - sceneZ;
#endif
	//UNITY_OUTPUT_DEPTH(i.depth);
	half4 c;
	c.r = sceneZ;
	c.g = sceneZ;
	c.b = sceneZ;
	c.a = 1;

	return c;
	}
		ENDCG
	}
	}
		//FallBack "VertexLit"
}