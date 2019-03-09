using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ResFreeImage.Demo {
    public class NavSceneMenuController : MonoBehaviour
    {
        public GameObject menuBlocker;
        public GameObject menuExtraCloseButton;

        public Toggle menuOpenToggle;

        private Animator animator;

        private struct TriggerHash {
            static public readonly int Shrink = Animator.StringToHash("Shrink");
            static public readonly int Expand = Animator.StringToHash("Expand");
        }

        // Start is called before the first frame update
        void Start()
        {
            // Set animator events
            animator = GetComponent<Animator>();
            var behaviours = animator.GetBehaviours<AnimStateProvider>();
            foreach(var bhv in behaviours) {
                // Debug.Log("Add listener to AnimStateProvider:" + bhv.userTag);
                bhv._onStateExitEvent.AddListener(OnStableStateExited);
                bhv._onStateEnterEvent.AddListener(OnStableStateEntered);
            }

            // Set close button event
            var btn = menuExtraCloseButton.GetComponent<Button>();
            btn.onClick.AddListener(OnCloseButtonClicked);

            // First time, Don't close when clicked out of menu.
            menuExtraCloseButton.SetActive(false);
        }

        // Update is called once per frame
        // void Update()
        // {
        // }

        // Utilities
        public void SetMenuOpen(bool isopen) {
            if(isopen != menuOpenToggle.isOn) {
                menuOpenToggle.isOn = isopen;
            }
        }

        // Toggle event
        public void OnToggleChanged(bool ison) {
            // Debug.Log("Toggle Menu open:" + ison);
            
            if(ison) {
                animator.SetTrigger(TriggerHash.Expand);
            } else {
                animator.SetTrigger(TriggerHash.Shrink);
                menuExtraCloseButton.SetActive(false);
            }
        }

        // Close button
        public void OnCloseButtonClicked() {
            // Debug.Log("Force close");
            menuOpenToggle.isOn = false;
        }

        // Animation state event
        public void OnStableStateEntered(string usertag, AnimStateProvider.StateInfo info) {
            // Debug.Log("OnStableStateEntered tag:" + usertag);
            menuBlocker.SetActive(false);
            if(usertag.Equals("Expanded")) {
                menuExtraCloseButton.SetActive(true);
            }
        }

        public void OnStableStateExited(string usertag, AnimStateProvider.StateInfo info) {
            // Debug.Log("OnStableStateExited tag:" + usertag);
            menuBlocker.SetActive(true);
        }
        
    }
}
