using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;

namespace SoleHeir
{
    public class WallGenerator : MonoBehaviour
    {
        public GameObject doorGetter;

        public float maxOpacity;
        public int fadeDivisions;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void Initialize(List<PrototypeDoorway> doors, float width, float height, float spacing)
        {
            GameObject wallBase = transform.Find("WallBase").gameObject;
            GameObject wallHat = transform.Find("WallHat").gameObject;
            //GameObject wallPaneling = transform.Find("WallPaneling").gameObject;
            CreateMesh createMesh = wallBase.GetComponent<CreateMesh>();
            CreateMesh createMesh2 = wallHat.GetComponent<CreateMesh>();

            float prevX = 0;
            float offset = width/2;
            foreach(PrototypeDoorway doorway in doors)
            {
                if(doorway.other != null)
                {
                    GameObject door = GameObject.Instantiate(doorGetter.GetComponent<DoorGetter>().GetDoorType(doorway), transform.position, transform.localRotation);
                    DoorPrototype doorPrototype = (DoorPrototype)door.GetComponent<DoorPrototype>();

                    float tempOffset = offset - doorPrototype.GetWidth()/2;

                    door.transform.parent = transform;
                    door.transform.localPosition += new Vector3(offset - doorPrototype.GetWidth()/2,0,0);
                    door.transform.localRotation = Quaternion.Euler(90,0,0);

                    createMesh.AddMesh(new Vector2(prevX,0), new Vector2(tempOffset, height));
                    prevX = tempOffset;
                    
                    for (int i = 1; i < doorPrototype.outline.Count; i++)
                    {
                        createMesh.AddMesh(
                            new Vector2(tempOffset + doorPrototype.outline[i-1].x, doorPrototype.outline[i-1].y),
                            new Vector2(tempOffset + doorPrototype.outline[i-1].x, height),
                            new Vector2(tempOffset+doorPrototype.outline[i].x, doorPrototype.outline[i].y),
                            new Vector2(tempOffset+doorPrototype.outline[i].x, height)
                            );
                        prevX = tempOffset + doorPrototype.outline[i].x;
                    }
                }
                offset += width + spacing;
            }

            GameObject fade = transform.Find("Fade").gameObject;
            GameObject quad = transform.Find("Fade/Quad").gameObject;
            
            
            for(int i=0; i<fadeDivisions; i++)
            {
                float z = 1f / fadeDivisions * i;
                float opacity = 1f / fadeDivisions * (i);
                GameObject newChild = GameObject.Instantiate(quad, fade.transform);
                newChild.GetComponent<MeshRenderer>().material.SetFloat("_Alpha", opacity*maxOpacity);
                newChild.transform.localPosition = new Vector3(0.5f,-0.5f,z);
            }

            GameObject.Destroy(quad);
            fade.transform.localScale = new Vector3((width+spacing)*doors.Count - spacing, height, -spacing/2);
            fade.transform.localRotation = Quaternion.Euler(-90,0,0);
            createMesh.AddMesh(new Vector2(prevX,0), new Vector2((width+spacing)*doors.Count-spacing, height));
            wallHat.transform.localRotation = Quaternion.Euler(90,0,0);
            createMesh2.AddMesh(
                new Vector2(0, 0),
                new Vector2(-spacing/2, spacing/2),
                new Vector2((width+spacing)*doors.Count-spacing,0),
                new Vector2((width+spacing)*doors.Count-spacing/2,spacing/2));
            wallHat.transform.localPosition = new Vector3(0,0,height);

            //Debug.Log(wallBase.GetComponent<MeshFilter>().mesh.GetVertices().Length);
            //wallPaneling.GetComponent<MeshFilter>().sharedMesh = wallBase.GetComponent<MeshFilter>().mesh;
            //wallPaneling.transform.localPosition = new Vector3(0,0.01f,0);
        }


    }
}

