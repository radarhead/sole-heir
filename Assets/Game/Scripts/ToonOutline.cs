using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    [ExecuteAlways]
    public class ToonOutline : MonoBehaviour
    {
        private float offset = 0.04f;
        private float boilAmt = 0.6f;
        public Material material;
        public Mesh mesh;
        public bool refresh = false;
        private Mesh m_mesh;
        private Mesh newMesh;
        void Update()
        {
            RenderObject();
            if(material == null)
            {
                material = Resources.Load<Material>("Entity Outline");
            }
            if(mesh == null)
            {
                foreach(MeshFilter mf in GetComponents<MeshFilter>())
                {
                    mesh = mf.sharedMesh;
                }
            }

            else if (m_mesh != mesh || refresh == true)
            {
                newMesh = Instantiate(mesh) as Mesh;

                if(mesh.normals.Length != mesh.vertices.Length) return;//

                Vector3[] verts = new Vector3[mesh.vertices.Length];
                for (int i = 0; i < mesh.vertices.Length; i++)
                {
                    verts[i] = transform.InverseTransformPoint(transform.TransformPoint(mesh.vertices[i]) + 
                        transform.TransformDirection(mesh.normals[i]).normalized * offset*(1+boilAmt));
                }

                newMesh.SetVertices(verts);
                foreach(MeshCollider c in GetComponents<MeshCollider>())
                {
                    c.sharedMesh = newMesh;
                }
                m_mesh = mesh;
                refresh = false;
            }

        }

        void RenderObject() {
            if(newMesh == null || material == null) return;
            material.SetFloat("_OutlineOffset", boilAmt*offset);
            Graphics.DrawMesh(newMesh, transform.localToWorldMatrix, material, 1);
        }

        void OnPostRender()
        {
            RenderObject();
        }
    }
}