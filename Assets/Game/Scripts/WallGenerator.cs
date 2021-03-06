﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;
using Mirror;

namespace SoleHeir
{
    public class WallGenerator : MonoBehaviour
    {

        public void Initialize(List<int> doors, float width, float height, float spacing)
        {
            GameObject wallBase = transform.Find("WallBase").gameObject;
            GameObject wallHat = transform.Find("WallHat").gameObject;
            CreateMesh createMesh = wallBase.GetComponent<CreateMesh>();
            CreateMesh createMesh2 = wallHat.GetComponent<CreateMesh>();

            float prevX = 0;
            float offset = width/2;

            DoorPrototype[] doorArr = Resources.LoadAll<DoorPrototype>("Doors");

            foreach(int doorIdx in doors)
            {
                if(doorIdx >= 0)
                {
                    DoorType doorway = (DoorType)doorIdx;
                    GameObject door = GameObject.Instantiate(doorArr.Where(e => e.doorType==doorway).First().gameObject, transform.position, transform.localRotation);
                    DoorPrototype doorPrototype = (DoorPrototype)door.GetComponent<DoorPrototype>();
                    doorPrototype.Initialize(spacing);
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

            createMesh.AddMesh(new Vector2(prevX,0), new Vector2((width+spacing)*doors.Count-spacing, height));
            wallHat.transform.localRotation = Quaternion.Euler(90,0,0);
            createMesh2.AddMesh(
                new Vector2(0, 0),
                new Vector2(-spacing/2, spacing/2),
                new Vector2((width+spacing)*doors.Count-spacing,0),
                new Vector2((width+spacing)*doors.Count-spacing/2,spacing/2));
            wallHat.transform.localPosition = new Vector3(0,0,height);
        }


    }
}

