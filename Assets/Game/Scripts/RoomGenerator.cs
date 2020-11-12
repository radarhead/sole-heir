using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;

namespace SoleHeir
{
    public class RoomGenerator : MonoBehaviour
    {
        public float roomWidth = 8.0f;
        public float roomHeight = 6.0f;
        public float roomSpacing = 1.0f;
        public float wallHeight = 3.0f;
        public GameObject floorPrefab;

        public Vector3 bottomLeft;
        public Vector3 topRight;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Initialize(PrototypeRoom prototypeRoom)
        {
            GameObject floor = transform.Find("Floor").gameObject;
            floor.GetComponent<FloorGenerator>().Initialize(prototypeRoom, roomWidth, roomHeight, roomSpacing);
            float xSize = prototypeRoom.GetSize().x*(roomWidth+roomSpacing) - roomSpacing;
            float ySize = prototypeRoom.GetSize().y*(roomHeight+roomSpacing)-roomSpacing;

            GameObject leftWall = transform.Find("LeftWall").gameObject;
            leftWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetLeftDoorways(), roomHeight, wallHeight, roomSpacing);
            
            GameObject rightWall = transform.Find("RightWall").gameObject;
            rightWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetRightDoorways(), roomHeight, wallHeight, roomSpacing);
            rightWall.transform.position += new Vector3(xSize,0,0);
            
            GameObject topWall = transform.Find("TopWall").gameObject;
            topWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetTopDoorways(), roomWidth, wallHeight, roomSpacing);
            topWall.transform.position += new Vector3(0,0,ySize);

            GameObject bottomWall = transform.Find("BottomWall").gameObject;
            bottomWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetBottomDoorways(), roomWidth, wallHeight, roomSpacing);

            transform.position = new Vector3(prototypeRoom.GetPosition().x*(roomWidth+roomSpacing),0,prototypeRoom.GetPosition().y*(roomHeight+roomSpacing));
            bottomLeft = transform.position;
            topRight = bottomLeft + new Vector3(xSize,0,ySize);
        }
    }
}
