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

        public GameObject target;
        [HideInInspector] [SyncVar] private NetworkIdentity saboteurId;
        [HideInInspector] public PlayerController saboteur { get { return saboteurId?.GetComponent<PlayerController>(); } set { saboteurId = value?.netIdentity; } }
        [HideInInspector] public PlayerController player { get { return carryable?.inventory?.GetComponent<PlayerController>(); } }


        void Awake()
        {
            carryable = GetComponent<Carryable>();
            action = GetComponent<IKitAction>();
            this.timer = interactionTime;
        }

        void Update()
        {
            if (PreparedToSabotage() && GetNeededPlayers() == 0 && player != null && player.status == PlayerStatus.KIT_INTERACTING && (target == player.nearestKitInteractable || target == player.gameObject))
            {
                this.timer = Mathf.Max(0, timer - Time.deltaTime);
                if(this.timer == 0)
                {
                    this.used = true;
                    this.action.PerformAction(player, target, sabotaged);
                }
            }
            else
            {
                this.timer = interactionTime;
                this.target = null;
            }
        }

        public void Sabotage(PlayerController pc)
        {
            this.sabotaged = true;
            this.saboteur = pc;
        }

        public void Interact(GameObject target)
        {
            this.timer = interactionTime;
            this.sabotaged = false;
            this.saboteur = null;
            this.target = target;
        }

        public bool PreparedToSabotage()
        {
            return (!used && (player?.nearestKitInteractable != null || target != null) && player?.HeldItem().gameObject == this.gameObject);
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
                if (player.nearestSabotageable == this) count++;
            }
            return count;
        }
    }
}