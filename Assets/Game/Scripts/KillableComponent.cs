using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir 
{
    public class KillableComponent : NetworkBehaviour
    {
        // Prefabs
        public GameObject carryablePrefab;

        // Config
        [SyncVar] public float maxHealth;
        [SyncVar] public string bodyResource;

        //Runtime
        [SyncVar] public float health;
        [SyncVar] public bool alive = true;
        [SyncVar] public NetworkIdentity playerIdentity;
        public Vector3 deathVector = Vector3.zero;
        void Start()
        {
            health = maxHealth;
        }

        public void DealDamage(float damage, Vector3 deathVector)
        {
            this.health -= damage;
            this.deathVector = deathVector;
        }

        void Update()
        {
            if(!isServer) return;
            if(health <= 0 && alive)
            {
                alive = false;
                Rigidbody body = GetComponentInChildren<Rigidbody>();
                GameObject corpse = Instantiate(carryablePrefab, body.position, body.rotation);
                corpse.GetComponent<Carryable>().type = CarryableType.BODY;
                corpse.GetComponent<Carryable>().entityName = bodyResource;
                if(GetComponent<PlayerIdentity>() != null)
                {
                    GetComponent<PlayerIdentity>().Clone(corpse.GetComponent<PlayerIdentity>());
                }
                NetworkServer.Spawn(corpse);
                corpse.GetComponentInChildren<Rigidbody>().AddForce(deathVector.normalized*200, ForceMode.Impulse);

                GetComponent<KillableInterface>().KillMe();
            }
        }
    }
}
