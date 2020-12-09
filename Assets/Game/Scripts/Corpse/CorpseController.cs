using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class CorpseController : NetworkChildWithAttributes
    {
        /*
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
            if(!CanInteract(player)) return;
            KitController k = player.HeldItem().GetComponentInChildren<KitController>();
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
            Debug.Log(GetInt(CorpseEnums.KILLER));
        }*/
    }

    public enum CorpseEnums
    {
        KILLER
    }
}