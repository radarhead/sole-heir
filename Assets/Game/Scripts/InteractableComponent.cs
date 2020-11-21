using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public abstract class InteractableComponent : NetworkBehaviour
    {
        // Configuration
        public float interactionTime;
        
        // Runtime
        public uint playerId;
        public float interactionTimer;

        void Update()
        {
            
        }
    }
}
