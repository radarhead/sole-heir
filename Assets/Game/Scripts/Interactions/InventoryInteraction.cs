using UnityEngine;

namespace SoleHeir
{
    public class InventoryInteraction : MonoBehaviour, IInteractAction, IInteractCondition
    {
        public void Interact(PlayerController player)
        {
            Inventory inventory = GetComponentInParent<Inventory>();
            Carryable playerItem = player.HeldItem();
            Carryable inventoryItem = inventory.Get(0);
            
            if(playerItem!=null)
            {
                playerItem.AddToInventory(inventory,0);
            }
            if(inventoryItem != null)
            {
                inventoryItem.AddToInventory(player.inventory, player.carriedItem);
            }
        }

        public bool CanInteract(PlayerController player)
        {
            Inventory inventory = GetComponentInParent<Inventory>();

            if(player.HeldItem() == null && inventory.Get(0) == null)
            {
                return false;
            }
            return true;
        }
    }
}
