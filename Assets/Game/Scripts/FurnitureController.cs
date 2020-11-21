using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class FurnitureController : NetworkBehaviour
    {
        [SyncVar]
        public int resourcesIndex;
        public GameObject spawnerPrefab;
        [SyncVar]
        public uint roomId;

        public override void OnStartClient()
        {
            if (NetworkIdentity.spawned.TryGetValue(roomId, out NetworkIdentity identity))
            {
                transform.parent = identity.gameObject.transform;
            }
            
            GameObject child = null;
            if(resourcesIndex < 0)
            {
                child = (GameObject)Instantiate(spawnerPrefab, transform);
            }
            else
            {
                child = (GameObject)Instantiate(Resources.LoadAll("Furniture")[resourcesIndex], transform);
            }

            child.transform.position = transform.position;
            child.transform.rotation = transform.rotation;
            //child.transform.localScale = transform.scale;
        }

        // Update is called once per frame
        void Update()
        {

            
        }
    }
}

