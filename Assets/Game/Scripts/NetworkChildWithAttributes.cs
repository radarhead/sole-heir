using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class NetworkChildWithAttributes : MonoBehaviour
    {
        public int GetInt(Enum e)
        {
            return GetComponentInParent<NetworkParentWithAttributes>().GetInt(Convert.ToInt16(e));
        }
        public string GetString(Enum e)
        {
            return GetComponentInParent<NetworkParentWithAttributes>().GetString(Convert.ToInt16(e));
        }
        public float GetFloat(Enum e)
        {
            return GetComponentInParent<NetworkParentWithAttributes>().GetFloat(Convert.ToInt16(e));
        }
        public bool GetBool(Enum e)
        {
            return GetComponentInParent<NetworkParentWithAttributes>().GetBool(Convert.ToInt16(e));
        }

        public void SetInt(Enum e, int value)
        {
            GetComponentInParent<NetworkParentWithAttributes>().SetInt(Convert.ToInt16(e), value);
        }

        public void SetString(Enum e, string value)
        {
            GetComponentInParent<NetworkParentWithAttributes>().SetString(Convert.ToInt16(e), value);
        }

        public void SetFloat(Enum e, float value)
        {
            GetComponentInParent<NetworkParentWithAttributes>().SetFloat(Convert.ToInt16(e), value);
        }
        public void SetBool(Enum e, bool value)
        {
            GetComponentInParent<NetworkParentWithAttributes>().SetBool(Convert.ToInt16(e), value);
        }
    }
}