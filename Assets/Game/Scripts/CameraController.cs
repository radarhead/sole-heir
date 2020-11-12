using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    public class CameraController : MonoBehaviour
    {
        public float smoothTime = 0.15f;
        public Vector3 offset;
        public Vector3 rotation;
        private Vector3 currentRoom;
        private Vector3 targetPosition;
        private Vector3 targetOffset = new Vector3(0,0,-10);
        private Vector3 velocity = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            GameObject player = null;
            foreach (GameObject thisPlayer in GameObject.FindGameObjectsWithTag("Player"))
            {
                if(thisPlayer.GetComponent<PlayerController>().isLocalPlayer)
                {
                    player = thisPlayer;
                }
            }

            if(player!=null)
            {
                foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
                {
                    RoomGenerator roomGenerator = room.GetComponent<RoomGenerator>();
                    if(player.transform.position.x > roomGenerator.bottomLeft.x 
                        && player.transform.position.x < roomGenerator.topRight.x
                        && player.transform.position.z > roomGenerator.bottomLeft.z
                        && player.transform.position.z < roomGenerator.topRight.z)
                    {
                        currentRoom = (roomGenerator.topRight + roomGenerator.bottomLeft) / 2;
                        targetOffset = offset + new Vector3(0,0, -Vector3.Distance(roomGenerator.bottomLeft, roomGenerator.topRight)/4);
                    }

                }
                
                transform.rotation = Quaternion.Euler(rotation);
                targetPosition = ((player.transform.position*2 + currentRoom) / 3) + transform.rotation*targetOffset;
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
        }
    }
}
