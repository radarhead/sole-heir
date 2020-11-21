using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class BulletController : NetworkBehaviour
    {
        private Vector3 prevPos;
        public uint trailId=0;
        private GameObject trail;
        public GameObject particlePrefab;
        // Start is called before the first frame update
        void Start()
        {
            if(!isServer)
            {
                gameObject.GetComponentInChildren<Rigidbody>().detectCollisions = false;
            }
            
            else
            {
                trail = Instantiate(particlePrefab, BulletHoleSystem.instance.transform);
                trail.transform.position = transform.position;
                NetworkServer.Spawn(trail);
            }
        }

        void Update()
        {
            if(isServer && trailId == 0)
            {
                this.trailId = trail.GetComponent<BulletParticleController>().netId;
                RpcSetTrailId(this.trailId);
            }
        }

        [ClientRpc]
        void RpcSetTrailId(uint trailId)
        {
            if (NetworkIdentity.spawned.TryGetValue(trailId, out NetworkIdentity identity))
            {
                trail = identity.gameObject;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(trail != null) trail.transform.position = transform.position;
        }

        void OnCollisionEnter(Collision collision) {
            if(isServer)
            {
                foreach(ContactPoint contact in collision.contacts)
                {
                    trail.GetComponent<BulletParticleController>().RpcDetonate(contact.point, contact.normal);

                    BulletHoleSystem.instance.SpawnHole(contact);
                    
                    Destroy(gameObject);
                    NetworkServer.Destroy(gameObject);
                    return;
                }
            }

        }
    }

}
