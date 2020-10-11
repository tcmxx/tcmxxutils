
Shader "TCShaders/ScreenSpaceShine"
{
    Properties
    {
        //[PerRendererData]
        _MainTex ("_MainTex", 2D) = "white" {}
        _Color ("COLOR", Color) = (1,1,1,1)
        //[PerRendererData]
        _Lighter ("MoreLighter", Range(0.2,1.8)) = 1.15
        _CircleShineParams("Circle Shine Parameters (x, y, radius, width)", Vector) = (0, 0, 40, 0.05)    //(x, y, radius, width), all in screenspace (0~1) (radius and width use the screen width as reference for 1)
        _LineShineParams("Line Shine Parameters (directionx, directiony, distance to origin, width)", Vector) = (0, 0, 40, 0.05)        //(directionx, directiony, distance to origin, width),  all in screenspace (0~1)(distance to origin and width use the screen width as reference for 1)
        _ShineTex("Shine Texture", 2D) = "white" {}
        _ShineColor("Shine Color", Color) = (1,1,1,1)
        // these six unused properties are required when a shader
        // is used in the UI system, or you get a warning.
        // look to UI-Default.shader to see these.
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }


    SubShader {
        Tags
        { 
            "CanUseSpriteAtlas"="True"
            "IgnoreProjector"="True" 
            "PreviewType"="Plane"
            "Queue"="Transparent" 
            "RenderType"="Transparent" 

        }

        Cull Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite  Off
        ColorMask[_ColorMask]
        /* UI */
        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
        /* -- */


   Pass {
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
          float4 vertex   : SV_POSITION;
          float2 uv : TEXCOORD0;
          float2 viewPos : TEXCOORD1;
        };

        sampler2D _MainTex;
        sampler2D _ShineTex;
        float4 _MainTex_ST;
        float4 _Color;
        float4 _ShineColor;
        float _Lighter;
        float4 _CircleShineParams;
        float4 _LineShineParams;

        float4 circleShineColor(float2 pos)
        {
            float2 dist = pos - _CircleShineParams.xy;
            dist.y *= _ScreenParams.y / _ScreenParams.x;
            float normalizedDistToRadius = (_CircleShineParams.z - length(dist)) / _CircleShineParams.w;

            //if the diff between the radius of current pixel and the required radius is in range of [-1,0] after normalzied with the shine width
            //we show the shine color
            float inRange = step(-1, normalizedDistToRadius) * (1 - step(0, normalizedDistToRadius));   //inRange is 1 if normalizedDistToRadius is within [-1,0]
            return inRange * tex2D(_ShineTex, float2(0.5f, normalizedDistToRadius + 1)) * _ShineColor;
        }

        float4 lineShineColor(float2 pos)
        {
            pos.y *= _ScreenParams.y / _ScreenParams.x;

            float2 dirNormalized = normalize(_LineShineParams.xy);
            float dist = dot(dirNormalized, pos);
            
            float normalizedDistToLine = (_LineShineParams.z - dist) / _LineShineParams.w;

            float inRange = step(-1, normalizedDistToLine) * (1 - step(0, normalizedDistToLine));   //inRange is 1 if normalizedDistToRadius is within [-1,0]
            return inRange * tex2D(_ShineTex, float2(0.5f, normalizedDistToLine + 1)) * _ShineColor;
        }

        v2f vert (appdata v)
        {
          v2f o;
          o.vertex = UnityObjectToClipPos(v.vertex);
          o.uv = TRANSFORM_TEX(v.uv, _MainTex) ;
          o.viewPos = 0.5 * (o.vertex.xy / o.vertex.w + 1); //map to 0-1 screen space
          return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 color = tex2D(_MainTex, i.uv)* _Color; 
            color.rgb += float3((_Lighter - 1.0),(_Lighter - 1.0),(_Lighter - 1.0));

            float4 colorCircleShine = circleShineColor(i.viewPos);
            float4 colorLineShine = lineShineColor(i.viewPos);
            color.rgb = lerp(color.rgb, colorCircleShine.rgb, colorCircleShine.a);
            color.rgb = lerp(color.rgb, colorLineShine.rgb, colorLineShine.a);
            return color;

        }
       



        ENDCG

      }  
   
   
   
   } 
}

