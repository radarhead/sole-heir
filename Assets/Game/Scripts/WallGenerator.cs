using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;

namespace SoleHeir
{
    public class WallGenerator : MonoBehaviour
    {
        public GameObject normalDoor;

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
            CreateMesh createMesh = wallBase.GetComponent<CreateMesh>();

            float prevX = 0;
            float offset = width/2;
            foreach(PrototypeDoorway doorway in doors)
            {
                if(doorway.other != null)
                {
                    DoorPrototype doorPrototype = (DoorPrototype)normalDoor.GetComponent<DoorPrototype>();
                    float tempOffset = offset - doorPrototype.GetWidth()/2;
                    createMesh.AddMesh(new Vector2(prevX,0), new Vector2(tempOffset, height));
                    prevX = tempOffset;

                    for (int i = 1; i < doorPrototype.top.Count; i++)
                    {
                        createMesh.AddMesh(new Vector2(tempOffset + doorPrototype.top[i-1].x, doorPrototype.top[i-1].y), new Vector2(tempOffset+doorPrototype.top[i].x, height));
                        prevX = tempOffset + doorPrototype.top[i].x;
                    }
                }
                offset += width + spacing;
            }
            createMesh.AddMesh(new Vector2(prevX,0), new Vector2((width+spacing)*doors.Count-spacing, height));
        }
    }
}

