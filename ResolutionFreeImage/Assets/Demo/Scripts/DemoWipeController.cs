using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using ResFreeImage.UI;

namespace ResFreeImage.Demo {

    public class DemoWipeController : MonoBehaviour
    {
        static private DemoWipeController _currentController;
        static public DemoWipeController CurrentController {
            get {
                return _currentController;
            }
        }

        //
        public GameObject wipeCanvasObject;
        public Graphic wipeGraphic;

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

        //+++++
        // FIXME? Simple fade wipe
        private const float kCloseTime = 0.2f;
        private const float kOpenTime = 0.2f;
        //+++++

        //
        void Awake() {
            _currentController = this;
        }

        // void Start()
        // {
        // }

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
                t = timePast / kCloseTime;
                SetWipeStep(Mathf.Min(1.0f, t));
            } while(timePast < kCloseTime);

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
                t = 1.0f + timePast / kOpenTime;
                SetWipeStep(Mathf.Min(2.0f, t));
            } while(timePast < kOpenTime);

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

            t = Mathf.Clamp(t, 0.0f, 2.0f);
            t = Mathf.Min(t, 2.0f - t);
            float a = Mathf.SmoothStep(0.0f, 1.0f, t);

            Color wipecol = wipeGraphic.color;
            wipecol.a = a;
            wipeGraphic.color = wipecol;
        }
    }
}
