using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ResFreeImage.UI {
    public class FragmentShape : MaskableGraphic
    {
        public Texture2D texture;
        
        public BoundRect margin;

        public float overrideCornerRadius = -1.0f;
        public float overrideBorderWidth = -1.0f;

        // Start is called before the first frame update
        // void Start()
        // {
        // }

        // Update is called once per frame
        // void Update()
        // {
        // }

        public override Texture mainTexture {
            get {
                if (texture == null) {
                    if (material != null && material.mainTexture != null) {
                        return material.mainTexture;
                    }
                    return s_WhiteTexture;
                }
                return texture;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            var rect = GetPixelAdjustedRect();
            float xmin = rect.xMin + margin.left;
            float xmax = rect.xMax - margin.right;
            float ymin = rect.yMin + margin.bottom;
            float ymax = rect.yMax - margin.top;
            float w = (xmax - xmin);
            float h = (ymax - ymin);

            Vector2[] verts = {
                new Vector2(xmin, ymin),
                new Vector2(xmin, ymax),
                new Vector2(xmax, ymax),
                new Vector2(xmax, ymin)
            };

            vh.Clear();

            UIVertex uiv = UIVertex.simpleVert;
            var uiverts = new List<UIVertex>(4);
            foreach(var v in verts) {
                uiv.position = new Vector2(v.x, v.y);
                uiv.uv0 = new Vector2((v.x - xmin) / w, (v.y - ymin) / h);
                uiv.uv1 = new Vector2(w, h);
                uiv.uv2 = new Vector2(overrideCornerRadius, overrideBorderWidth);
                uiv.color = color;
                // vh.AddVert(uiv); // This method is not assign uv2 in 2018.3.6f1.
                uiverts.Add(uiv);
            }
            // vh.AddTriangle(0, 1, 2);
            // vh.AddTriangle(2, 3, 0);

            var triinds = new List<int>(new int[] {
                0, 1, 2,
                2, 3, 0
            });
            vh.AddUIVertexStream(uiverts, triinds);
        }
    }
}
