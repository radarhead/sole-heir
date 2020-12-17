using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir {
    public class GunController : NetworkBehaviour
    {
        public float damage;
        public float cooldownTime;
        public float reloadTime;
        public bool automatic;

        public int ammo;        void Update()
        {
        }
    }
}

