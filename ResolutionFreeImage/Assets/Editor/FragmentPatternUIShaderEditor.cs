using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResFreeImage.UI {

    public class FragmentPatternUIShaderEditor : ShaderGUI
    {
        private struct PatternSpec {
            public string name;
            public bool useColor2;
            public string paramUsage;

            public PatternSpec(string nm, bool uc2, string[] pu) {
                name = nm;
                useColor2 = uc2;
                if(pu != null) {
                    paramUsage = string.Join("\n", pu);
                } else {
                    paramUsage = null;
                }
                
            }
        }

        static PatternSpec[] pattern01Specs = {
            new PatternSpec("Checker", false, null),
            new PatternSpec("RegularDots", false, new string[]{"x: Dot radius"}),
            new PatternSpec("StaggerDots", false, new string[]{"x: Dot radius"}),
            new PatternSpec("Houndstooth", false, null),
            new PatternSpec("SimpleTartan", true, new string[]{"x: Line 1 width", "y: Line 2 width"}),
            new PatternSpec("Argyle", true, new string[]{"x: Pattern aspect ratio", "y: Line width"})
        };

        //
        MaterialProperty mainTexProp = null;
        MaterialProperty tintColorProp = null;
        MaterialProperty patternUnitProp = null;
        MaterialProperty edgeSmoothProp = null;
        MaterialProperty patternTypeProp = null;
        MaterialProperty patternColor0Prop = null;
        MaterialProperty patternColor1Prop = null;
        MaterialProperty patternColor2Prop = null;
        MaterialProperty patternParams0Prop = null;
        MaterialProperty patternSetIdProp = null;

        MaterialProperty stencilCompProp = null;
        MaterialProperty stencilProp = null;
        MaterialProperty stencilOpProp = null;
        MaterialProperty stencilWriteMaskProp = null;
        MaterialProperty stencilReadMaskProp = null;
        MaterialProperty colorMaskProp = null;
        MaterialProperty useUIAlphaClipProp = null;

        public void FindRequireProperties(MaterialProperty[] props) {
            mainTexProp = FindProperty("_MainTex", props);
            tintColorProp = FindProperty("_Color", props);

            patternUnitProp = FindProperty("_PatternUnit", props);
            edgeSmoothProp = FindProperty("_EdgeSmooth", props);

            patternTypeProp = FindProperty("FRGPTN_TYPE", props);
            patternColor0Prop = FindProperty("_PatternColor0", props);
            patternColor1Prop = FindProperty("_PatternColor1", props);
            patternColor2Prop = FindProperty("_PatternColor2", props);
            patternParams0Prop = FindProperty("_PatternParams0", props);
            
            patternSetIdProp = FindProperty("_PatternSetId", props);

            stencilCompProp = FindProperty("_StencilComp", props);
            stencilProp = FindProperty("_Stencil", props);
            stencilOpProp = FindProperty("_StencilOp", props);
            stencilWriteMaskProp = FindProperty("_StencilWriteMask", props);
            stencilReadMaskProp = FindProperty("_StencilReadMask", props);
            colorMaskProp = FindProperty("_ColorMask", props);
            useUIAlphaClipProp = FindProperty("_UseUIAlphaClip", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
            // base.OnGUI(materialEditor, properties);
            FindRequireProperties(properties);
            
            PatternSpec[] speclist;
            switch((int)patternSetIdProp.floatValue) {
                case 0:
                default:
                    speclist = pattern01Specs;
                    break;
            }

            // EditorGUIUtility.labelWidth = 0.0f;
            materialEditor.SetDefaultGUIWidths();

            EditorGUI.BeginChangeCheck();
            {
                // makeDefaultShaderProperty(materialEditor, mainTexProp);
                makeDefaultShaderProperty(materialEditor, tintColorProp);

                EditorGUILayout.Space();
                makeDefaultShaderProperty(materialEditor, patternUnitProp);
                makeDefaultShaderProperty(materialEditor, edgeSmoothProp);

                var spec = speclist[(int)patternTypeProp.floatValue];
                EditorGUILayout.Space();
                makeDefaultShaderProperty(materialEditor, patternTypeProp);
                makeDefaultShaderProperty(materialEditor, patternColor0Prop);
                makeDefaultShaderProperty(materialEditor, patternColor1Prop);
                if(spec.useColor2) {
                    makeDefaultShaderProperty(materialEditor, patternColor2Prop);
                }
                if(spec.paramUsage != null) {
                    makeDefaultShaderProperty(materialEditor, patternParams0Prop);
                    EditorGUILayout.HelpBox(spec.paramUsage, MessageType.Info);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                makeDefaultShaderProperty(materialEditor, stencilCompProp);
                makeDefaultShaderProperty(materialEditor, stencilProp);
                makeDefaultShaderProperty(materialEditor, stencilOpProp);
                makeDefaultShaderProperty(materialEditor, stencilWriteMaskProp);
                makeDefaultShaderProperty(materialEditor, stencilReadMaskProp);
                makeDefaultShaderProperty(materialEditor, colorMaskProp);

                EditorGUILayout.Space();
                makeDefaultShaderProperty(materialEditor, useUIAlphaClipProp);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                materialEditor.RenderQueueField();
            }
            if(EditorGUI.EndChangeCheck()) {
                //
            }
        }

        private void makeDefaultShaderProperty(MaterialEditor materialEditor, MaterialProperty prop) {
            materialEditor.ShaderProperty(prop, prop.displayName);
        }
    }
}
