using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{

    public class HealthKit : NetworkBehaviour, IKitAction
    {

        public bool CanUse(GameObject go)
        {
            return go.GetComponent<Killable>()!=null;
        }

        public void PerformAction(PlayerController player, GameObject target, bool sabotaged)
        {
            if(sabotaged)
            {
                target.GetComponent<Killable>().DealDamage(100, new Vector3(0,100,1), player.killable.playerIdentity);
            }
        }
    }
}