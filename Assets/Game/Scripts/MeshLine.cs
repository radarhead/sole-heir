using UnityEngine;
using System.Runtime.InteropServices;

namespace SoleHeir {
    [ExecuteAlways]
    public class MeshLine : MonoBehaviour
    {
        private Material material;
        public Transform origin;
        private float offset = 0.05f;
        Mesh mesh;
        int[] indices;
        Vector3[] vertices;
        Vector3[][] edges;

        void Start()
        {
        }

        void Update()
        {
            if(material == null)
            {
                this.material = Resources.Load<Material>("Inner Line");
                this.mesh = GetComponent<MeshFilter>().sharedMesh;
                this.origin = transform;
            }
            
        }

        void OnRenderObject()
        {
            DrawMesh(mesh);
        }

        Vector3[] GetEdge(int i)
        {
            if(edges[indices[i]] != null) return edges[indices[i]];
            Vector3[] edge = new Vector3[2];
            
            
            Vector3 vec = vertices[indices[i]];
            Vector3 vec1 = vertices[indices[(i/2)*2]];
            Vector3 vec2 = vertices[indices[(i/2)*2] + 1];

            //Vector3 dirVec = (vertices[indices[(i/2)*2]]-vertices[indices[(i/2)*2 + 1]]).normalized;
            Vector3 camVec = (
                 Camera.current.WorldToScreenPoint( transform.TransformPoint( vec1) ) -
                 Camera.current.WorldToScreenPoint( transform.TransformPoint(vec2) )
            );

            camVec = (Matrix4x4.Scale(new Vector3(1,1,0)) * camVec).normalized;
            Quaternion rotation = Camera.current.transform.rotation;
            Vector3 finalVec = ( transform.worldToLocalMatrix * Camera.current.transform.localToWorldMatrix * Vector3.Cross(camVec, Vector3.forward) )* offset/2;
            edge[0] = vec + finalVec;
            edge[1] = vec - finalVec;
            
            edges[indices[i]] = edge;
            edges[indices[i]] = edge;
            return edge;
        }
        void DrawMesh(Mesh mesh)
        {
            if(Camera.current == null) return;
            if(mesh == null) return;
            GL.Begin(GL.QUADS);
            GL.PushMatrix();

            //GL.LoadIdentity();

            material.SetPass(0);
            GL.Color(Color.black);

            indices = mesh.GetIndices(0);
            vertices = mesh.vertices;
            edges = new Vector3[vertices.Length][];

            float ro = offset;

            for(int i = 0; i<indices.Length; i+=2)
            {


                Vector3[] edge1 = GetEdge(i);
                Vector3[] edge2 = GetEdge(i+1);

                GL.Vertex(transform.TransformPoint(edge1[0]));
                GL.Vertex(transform.TransformPoint(edge1[1]));
                GL.Vertex(transform.TransformPoint(edge2[1]));
                GL.Vertex(transform.TransformPoint(edge2[0]));
            }
            
            GL.End();
            GL.PopMatrix();
        }
    }
}

