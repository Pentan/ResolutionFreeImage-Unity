Shader "ResFreeImage/FragmentPatterns01UI"
{
    Properties
    {
        [PerShaderData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [KeywordEnum(Local, World)] FRGPTN_COORD ("Coordinate", Float) = 0
        _PatternUnit ("Pattern Unit", Float) = 32.0
        _EdgeSmooth ("Edge Smooth", Range(0.0, 2.0)) = 0.5

        [KeywordEnum(Checker, Regular Dots, Stagger Dots, Houndstooth, Simple Tartan, Argyle)] FRGPTN_TYPE ("Pattern Type", Float) = 0
        _PatternColor0 ("Pattern Color0", Color) = (0.2,0.2,0.2,1)
        _PatternColor1 ("Pattern Color1", Color) = (0.5,0.5,0.5,1)
        _PatternColor2 ("Pattern Color2", Color) = (0.8,0.8,0.8,1)
        _PatternParams0 ("Pattern Params", Vector) = (1.0,1.0,1.0,1.0)
        
        [HideInInspector] _PatternSetId ("Pattern Set Id", Float) = 0

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

            #include "ShapeUtils.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #pragma shader_feature FRGPTN_COORD_LOCAL FRGPTN_COORD_WORLD
            // #pragma shader_feature __ FRGSHP_USE_SHADING
            // #pragma shader_feature FRGSHP_SHAPE_RECT FRGSHP_SHAPE_ROUNDRECT FRGSHP_SHAPE_TRIMEDRECT
            #pragma multi_compile FRGPTN_TYPE_CHECKER FRGPTN_TYPE_REGULAR_DOTS FRGPTN_TYPE_STAGGER_DOTS FRGPTN_TYPE_HOUNDSTOOTH FRGPTN_TYPE_SIMPLE_TARTAN FRGPTN_TYPE_ARGYLE
            
            #include "Patterns/Checker.cginc"
            #include "Patterns/DotsRegular.cginc"
            #include "Patterns/DotsStagger.cginc"
            #include "Patterns/Houndstooth.cginc"
            #include "Patterns/TartanRegular.cginc"
            #include "Patterns/Argyle.cginc"

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

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Colors
                fixed4 basecolor = (tex2D(_MainTex, i.texcoord) + _TextureSampledAdd) * i.color;
                // color.rgb = float3(i.texcoord.x, i.texcoord.y, 0.0);
                
                #ifdef FRGPTN_COORD_LOCAL
                float2 rectcoord = i.rectParams.xy;
                #endif
                #ifdef FRGPTN_COORD_WORLD
                float2 rectcoord = i.worldPosition.xy;
                #endif

                float2 rectsize = i.rectParams.zw;

                float2 p = rectcoord / _PatternUnit;
                float es = _EdgeSmooth / _PatternUnit;

                fixed4 patterncolor = fixed4(1.0, 0.0, 0.0, 1.0);

                #ifdef FRGPTN_TYPE_CHECKER
                patterncolor = checkerPattern(p, es, _PatternColor0, _PatternColor1);
                #endif
                #ifdef FRGPTN_TYPE_REGULAR_DOTS
                patterncolor = dotRegulatPattern(p, es, _PatternParams0.x, _PatternColor0, _PatternColor1);
                #endif
                #ifdef FRGPTN_TYPE_STAGGER_DOTS
                patterncolor = dotStaggerPattern(p, es, _PatternParams0.x, _PatternColor0, _PatternColor1);
                #endif
                #ifdef FRGPTN_TYPE_HOUNDSTOOTH
                patterncolor = houndstoothPattern(p, es, _PatternColor0, _PatternColor1);
                #endif
                #ifdef FRGPTN_TYPE_SIMPLE_TARTAN
                patterncolor = tartanRegulatPattern(p, es, _PatternParams0.xy, _PatternColor0, _PatternColor1, _PatternColor2);
                #endif
                #ifdef FRGPTN_TYPE_ARGYLE
                patterncolor = argylePattern(p, es, _PatternParams0.x, _PatternParams0.y, _PatternColor0, _PatternColor1, _PatternColor2);
                #endif

                fixed4 color = patterncolor * basecolor;

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

    CustomEditor "ResFreeImage.UI.FragmentPatternUIShaderEditor"
}
