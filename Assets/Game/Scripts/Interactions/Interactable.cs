using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class Interactable : NetworkBehaviour
    {
        public float interactionTime;
        [HideInInspector] public IInteractCondition interactCondition; 
        [HideInInspector] public IInteractAction interactAction; 

        // Runtime
        [HideInInspector] [SyncVar] private NetworkIdentity interactorId;
        [HideInInspector] public PlayerController interactor {get{return interactorId?.GetComponent<PlayerController>();} set{interactorId = value?.netIdentity;}}
        [HideInInspector] [SyncVar] public InteractionStatus status;
        [HideInInspector] [SyncVar] public float interactionTimer;
        public bool endInteraction = true;

        void Start()
        {
            this.interactAction = GetComponentInChildren<IInteractAction>();
            this.interactCondition = GetComponentInChildren<IInteractCondition>();
        }
        void Update()
        {

            float oldInteractionTimer = interactionTimer;
            interactionTimer = Mathf.Max(0, interactionTimer - Time.deltaTime);
            
            if(isServer)
            {
                if(status == InteractionStatus.INTERACTING)
                {
                    
                    if(interactor != null && interactor.status == PlayerStatus.INTERACTING)
                    {
                        if(interactionTimer == 0)
                        {
                            CompleteInteraction();
                        }
                    }

                    else
                    {
                        if(interactor!=null)
                        {
                            interactor.status = PlayerStatus.FREE;
                        }
                        this.status = InteractionStatus.FREE;
                        this.interactorId = null;
                        this.interactionTimer = 0;
                    }
                }
            }
        }


        void CompleteInteraction()
        {
            interactor.status = PlayerStatus.FREE;
            status = InteractionStatus.FREE;
            IInteractAction action = GetComponentInChildren<IInteractAction>();
            if(action == null) return;
            action.Interact(interactor);
        }
        public bool CanSabotage(PlayerController player)
        {
            if (GetComponentInChildren<ISabotageAction>() == null) return false;
            return (status == InteractionStatus.INTERACTING);
        }

        public bool CanInteract(PlayerController player)
        {
            if(GetComponentInChildren<IInteractCondition>() != null)
            {
                if(!GetComponentInChildren<IInteractCondition>().CanInteract(player))
                {
                    return false;
                }
            }

            if(status == InteractionStatus.FREE || interactor!=null && (player.netId == interactor.netId))
            {
                return true;
            }
            return false;
        }

        public void Interact(PlayerController player)
        {
            if(!CanInteract(player)) return;
            this.interactorId = player.netIdentity;
            this.status = InteractionStatus.INTERACTING;
            this.interactionTimer = this.interactionTime;
        }
    }
}