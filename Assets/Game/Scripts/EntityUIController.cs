using System;
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
        private bool kit = false;

        public string text1 = null;
        public string text2=null;
        public GameObject text1Object;
        public GameObject text2Object;
        public Interactable ic;
        public Animator a;

        void Awake()
        {
        }

        void Update()
        {
            if(interact || pickUp || sabotage || kit)
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
                    interactableDisplay.transform.SetParent(GameplayUIController.instance.transform);
                    text1Object = interactableDisplay.transform.Find("Group/Text 1").gameObject;
                    text2Object = interactableDisplay.transform.Find("Group/Text 2").gameObject;
                    ic = GetComponent<Interactable>();
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
                if(ic.interactor != null && ic.interactor.isLocalPlayer && ic.interactionTimer != 0)
                {
                    interactCircle = true;
                }


                else if(ic.interactAction is InventoryInteraction)
                {
                    Carryable pcItem = ClientScene.localPlayer.GetComponent<PlayerController>().HeldItem();
                    Carryable inventoryItem = ic.GetComponentInParent<Inventory>().Get(0);
                    if(pcItem == null && inventoryItem != null)
                    {
                        SetTextVars(inventoryItem.name);
                        SetTextVars("(E) Take");
                    }
                    else if(pcItem != null && inventoryItem == null)
                    {
                        SetTextVars("(E) Put Away");
                    }
                    else if(pcItem != null && inventoryItem != null)
                    {
                        SetTextVars(inventoryItem.name);
                        SetTextVars("(E) Swap");
                    }
                }

                // If it is a regular interactable
                else
                {
                    SetTextVars("(E) Interact");
                }
            }

            if(kit)
            {
                PlayerController pc = ClientScene.localPlayer.GetComponent<PlayerController>();
                KitController kit =  pc?.HeldItem()?.GetComponentInChildren<KitController>();
                if(kit!=null && !kit.used)
                {
                    int remainingPlayers = kit.GetNeededPlayers();
                    if(remainingPlayers == 0)
                    {
                        SetTextVars(String.Format("(E) Use", remainingPlayers));
                    }
                }
                
            }

            if(pickUp)
            {
                SetTextVars("(RMB) Pick Up");
            }

            if(sabotage)
            {
                KitController myKit = null;

                if(kit)
                {
                    myKit = ClientScene.localPlayer.GetComponent<PlayerController>().HeldItem()?.GetComponentInChildren<KitController>();
                }
                else
                {
                    myKit = GetComponentInChildren<KitController>();
                }
                if(myKit != null)
                {
                    int remainingPlayers = myKit.GetNeededPlayers();
                    if(remainingPlayers > 1)
                    {
                        SetTextVars(String.Format("{0} Players Needed", remainingPlayers));
                    }
                    else if (remainingPlayers == 1)
                    {
                        SetTextVars(String.Format("1 Player Needed", remainingPlayers));
                    }

                    a.SetFloat("interact progress", (myKit.interactionTime - myKit.timer) / myKit.interactionTime);
                    if(myKit.interactionTime>myKit.timer)
                    {
                        interactCircle = true;
                        SetTextVars("(Q) Sabotage");
                    }
                }
            }

            if(interact && interactCircle && ic!=null && ic.interactor != null)
            {
                a.SetFloat("interact progress", (ic.interactionTime - ic.interactionTimer) / ic.interactionTime);
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
            kit = false;
        }

        void FixedUpdate()
        {
            SetDisplayPosition();
        }
        void SetDisplayPosition()
        {
            if(interactableDisplay != null && GetComponentInChildren<Collider>() != null)
            {
                interactableDisplay.transform.rotation = Quaternion.identity;
                interactableDisplay.transform.GetChild(0).position = Camera.main.WorldToScreenPoint(GetComponentInChildren<Collider>().bounds.center
                     + new Vector3(0, GetComponentInChildren<Collider>().bounds.size.y/2, GetComponentInChildren<Collider>().bounds.size.z/2));
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

        public void CanKit()
        {
            kit = true;
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