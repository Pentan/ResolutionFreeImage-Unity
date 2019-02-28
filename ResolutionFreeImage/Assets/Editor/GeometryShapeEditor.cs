using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace ResFreeImage.UI {

    [CustomEditor(typeof(GeometryShape))]
    [CanEditMultipleObjects]
    public class GeometryShapeEditor : GraphicEditor
    {
        protected SerializedProperty textureProp;
        protected SerializedProperty marginProp;
        protected SerializedProperty borderWidthProp;
        protected SerializedProperty borderColorProp;
        protected SerializedProperty shapeTypeProp;
        protected SerializedProperty cornerRadiusProp;
        protected SerializedProperty maxArcLengthProp;

        protected override void OnEnable() {
            base.OnEnable();

            textureProp = serializedObject.FindProperty("texture");
            marginProp = serializedObject.FindProperty("margin");
            borderWidthProp = serializedObject.FindProperty("borderWidth");
            borderColorProp = serializedObject.FindProperty("borderColor");
            shapeTypeProp = serializedObject.FindProperty("shapeType");
            cornerRadiusProp = serializedObject.FindProperty("cornerRadius");
            maxArcLengthProp = serializedObject.FindProperty("maxArcLength");
        }

        public override void OnInspectorGUI() {
            // base.OnInspectorGUI();
            // DrawDefaultInspector();

            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            {
                AppearanceControlsGUI();
                RaycastControlsGUI();
                
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(textureProp);
                EditorGUILayout.PropertyField(marginProp, true);
                EditorGUILayout.PropertyField(borderWidthProp);
                EditorGUILayout.PropertyField(borderColorProp);
                EditorGUILayout.PropertyField(shapeTypeProp);
                
                if(shapeTypeProp.enumValueIndex != (int)GeometryShape.ShapeType.Rectangle) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(cornerRadiusProp);
                    if(shapeTypeProp.enumValueIndex != (int)GeometryShape.ShapeType.TrimedRect) {
                        EditorGUILayout.PropertyField(maxArcLengthProp);
                    }
                    EditorGUI.indentLevel--;
                }
            }
            serializedObject.ApplyModifiedProperties();
            if(EditorGUI.EndChangeCheck()) {
                // noop
            }
        }

        // Start is called before the first frame update
        // void Start()
        // {
            
        // }

        // Update is called once per frame
        // void Update()
        // {
            
        // }
    }
}
