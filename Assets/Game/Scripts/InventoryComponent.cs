using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;

namespace SoleHeir
{
    public class InventoryComponent : NetworkBehaviour
    {
        // Config
        [SyncVar] public int size;

        public List<Carryable> GetInventory()
        {
            List<Carryable> output = new List<Carryable>();
            for(int i=0; i<size; i++)
            {
                output.Add(Get(i));
            }
            return output;
        }

        public Carryable Get(int i)
        {
            foreach(Carryable c in GetComponentsInChildren<Carryable>())
            {
                if(c.inventorySpace == i)
                {
                    return c;
                }
            }

            return null;
        }
    }
}