Shader "ResFreeImage/FragmentShapeUI"
{
    Properties
    {
        [PerShaderData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [KeywordEnum(Rect, Capsule, RoundRect, TrimedRect)] FRGSHP_SHAPE ("Shape Type", Float) = 0
        _EdgeSmooth ("Edge Smooth", Range(0.0, 2.0)) = 0.5
        
        _CornerRadius ("Corner Radius", Float) = 8.0
        _BornderWidth ("Border Width", Float) = 0.0
        [Toggle(FRGSHP_UV2OVERRIDE)] _UV2Override ("Override above params by Componet.", Float) = 0

        _BorderColor ("BorderColor", Color) = (0.2,0.2,0.2,1)

        [Toggle(FRGSHP_USE_SHADING)] _UseShading ("Use Shading", Float) = 0
        [KeywordEnum(Multiply, Lerp)] FRGSHP_SHADING_TYPE ("Shading Type", Float) = 0
        _ShadingAmbient ("Shading Ambient", Color) = (0.5, 0.5, 0.5, 1.0)
        [KeywordEnum(Linear, Sphere, Parabola, Power)] FRGSHP_SHADING_MAIN_PROFILE ("Main Shading Profile", Float) = 1
        _ShadingMainWidth ("Main Shading Width", FLoat) = 8.0
        _ShadingMainPowFactor ("Main Shading Pow Factor", FLoat) = 4.0
        [KeywordEnum(Linear, Sphere, Parabola, Power)] FRGSHP_SHADING_BORDER_PROFILE ("Border Shading Profile", Float) = 1
        _ShadingBorderPowFactor ("Border Shading Pow Factor", FLoat) = 4.0

        [Toggle(FRGSHP_USE_OUTLINEGLOW)] _UseOutlineGlow ("Use Outline Glow", Float) = 0
        _OutlineGlowWidth ("Outline glow width", Float) = 8.0
        _OutlineGlowColor ("Outline glow color", Color) = (0.0, 0.0, 0.0, 0.5)
        _OutlineGlowFade ("Outline glow fade factor", Float) = 2.0

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
            #include "ShapeFuncs.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #pragma shader_feature __ FRGSHP_USE_SHADING
            #pragma shader_feature __ FRGSHP_UV2OVERRIDE
            #pragma shader_feature __ FRGSHP_USE_OUTLINEGLOW
            #pragma shader_feature FRGSHP_SHAPE_RECT FRGSHP_SHAPE_CAPSULE FRGSHP_SHAPE_ROUNDRECT FRGSHP_SHAPE_TRIMEDRECT
            #pragma shader_feature FRGSHP_SHADING_TYPE_MULTIPLY FRGSHP_SHADING_TYPE_LERP
            #pragma shader_feature FRGSHP_SHADING_MAIN_PROFILE_LINEAR FRGSHP_SHADING_MAIN_PROFILE_SPHERE FRGSHP_SHADING_MAIN_PROFILE_PARABOLA FRGSHP_SHADING_MAIN_PROFILE_POWER
            #pragma shader_feature FRGSHP_SHADING_BORDER_PROFILE_LINEAR FRGSHP_SHADING_BORDER_PROFILE_SPHERE FRGSHP_SHADING_BORDER_PROFILE_PARABOLA FRGSHP_SHADING_BORDER_PROFILE_POWER

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1; // Shape bound (width, height)
                float2 uv2 : TEXCOORD2; // Override (corner radius, border width)
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 rectParams : TEXCOORD2;
                float2 shapeParams : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _TextureSampledAdd;
            float4 _ClipRect;

            float _EdgeSmooth;

            float4 _BorderColor;
            float _BornderWidth;
            float _CornerRadius;

            #ifdef FRGSHP_USE_SHADING
            fixed4 _ShadingAmbient;
            float _ShadingMainWidth;
                #ifdef FRGSHP_SHADING_MAIN_PROFILE_POWER
                float _ShadingMainPowFactor;
                #endif
                #ifdef FRGSHP_SHADING_BORDER_PROFILE_POWER
                float _ShadingBorderPowFactor;
                #endif
            #endif

            #ifdef FRGSHP_USE_OUTLINEGLOW
            float _OutlineGlowWidth;
            fixed4 _OutlineGlowColor;
            float _OutlineGlowFade;
            #endif

            // Utilities
            float distanceFromShape(float2 p, float2 s, float r) {
                // Rect
                #ifdef FRGSHP_SHAPE_RECT
                return distanceFromRect(p, s);
                #endif
                
                // Capsule
                #ifdef FRGSHP_SHAPE_CAPSULE
                return distanceFromCapsule(p, s);
                #endif

                // Round Rect
                #ifdef FRGSHP_SHAPE_ROUNDRECT
                return distanceFromRoundRect(p, s, r);
                #endif

                // Trimed Rect
                #ifdef FRGSHP_SHAPE_TRIMEDRECT
                return distanceFromTrimedRect(p, s, r);
                #endif
            }

            fixed3 applyShading(fixed3 rgb, fixed3 ambient, float factor) {
                #ifdef FRGSHP_SHADING_TYPE_MULTIPLY
                return rgb * lerp(ambient, fixed3(1.0, 1.0, 1.0), saturate(factor));
                #endif

                #ifdef FRGSHP_SHADING_TYPE_LERP
                return lerp(ambient, rgb, saturate(factor));
                #endif
            }

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

                #ifdef FRGSHP_UV2OVERRIDE
                o.shapeParams = v.uv2;
                #else
                o.shapeParams = float2(-1.0, -1.0);
                #endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Params
                float cornerR;
                float borderW;

                #ifdef FRGSHP_UV2OVERRIDE
                cornerR = (i.shapeParams.x < 0.0) ? _CornerRadius : i.shapeParams.x;
                borderW = (i.shapeParams.y < 0.0) ? _BornderWidth : i.shapeParams.y;
                #else
                cornerR = _CornerRadius;
                borderW = _BornderWidth;
                #endif

                // Colors
                fixed4 basecolor = (tex2D(_MainTex, i.texcoord) + _TextureSampledAdd) * i.color;
                // color.rgb = float3(i.texcoord.x, i.texcoord.y, 0.0);
                fixed4 bordercol = _BorderColor * i.color;

                // Shape
                float2 rectcoord = i.rectParams.xy;
                float2 rectsize = i.rectParams.zw;
                #ifdef FRGSHP_USE_OUTLINEGLOW
                rectcoord -= float2(_OutlineGlowWidth, _OutlineGlowWidth);
                rectsize -= float2(_OutlineGlowWidth, _OutlineGlowWidth) * 2.0;
                #endif
                float d = distanceFromShape(rectcoord, rectsize, cornerR);
                d -= _EdgeSmooth;
                // d /= min(rectsize.x, rectsize.y) * 0.25;

                // Modify Colors
                #ifdef FRGSHP_USE_SHADING
                float shade;
                
                // Base
                shade = (d - borderW) / (_ShadingMainWidth - borderW);
                
                #ifdef FRGSHP_SHADING_MAIN_PROFILE_LINEAR
                // noop
                #endif
                #ifdef FRGSHP_SHADING_MAIN_PROFILE_SPHERE
                shade = modifyFactorToSphere(shade);
                #endif
                #ifdef FRGSHP_SHADING_MAIN_PROFILE_PARABOLA
                shade = modifyFactorToParabola(shade);
                #endif
                #ifdef FRGSHP_SHADING_MAIN_PROFILE_POWER
                shade = modifyFactorToPow(shade, _ShadingMainPowFactor);
                #endif

                basecolor.rgb = applyShading(basecolor.rgb, _ShadingAmbient.rgb, shade);

                // Border
                shade = d / borderW;
                shade = min(shade, 1.0 - shade) * 2.0;
                #ifdef FRGSHP_SHADING_BORDER_PROFILE_LINEAR
                // noop
                #endif
                #ifdef FRGSHP_SHADING_BORDER_PROFILE_SPHERE
                shade = modifyFactorToSphere(shade);
                #endif
                #ifdef FRGSHP_SHADING_BORDER_PROFILE_PARABOLA
                shade = modifyFactorToParabola(shade);
                #endif
                #ifdef FRGSHP_SHADING_BORDER_PROFILE_POWER
                shade = modifyFactorToPow(shade, _ShadingBorderPowFactor);
                #endif

                bordercol.rgb = applyShading(bordercol.rgb, _ShadingAmbient.rgb, shade);
                #endif // FRGSHP_USE_SHADING

                // Composite
                float t;
                t = edgeWeight(d - borderW, _EdgeSmooth);
                fixed4 color = lerp(bordercol, basecolor, t);
                color.a *= edgeWeight(d, _EdgeSmooth);
                
                // Outline glow
                #ifdef FRGSHP_USE_OUTLINEGLOW
                t = pow(saturate(1.0 - max(0.0, -d / _OutlineGlowWidth)), _OutlineGlowFade);
                fixed4 glowcol = _OutlineGlowColor;
                glowcol.a = lerp(0.0, _OutlineGlowColor.a, t);
                color.rgb = lerp(glowcol.rgb, color.rgb, edgeWeight(d, _EdgeSmooth));
                color.a = max(color.a, glowcol.a * smoothstep(_EdgeSmooth, 0.0, d));
                #endif

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

    CustomEditor "ResFreeImage.UI.FragmentShapeUIShaderEditor"
}
