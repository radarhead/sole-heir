using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class BulletHoleSystem : NetworkBehaviour
    {
        public int count = 10;
        public GameObject bulletHolePrefab;
        public static BulletHoleSystem instance = null;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Start is called before the first frame update
        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else if(instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void SpawnHole(ContactPoint contact)
        {
            if(contact.otherCollider.GetComponentInParent<WallGenerator>() != null)
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, contact.point + new Vector3(0, UnityEngine.Random.Range(-0.2f, 0.2f),0), Quaternion.LookRotation(contact.normal));
                bulletHole.transform.parent = transform;
                NetworkServer.Spawn(bulletHole);
            }

            if(transform.childCount > count)
            {
                GameObject child = transform.GetChild(0).gameObject;
                Destroy(child);
                NetworkServer.Destroy(child);
            }
        }
    }

}
