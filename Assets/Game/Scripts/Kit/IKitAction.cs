using UnityEngine;

namespace SoleHeir
{
    public interface IKitAction
    {
        bool CanUse(GameObject go);
        void PerformAction(PlayerController player, GameObject target, bool sabotaged);
    }
}