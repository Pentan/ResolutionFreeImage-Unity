using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ResFreeImage.UI;

namespace ResFreeImage.Demo {
    public class MeshIconDemoManager : MonoBehaviour
    {
        public MeshIcon backfaceRemovalTarget;
        public Toggle backfaceRemovalToggle;
        public MeshIcon zsortTarget;
        public Toggle zsortToggle;

        // Start is called before the first frame update
        void Start()
        {
            ChangeZSortMode(zsortToggle.isOn);
            ChangeBackfaceRemoval(backfaceRemovalToggle.isOn);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        // events
        public void ToggleZSortEnabled(bool ison) {
            ChangeZSortMode(ison);
        }
        
        public void ToggleBackfaceRemovalEnabled(bool ison) {
            ChangeBackfaceRemoval(ison);
        }

        //
        private void ChangeZSortMode(bool ison) {
            var sortmode = ison ? MeshIcon.SortMode.Normal : MeshIcon.SortMode.None;
            zsortTarget.sortTriangles = sortmode;
            zsortTarget.SetAllDirty();
        }
        
        private void ChangeBackfaceRemoval(bool ison) {
            backfaceRemovalTarget.removeBackFace = ison;
            backfaceRemovalTarget.SetAllDirty();
        }
    }
}
