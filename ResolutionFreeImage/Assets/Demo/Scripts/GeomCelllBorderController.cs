using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ResFreeImage.UI;

namespace ResFreeImage.Demo {

    public class GeomCelllBorderController : MonoBehaviour
    {
        public Slider slider;

        private GeometryShape shape;

        void Start() {
            shape = GetComponent<GeometryShape>();
            OnSliderChanged(slider.value);
        }

        public void OnSliderChanged(float val) {
            shape.borderWidth = val;
            shape.SetAllDirty();
        }
    }
}
