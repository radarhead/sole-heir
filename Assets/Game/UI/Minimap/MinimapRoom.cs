﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;

namespace SoleHeir
{
    public class MinimapRoom : MonoBehaviour
    {
        public RoomGenerator room;
        private GameObject rectangle;
        private Shapes2D.Shape shape;
        public GameObject minimapDoorwayPrefab;

        // Start is called before the first frame update
        void Start()
        {
            rectangle = transform.Find("Rectangle").gameObject;
            shape = rectangle.GetComponent<Shapes2D.Shape>();
            int roomSize = GetComponentInParent<MinimapController>().roomSize;
            //rectangle.transform.localScale = new Vector3(room.prototypeRoom.GetSize().x, room.prototypeRoom.GetSize().y,0);


            // Get doorways
            float offset = 0;
            foreach(DoorType? d in room.bottomDoors)
            {
                if(d != null)
                {
                    //AddDoorway(offset, -0.5f, true);
                }
                offset+=1;
            }
            offset = 0;
            foreach(DoorType? d in room.rightDoors)
            {
                if(d != null)
                {
                    //AddDoorway(room.prototypeRoom.GetSize().x+0.5f, offset, false);
                }
                offset+=1;
            }
            offset = 0;
            foreach(DoorType? d in room.leftDoors)
            {
                if(d != null)
                {
                    //AddDoorway(0.5f, offset, false);
                }
                offset+=1;
            }
            offset = 0;
            foreach(DoorType? d in room.topDoors)
            {
                if(d != null)
                {
                    //AddDoorway(offset, room.prototypeRoom.GetSize().y-0.5f, true);
                }
                offset+=1;
            }

            //transform.localPosition = new Vector3(room.prototypeRoom.GetPosition().x, room.prototypeRoom.GetPosition().y,0) * roomSize;
            transform.localScale = new Vector3(roomSize,roomSize,1);
        }

        void AddDoorway(float x, float y, bool horizontal)
        {

            GameObject sweet = Instantiate(minimapDoorwayPrefab,transform);
            sweet.transform.localPosition = new Vector3(x,y,0);
            if(!horizontal)
            {
                sweet.transform.localRotation = Quaternion.Euler(0,0,90);
            }
        }

        void Update()
        {
            shape.settings.fillColor = Shader.GetGlobalColor("_PaletteDarkAccentColor");
            shape.settings.outlineColor = Shader.GetGlobalColor("_PaletteDarkColor");
        }
    }

}
