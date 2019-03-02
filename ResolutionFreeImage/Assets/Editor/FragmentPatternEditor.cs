using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace ResFreeImage.UI {

    [CustomEditor(typeof(FragmentPattern))]
    [CanEditMultipleObjects]
    public class FragmentPatternEditor : GraphicEditor
    {
        // protected SerializedProperty textureProp;
        protected SerializedProperty marginProp;

        protected override void OnEnable() {
            base.OnEnable();

            // textureProp = serializedObject.FindProperty("texture");
            marginProp = serializedObject.FindProperty("margin");
        }

        public override void OnInspectorGUI() {
            // base.OnInspectorGUI();
            // DrawDefaultInspector();

            var fgcomp = (FragmentPattern)target;
            var canvas = fgcomp.gameObject.GetComponentInParent<Canvas>();

            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            {
                AppearanceControlsGUI();
                RaycastControlsGUI();
                
                EditorGUILayout.Space();
                // EditorGUILayout.PropertyField(textureProp);
                EditorGUILayout.PropertyField(marginProp, true);

                if((canvas.additionalShaderChannels & AdditionalCanvasShaderChannels.TexCoord1) == 0) {
                    EditorGUILayout.Space();
                    string msg = "FragmentShape use Texcoord1. Check \"Additional Shader Channel\" field of Canvas (" + canvas.name + ").";
                    EditorGUILayout.HelpBox(msg, MessageType.Error);
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
