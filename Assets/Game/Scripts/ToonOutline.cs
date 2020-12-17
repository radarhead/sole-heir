using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    [ExecuteAlways]
    public class ToonOutline : MonoBehaviour
    {
        public readonly float offset = 0.05f;
        public Material material;
        public Mesh mesh;
        public bool refresh = false;
        private Mesh m_mesh;
        private Mesh newMesh;

        void Awake()
        {
            Update();
        }
        
        void Update()
        {
        
            if (refresh == true || mesh == null || material == null || newMesh == null)
            {
                material = Resources.Load<Material>("Entity Outline");
                MeshFilter mf = GetComponentInChildren<MeshFilter>();
                if(mf != null)
                {
                    this.mesh = mf.sharedMesh;
                    newMesh = Instantiate(mesh) as Mesh;

                    if(mesh.normals.Length != mesh.vertices.Length) return;

                    Vector3[] verts = new Vector3[mesh.vertices.Length];
                    for (int i = 0; i < mesh.vertices.Length; i++)
                    {
                        verts[i] = transform.InverseTransformPoint(transform.TransformPoint(mesh.vertices[i]) + 
                            transform.TransformDirection(mesh.normals[i]).normalized * offset);
                    }

                    newMesh.SetVertices(verts);
                    foreach(MeshCollider c in GetComponents<MeshCollider>())
                    {
                        c.convex = true;
                        c.sharedMesh = newMesh;
                    }
                    m_mesh = mesh;
                    refresh = false;
                }
            }
        }

        void OnRenderObject() {
            if(newMesh == null || material == null) return;
            material.SetPass(0);
            Graphics.DrawMeshNow(newMesh, transform.localToWorldMatrix, 1);
        }

        void OnPostRender()
        {
            //RenderObject();
        }
    }
}