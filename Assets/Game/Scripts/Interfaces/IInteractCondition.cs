using UnityEngine;

namespace SoleHeir
{
    public interface IInteractCondition
    {
        bool CanInteract(PlayerController player);
    }
}