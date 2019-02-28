using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ResFreeImage.UI {

    public class FragmentShapeUIShaderEditor : ShaderGUI
    {
        MaterialProperty mainTexProp = null;
        MaterialProperty tintColorProp = null;
        MaterialProperty shapeTypeProp = null;
        MaterialProperty borderColorProp = null;
        MaterialProperty borderWidthProp = null;
        MaterialProperty trimWidthProp = null;
        MaterialProperty edgeSmoothProp = null;
        MaterialProperty useShadingProp = null;
        MaterialProperty shadingTypeProp = null;
        MaterialProperty shadingAmbientProp = null;
        MaterialProperty shadingMainProfileProp = null;
        MaterialProperty shadingMainPowFactorProp = null;
        MaterialProperty shadingBorderProfileProp = null;
        MaterialProperty shadingBorderPowFactorProp = null;

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
            shapeTypeProp = FindProperty("FRGSHP_SHAPE", props);
            borderColorProp = FindProperty("_BorderColor", props);
            borderWidthProp = FindProperty("_BornderWidth", props);
            trimWidthProp = FindProperty("_TrimWidth", props);
            edgeSmoothProp = FindProperty("_EdgeSmooth", props);
            useShadingProp = FindProperty("_UseShading", props);
            shadingTypeProp = FindProperty("FRGSHP_SHADING_TYPE", props);
            shadingAmbientProp = FindProperty("_ShadingAmbient", props);
            shadingMainProfileProp = FindProperty("FRGSHP_SHADING_MAIN_PROFILE", props);
            shadingMainPowFactorProp = FindProperty("_ShadingMainPowFactor", props);
            shadingBorderProfileProp = FindProperty("FRGSHP_SHADING_BORDER_PROFILE", props);
            shadingBorderPowFactorProp = FindProperty("_ShadingBorderPowFactor", props);

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

            // EditorGUIUtility.labelWidth = 0.0f;
            materialEditor.SetDefaultGUIWidths();

            EditorGUI.BeginChangeCheck();
            {
                makeDefaultShaderProperty(materialEditor, mainTexProp);
                makeDefaultShaderProperty(materialEditor, tintColorProp);

                EditorGUILayout.Space();
                makeDefaultShaderProperty(materialEditor, shapeTypeProp);
                makeDefaultShaderProperty(materialEditor, trimWidthProp);
                makeDefaultShaderProperty(materialEditor, edgeSmoothProp);

                EditorGUILayout.Space();
                makeDefaultShaderProperty(materialEditor, borderColorProp);
                makeDefaultShaderProperty(materialEditor, borderWidthProp);

                EditorGUILayout.Space();
                makeDefaultShaderProperty(materialEditor, useShadingProp);
                if(useShadingProp.floatValue != 0.0f) {
                    EditorGUI.indentLevel++;
                    makeDefaultShaderProperty(materialEditor, shadingTypeProp);
                    makeDefaultShaderProperty(materialEditor, shadingAmbientProp);

                    makeDefaultShaderProperty(materialEditor, shadingMainProfileProp);
                    if(shadingMainProfileProp.floatValue == 2.0f) {
                        EditorGUI.indentLevel++;
                        makeDefaultShaderProperty(materialEditor, shadingMainPowFactorProp);
                        EditorGUI.indentLevel--;
                    }

                    makeDefaultShaderProperty(materialEditor, shadingBorderProfileProp);
                    if(shadingBorderProfileProp.floatValue == 2.0f) {
                        EditorGUI.indentLevel++;
                        makeDefaultShaderProperty(materialEditor, shadingBorderPowFactorProp);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
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
