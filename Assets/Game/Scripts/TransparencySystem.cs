using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    public class TransparencySystem : MonoBehaviour
    {
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
                RoomGenerator currentRoom = null;
                foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
                {
                    RoomGenerator roomGenerator = room.GetComponent<RoomGenerator>();
                    if(player.transform.position.x > roomGenerator.bottomLeft.x 
                        && player.transform.position.x < roomGenerator.topRight.x
                        && player.transform.position.z > roomGenerator.bottomLeft.z
                        && player.transform.position.z < roomGenerator.topRight.z)
                    {
                        currentRoom = roomGenerator;
                    }

                }
                if(currentRoom != null)
                {
                    foreach (TransparencyComponent transparency in GameObject.FindObjectsOfType<TransparencyComponent>())
                    {
                        if(transparency.currentRoom != currentRoom)
                        {
                            transparency.enabled = false;
                        }
                        else
                        {
                            transparency.enabled = true;
                        }
                    }
                }

            }
        }
    }

}
