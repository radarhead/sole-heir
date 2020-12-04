using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class EntityUIController : MonoBehaviour
    {
        public GameObject interactableDisplay;

        private float fade = 0;
        private bool interact = false;
        private bool pickUp = false;

        private bool sabotage = false;

        public string text1 = null;
        public string text2=null;
        public GameObject text1Object;
        public GameObject text2Object;
        public InteractableComponent ic;
        public Animator a;

        void Start()
        {
            
        }

        void Update()
        {
            if(GetComponentInParent<PlayerController>() != null)
            {
                if(interactableDisplay != null)
                {
                    Destroy(interactableDisplay);
                }
                return;
            }

            if(interact || pickUp || sabotage)
            {
                fade = Mathf.Min(1f, fade + Time.deltaTime*5);
            }
            else
            {
                fade = Mathf.Max(0f, fade - Time.deltaTime*5);
            }

            if(fade > 0.01)
            {
                if(interactableDisplay == null)
                {
                    interactableDisplay = Instantiate(Resources.Load("Entity UI")) as GameObject;
                    text1Object = interactableDisplay.transform.Find("Group/Text 1").gameObject;
                    text2Object = interactableDisplay.transform.Find("Group/Text 2").gameObject;
                    ic = GetComponent<InteractableComponent>();
                    a = interactableDisplay.GetComponentInChildren<Animator>();
                    SetDisplayPosition();
                    
                }
                
            }
            
            else
            {
                if(interactableDisplay != null)
                {
                    Destroy(interactableDisplay);
                }
                return;
            }



            
            a.SetFloat("show", fade);
            
            float target;
            bool interactCircle = false;
            text1 = null;
            text2 = null;

            if(interact && ic!=null)
            {
                if(ic.GetPlayer() != null && ic.GetPlayer().isLocalPlayer)
                {
                    interactCircle = true;
                }

                // If the item can be sabotaged
                if(ic.interactAction is CorpseInteraction)
                {
                    SetTextVars("(E) Use");
                    if(ic.CountPlayersInRange() == 1)
                    {
                        SetTextVars("Need 2+ Players");
                    }
                }

                else if(ic.interactAction is InventoryInteraction)
                {
                    Carryable pcItem = ClientScene.localPlayer.GetComponent<PlayerController>().HeldItem();
                    Carryable inventoryItem = ic.GetComponentInParent<InventoryComponent>().Get(0);
                    if(pcItem == null && inventoryItem != null)
                    {
                        SetTextVars(inventoryItem.entityName);
                        SetTextVars("(E) Take");
                    }
                    else if(pcItem != null && inventoryItem == null)
                    {
                        SetTextVars("(E) Put Away");
                    }
                    else if(pcItem != null && inventoryItem != null)
                    {
                        SetTextVars(inventoryItem.entityName);
                        SetTextVars("(E) Swap");
                    }
                }

                // If it is a regular interactable
                else
                {
                    SetTextVars("(E) Interact");
                }
            }

            if(pickUp)
            {
                SetTextVars("(RMB) Pick Up");
            }

            if(sabotage && ic!=null)
            {
                interactCircle = true;
                SetTextVars("(Q) Sabotage");
            }

            if(interactCircle && ic.GetPlayer() != null)
            {
                a.SetFloat("interact progress", (ic.config.interactionTime-ic.interactionTimer) / ic.config.interactionTime);
            }

            if(text1 != null)
            {
                text1Object.GetComponentInChildren<UnityEngine.UI.Text>().text = text1;
            }
            if(text2 != null)
            {
                text2Object.GetComponentInChildren<UnityEngine.UI.Text>().text = text2;
            }
            if(text1Object.GetComponentInChildren<UnityEngine.UI.Text>().text==text2Object.GetComponentInChildren<UnityEngine.UI.Text>().text)
            {
                a.SetFloat("show text 2", 0);
            }

            target = text1!=null ? 1 : 0;
            a.SetFloat("show text 1", Mathf.Lerp(a.GetFloat("show text 1"), target, Time.deltaTime*5));

            target = text2!=null ? 1 : 0;
            a.SetFloat("show text 2", Mathf.Lerp(a.GetFloat("show text 2"), target, Time.deltaTime*5));

            target = interactCircle ? 1 : 0;
            a.SetFloat("show interact circle", Mathf.Lerp(a.GetFloat("show interact circle"), target, Time.deltaTime*10));

            interact = false;
            pickUp = false;
            sabotage = false;
        }

        void FixedUpdate()
        {
            SetDisplayPosition();
        }
        void SetDisplayPosition()
        {
            if(interactableDisplay != null)
            {
                interactableDisplay.transform.rotation = Quaternion.identity;
                interactableDisplay.transform.GetChild(0).position = Camera.main.WorldToScreenPoint(GetComponentInChildren<Collider>().bounds.center + new Vector3(0, GetComponentInChildren<Collider>().bounds.size.y/2, 0));
            }
        }

        public void CanSabotage()
        {
            sabotage = true;
        }

        public void CanPickUp()
        {
            pickUp = true;
        }

        public void CanInteract()
        {
            interact = true;
        }

        private void SetTextVars(string text)
        {
            if(text1 == null)
            {
                text1 = text;
            }
            else if(text2==null)
            {
                text2 = text;
            }
        }
    }
}