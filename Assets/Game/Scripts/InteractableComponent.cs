using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class InteractableComponent : NetworkBehaviour
    {
        public InteractableConfig config;

        // Runtime
        [SyncVar] public NetworkIdentity playerId;
        [SyncVar] public InteractionStatus status;
        [SyncVar] public float interactionTimer;
        [SyncVar] public NetworkIdentity furniture;

        public GameObject interactableDisplay;
        public float displayHideTimer;

        void Start()
        {
            transform.parent = furniture.transform;
            this.config = furniture.gameObject.GetComponentInChildren<InteractableConfig>();

            interactableDisplay = transform.Find("Interactable Display").gameObject;
            interactableDisplay.transform.parent = GameObject.Find("Canvas").transform;
            interactableDisplay.transform.rotation = Quaternion.identity;
        }

        void Update()
        {
            displayHideTimer = Mathf.Max(0, displayHideTimer - Time.deltaTime);

            

            if(!isServer) return;
            if(status == InteractionStatus.INTERACTING)
            {
                interactableDisplay.GetComponent<Animator>().SetFloat("progress", ((config.interactionTime-interactionTimer)/config.interactionTime));
                
                if(playerId != null 
                        && GetDistanceToPlayer(playerId) < config.interactionDistance 
                        && playerId.GetComponent<PlayerController>().status == PlayerStatus.INTERACTING
                        && (interactionTimer > 0 || config.holdInteraction))
                {
                    interactionTimer -= Time.deltaTime;
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
            
            interactableDisplay.GetComponent<Animator>().SetBool("interacting", (status == InteractionStatus.INTERACTING));
            
            interactableDisplay.transform.position = Camera.main.WorldToScreenPoint(GetCenter() + new Vector3(0, furniture.GetComponentInChildren<Collider>().bounds.max.y, 0));
            if(displayHideTimer == 0)
            {
                
                interactableDisplay.GetComponent<Animator>().SetBool("open", false);
            }
        }

        public float GetDistanceToPlayer(NetworkIdentity playerId)
        {
            
            if (playerId != null)
            {
                GameObject player = playerId.gameObject;
                Collider thisCollider = furniture.GetComponentInChildren<Collider>();
                return (player.transform.position - thisCollider.ClosestPoint(player.transform.position)).magnitude;
            }
            else
            {
                return 100;
            }
        }

        public Vector3 GetCenter()
        {
            Vector3 center = furniture.GetComponentInChildren<Collider>().bounds.center;
            return new Vector3(center.x, 0, center.z);
        }

        public void Interact(NetworkIdentity playerId)
        {
            if(status == InteractionStatus.FREE)
            {
                if(GetDistanceToPlayer(playerId) < config.interactionDistance)
                {
                    this.playerId = playerId;
                    this.interactionTimer = config.interactionTime;
                    this.status = InteractionStatus.INTERACTING;
                }
            }
        }

        void OnDestroy()
        {
            GameObject.Destroy(interactableDisplay);
        }
    }
}