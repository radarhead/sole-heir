using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class FurnitureController : NetworkBehaviour
    {
        [SyncVar] public int resourcesIndex;
        public GameObject spawnerPrefab;
        public GameObject interactorPrefab;
        [SyncVar] public NetworkIdentity room;

        /*public override void OnStartServer()
        {

        }*/

        void Start()
        {
            transform.parent = room.gameObject.transform;

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

            if(isServer)
            {
                if(GetComponentInChildren<InteractableConfig>())
                {
                    GameObject interactor = Instantiate(interactorPrefab, transform.position, transform.rotation);
                    interactor.GetComponent<InteractableComponent>().furniture = netIdentity;
                    NetworkServer.Spawn(interactor);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

