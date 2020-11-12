using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    public class DoorPrototype : MonoBehaviour
    {
        public List<Vector2> top;

        public float GetWidth()
        {
            return(top[0].x + top[top.Count-1].x);
        }
    }
}


