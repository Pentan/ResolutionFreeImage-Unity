using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ResFreeImage.Demo {
    public class ToggleGroupSupport : MonoBehaviour
    {
        public ToggleGroup toggleGroup;
        public Toggle[] targetToggles;

        [System.Serializable]
        public class ToggleGroupSupportEvent : UnityEvent<int, GameObject> {}

        public ToggleGroupSupportEvent OnValueChanged = new ToggleGroupSupportEvent();

        private Dictionary<int, int> toggleIdToIndexTable = new Dictionary<int, int>();
        private int currentActiveIndex;

        // Start is called before the first frame update
        void Start()
        {
            for(int i = 0; i < targetToggles.Length; i++) {
                var toggle = targetToggles[i];
                // Initialize toggle
                toggle.isOn = false;
                toggle.group = toggleGroup;
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
                // Register table
                toggleIdToIndexTable.Add(toggle.GetInstanceID(), i);
            }
            currentActiveIndex = -1;
        }

        // Update is called once per frame
        // void Update()
        // {
        // }

        public void OnToggleValueChanged(bool ison) {
            if(!ison) {
                return;
            }

            var toggleenum = toggleGroup.ActiveToggles().GetEnumerator();
            if(!toggleenum.MoveNext()) {
                return;
            }

            var active = toggleenum.Current;
            int index = toggleIdToIndexTable[active.GetInstanceID()];
            // Debug.Log(active.name + " [" + index + "] +" + ison);

            if(currentActiveIndex != index) {
                OnValueChanged.Invoke(index, active.gameObject);
                currentActiveIndex = index;
            }
        }
    }
}
