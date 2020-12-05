using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class Carryable : NetworkParentWithAttributes
    {
        public GameObject prefab;
        public EntityUIController uiController;
        private GameObject entity;
        [SyncVar] public CarryableType type;
        [SyncVar] public string entityName;
        [SyncVar] private bool isChanged = false;
        [SyncVar] public int inventorySpace;
        [SyncVar] public NetworkIdentity owner;
        
        //SyncDictionary<short, bool> boolTable = new SyncDictionary<short, bool>();

        private Vector3 lastPosition = Vector3.zero;
        private Vector3 velocity;

        // Start is called before the first frame update
        void Start()
        {
            if(entity == null) LoadEntity();
            uiController = GetComponent<EntityUIController>();
        }

        public CarryableType GetCarryableType() {return type;}
        public string GetEntityName() {return entityName;}

        public void SetType(CarryableType type)
        {
            this.type=type;
            isChanged = true;
        }
        public void SetEntityName(string entityName)
        {
            this.entityName = entityName;
            isChanged = true;
        }

        public void AddToInventory(InventoryComponent inventoryComponent, int index)
        {
            this.inventorySpace = index;
            this.owner = inventoryComponent.netIdentity;
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

        public void Spawn(Vector3 force)
        {
            bool inRoom = false;
            foreach(RoomGenerator room in Object.FindObjectsOfType<RoomGenerator>())
            {
                Collider c = GetComponentInChildren<Collider>();
                if( c.bounds.min.x > room.bottomLeft.x && c.bounds.min.z > room.bottomLeft.z &&
                    c.bounds.max.x < room.topRight.x && c.bounds.max.z < room.topRight.z)
                    {
                        inRoom = true;
                    }
            }

            if(!inRoom) return;

            entity.SetActive(true);
            gameObject.GetComponent<Rigidbody>().detectCollisions = true;
            this.inventorySpace = 0;
            transform.parent = CarryableManager.instance.transform;
            GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            this.owner = null;
        }

        // Ensures that the item has the correct parent
        void StateChange()
        {
            if(entity != null)
            {
                // If the item is in an inventory
                if(owner != null)
                {
                    // If the item is being carried by a player
                    if(owner.GetComponent<PlayerController>() && owner.GetComponent<PlayerController>().carriedItem == inventorySpace)
                    {
                        //gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        gameObject.GetComponent<Rigidbody>().detectCollisions = false;
                        entity.SetActive(true);
                        transform.parent = owner.transform;
                    }
                    // If the item is hidden in an inventory
                    else
                    {
                        entity.SetActive(false);
                        transform.parent = owner.transform;
                    }
                }
                // If the item is spawned
                else
                {
                    entity.SetActive(true);
                    gameObject.GetComponent<Rigidbody>().detectCollisions = true;
                    this.inventorySpace = 0;
                    transform.parent = CarryableManager.instance.transform;
                }
                        
            }
        }

        public bool CanPickup(PlayerController player)
        {
            if(owner != null) return false;
            return true;
        }

        void LoadEntity()
        {
            if(entity != null)
            {
                //Destroy the old one, yeh yeh
            }
            
            if(type == CarryableType.BODY)
            {
                entity = Resources.Load("Carryables/Bodies/"+entityName) as GameObject;
                gameObject.GetComponent<Rigidbody>().mass = 100;
            }

            else if(type == CarryableType.KIT)
            {
                entity = Resources.Load("Carryables/Kits/"+entityName) as GameObject;
                gameObject.GetComponent<Rigidbody>().mass = 20;
            }


            else if(type == CarryableType.GUN)
            {
                entity = Resources.Load("Carryables/Guns/"+entityName) as GameObject;
                gameObject.GetComponent<Rigidbody>().mass = 5;
            }

            if(entity != null)
            {
                entity = Instantiate(entity, transform);
            }
            else
            {
                Debug.Log("Item not found");
                GameObject.Destroy(gameObject);
                NetworkServer.Destroy(gameObject);
            }
            isChanged = false;
        }
    }


    public enum CarryableType
    {
        GUN,
        KIT,
        BODY
    }
}
