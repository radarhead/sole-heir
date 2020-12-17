using System.Linq;
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

            DoorPrototype[] doors = Resources.LoadAll<DoorPrototype>("Doors");

            offset = roomHeight/2;
            foreach (PrototypeDoorway doorway in prototypeRoom.GetLeftDoorways())
            {
                
                if(doorway.other != null)
                {
                    float width = doors.Where(e => e.doorType==doorway.doorType).First().GetWidth();
                    createMesh.AddMesh(-roomSpacing/2, offset-width/2, 0, offset+width/2);
                }
                offset += roomHeight + roomSpacing;

            }

            offset = roomWidth/2;
            foreach (PrototypeDoorway doorway in prototypeRoom.GetBottomDoorways())
            {
                if(doorway.other != null)
                {
                    float width = doors.Where(e => e.doorType==doorway.doorType).First().GetWidth();
                    createMesh.AddMesh(offset-width/2, -roomSpacing/2, offset+width/2, 0);
                }
                offset += roomWidth + roomSpacing;

            }
            offset = roomHeight/2;
            foreach (PrototypeDoorway doorway in prototypeRoom.GetRightDoorways())
            {
                if(doorway.other != null)
                {
                    float width = doors.Where(e => e.doorType==doorway.doorType).First().GetWidth();
                    createMesh.AddMesh(xSize, offset-width/2, xSize+roomSpacing/2, offset+width/2);
                }
                offset += roomHeight + roomSpacing;

            }

            offset = roomWidth/2;
            foreach (PrototypeDoorway doorway in prototypeRoom.GetTopDoorways())
            {
                if(doorway.other != null)
                {
                    float width = doors.Where(e => e.doorType==doorway.doorType).First().GetWidth();
                    createMesh.AddMesh(offset-width/2, ySize, offset+width/2, ySize+roomSpacing/2);
                }
                offset += roomWidth + roomSpacing;

            }
        }
    }
}
