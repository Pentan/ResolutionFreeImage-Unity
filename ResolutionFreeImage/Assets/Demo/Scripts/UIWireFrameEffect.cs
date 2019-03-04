using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ResFreeImage.Demo.UI {

    public class UIWireFrameEffect : BaseMeshEffect
    {
        public float frameWidth = 1.0f;
        public Color frameColor = Color.blue;

        public override void ModifyMesh(Mesh mesh) {
            // Debug.Log("ModifyMesh(mesh)");
            var verts = new List<Vector3>(mesh.vertices);
            var tris = new List<int>(mesh.triangles);
            var uvs = new List<Vector2>(mesh.uv);
            var cols = new List<Color>(mesh.colors);
            
            var trislen = mesh.triangles.Length;
            for(int i = 0; i < trislen; i+=3) {
                var i0 = tris[i];
                var i1 = tris[i + 1];
                var i2 = tris[i + 2];
                var v0 = verts[i0];
                var v1 = verts[i1];
                var v2 = verts[i2];

                var v01 = (v1 - v0);
                var v12 = (v2 - v1);
                var v20 = (v0 - v2);
                var n01 = v01.normalized;
                var n12 = v12.normalized;
                var n20 = v20.normalized;
                n01.Set(n01.y, -n01.x, n01.z);
                n12.Set(n12.y, -n12.x, n12.z);
                n20.Set(n20.y, -n20.x, n20.z);

                var nv0 = InsetPoint(v0, n20, n01, frameWidth, (v01 - v20).magnitude * 0.25f);
                var nv1 = InsetPoint(v1, n01, n12, frameWidth, (v12 - v01).magnitude * 0.25f);
                var nv2 = InsetPoint(v2, n12, n20, frameWidth, (v20 - v12).magnitude * 0.25f);

                var ni0 = verts.Count;
                var ni1 = ni0 + 1;
                var ni2 = ni0 + 2;
                var ni3 = ni0 + 3;
                var ni4 = ni0 + 4;
                var ni5 = ni0 + 5;
                verts.Add(v0);
                verts.Add(v1);
                verts.Add(v2);
                verts.Add(nv0);
                verts.Add(nv1);
                verts.Add(nv2);

                uvs.Add(uvs[i0]);
                uvs.Add(uvs[i1]);
                uvs.Add(uvs[i2]);
                uvs.Add(uvs[i0]);
                uvs.Add(uvs[i1]);
                uvs.Add(uvs[i2]);

                for(int icol = 0; icol < 6; icol++) {
                    cols.Add(frameColor);
                }

                tris.Add(ni0);
                tris.Add(ni1);
                tris.Add(ni3);
                tris.Add(ni3);
                tris.Add(ni1);
                tris.Add(ni4);
                
                tris.Add(ni1);
                tris.Add(ni2);
                tris.Add(ni4);
                tris.Add(ni4);
                tris.Add(ni2);
                tris.Add(ni5);
                
                tris.Add(ni2);
                tris.Add(ni0);
                tris.Add(ni5);
                tris.Add(ni5);
                tris.Add(ni0);
                tris.Add(ni3);
            }

            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetColors(cols);
            mesh.SetTriangles(tris, 0);
        }

        public override void ModifyMesh(VertexHelper vh) {
            // VSCode requests this signature method...?
            Debug.Log("ModifyMesh(vh)");
        }

        private Vector3 InsetPoint(Vector3 p, Vector3 n01, Vector3 n12, float inset, float inmax) {
            var n = (n01 + n12).normalized;
            float l = Mathf.Min(inmax, 1.0f / Vector3.Dot(n, n01) * inset);
            return p + n * l;
        }
    }
}
