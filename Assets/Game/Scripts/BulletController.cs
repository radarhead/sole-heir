using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class BulletController : NetworkBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            //Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if(hit.distance < 2)
                {
                    HasHit(hit);

                }
            } 
        }

        void HasHit(RaycastHit hit)
        {
            transform.Find("Trail").position = hit.point;
            transform.Find("Trail").parent = null;
            //t = null;
            
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }

}
