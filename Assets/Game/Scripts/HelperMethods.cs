using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class HelperMethods
    {
        public static Vector3 GetPosition2D(Vector3 oldPos)
        {
            return new Vector3(oldPos.x,0,oldPos.z);
        }

        public static Vector3 GetCenter(GameObject go)
        {
            if(go == null) return Vector3.zero;
            return go.GetComponentInChildren<Collider>().bounds.center;
        }
    }
}