using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;

namespace SoleHeir
{
    public class Inventory : NetworkBehaviour
    {
        // Config
        [SyncVar] public int size;

        private readonly SyncList<NetworkIdentity> inventoryList = new SyncList<NetworkIdentity>();

        void Awake()
        {
            inventoryList.Clear();
            for(int i=0; i<size; i++)
            {
                inventoryList.Add(null);
            }
        }

        public Carryable Get(int i)
        {
            if(i<size)
            {
                return inventoryList[i]?.GetComponent<Carryable>();
            }
            return null;
        }

        public bool Insert(Carryable c)
        {
            int t=0;
            foreach(var item in inventoryList)
            {
                if (item==null)
                {
                    Set(t, c);
                    return true;
                }
                t++;
            }
            return false;
        }

        public bool IsFull()
        {
            foreach(var item in inventoryList)
            {
                if (item==null) return false;
            }
            return true;
        }

        public void Set(int i, Carryable c)
        {
            if(i >= size) Debug.Log(i);
            inventoryList[i] = c.netIdentity;
            c.inventory = this;
        }

        void Update()
        {
            for(int i=0; i<size; i++)
            {
                if(inventoryList[i] != null && inventoryList[i].TryGetComponent<Carryable>(out var c))
                {
                    if(c.inventory!=this)
                    {
                        inventoryList[i] = null;
                    }
                }
            }
        }
    }
}