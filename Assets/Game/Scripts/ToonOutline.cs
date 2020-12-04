using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    [ExecuteAlways]
    public class ToonOutline : MonoBehaviour
    {
        private float offset = 0.05f;
        void Awake()
        {
            Material material = Resources.Load("Entity Outline") as Material;
            material.SetFloat("_OutlineOffset", offset);

            foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
            {
                r.sharedMaterials = new Material[] {r.sharedMaterial, material};
                
                if(Application.isPlaying)
                {
                    MeshCollider c = r.GetComponent<MeshCollider>();
                    if(c!=null)
                    {
                        Mesh newMesh = Instantiate(c.sharedMesh) as Mesh;

                        Vector3[] verts = new Vector3[newMesh.vertices.Length];
                        for (int i = 0; i < newMesh.vertices.Length; i++)
                        {
                            verts[i] = c.transform.InverseTransformPoint(c.transform.TransformPoint(c.sharedMesh.vertices[i]) + 
                                c.transform.TransformDirection(c.sharedMesh.normals[i]).normalized * offset*1.31f);
                        }

                        newMesh.SetVertices(verts);
                        c.sharedMesh = newMesh;
                    }
                }
            }


        }
    }
}