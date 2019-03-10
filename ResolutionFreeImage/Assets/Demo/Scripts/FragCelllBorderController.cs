using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ResFreeImage.UI;

namespace ResFreeImage.Demo {

    public class FragCelllBorderController : MonoBehaviour
    {
        public Slider slider;

        private FragmentShape shape;

        void Start() {
            shape = GetComponent<FragmentShape>();
            OnSliderChanged(slider.value);
        }

        public void OnSliderChanged(float val) {
            shape.overrideBorderWidth = val;
            shape.SetAllDirty();
        }
    }
}
