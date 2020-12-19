using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class BulletManager : NetworkBehaviour
    {

        public int count = 100;
        public static BulletManager instance = null;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        void Update()
        {
            if(transform.childCount > count)
            {
                GameObject ugly = transform.GetChild(0).gameObject;
                Destroy(transform.GetChild(0).gameObject);
                NetworkServer.Destroy(ugly);
            }
        }
    }

}
