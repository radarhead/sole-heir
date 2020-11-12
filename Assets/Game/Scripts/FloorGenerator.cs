using System.Collections;
using System.Collections.Generic;
using SoleHeir.GenerationUtils;
using UnityEngine;

namespace SoleHeir
{
    public class FloorGenerator : MonoBehaviour
    {
        
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Initialize(PrototypeRoom prototypeRoom, float roomWidth, float roomHeight, float roomSpacing)
        {
            CreateMesh createMesh = gameObject.GetComponent<CreateMesh>();
            float xSize = prototypeRoom.GetSize().x*(roomWidth+roomSpacing) - roomSpacing;
            float ySize = prototypeRoom.GetSize().y*(roomHeight+roomSpacing) - roomSpacing;
            createMesh.AddMesh(new Vector2(0,0), new Vector2(xSize,ySize));

            float offset;

            offset = roomHeight/2 - 1;
            foreach (PrototypeDoorway doorway in prototypeRoom.GetLeftDoorways())
            {
                if(doorway.other != null)
                {
                    createMesh.AddMesh(-0.5f, offset, 0, offset+2);
                }
                offset += roomHeight + roomSpacing;

            }

            offset = roomWidth/2-1;
            foreach (PrototypeDoorway doorway in prototypeRoom.GetBottomDoorways())
            {
                if(doorway.other != null)
                {
                    createMesh.AddMesh(offset, -0.5f, offset+2, 0);
                }
                offset += roomWidth + roomSpacing;

            }
            offset = roomHeight/2 - 1;
            foreach (PrototypeDoorway doorway in prototypeRoom.GetRightDoorways())
            {
                if(doorway.other != null)
                {
                    createMesh.AddMesh(xSize, offset, xSize+0.5f, offset+2);
                }
                offset += roomHeight + roomSpacing;

            }

            offset = roomWidth/2-1;
            foreach (PrototypeDoorway doorway in prototypeRoom.GetTopDoorways())
            {
                if(doorway.other != null)
                {
                    createMesh.AddMesh(offset, ySize, offset+2, ySize+0.5f);
                }
                offset += roomWidth + roomSpacing;

            }
        }
    }
}
