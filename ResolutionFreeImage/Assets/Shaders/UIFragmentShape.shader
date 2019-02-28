Shader "ResFreeImage/FragmentShapeUI"
{
    Properties
    {
        [PerShaderData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [KeywordEnum(Rect, RoundRect, TrimedRect)] FRGSHP_SHAPE ("Shape Type", Float) = 0
        _TrimWidth ("Trim Width", Float) = 4.0
        _EdgeSmooth ("Edge Smooth", Range(0.0, 2.0)) = 0.5

        _BornderWidth ("Border Width", Float) = 0.0
        _BorderColor ("BorderColor", Color) = (0.2,0.2,0.2,1)

        [Toggle(FRGSHP_USE_SHADING)] _UseShading ("Use Shading", Float) = 0
        [KeywordEnum(Multiply, Lerp)] FRGSHP_SHADING_TYPE ("Shading Type", Float) = 0
        _ShadingAmbient ("Shading Ambient", Color) = (0.5, 0.5, 0.5, 1.0)
        [KeywordEnum(Linear, Sphere, Power)] FRGSHP_SHADING_MAIN_PROFILE ("Main Shading Profile", Float) = 1
        _ShadingMainPowFactor ("Main Shading Pow Factor", FLoat) = 4.0
        [KeywordEnum(Linear, Sphere, Power)] FRGSHP_SHADING_BORDER_PROFILE ("Border Shading Profile", Float) = 1
        _ShadingBorderPowFactor ("Border Shading Pow Factor", FLoat) = 4.0

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

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #pragma shader_feature __ FRGSHP_USE_SHADING
            #pragma shader_feature FRGSHP_SHAPE_RECT FRGSHP_SHAPE_ROUNDRECT FRGSHP_SHAPE_TRIMEDRECT
            #pragma shader_feature FRGSHP_SHADING_TYPE_MULTIPLY FRGSHP_SHADING_TYPE_LERP
            #pragma shader_feature FRGSHP_SHADING_MAIN_PROFILE_LINEAR FRGSHP_SHADING_MAIN_PROFILE_SPHERE FRGSHP_SHADING_MAIN_PROFILE_POWER
            #pragma shader_feature FRGSHP_SHADING_BORDER_PROFILE_LINEAR FRGSHP_SHADING_BORDER_PROFILE_SPHERE FRGSHP_SHADING_BORDER_PROFILE_POWER

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

            float _EdgeSmooth;

            float4 _BorderColor;
            float _BornderWidth;
            float _TrimWidth;

            #ifdef FRGSHP_USE_SHADING
            fixed4 _ShadingAmbient;
                #ifdef FRGSHP_SHADING_MAIN_PROFILE_POWER
                float _ShadingMainPowFactor;
                #endif
                #ifdef FRGSHP_SHADING_BORDER_PROFILE_POWER
                float _ShadingBorderPowFactor;
                #endif
            #endif

            // Utilities
            float distanceFromShape(float2 p, float2 s, float r) {
                // Rect
                #ifdef FRGSHP_SHAPE_RECT
                float2 hs = s * 0.5;
                float2 cp = hs - abs(p - hs);
                return min(cp.x, cp.y);
                #endif

                // Round Rect
                #ifdef FRGSHP_SHAPE_ROUNDRECT
                float2 hs = s * 0.5;
                float2 ap = abs(p - hs);
                float2 cp = float2(hs.x - r, hs.y - r);
                float2 vac = ap - cp;

                float d0 = min(0.0, max(vac.x, vac.y));
                float d1 = length(max(float2(0.0, 0.0), vac));

                return r - (d0 + d1);
                #endif

                // Trimed Rect
                #ifdef FRGSHP_SHAPE_TRIMEDRECT
                float2 hs = s * 0.5;
                float2 ap = abs(p - hs);
                float2 rp = hs - ap;
                float td = dot(ap - (hs - float2(r, 0.0)), float2(-0.707106781, -0.707106781));
                return min(td, min(rp.x, rp.y));
                #endif
            }

            float modifyFactorToSphere(float f) {
                // Half Sphere
                float x = 1.0 - saturate(f);
                return sqrt(1.0 - x * x);
            }

            // float modifyFactorToParabola(float f) {
            //     // Parabola
            //     float x = 1.0 - saturate(f);
            //     return 1.0 - x * x;
            // }

            float modifyFactorToPow(float f, float p) {
                // Pow
                float x = 1.0 - saturate(f);
                return 1.0 - pow(x, p);
            }

            fixed3 applyShading(fixed3 rgb, fixed3 ambient, float factor) {
                #ifdef FRGSHP_SHADING_TYPE_MULTIPLY
                return rgb * lerp(ambient, fixed3(1.0, 1.0, 1.0), saturate(factor));
                #endif

                #ifdef FRGSHP_SHADING_TYPE_LERP
                return lerp(ambient, rgb, saturate(factor));
                #endif
            }

            float edgeWeight(float t) {
                return smoothstep(-_EdgeSmooth, _EdgeSmooth, t);
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

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Colors
                fixed4 basecolor = (tex2D(_MainTex, i.texcoord) + _TextureSampledAdd) * i.color;
                // color.rgb = float3(i.texcoord.x, i.texcoord.y, 0.0);
                fixed4 bordercol = _BorderColor;

                // Shape
                float2 rectcoord = i.rectParams.xy;
                float2 rectsize = i.rectParams.zw;
                float d = distanceFromShape(rectcoord, rectsize, _TrimWidth);
                d -= _EdgeSmooth;
                // d /= min(rectsize.x, rectsize.y) * 0.25;

                // Modify Colors
                #ifdef FRGSHP_USE_SHADING
                float shade;
                
                // Base
                shade = (d - _BornderWidth) / (_TrimWidth - _BornderWidth);
                
                #ifdef FRGSHP_SHADING_MAIN_PROFILE_LINEAR
                // noop
                #endif
                #ifdef FRGSHP_SHADING_MAIN_PROFILE_SPHERE
                shade = modifyFactorToSphere(shade);
                #endif
                #ifdef FRGSHP_SHADING_MAIN_PROFILE_POWER
                shade = modifyFactorToPow(shade, _ShadingMainPowFactor);
                #endif

                basecolor.rgb = applyShading(basecolor.rgb, _ShadingAmbient.rgb, shade);

                // Border
                shade = d / _BornderWidth;
                shade = min(shade, 1.0 - shade) * 2.0;
                #ifdef FRGSHP_SHADING_BORDER_PROFILE_LINEAR
                // noop
                #endif
                #ifdef FRGSHP_SHADING_BORDER_PROFILE_SPHERE
                shade = modifyFactorToSphere(shade);
                #endif
                #ifdef FRGSHP_SHADING_BORDER_PROFILE_POWER
                shade = modifyFactorToPow(shade, _ShadingBorderPowFactor);
                #endif

                bordercol.rgb = applyShading(bordercol.rgb, _ShadingAmbient.rgb, shade);
                #endif // FRGSHP_USE_SHADING

                // Composite
                float t;
                t = edgeWeight(d - _BornderWidth);
                fixed4 color = lerp(bordercol, basecolor, t);
                color.a *= edgeWeight(d);

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
