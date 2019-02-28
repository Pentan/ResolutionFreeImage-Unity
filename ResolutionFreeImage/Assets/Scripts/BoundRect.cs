using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResFreeImage.UI {

    [System.Serializable]
    public struct BoundRect
    {
        public float top;
        public float right;
        public float bottom;
        public float left;

        public BoundRect(float t, float r, float b, float l) {
            top = t;
            right = r;
            bottom = b;
            left = l;
        }

        public float GetWidth() {
            return right - left;
        }

        public float GetHeight() {
            return top - bottom;
        }

        public float GetAspectRatio() {
            return (right - left) / (top - bottom);
        }
    }
}
