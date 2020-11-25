using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class FurnitureController : NetworkBehaviour
    {
        [SyncVar] public int resourcesIndex;
        public GameObject interactorPrefab;
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

            if(isServer)
            {
                if(GetComponentInChildren<InteractableConfig>())
                {
                    GameObject interactor = Instantiate(interactorPrefab, transform.position, transform.rotation);
                    interactor.GetComponent<InteractableComponent>().furniture = netIdentity;
                    NetworkServer.Spawn(interactor);
                }

                if(GetComponentInChildren<Furniture>().inventorySize > 0)
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

