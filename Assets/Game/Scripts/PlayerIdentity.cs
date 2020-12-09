using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class PlayerIdentity : NetworkBehaviour
    {
        [SyncVar] public int id = -1;
        PlayerIdentitySystem playerIdentitySystem;

        void Awake()
        {
            playerIdentitySystem = Object.FindObjectOfType<PlayerIdentitySystem>();
        }
        public PlayerIdentityStruct Get()
        {
            return playerIdentitySystem.GetPIS(id);
        }

        public void Create(string name)
        {
            id = playerIdentitySystem.NewPIS(name).id;
        }

        public void Clone(PlayerIdentity clone)
        {
            clone.id = this.id;
        }   
    }
}
