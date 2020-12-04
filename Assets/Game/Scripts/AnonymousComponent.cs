using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    
    public class AnonymousComponent : MonoBehaviour
    {
        public RoomGenerator currentRoom;
        private HouseController house;
        public PlayerIdentity identity;
        public bool initialized = false;


        // Start is called before the first frame update
        void Start()
        {
            house = GameObject.FindObjectOfType<HouseController>();
            identity = GetComponentInParent<PlayerIdentity>();
            FindCurrentRoom();


            if(currentRoom != null)
            {
                if(currentRoom.isLocalRoom)
                {
                    
                }
                else 
                {
                }
            }       
        }

        Color GetTargetColor()
        {
            if(currentRoom.isLocalRoom)
            {
                return currentRoom.colorPalette.DarkAccent();
            }
            return currentRoom.colorPalette.Dark();
        }



        // Update is called once per frame
        void Update()
        {

            FindCurrentRoom();

            if(currentRoom != null)
            {
                if(currentRoom.isLocalRoom)
                {
                }
                else 
                {
                }
            }       
        }

        void FindCurrentRoom()
        {
            // Find the current room
            foreach(RoomGenerator roomGenerator in house.GetComponentsInChildren<RoomGenerator>())
            {
                if(transform.position.x > roomGenerator.bottomLeft.x - roomGenerator.roomSpacing/2 
                    && transform.position.x < roomGenerator.topRight.x + roomGenerator.roomSpacing/2
                    && transform.position.z > roomGenerator.bottomLeft.z - roomGenerator.roomSpacing/2
                    && transform.position.z < roomGenerator.topRight.z + roomGenerator.roomSpacing/2)
                {
                    currentRoom = roomGenerator;
                }
                if(GetComponent<PlayerController>() != null && GetComponent<PlayerController>().isLocalPlayer)
                {
                    roomGenerator.SetEnabled(currentRoom == roomGenerator);
                }
            }
        }        
    }

}
