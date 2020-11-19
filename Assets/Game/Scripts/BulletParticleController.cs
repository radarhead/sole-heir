using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class BulletParticleController : NetworkBehaviour
    {
        public bool isAlive;

        void Start(){
            isAlive = true;
        }

        [ClientRpc]
        public void RpcDetonate(Vector3 loc, Vector3 rot)
        {
            transform.position = loc;
            transform.rotation = Quaternion.LookRotation(rot);
            GetComponentInChildren<ParticleSystem>().Play();
            GetComponentInChildren<BulletParticleController>().isAlive = false;
        }

        // Update is called once per frame
        void Update()
        {
            if(!GetComponent<ParticleSystem>().IsAlive() && !isAlive)
            {
                Destroy(gameObject);
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
