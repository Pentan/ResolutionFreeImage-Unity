using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ResFreeImage.Demo {
    public class EmptyGraphic : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
        }
    }
}
