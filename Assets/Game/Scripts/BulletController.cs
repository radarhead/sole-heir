using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class BulletController : NetworkBehaviour
    {
        public GameObject playerIdentity;

        public float damage;

        private Vector3 prevPos;
        private Vector3 angle;
        public uint trailId=0;
        [SyncVar] public NetworkIdentity trail;
        public GameObject particlePrefab;
        // Start is called before the first frame update
        void Start()
        {
            angle = GetComponent<Rigidbody>().velocity;
            prevPos = transform.position;
            if(!isServer)
            {
                gameObject.GetComponentInChildren<Rigidbody>().detectCollisions = false;
            }
            
            
            else
            {
                GameObject trailObject = Instantiate(particlePrefab, BulletHoleSystem.instance.transform);
                trailObject.transform.position = transform.position;
                trailObject.GetComponent<BulletParticleController>().bullet = gameObject;
                NetworkServer.Spawn(trailObject);
                trail = trailObject.GetComponent<BulletParticleController>().netIdentity;
            }
        }

        void Update()
        {
            /*RaycastHit hit;
            if(Physics.Raycast(transform.position, angle, out hit, (transform.position - prevPos).magnitude))
            {
                Debug.Log(hit.distance);
            }
            prevPos = transform.position;*/
        }

        void Collide(ContactPoint contact)
        {
            trail.GetComponent<BulletParticleController>().RpcDetonate(contact.point, contact.normal);
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
            return;
        }

        void OnCollisionEnter(Collision collision) {
            if(isServer)
            {
                foreach(ContactPoint contact in collision.contacts)
                {
                    KillableComponent killable = contact.otherCollider.GetComponentInParent<KillableComponent>();
                    if(killable != null)
                    {
                        killable.DealDamage(damage, (angle).normalized );
                        Collide(contact);
                        return;
                    }
                }

                if(collision.contacts.Length > 0)
                {
                    ContactPoint contact = collision.contacts[0];
                    BulletHoleSystem.instance.SpawnHole(contact);
                    Collide(contact);
                }
            }
        }
    }

}
