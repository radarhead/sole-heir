using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir {
    public class GunController : NetworkBehaviour
    {
        float reloadTimer =0;
        public GameObject bulletPrefab;

        void Update()
        {
            reloadTimer = Math.Max(0f, reloadTimer - Time.deltaTime);
        }

        [Command(ignoreAuthority = true)]
        public void CmdInteract(NetworkConnectionToClient sender = null)
        {
            if(reloadTimer == 0f)
            {
                transform.parent.GetComponent<Rigidbody>().AddForce(transform.rotation * new Vector3(0,0,-200));

                GameObject bullet = Instantiate(bulletPrefab, transform.position +new Vector3(0,1,0)+transform.rotation * new Vector3(0,0,2), transform.rotation);
                NetworkServer.Spawn(bullet);
                bullet.GetComponent<Rigidbody>().velocity = transform.rotation * new Vector3(0,0,80);
                reloadTimer = 0.3f;
            }
        }
    }
}

