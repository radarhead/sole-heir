using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class KitController : NetworkChildWithAttributes
    {
        public int playerCount;
        public float interactionTime;
        // References
        Carryable carryable;

        public IKitAction action;

        void Awake()
        {
            carryable = GetComponentInParent<Carryable>();
            action = GetComponent<IKitAction>();
        }

        void Update()
        {
            if(carryable.owner == null) return;
            PlayerController player = carryable.owner.gameObject.GetComponent<PlayerController>();
            // If the item is being held
            if(player != null && player.carriedItem == carryable.inventorySpace)
            {
                if(!GetBool(ParentBools.Used))
                {
                    if(player.status == PlayerStatus.KIT_INTERACTING)
                    {
                        SetFloat(ParentFloats.Timer, Mathf.Max(0f, GetFloat(ParentFloats.Timer) - Time.deltaTime));
                        if(GetFloat(ParentFloats.Timer) == 0f)
                        {
                            SetBool(ParentBools.Used, true);
                        }
                    }
                    else
                    {
                        SetFloat(ParentFloats.Timer, interactionTime);
                    }
                }
            }
        }

        public int GetNeededPlayers()
        {
            return Mathf.Max(0, playerCount - CountPlayersInRange());
        }

        public int CountPlayersInRange()
        {
            int count = 0;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3);
            
            foreach (Collider hitCollider in hitColliders)
            {   
                if(hitCollider.tag == "player hitbox")
                {
                    count++;
                }
            }
            return count;
        }
    }


}