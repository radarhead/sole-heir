using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;

namespace SoleHeir {
    [ExecuteAlways]
    public class MeshLine : MonoBehaviour
    {
        public Material material;
        private float offset = 0.05f;
        public Mesh mesh;
        int[] indices;
        Vector3[] vertices;
        int[][] edges;
        List<Vector3> newVertices;
        Mesh newMesh;
        List<int> tris;
        void Start()
        {
            
            newMesh = new Mesh();
        }

        void Update()
        {
            if(mesh == null)
            {
                this.mesh = GetComponent<MeshFilter>().sharedMesh;
                
            }
            if(material == null)
            {
                this.material = Resources.Load<Material>("Inner Line");
            }
        }

        void FixedUpdate()
        {
            
        }
        void OnRenderObject()
        {
            this.indices = mesh.GetIndices(0);
            this.vertices = mesh.vertices;
            newVertices = new List<Vector3>();
            edges = new int[vertices.Length][];
            tris = new List<int>();

            for(int i = 0; i<indices.Length-2; i+=2)
            {
                int[] edge = GetEdge(i);
                int[] edge2 = GetEdge(i+1);

                tris.Add(edge[0]);
                tris.Add(edge[1]);
                tris.Add(edge2[1]);

                tris.Add(edge2[1]);
                tris.Add(edge2[0]);
                tris.Add(edge[0]);
            }
            newMesh.SetVertices(newVertices.ToArray());
            newMesh.SetTriangles(tris.ToArray(), 0);
            material.SetPass(0);
            Graphics.DrawMeshNow(newMesh, transform.localToWorldMatrix, 1);
        }

        int[] GetEdge(int i)
        {
            if(edges[indices[i]] != null) return edges[indices[i]];
            int[] edge = new int[2];
            
            if(indices[i] + 1 >= vertices.Length) return edge;
            
            Vector3 vec = vertices[indices[i]];
            Vector3 vec1 = vertices[indices[i]];
            Vector3 vec2 = vertices[indices[i] + 1];
            Vector3 camVec = (
                 Camera.current.WorldToScreenPoint( transform.TransformPoint( vec1) ) -
                 Camera.current.WorldToScreenPoint( transform.TransformPoint(vec2) )
            );

            camVec = (Matrix4x4.Scale(new Vector3(1,1,0)) * camVec).normalized;
            Quaternion rotation = Camera.current.transform.rotation;
            Vector3 finalVec = ( transform.worldToLocalMatrix * Camera.current.transform.localToWorldMatrix * Vector3.Cross(camVec, Vector3.forward) )* offset/2;
            
            newVertices.Add(vec + finalVec);
            newVertices.Add(vec - finalVec);
            
            edge[0] = newVertices.Count-2;
            edge[1] = newVertices.Count-1;
            
            edges[indices[i]] = edge;
            return edge;
        }
        
    }
}

