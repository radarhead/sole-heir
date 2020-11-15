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
                foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
                {
                    RoomGenerator roomGenerator = room.GetComponent<RoomGenerator>();
                    if(player.transform.position.x > roomGenerator.bottomLeft.x - roomGenerator.roomSpacing/2
                        && player.transform.position.x < roomGenerator.topRight.x + roomGenerator.roomSpacing/2
                        && player.transform.position.z > roomGenerator.bottomLeft.z - roomGenerator.roomSpacing/2
                        && player.transform.position.z < roomGenerator.topRight.z + roomGenerator.roomSpacing/2)
                    {
                        roomGenerator.SetEnabled(true);
                    }
                    else
                    {
                        roomGenerator.SetEnabled(false);
                    }
                }
            }
        }
    }

}
