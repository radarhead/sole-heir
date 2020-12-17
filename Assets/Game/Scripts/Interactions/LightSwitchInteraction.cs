using UnityEngine;

namespace SoleHeir
{
    public class LightSwitchInteraction : MonoBehaviour, IInteractAction, IInteractCondition
    {
        public void Interact(PlayerController player)
        {
            RoomGenerator room = HelperMethods.FindCurrentRoom(this);
            if(room!=null)
            {
                room.lights = !room.lights;
            }
        }

        public bool CanInteract(PlayerController player)
        {
            return true;
        }
    }
}
