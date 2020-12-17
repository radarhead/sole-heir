using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class KitController : NetworkBehaviour
    {
        public int playerCount;
        public float interactionTime;
        [HideInInspector] public Carryable carryable;

        [HideInInspector] public IKitAction action;
        [HideInInspector] public float timer;
        [HideInInspector] [SyncVar] public bool sabotaged;
        [HideInInspector] [SyncVar] public bool used = false;

        [HideInInspector] [SyncVar] private NetworkIdentity saboteurId;
        [HideInInspector] public PlayerController saboteur {get{return saboteurId?.GetComponent<PlayerController>();} set{saboteurId = value?.netIdentity;}}


        void Awake()
        {
            carryable = GetComponent<Carryable>();
            action = GetComponent<IKitAction>();
        }

        void Update()
        {
           
        }

        public void Sabotage(PlayerController pc)
        {
            this.sabotaged = true;
            this.saboteur = pc;
        }

        public void Interact()
        {
            this.timer = interactionTime;
            this.sabotaged = false;
            this.saboteur = null;
        }

        public bool PreparedToSabotage()
        {
            return (!used && carryable?.inventory != null && carryable.inventory.TryGetComponent<PlayerController>(out var pc) && pc.nearestKitInteractable != null);
        }

        public int GetNeededPlayers()
        {
            return Mathf.Max(0, playerCount - CountPlayersInRange());
        }

        public int CountPlayersInRange()
        {
            int count = 0;
            
            foreach (var player in PlayerManager.instance.GetComponentsInChildren<PlayerController>())
            {
                if(player.nearestSabotageable == this) count++;
            }
            return count;
        }
    }
}