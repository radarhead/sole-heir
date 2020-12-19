using UnityEngine;
using UnityEngine.AI;
using Mirror;
using System.Collections.Generic;

namespace SoleHeir
{
    public class NPCController : NetworkBehaviour, KillableInterface
    {
        public bool spawned = false;

        public Rigidbody body;
        public NavMeshAgent agent;
        private float timer=0;
        public float targetTime=10;

        public void KillMe()
        {
            NetworkServer.Destroy(gameObject);
        }

        void Update()
        {
            //timer -= Time.deltaTime;
            if(agent.remainingDistance<2)
            {
                timer-=Time.deltaTime;
            }

            if(timer<0)
            {
                timer = targetTime;
                var list = Object.FindObjectsOfType<NetworkStartPosition>();
                agent.destination = list[Random.Range(0, list.Length)].transform.position;
            }
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
                        spawned = true;
                        
                    }
                }
            }
        }

        void Start()
        {
            this.agent = GetComponent<NavMeshAgent>();
            this.body = GetComponent<Rigidbody>();
            var list = Object.FindObjectsOfType<NetworkStartPosition>();
            agent.destination = list[Random.Range(0, list.Length)].transform.position;
            transform.position = list[Random.Range(0, list.Length)].transform.position;
        }
    }
}