// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "TCShaders/StylizedWaterSurface"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 0.5)
		_SpecularColor("specularColor", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
	    _DisplacementMap("DisplacementMap", 2D) = "black" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_Noise("Noise", 2D) = "white" {}
		_MoveXSpeed("general waves move speed x", Float) = 0
		_MoveZSpeed("general waves move speed z", Float) = 0
		_MoveXNoise("noise move speed x", Float) = 0
		_MoveZNoise("noise move speed z", Float) = 0
		_GeneralWaveDensity("general waves Density", Float) = 1
		_GeneralWaveAmplitude("general waves amplitude", Float) = 1
		_Shininess("specular", Float) = 1
		_Distortion("distortion", Float) = 1
		_WhitenessRange("whiteness", Vector) = (1, 1, 1, 1)			//min height, maz height, direction x, direction z
		_Collision1("collision parameters 1", Vector) = (0,0,1,1)	//startx, starty, depth, time
		_Collision2("collision parameters 2", Vector) = (0,0,1,1)	//startx, starty, depth, time
		_Collision3("collision parameters 3", Vector) = (0,0,1,1)	//startx, starty, depth, time
		_ColA("collisionWaveAmp", Float) = 4	
		_ColD("collisionWaveDensity", Float) = 50 
		_ColF("collisionWaveFrequency", Float) = -3.9
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 100
		ZWrite Off
		Cull off
		Pass
		{


			Blend SrcAlpha One , SrcAlpha OneMinusSrcAlpha // standard alpha blending
			CGPROGRAM
			#pragma target 4.5
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 viewDir : TEXCOORD1; //view direction
				half3 tspace0 : TEXCOORD2; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD3; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD4; // tangent.z, bitangent.z, normal.z

				float4 displacementuv : TEXCOORD5;	//displacement uv, dispalcement, , collision displacement
			};

			sampler2D _MainTex;
			sampler2D _DisplacementMap;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _SpecularColor;
			float4 _WhitenessRange;
			float4 _Collision1;
			float4 _Collision2;
			float4 _Collision3;
			float _ColA;
			float _ColF; 
			float _ColD;

			float _MoveXSpeed;
			float _MoveZSpeed;
			float _GeneralWaveAmplitude;
			float _GeneralWaveDensity;
			float  _Shininess;
			sampler2D _BumpMap;
			sampler2D _Noise;
			float _Distortion;
			float _MoveXNoise;
			float _MoveZNoise;
			
			float getColAmplitude(float2 vertxz, float4 params) {
				//collision displacement related
				float distanceFromCollision = length(vertxz - params.xy);
				float timeScale = params.z*pow(0.4, (1 + params.w + distanceFromCollision*0.4)) + 0.05*min(max(0, 1 - params.w * 0.03), 1);
				float colDisplacement = _ColA *timeScale*(sin(_ColD*distanceFromCollision + _ColF*params.w));
				if (distanceFromCollision > -_ColF*params.w / _ColD) {
					colDisplacement = 0;
				}
				return colDisplacement;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float2 noiseMovedUV = o.uv + float2(_Time.x*_MoveXNoise, _Time.x*_MoveZNoise);

				fixed distortX = tex2Dlod(_Noise, float4(noiseMovedUV + float2(0.01,0),0,0)).r - tex2Dlod(_Noise, float4(noiseMovedUV - float2(0.01, 0), 0, 0)).r;
				fixed distortY = tex2Dlod(_Noise, float4(noiseMovedUV + float2(0, 0.01), 0, 0)).r - tex2Dlod(_Noise, float4(noiseMovedUV - float2(0, 0.01), 0, 0)).r;
				
				o.displacementuv.xy = o.uv + float2(_Time.x*_MoveXSpeed, _Time.x*_MoveZSpeed) + float2(distortX*_Distortion, distortY*_Distortion);
				o.displacementuv.xy = o.displacementuv*_GeneralWaveDensity;
				o.displacementuv.z = (tex2Dlod(_DisplacementMap, float4(o.displacementuv.xy,0,0)).x - 0.5)*_GeneralWaveAmplitude;

				//collision displacement related
				float colDisplacement = getColAmplitude(v.vertex.xz, _Collision1);
				colDisplacement += getColAmplitude(v.vertex.xz, _Collision2);
				colDisplacement += getColAmplitude(v.vertex.xz, _Collision3);
				o.displacementuv.w = colDisplacement;
				
				//write the collision scale
				

				//apply the displacement
				o.vertex = UnityObjectToClipPos(v.vertex + float4(0,1,0,0)*(o.displacementuv.z + colDisplacement));
				o.viewDir.xyz = normalize(
					_WorldSpaceCameraPos - v.vertex.xyz);
				
				half3 wNormal = UnityObjectToWorldNormal(v.normal);
				half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
				// compute bitangent from cross product of normal and tangent
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				// output the tangent space matrix
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);


				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the normal map, and decode from the Unity encoding
				half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.displacementuv.xy));
				// transform normal from tangent to world space
				half3 normalDirection;
				normalDirection.x = dot(i.tspace0, tnormal);
				normalDirection.y = dot(i.tspace1, tnormal);
				normalDirection.z = dot(i.tspace2, tnormal);

				float3 viewDirection = normalize(i.viewDir.xyz);
				float3 lightDirection;
				float attenuation;
				if (0.0 == _WorldSpaceLightPos0.w) // directional light?
				{
					attenuation = 1.0; // no attenuation
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				}
				else // point or spot light
				{
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz
						- mul(unity_ObjectToWorld, i.vertex).xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // linear attenuation 
					lightDirection = normalize(vertexToLightSource);
				}

				float3 specularReflection;

					specularReflection = attenuation*_LightColor0.rgb*_SpecularColor.rgb * pow(max(0.0, dot(
						reflect(lightDirection, normalDirection),
						viewDirection)), _Shininess);
				
				float whiteness1 = pow(max(_WhitenessRange.y - i.displacementuv.z, 0) / (_WhitenessRange.y - _WhitenessRange.x),3);
				if (whiteness1 >= 1) {
					whiteness1 = 0;
				}


				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv)*_Color;
				col.xyz = specularReflection + col.xyz +
					whiteness1  * fixed4(0.5, 0.5, 0.5, 0.5)*(max(normalDirection.x*_WhitenessRange.z, 0) + max(normalDirection.z*_WhitenessRange.w, 0));// +
					//whiteness2*fixed4(1,1,1,1);
				
				//col.xyz = whiteness * fixed4(1,1,1,1);
				//col.xyz = normalDirection;
				//col.xyz = specularReflection;
				return col;
			}
			ENDCG
		}


	}
}
