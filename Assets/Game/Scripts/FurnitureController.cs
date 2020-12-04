using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class FurnitureController : NetworkBehaviour
    {
        [SyncVar] public int resourcesIndex;
        [SyncVar] public NetworkIdentity room;

        /*public override void OnStartServer()
        {

        }*/

        void Start()
        {
            transform.parent = room.gameObject.transform;

            GameObject child = (GameObject)Instantiate(Resources.LoadAll("Furniture")[resourcesIndex], transform);

            child.transform.position = transform.position;
            child.transform.rotation = transform.rotation;


            if (GetComponentInChildren<Furniture>().inventorySize > 0)
            {
                if (isServer)
                {
                    GetComponent<InventoryComponent>().size = GetComponentInChildren<Furniture>().inventorySize;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

