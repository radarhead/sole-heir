using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{

    public class BasicKit : MonoBehaviour, IKitAction
    {
        public GameObject CanUse(GameObject go)
        {
            foreach(Carryable c in go.GetComponentsInParent<Carryable>())
            {
                if (c.GetCarryableType() == CarryableType.BODY) 
                {
                    return c.gameObject;
                }
            }
            return null;
        }

        public string GetResult()
        {
            return "Yummy";
        }
    }
}