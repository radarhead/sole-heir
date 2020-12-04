using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class KitController : MonoBehaviour
    {
        // References
        Carryable carryable;

        void Awake()
        {
            carryable = GetComponentInParent<Carryable>();
        }

        public bool GetBool(KitBools boolType)
        {
            return carryable.GetBool((int)boolType);
        }

        public void SetBool(KitBools boolType, bool value)
        {
            carryable.SetBool((int)boolType, value);
        }
    }

    public enum KitBools
    {
        Used,
        Sabotaged
    }
}