using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class Carryable : NetworkBehaviour
    {
        public float rarity;
        [HideInInspector] [SyncVar] private NetworkIdentity inventoryId;
        [HideInInspector] public Inventory inventory { get { return inventoryId?.GetComponent<Inventory>(); } set { inventoryId = value?.netIdentity; } }
        public Rigidbody body;


        public void AddToInventory(Inventory newInventory, int index)
        {
            newInventory.Set(index, this);
        }

        void Awake()
        {
            transform.parent = CarryableManager.instance.transform;
            this.body = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        public void Spawn(Vector3 force)
        {
            if (GetComponentInChildren<Collider>() != null)
            {
                bool inRoom = false;
                foreach (RoomGenerator room in Object.FindObjectsOfType<RoomGenerator>())
                {
                    Collider c = GetComponentInChildren<Collider>();
                    if (c.bounds.min.x > room.bottomLeft.x && c.bounds.min.z > room.bottomLeft.z &&
                        c.bounds.max.x < room.topRight.x && c.bounds.max.z < room.topRight.z)
                    {
                        inRoom = true;
                    }
                }

                if (!inRoom) return;
            }


            //SetActive(true);
            body.isKinematic = false;
            body.detectCollisions = true;
            transform.parent = CarryableManager.instance.transform;
            body.AddForce(force, ForceMode.Impulse);
            this.inventory = null;
        }

        // Ensures that the item has the correct parent
        void Update()
        {
            // If the item is in an inventory
            if (inventory != null)
            {
                // If the item is being carried by a player
                if (inventory.GetComponent<PlayerController>() != null && inventory.GetComponent<PlayerController>().HeldItem() == this)
                {
                    HelperMethods.HideOrShowObject(gameObject, true);
                }
                // If the item is hidden in an inventory
                else
                {
                    HelperMethods.HideOrShowObject(gameObject, false);
                }

                body.isKinematic = true;
                body.detectCollisions = false;
            }

            // If the item is spawned
            else
            {
                HelperMethods.HideOrShowObject(gameObject, true);
                body.isKinematic = false;
                body.detectCollisions = true;
            }
        }

        public bool CanPickup(PlayerController player)
        {
            return inventory == null;
        }
    }
}
