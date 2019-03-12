using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using ResFreeImage.UI;

namespace ResFreeImage.Demo {

    public class DemoWipeController : MonoBehaviour
    {
        private class PatternDesc {
            public string keyword;
            public float unit;
            public Vector4 offset;
            
            public PatternDesc(string k, float u, float x, float y=1.0f, float z=1.0f, float w=1.0f) {
                keyword = k;
                unit = u;
                offset = new Vector4(x, y, z, w);
                
            }
        }

        static private readonly PatternDesc[] WipePatterns = new PatternDesc[]{
            new PatternDesc("WIPE_TYPE_CHECKER", 128.0f, 1.0f, 0.5f),
            new PatternDesc("WIPE_TYPE_REGULAR_DOTS", 64.0f, 1.0f),
            new PatternDesc("WIPE_TYPE_STAGGER_DOTS", 96.0f, 1.0f),
            new PatternDesc("WIPE_TYPE_HOUNDSTOOTH", 72.0f, 1.0f, 0.5f)
        };
        static private readonly string[] WipeDirKeyWords = new string[]{
            "WIPE_FROM_LEFT",
            // "WIPE_FROM_RIGHT",
            "WIPE_FROM_TOP",
            "WIPE_FROM_BOTTOM"
        };

        static private DemoWipeController _currentController;
        static public DemoWipeController CurrentController {
            get {
                return _currentController;
            }
        }

        //
        public GameObject wipeCanvasObject;
        public Graphic wipeGraphic;
        public float WipeDuration = 0.35f;

        //
        [System.Serializable]
        public class WipeEvent : UnityEvent {}

        public WipeEvent OnWipeStart = new WipeEvent();
        public WipeEvent OnWipeClosed = new WipeEvent();
        public WipeEvent OnWipeResume = new WipeEvent();
        public WipeEvent OnWipeOpened = new WipeEvent();

        public enum WipeState {
            Opened,
            Closing,
            Closed,
            Opening
        }

        private WipeState currentState = WipeState.Opened;
        public WipeState CurrentWipeState {
            get {
                return currentState;
            }
        }

        private Coroutine  currentCoroutine;
        private System.Random sysrand = null;

        private Material wipeMaterial;
        private struct wipeMatProps {
            static readonly public int patternUnitId = Shader.PropertyToID("_PatternUnit");
            static readonly public int wipeStepId = Shader.PropertyToID("_WipeStep");
            static readonly public int wipeOffsetId = Shader.PropertyToID("_WipeOffset");
        }

        //
        void Awake() {
            _currentController = this;
            sysrand = new System.Random();
        }

        void Start()
        {
            wipeMaterial = wipeGraphic.material;
            wipeCanvasObject.SetActive(false);
        }

        // void Update()
        // {
        // }

        //
        public void StartWipe(float delay=0.0f) {

            switch(currentState) {
                case WipeState.Closing:
                case WipeState.Closed:
                    return;
            }

            if(currentCoroutine != null) {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }


            currentCoroutine = StartCoroutine(WipeCloseRoutine(delay));
        }
        
        public void ResumeWipe(float delay=0.0f) {

            switch(currentState) {
                case WipeState.Opening:
                case WipeState.Opened:
                    return;
            }

            if(currentCoroutine != null) {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            currentCoroutine = StartCoroutine(WipeOpenRoutine(delay));
        }

           

        //
        private IEnumerator WipeCloseRoutine(float delay) {
            var startTime = Time.time;

            int wipeid = sysrand.Next(WipePatterns.Length);
            for(int i = 0; i < WipePatterns.Length; i++) {
                var wp = WipePatterns[i];
                if(i == wipeid) {
                    wipeMaterial.SetVector(wipeMatProps.wipeOffsetId, wp.offset);
                    wipeMaterial.SetFloat(wipeMatProps.patternUnitId, wp.unit);
                    wipeMaterial.EnableKeyword(wp.keyword);
                } else {
                    if(wipeMaterial.IsKeywordEnabled(wp.keyword)) {
                        wipeMaterial.DisableKeyword(wp.keyword);
                    }
                }
            }

            int dirid = sysrand.Next(WipeDirKeyWords.Length);
            for(int i = 0; i < WipeDirKeyWords.Length; i++) {
                if(i == dirid) {
                    wipeMaterial.EnableKeyword(WipeDirKeyWords[i]);
                } else {
                    wipeMaterial.DisableKeyword(WipeDirKeyWords[i]);
                }
            }

            // Initialize
            wipeCanvasObject.SetActive(true);
            SetWipeStep(0.0f);

            // start delay
            if(delay > 0.0f) {
                yield return new WaitForSeconds(delay);

                // Resign start time
                startTime = Time.time;
            }

            // Start
            currentState = WipeState.Closing;
            OnWipeStart.Invoke();

            float timePast;
            float t;
            do {
                yield return null;
                timePast = Time.time - startTime;
                t = timePast / WipeDuration;
                SetWipeStep(Mathf.Min(1.0f, t));
            } while(timePast < WipeDuration);

            SetWipeStep(1.0f);
            yield return null;

            currentState = WipeState.Closed;
            OnWipeClosed.Invoke();

            currentCoroutine = null;
        }
        
        private IEnumerator WipeOpenRoutine(float delay) {
            var startTime = Time.time;

            // Initialize
            SetWipeStep(1.0f);
            
            // start delay
            if(delay > 0.0f) {
                yield return new WaitForSeconds(delay);

                // Resign start time
                startTime = Time.time;
            }

            // Start
            currentState = WipeState.Opening;
            OnWipeResume.Invoke();

            float timePast;
            float t;
            do {
                yield return null;
                timePast = Time.time - startTime;
                t = 1.0f + timePast / WipeDuration;
                SetWipeStep(Mathf.Min(2.0f, t));
            } while(timePast < WipeDuration);

            SetWipeStep(2.0f);
            yield return null;

            currentState = WipeState.Opened;
            OnWipeOpened.Invoke();

            wipeCanvasObject.SetActive(false);

            wipeCanvasObject.SetActive(false);
            currentCoroutine = null;
        }

        // t = 0.0(opened), 1.0(closed), 2.0(opened)
        private void SetWipeStep(float t) {
            // Simple fade

            float floor = Mathf.Floor(t);
            float frac = t - floor;
            float step = Mathf.SmoothStep(0.0f, 1.0f, frac) + floor;

            // Color wipecol = wipeGraphic.color;
            // wipecol.a = a;
            // wipeGraphic.color = wipecol;
            wipeMaterial.SetFloat(wipeMatProps.wipeStepId, step);
        }
    }
}
