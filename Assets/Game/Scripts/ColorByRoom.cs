using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class ColorByRoom : MonoBehaviour
    {
        public RoomGenerator room;

        void Start()
        {
            this.room = GetComponentInParent<RoomGenerator>();
        }
    }
}