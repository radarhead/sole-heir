using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class CorpseInteraction : MonoBehaviour, IInteractAction, IInteractCondition, ISabotageAction
    {
        public bool CanInteract(PlayerController player)
        {
            if(player.HeldItem() != null && player.HeldItem().type == CarryableType.KIT)
            {
                KitController kc = player.HeldItem().GetComponentInChildren<KitController>();
                if(kc != null && !kc.GetBool(KitBools.Used)) {
                    return true;
                }
            }
            return false;
        }
        public void Interact(PlayerController player)
        {
            Carryable c = player.HeldItem();
            if (c == null) return;
            KitController k = c.GetComponentInChildren<KitController>();
            if(k == null) return;
            k.SetBool(KitBools.Used, true);
        }
        public void Sabotage(PlayerController player)
        {
            InteractableComponent interactable = GetComponentInParent<InteractableComponent>();
            if(interactable.GetPlayer() == null) return;
            if(interactable.GetPlayer().HeldItem() == null) return;
            KitController kit = interactable.GetPlayer().HeldItem().GetComponentInChildren<KitController>();
            if(kit == null) return;
            
            kit.SetBool(KitBools.Sabotaged, true);
            
        }
    }
}