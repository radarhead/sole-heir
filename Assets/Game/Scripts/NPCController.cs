using UnityEngine;
using Mirror;
using System.Collections.Generic;

namespace SoleHeir
{
    public class NPCController : NetworkBehaviour, KillableInterface
    {
        public bool spawned = false;
        public void KillMe()
        {
            NetworkServer.Destroy(gameObject);
        }

        void Update()
        {
            if(isServer)
            {
                if(spawned == false)
                {
                    Random.InitState(System.DateTime.Now.Millisecond);
                    UnityEngine.Object[] spawners = GameObject.FindGameObjectsWithTag("SpawnLocation");
                    if(spawners.Length > 0)
                    {
                        int randVal = Random.Range(0, spawners.Length);
                        GameObject spawner = spawners[randVal] as GameObject;
                        this.transform.position = spawner.transform.position + new Vector3(1,0,1);
                        Debug.Log(spawner.transform.position);
                        Debug.Log(randVal);
                        spawned = true;
                    }
                }
            }
        }

        void Start()
        {


        }
    }
}