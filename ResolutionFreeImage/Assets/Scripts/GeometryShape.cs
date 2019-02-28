using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ResFreeImage.UI {
    
    public class GeometryShape : MaskableGraphic
    {
        // Utilities
        public enum ShapeType
        {
            Rectangle,
            Ellipse,
            Capsule,
            RoundRect,
            TrimedRect
        }

        // Common settings
        public Texture2D texture;
        
        public BoundRect margin;

        public float borderWidth = 2.0f;
        public Color borderColor = Color.gray;

        public ShapeType shapeType = ShapeType.Rectangle;

        // Round rect settings
        public float cornerRadius = 8.0f;
        public float maxArcLength = 4.0f;

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
            var shapebounds = new BoundRect(
                rect.yMax - margin.top,
                rect.xMax - margin.right,
                rect.yMin + margin.bottom,
                rect.xMin + margin.left
            );
            
            // Vertices list is clock wise.
            List<Vector2> verts;

            switch(shapeType) {
                default:
                case ShapeType.Rectangle:
                    verts = MakeRectangleLineLoop(shapebounds);
                    break;
                case ShapeType.Ellipse:
                    verts = MakeElipseLineLoop(shapebounds, maxArcLength);
                    break;
                case ShapeType.Capsule:
                    verts = MakeCapsuleLineLoop(shapebounds, maxArcLength);
                    break;
                case ShapeType.RoundRect:
                    verts = MakeRoundRectLineLoop(shapebounds, cornerRadius, maxArcLength);
                    break;
                case ShapeType.TrimedRect:
                    verts = MakeTrimedRectLineLoop(shapebounds, cornerRadius);
                    break;
            }
            // var verts = MakeRectangleLineLoop(shapebounds);
            // var verts = MakeElipseLineLoop(shapebounds);
            // var verts = MakeCapsuleLineLoop(shapebounds);
            // var verts = MakeRoundRectLineLoop(shapebounds);

            vh.Clear();

            if(borderWidth > 0.0f) {
                // Inset loop
                var insetverts = MakeInsetLineLoop(verts, borderWidth);
                // Create border
                BridgeFillLineLoops(vh, borderColor, insetverts, verts, shapebounds);
                
                // Fill shape
                FillConvexLineLoop(vh, color, insetverts, shapebounds);

            } else {
                // Fill shape
                FillConvexLineLoop(vh, color, verts, shapebounds);
            }
        }

        // Start is called before the first frame update
        // void Start()
        // {
            
        // }

        // Update is called once per frame
        // void Update()
        // {
            
        // }

        private List<Vector2> MakeRectangleLineLoop(BoundRect br) {
            var ret = new List<Vector2>(4);

            ret.Add(new Vector2(br.left,  br.bottom));
            ret.Add(new Vector2(br.left,  br.top));
            ret.Add(new Vector2(br.right, br.top));
            ret.Add(new Vector2(br.right, br.bottom));

            return ret;
        }

        private List<Vector2> MakeElipseLineLoop(BoundRect br, float arcMax) {
            // Ellipse parameters
            float a = br.GetWidth() * 0.5f;  // x radius
            float b = br.GetHeight() * 0.5f; // y radius
            float h = ((a - b) * (a - b)) / ((a + b) * (a + b));
            float c = Mathf.PI * (a + b) * (1.0f + 3.0f * h / (10.0f + Mathf.Sqrt(4.0f - 3.0f * h))); // Ramanujan's approximation.
            // Debug.Log("h:" + h + ", c:" + c);

            int DIV = Mathf.Max(8, (int)Mathf.Ceil(c / arcMax)) & (~0x1);

            var ret = new List<Vector2>(DIV);
            float cx = (br.left + br.right) * 0.5f;
            float cy = (br.bottom + br.top) * 0.5f;
            float angle_offset = (a < b) ? 0.0f : (Mathf.PI * 0.5f);

            for(int i = 0; i < DIV; i++) {
                float angle = (float)i / (float)DIV * Mathf.PI * 2.0f + angle_offset;
                ret.Add(new Vector2(Mathf.Sin(angle) * a + cx,  Mathf.Cos(angle) * b + cy));
            }
            
            return ret;
        }

        private List<Vector2> MakeCapsuleLineLoop(BoundRect br, float arcMax) {
            float capRadius = Mathf.Min(br.GetWidth(), br.GetHeight()) * 0.5f;
            int capDiv = Mathf.Max(4, (int)Mathf.Ceil(capRadius * Mathf.PI / arcMax)) & (~0x1);
            // Debug.Log("div:" + cornerDiv);

            float angle_offset;
            var capPivots = new Vector2[4];

            if(br.GetAspectRatio() < 1.0f) {
                angle_offset = Mathf.PI * -0.5f;
                capPivots[0] = new Vector2((br.left + br.right) * 0.5f, br.top - capRadius);
                capPivots[1] = new Vector2((br.left + br.right) * 0.5f, br.bottom + capRadius);
            } else {
                angle_offset = 0.0f;
                capPivots[0] = new Vector2(br.right - capRadius, (br.top + br.bottom) * 0.5f);
                capPivots[1] = new Vector2(br.left + capRadius, (br.top + br.bottom) * 0.5f);
            }

            var ret = new List<Vector2>(capDiv * 2 + 2);

            for(int i = 0; i < 2; i++) {
                Vector2 cp = capPivots[i];
                float base_angle = Mathf.PI * 2.0f * (float)i * 0.5f + angle_offset;

                for(int div = 0; div <= capDiv; div++) {
                    float angle = (float)div / (float)capDiv * Mathf.PI + base_angle;

                    var v = new Vector2(
                        Mathf.Sin(angle) * capRadius + cp.x,
                        Mathf.Cos(angle) * capRadius + cp.y
                    );
                    ret.Add(v);
                }
            }

            // To make beaty fill mesh.
            for(int i = 0; i < capDiv / 2; i++) {
                var tmpv = ret[0];
                ret.RemoveAt(0);
                ret.Add(tmpv);
            }

            return ret;
        }

        private List<Vector2> MakeRoundRectLineLoop(BoundRect br, float cornerR, float arcMax) {
            int cornerDiv = Mathf.Max(4, (int)Mathf.Ceil(cornerR * Mathf.PI * 0.5f / arcMax));
            // Debug.Log("div:" + cornerDiv);

            float angle_offset;
            var innerCorners = new Vector2[4];

            if(br.GetAspectRatio() < 1.0f) {
                angle_offset = 0.0f;
                innerCorners[0] = new Vector2(br.right - cornerR, br.top - cornerR);
                innerCorners[1] = new Vector2(br.right - cornerR, br.bottom + cornerR);
                innerCorners[2] = new Vector2(br.left + cornerR, br.bottom + cornerR);
                innerCorners[3] = new Vector2(br.left + cornerR, br.top - cornerR);
            } else {
                angle_offset = Mathf.PI * 0.5f;
                innerCorners[0] = new Vector2(br.right - cornerR, br.bottom + cornerR);
                innerCorners[1] = new Vector2(br.left + cornerR, br.bottom + cornerR);
                innerCorners[2] = new Vector2(br.left + cornerR, br.top - cornerR);
                innerCorners[3] = new Vector2(br.right - cornerR, br.top - cornerR);
            }

            var ret = new List<Vector2>(cornerDiv * 4 + 4);

            for(int i = 0; i < 4; i++) {
                Vector2 ip = innerCorners[i];
                float base_angle = Mathf.PI * 2.0f * (float)i * 0.25f + angle_offset;

                for(int div = 0; div <= cornerDiv; div++) {
                    float angle = (float)div / (float)cornerDiv * Mathf.PI * 0.5f + base_angle;

                    var v = new Vector2(
                        Mathf.Sin(angle) * cornerR + ip.x,
                        Mathf.Cos(angle) * cornerR + ip.y
                    );
                    ret.Add(v);
                }
            }

            // To make beaty fill mesh.
            int lastindex = ret.Count - 1;
            var tmpv = ret[lastindex];
            ret.RemoveAt(lastindex);
            ret.Insert(0, tmpv);

            return ret;
        }
        
        private List<Vector2> MakeTrimedRectLineLoop(BoundRect br, float trimW) {
            var ret = new List<Vector2>(4);

            ret.Add(new Vector2(br.left,  br.bottom + trimW));
            ret.Add(new Vector2(br.left,  br.top - trimW));
            ret.Add(new Vector2(br.left + trimW,  br.top));
            ret.Add(new Vector2(br.right - trimW, br.top));
            ret.Add(new Vector2(br.right, br.top - trimW));
            ret.Add(new Vector2(br.right, br.bottom + trimW));
            ret.Add(new Vector2(br.right - trimW, br.bottom));
            ret.Add(new Vector2(br.left + trimW, br.bottom));

            return ret;
        }

        private List<Vector2> MakeInsetLineLoop(List<Vector2> srcloop, float inset) {
            var ret = new List<Vector2>(srcloop.Count);

            var n0 = (srcloop[0] - srcloop[srcloop.Count - 1]).normalized;
            n0.Set(n0.y, -n0.x);

            for(int i = 0; i < srcloop.Count; i++) {
                var inext = (i + 1) % srcloop.Count;
                var p1 = srcloop[i];
                var p2 = srcloop[inext];
                var n1 = (p2 - p1).normalized;
                n1.Set(n1.y, -n1.x);
                n1.Normalize();

                var n = (n0 + n1).normalized;
                float s = 1.0f / Vector2.Dot(n, n1);
                var p = p1 + n * s * inset;
                
                ret.Add(p);

                n0 = n1;
            }

            return ret;
        }

        private void BridgeFillLineLoops(VertexHelper vh, Color col, List<Vector2> innerVerts, List<Vector2> outerVerts, BoundRect br) {
            // Debug.Assert(outerVerts.Count == innerVerts.Count, "BridgeFillLineLoops: different size verts.");

            int vstart = vh.currentVertCount;

            // Add vertices
            UIVertex uivert = UIVertex.simpleVert;
            
            float bw = br.GetWidth();
            float bh = br.GetHeight();
            UIVertex[] uiv01 = new UIVertex[2];

            for(int i = 0; i < outerVerts.Count; i++) {
                for(int j = 0; j < 2; j++) {
                    var v = (j == 0)? innerVerts[i] : outerVerts[i];

                    float tx = (v.x - br.left) / bw;
                    float ty = (v.y - br.bottom) / bh;
                    uivert.position = new Vector2(v.x, v.y);
                    uivert.uv0 = new Vector2(tx, ty);
                    uivert.color = col;
                    vh.AddVert(uivert);

                    if(i == 0) {
                        uiv01[j] = uivert;
                    }
                }
            }
            vh.AddVert(uiv01[0]);
            vh.AddVert(uiv01[1]);
            
            // Add faces
            int i0 = 0 + vstart;
            int i1 = 1 + vstart;
            int vcount = outerVerts.Count * 2 + 2;
            for(int i = 2; i < vcount; i++) {
                int i2 = i + vstart;

                if((i & 1) == 0) {
                    vh.AddTriangle(i0, i1, i2);
                } else {
                    vh.AddTriangle(i1, i0, i2);
                }
                
                i0 = i1;
                i1 = i2;
            }
        }

        private void FillConvexLineLoop(VertexHelper vh, Color col, List<Vector2> verts, BoundRect br) {
            int vstart = vh.currentVertCount;

            // Add vertices
            UIVertex uivert = UIVertex.simpleVert;

            float bw = br.GetWidth();
            float bh = br.GetHeight();

            foreach(Vector2 v in verts) {
                float tx = (v.x - br.left) / bw;
                float ty = (v.y - br.bottom) / bh;
                uivert.position = new Vector2(v.x, v.y);
                uivert.uv0 = new Vector2(tx, ty);
                uivert.color = col;
                vh.AddVert(uivert);
                // iv++;
            }

            // Add faces
            int i0 = 0 + vstart;
            int i1 = 1 + vstart;
            int vcount = verts.Count;
            for(int i = 0; i < vcount - 2; i++) {
                int i2;

                if((i & 1) == 0) {
                    i2 = vcount - 1 - i / 2 + vstart;
                    vh.AddTriangle(i0, i1, i2);
                } else {
                    i2 = 2 + i / 2 + vstart;
                    vh.AddTriangle(i1, i0, i2);
                }

                i0 = i1;
                i1 = i2;
            }
        }
    }
}
