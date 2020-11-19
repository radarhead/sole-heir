using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class Carryable : NetworkBehaviour
    {
        public GameObject prefab;
        private GameObject entity;
        [SyncVar]
        public CarryableState state;
        [SyncVar]
        public CarryableType type;
        [SyncVar]
        public string entityName;
        [SyncVar]
        private bool isChanged = false;
        [SyncVar]
        public int inventorySpace;
        [SyncVar]
        public float ownerId;

        // Start is called before the first frame update
        void Start()
        {
            if(entity == null) LoadEntity();
        }

        public void SetState(CarryableState state)
        {
            this.state = state;
            StateChange();
        }

        public CarryableState GetState() {return state;}
        public CarryableType GetType() {return type;}
        public string GetEntityName() {return entityName;}

        public void SetType(CarryableType type)
        {
            this.type=type;
            isChanged = true;
        }

        public void SetOwnerId(float ownerId)
        {
            this.ownerId = ownerId;
        }

        public void SetEntityName(string entityName)
        {
            this.entityName = entityName;
            isChanged = true;
        }

        public void SetInventorySpace(int inventorySpace)
        {
            this.inventorySpace = inventorySpace;
        }

        // Update is called once per frame
        void Update()
        {
            if(isChanged)
            {
                LoadEntity();
            }

            StateChange();
                
        }

        void StateChange()
        {
            if(entity != null)
            {
                switch(state)
                {
                    case CarryableState.INVENTORY:
                        entity.SetActive(false);
                        break;
                    case CarryableState.SPAWNED:
                        entity.SetActive(true);
                        gameObject.GetComponent<Rigidbody>().isKinematic = false;
                        gameObject.GetComponent<Rigidbody>().detectCollisions = true;
                        this.ownerId = 0;
                        this.inventorySpace = 0;
                        transform.parent = CarryableManager.instance.transform;
                        break;
                    case CarryableState.CARRIED:
                        entity.SetActive(true);
                        gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        gameObject.GetComponent<Rigidbody>().detectCollisions = false;
                        foreach (PlayerController pc in PlayerManager.instance.GetComponentsInChildren<PlayerController>())
                        {
                            if(pc.netId==ownerId)
                            {
                                transform.parent = pc.transform;
                            }
                        }
                        break;
                }
            }
        }

        void LoadEntity()
        {
            if(entity != null)
            {
                //Destroy the old one, yeh yeh
            }

            if(type == CarryableType.GUN)
            {
                entity = Resources.Load("Carryables/Guns/"+entityName) as GameObject;
            }

            if(entity != null)
            {
                entity = Instantiate(entity, transform);
            }
            isChanged = false;
        }
    }



    public enum CarryableState
    {
        INVENTORY,
        SPAWNED,
        CARRIED
    }

    public enum CarryableType
    {
        GUN,
        KIT,
        BODY
    }
}
