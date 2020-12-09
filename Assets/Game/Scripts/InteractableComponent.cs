using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class InteractableComponent : NetworkBehaviour
    {
        public InteractableConfig config;
        public IInteractCondition interactCondition; 
        public IInteractAction interactAction; 
        public EntityUIController uiController; 

        // Runtime
        [SyncVar] public NetworkIdentity playerId;
        [SyncVar] public InteractionStatus status;
        [SyncVar] public float interactionTimer;

        void Start()
        {
            this.config = gameObject.GetComponentInChildren<InteractableConfig>();
            this.uiController = GetComponentInChildren<EntityUIController>();
            this.interactAction = GetComponentInChildren<IInteractAction>();
            this.interactCondition = GetComponentInChildren<IInteractCondition>();
            if(config == null) this.enabled=false;
        }
        void Update()
        {
            float oldInteractionTimer = interactionTimer;
            // Only reduce timer if you have to

            interactionTimer = Mathf.Max(0, interactionTimer - Time.deltaTime);
            
            if(isServer)
            {
                if(status == InteractionStatus.INTERACTING)
                {
                    
                    if(     playerId != null 
                            && GetDistanceToPlayer(playerId.GetComponent<PlayerController>()) < config.interactionDistance 
                            && playerId.GetComponent<PlayerController>().status == PlayerStatus.INTERACTING
                            && (oldInteractionTimer > 0 || config.holdInteraction))
                    {
                        if(interactionTimer == 0)
                        {
                            CompleteInteraction();
                        }
                    }

                    else
                    {
                        if(playerId != null)
                        {
                            playerId.GetComponent<PlayerController>().status = PlayerStatus.FREE;
                        }
                        this.status = InteractionStatus.FREE;
                        this.playerId = null;
                        this.interactionTimer = 0;
                    }
                }
            }
        }

        [Command]
        public void CmdSetTimer()
        {
            this.interactionTimer = config.interactionTime;
        }
        void CompleteInteraction()
        {
            IInteractAction action = GetComponentInChildren<IInteractAction>();
            if(action == null) return;
            action.Interact(playerId.GetComponent<PlayerController>());
        }

        public float GetDistanceToPlayer(PlayerController player)
        {
            
            if (player != null)
            {
                Collider thisCollider = GetComponentInChildren<Collider>();
                return (player.transform.position - thisCollider.ClosestPoint(player.transform.position)).magnitude;
            }
            else
            {
                return 100;
            }
        }

        public Vector3 GetCenter()
        {
            Vector3 center = GetComponentInChildren<Collider>().bounds.center;
            return new Vector3(center.x, 0, center.z);
        }

        public PlayerController GetPlayer()
        {
            if (playerId==null) return null;
            return playerId.GetComponent<PlayerController>();
        }

        public bool CanSabotage(PlayerController player)
        {
            if(!InInteractRange(player)) return false;
            if (GetComponentInChildren<ISabotageAction>() == null) return false;
            if(playerId == null) return false;
            return true;
        }

        private bool InInteractRange(PlayerController player)
        {
            if (player == null) return false;
            if (config == null) return false;
            if(!player.IsAlive()) return false;
            if(player.HeldItem() != null && player.HeldItem().type == CarryableType.BODY) return false;
            if(GetDistanceToPlayer(player) < config.interactionDistance)
            {
                return true;
            }
            return false;
        }

        public bool CanInteract(PlayerController player)
        {
            if(!InInteractRange(player)) return false;
            if(GetComponentInChildren<IInteractCondition>() != null)
            {
                if(!GetComponentInChildren<IInteractCondition>().CanInteract(player))
                {
                    return false;
                }
            }

            if(status == InteractionStatus.FREE || playerId == player.netIdentity)
            {
                return true;
            }
            return false;
        }

        public void Interact(PlayerController player)
        {
            if(!CanInteract(player)) return;
            this.playerId = player.netIdentity;
            this.interactionTimer = config.interactionTime;
            this.status = InteractionStatus.INTERACTING;
        }
    }
}