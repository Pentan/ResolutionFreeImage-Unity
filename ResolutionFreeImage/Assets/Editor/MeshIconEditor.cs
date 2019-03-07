using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace ResFreeImage.UI {

    [CustomEditor(typeof(MeshIcon))]
    [CanEditMultipleObjects]
    public class MeshIconEditor : GraphicEditor
    {
        protected SerializedProperty textureProp;
        protected SerializedProperty sourceMeshProp;
        protected SerializedProperty marginProp;
        protected SerializedProperty scalingModeProp;
        protected SerializedProperty useMeshVertexColorProp;
        protected SerializedProperty removeBackFaceProp;
        protected SerializedProperty sortTrianglesProp;
        protected SerializedProperty keepMeshOriginProp;
        protected SerializedProperty keepMeshZProp;

        protected override void OnEnable() {
            base.OnEnable();

            textureProp = serializedObject.FindProperty("texture");
            sourceMeshProp = serializedObject.FindProperty("sourceMesh");
            marginProp = serializedObject.FindProperty("margin");
            scalingModeProp = serializedObject.FindProperty("scalingMode");
            useMeshVertexColorProp = serializedObject.FindProperty("useMeshVertexColor");
            removeBackFaceProp = serializedObject.FindProperty("removeBackFace");
            sortTrianglesProp = serializedObject.FindProperty("sortTriangles");
            keepMeshOriginProp = serializedObject.FindProperty("keepMeshOrigin");
            keepMeshZProp = serializedObject.FindProperty("keepMeshZ");
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
                EditorGUILayout.PropertyField(sourceMeshProp);
                
                EditorGUILayout.PropertyField(marginProp, true);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(scalingModeProp);
                EditorGUILayout.PropertyField(useMeshVertexColorProp);
                EditorGUILayout.PropertyField(removeBackFaceProp);
                EditorGUILayout.PropertyField(sortTrianglesProp);
                EditorGUILayout.PropertyField(keepMeshOriginProp);
                EditorGUILayout.PropertyField(keepMeshZProp);
            }
            serializedObject.ApplyModifiedProperties();
            if(EditorGUI.EndChangeCheck()) {
                // noop
            }
        }
    }
}
