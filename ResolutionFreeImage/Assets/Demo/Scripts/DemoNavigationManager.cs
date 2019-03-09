using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using UnityEngine.Events;

using ResFreeImage.UI;

namespace ResFreeImage.Demo {
    public class DemoNavigationManager : MonoBehaviour
    {
        static private DemoNavigationManager _currentManager = null;
        static public DemoNavigationManager currentNavigationManager {
            get { return _currentManager;}
        }

        public NavSceneMenuController menuController;

        public float wipeStartDelay = 0.2f;

        private string[] scenePaths = {
            "Demo/Scenes/DemoGeometry",
            "Demo/Scenes/DemoFragment",
            "Demo/Scenes/DemoMeshIcon",
            "Demo/Scenes/DemoTitle"
        };
        private int startupSceneIndex = 3; // title
        private int requestedSceneIndex = -1;
        private int currentSceneIndex = 0;

        void Awake() {
            _currentManager = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            // SceneManager events
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnLoaded;

            // Wipe events
            DemoWipeController.CurrentController.OnWipeClosed.AddListener(OnWipeClosed);
            DemoWipeController.CurrentController.OnWipeOpened.AddListener(OnWipeFinished);

            // Load initial scene or not
            if(startupSceneIndex >= 0) {
                // Remove EventSystem
                var es = FindObjectOfType<EventSystem>();
                Destroy(es.gameObject);

                // Load start up scene
                LoadSelectedScene(startupSceneIndex);

            } else {
                // Solo
                var es = FindObjectOfType<EventSystem>();
                if(es == null){
                    // No EventSystem found
                    var comps = new System.Type[] {
                        typeof(EventSystem),
                        typeof(StandaloneInputModule)
                    };
                    var go = new GameObject("Navigation Root EventSystem",  comps);
                }
            }
        }

        // Update is called once per frame
        // void Update()
        // {
        // }

        // Utilities
        private void LoadSelectedScene(int sceneid) {
            if(sceneid < 0 || sceneid >= scenePaths.Length) {
                Debug.Log("Undefined scene requested. index:" + sceneid);
            }

            string path = scenePaths[sceneid];
            SceneManager.LoadScene(path, LoadSceneMode.Additive);
        }

        // Events
        // Menu changed
        public void OnSceneSelectorChanged(int selectedId, GameObject sender) {
            // Debug.Log("Scene selected:" + selectedId);

            requestedSceneIndex = selectedId;

            // Wipe start
            DemoWipeController.CurrentController.StartWipe(wipeStartDelay);
        }
        
        // Wipe
        public void OnWipeClosed() {
            // Debug.Log("Wipe closed");
            // Begin load
            LoadSelectedScene(requestedSceneIndex);
        }

        public void OnWipeFinished() {
            // Debug.Log("Wipe finished");
            // Close menu
            menuController.SetMenuOpen(false);
        }

        // SceneManager
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            // Debug.Log("Scene " + scene.name + " loaded in mode " + mode);
            // Swap scene
            var curcene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(scene);
            if(SceneManager.sceneCount > 2) {
                SceneManager.UnloadSceneAsync(curcene);
            }
            currentSceneIndex = requestedSceneIndex;
            requestedSceneIndex = -1;

            // Wipe open
            DemoWipeController.CurrentController.ResumeWipe();
        }

        public void OnSceneUnLoaded(Scene scene) {
            // Debug.Log("Scene " + scene.name + " unloaded");
        }

        //+++
        // public void OnSomethingDebugEvent(string message) {
        //     Debug.Log(message);
        // }
    }
}