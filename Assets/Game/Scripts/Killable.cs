using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir 
{
    public class Killable : NetworkBehaviour
    {
        // Prefabs
        public GameObject corpsePrefab;

        // Config
        [SyncVar] public float maxHealth;

        //Runtime
        [SyncVar] public float health;
        [SyncVar] public bool alive = true;

        [SyncVar] public int playerIdentity;

        public int lastPlayer;
        public Vector3 deathVector = Vector3.zero;

        void Start()
        {
            health = maxHealth;
            playerIdentity = PlayerIdentitySystem.instance.NewPIS("Player").id;
        }

        public void DealDamage(float damage, Vector3 deathVector, int lastPlayer)
        {
            this.health -= damage;
            this.deathVector = deathVector;
            this.lastPlayer = lastPlayer;
        }

             

        void Update()
        {
            if(!isServer) return;
            if(health <= 0 && alive)
            {
                GetComponent<KillableInterface>().KillMe();
                if(corpsePrefab != null)
                {
                    var go = Instantiate(corpsePrefab, transform.position, transform.rotation);
                    NetworkServer.Spawn(go);
                }
            }
        }
    }
}
