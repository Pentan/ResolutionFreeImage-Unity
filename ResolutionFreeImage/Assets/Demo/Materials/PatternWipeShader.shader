Shader "Demo/PatternWipe"
{
    Properties
    {
        [PerShaderData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        
        [KeywordEnum(Local, World)] WIPE_COORD ("Coordinate", Float) = 0
        _PatternUnit ("Pattern Unit", Float) = 32.0
        _EdgeSmooth ("Edge Smooth", Range(0.0, 2.0)) = 0.5

        _WipeStep ("Wipe Step", Range(0.0, 2.0)) = 1.0
        [KeywordEnum(Checker, Regular Dots, Stagger Dots, Houndstooth)] WIPE_TYPE ("Pattern Type", Float) = 0
        _WipeOffset ("Wipe Offset", Vector) = (1.0, 0.5, 0.25, 0.125)
        [KeywordEnum(Left, Right, Top, Bottom)] WIPE_FROM ("Wipe Direction", Float) = 0
        [KeywordEnum(OneWay, Reverse)] WIPE_DISAPPEAR ("Wipe Disappear", Float) = 0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #include "../../Shaders/ShapeUtils.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #pragma shader_feature WIPE_COORD_LOCAL WIPE_COORD_WORLD
            // #pragma shader_feature __ FRGSHP_USE_SHADING
            // #pragma shader_feature FRGSHP_SHAPE_RECT FRGSHP_SHAPE_ROUNDRECT FRGSHP_SHAPE_TRIMEDRECT
            #pragma multi_compile WIPE_TYPE_CHECKER WIPE_TYPE_REGULAR_DOTS WIPE_TYPE_STAGGER_DOTS WIPE_TYPE_HOUNDSTOOTH
            #pragma multi_compile WIPE_FROM_LEFT WIPE_FROM_RIGHT WIPE_FROM_TOP WIPE_FROM_BOTTOM
            #pragma multi_compile WIPE_DISAPPEAR_ONEWAY WIPE_DISAPPEAR_REVERSE

            #include "../../Shaders/Patterns/Checker.cginc"
            #include "../../Shaders/Patterns/DotsRegular.cginc"
            #include "../../Shaders/Patterns/DotsStagger.cginc"
            #include "../../Shaders/Patterns/Houndstooth.cginc"
            // #include "../../Shaders/Patterns/TartanRegular.cginc"
            // #include "../../Shaders/Patterns/Argyle.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                // float2 uv2 : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 rectParams : TEXCOORD2;
                float2 rectUV : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _TextureSampledAdd;
            float4 _ClipRect;

            float _PatternUnit;
            fixed4 _PatternColor0;
            fixed4 _PatternColor1;
            fixed4 _PatternColor2;
            float4 _PatternParams0;
            float _EdgeSmooth;

            float _WipeStep;
            float4 _WipeOffset;

            // Vertex
            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(V);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.position = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                o.texcoord = TRANSFORM_TEX(v.uv0, _MainTex);
                o.worldPosition = v.vertex;
                o.rectParams.xy = v.uv0 * v.uv1;
                o.rectParams.zw = v.uv1;
                o.rectUV = v.uv0;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Colors
                fixed4 color = (tex2D(_MainTex, i.texcoord) + _TextureSampledAdd) * i.color;
                
                // color.rgb = float3(i.texcoord.x, i.texcoord.y, 0.0);
                #ifdef WIPE_COORD_LOCAL
                float2 rectcoord = i.rectParams.xy;
                #endif
                #ifdef WIPE_COORD_WORLD
                float2 rectcoord = i.worldPosition.xy;
                #endif

                float2 rectsize = i.rectParams.zw;

                float2 p = rectcoord / _PatternUnit;
                float es = _EdgeSmooth / _PatternUnit;

                // Wipe. Appear from 1.0
                // Pattern gradient (0.0,1.0)
                float ptngrad = 1.0;
                
                #ifdef WIPE_TYPE_CHECKER
                ptngrad = checkerDistance(p); // |max|:0.25
                ptngrad *= 4.0f;
                #endif
                #ifdef WIPE_TYPE_REGULAR_DOTS
                ptngrad = dotRegularDistance(p, 0.0); // max:sqrt(2)/2=sqrt(2)
                ptngrad = 1.0 - ptngrad * 1.41421; // sqrt(2)
                #endif
                #ifdef WIPE_TYPE_STAGGER_DOTS
                ptngrad = dotStaggerDistance(p, 0.0); // max:0.5
                ptngrad = 1.0 - ptngrad * 2.0;
                #endif
                #ifdef WIPE_TYPE_HOUNDSTOOTH
                ptngrad = houndstoothDistance(p); // max:0.25
                ptngrad *= 4.0;
                #endif

                ptngrad = max(ptngrad * _WipeOffset.x, -ptngrad * _WipeOffset.y);

                // Direction gradient (0.0, 1.0)
                float dirgrad;
                #ifdef WIPE_FROM_LEFT 
                dirgrad = 1.0 - i.rectUV.x;   // From Left
                #endif
                #ifdef WIPE_FROM_RIGHT
                dirgrad = i.rectUV.x;   // From Right
                #endif
                #ifdef WIPE_FROM_TOP
                dirgrad = i.rectUV.y;   // From Top
                #endif
                #ifdef WIPE_FROM_BOTTOM
                dirgrad = 1.0 - i.rectUV.y;   // From Bottom
                #endif

                #ifdef WIPE_DISAPPEAR_ONEWAY
                // Reverse direction gradient
                dirgrad = (_WipeStep > 1.0) ? (1.0 - dirgrad) : dirgrad;
                #endif
                #ifdef WIPE_DISAPPEAR_REVERSE
                // Noop
                #endif

                // Mix gradients
                float grad;
                grad = (dirgrad + ptngrad) / 2.0;

                float t = max(0.0, min(_WipeStep, 2.0 - _WipeStep));
                t = t * (1.0 + es * 2.0) - es;
                float thre = 1.0 - t;

                float wipe;
                wipe = edgeWeight(grad - thre, es);

                color.a = wipe;
                // color = fixed4(wipe, grad, step(grad, 0.0), 1.0);
                // color = fixed4(-d, d, exp(-d*d*5000.0), 1.0);
                // color = fixed4(i.rectUV.x, i.rectUV.y, 0.0, 1.0);

                //
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}
