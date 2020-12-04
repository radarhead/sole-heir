using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoleHeir
{
    public class CreateMesh : MonoBehaviour
    {
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        private List<Vector3> vertices;
        private List<Vector3> normals;
        private List<Vector2> uv;
        private List<int> tris;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void AddMesh(float x1, float y1, float x2, float y2)
        {
            AddMesh(new Vector2(x1,y1), new Vector2(x2,y2));
        }

        public void DisableCollisions()
        {
            meshCollider.enabled = false;
        }

        void UpdateStuff()
        {
            if(vertices == null) 
            {
                vertices = new List<Vector3>();
                normals = new List<Vector3>();
                uv = new List<Vector2>();
                tris =new List<int>();
                
                meshFilter = gameObject.GetComponent<MeshFilter>();
                meshCollider = gameObject.GetComponent<MeshCollider>();

                if(meshFilter == null)
                {
                    meshFilter = gameObject.AddComponent<MeshFilter>();
                }

                if(meshCollider == null)
                {
                    meshCollider = gameObject.AddComponent<MeshCollider>();
                }
            }
        }

        public void AddMesh(Vector2 start, Vector2 end)
        {
            UpdateStuff();

            Mesh mesh = new Mesh();
            int count = vertices.Count;

            vertices.Add(new Vector3(start.x,0,start.y));
            vertices.Add(new Vector3(end.x,0,start.y));
            vertices.Add(new Vector3(start.x,0,end.y));
            vertices.Add(new Vector3(end.x,0,end.y));

            mesh.vertices = vertices.ToArray();;

            // Lower Left
            tris.Add(count+0);
            tris.Add(count+2);
            tris.Add(count+1);
            // Upper right
            tris.Add(count+2);
            tris.Add(count+3);
            tris.Add(count+1);
            mesh.triangles = tris.ToArray();

            normals.Add(-Vector3.down);
            normals.Add(-Vector3.down);
            normals.Add(-Vector3.down);
            normals.Add(-Vector3.down);
            mesh.normals = normals.ToArray();


            uv.Add(new Vector2(start.x, start.y));
            uv.Add(new Vector2(end.x, start.y));
            uv.Add(new Vector2(start.x, end.y));
            uv.Add(new Vector2(end.x, end.y));
            mesh.uv = uv.ToArray();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public void AddMesh(Vector2 bottomL, Vector2 topL, Vector2 bottomR, Vector2 topR)
        {
            UpdateStuff();

            Mesh mesh = new Mesh();
            int count = vertices.Count;

            vertices.Add(new Vector3(bottomL.x,0,bottomL.y));
            vertices.Add(new Vector3(bottomR.x,0,bottomR.y));
            vertices.Add(new Vector3(topL.x,0,topL.y));
            vertices.Add(new Vector3(topR.x,0,topR.y));

            mesh.vertices = vertices.ToArray();;

            // Lower Left
            tris.Add(count+0);
            tris.Add(count+2);
            tris.Add(count+1);
            // Upper right
            tris.Add(count+2);
            tris.Add(count+3);
            tris.Add(count+1);
            mesh.triangles = tris.ToArray();

            normals.Add(-Vector3.down);
            normals.Add(-Vector3.down);
            normals.Add(-Vector3.down);
            normals.Add(-Vector3.down);
            mesh.normals = normals.ToArray();


            uv.Add(new Vector2(bottomL.x, bottomL.y));
            uv.Add(new Vector2(bottomR.x, bottomR.y));
            uv.Add(new Vector2(topL.x, topL.y));
            uv.Add(new Vector2(topR.x, topR.y));
            mesh.uv = uv.ToArray();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public void AddMesh(Vector2 start, Vector2 end, float depth)
        {
            UpdateStuff();

            Mesh mesh = new Mesh();
            int count = vertices.Count;

            vertices.Add(new Vector3(start.x,0,start.y));
            vertices.Add(new Vector3(start.x,depth,start.y));
            vertices.Add(new Vector3(end.x,0,end.y));
            vertices.Add(new Vector3(end.x,depth,end.y));

            mesh.vertices = vertices.ToArray();;

            // Lower Left
            tris.Add(count+0);
            tris.Add(count+2);
            tris.Add(count+1);
            // Upper right
            tris.Add(count+2);
            tris.Add(count+3);
            tris.Add(count+1);
            mesh.triangles = tris.ToArray();

            normals.Add(-Vector3.down);
            normals.Add(-Vector3.down);
            normals.Add(-Vector3.down);
            normals.Add(-Vector3.down);
            mesh.normals = normals.ToArray();


            uv.Add(new Vector2(start.x, start.y));
            uv.Add(new Vector2(end.x, start.y));
            uv.Add(new Vector2(start.x, end.y));
            uv.Add(new Vector2(end.x, end.y));
            mesh.uv = uv.ToArray();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
    }
}
