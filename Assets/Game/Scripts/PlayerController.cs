using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using System;

namespace SoleHeir
{
    public class PlayerController : NetworkBehaviour
    {
        // Config
        public float speed;
        public float smoothTime;
        public float rotationTime;
        public float overlapRadius;
        public float rotationSpeed;
        public float dodgeRollSpeed;
        public float dodgeRollTime;
        public float dodgeRollSlowDown;
        public float dodgeRollLag;

        // Timers
        float reloadTimer = 0;
        float rotationTimer = 0;

        public GameObject carryablePrefab;
        private Vector2 moveInput;
        private Vector3 acceleration = Vector3.zero;
        private Vector2 cameraPosition = Vector2.zero;
        Vector3 aimTarget;
        Quaternion rotation = Quaternion.identity;
        [SyncVar] public PlayerStatus status;

        Rigidbody body;
        private bool mouseAim = false;
        private Vector2 mousePos = Vector2.zero;
        public GameObject gunPrefab;
        public GameObject bulletPrefab;

        [SyncVar]  public int carriedItem = 0;

        // Start is called before the first frame update
        public override void OnStartClient()
        { 
            transform.parent = PlayerManager.instance.transform;

            body = GetComponent<Rigidbody>();
            UnityEngine.Object[] spawners = GameObject.FindGameObjectsWithTag("SpawnLocation");
            if(spawners.Length > 0)
            {
                GameObject spawner = spawners[UnityEngine.Random.Range(0, spawners.Length)] as GameObject;
                this.transform.position = spawner.transform.position + new Vector3(1,0,1);
            }

            //heldItem = transform.Find("Gun").gameObject;

            if(isLocalPlayer)
            {
                GetComponent<PlayerInput>().enabled = true;
                CmdSetGun();
            }
        }

        [Command]
        void CmdSetGun()
        {
            GameObject gun = Instantiate(carryablePrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(gun);
            gun.GetComponent<Carryable>().SetOwnerId(netId);
            gun.GetComponent<Carryable>().SetType(CarryableType.GUN);
            gun.GetComponent<Carryable>().SetState(CarryableState.CARRIED);
            gun.GetComponent<Carryable>().SetEntityName("Pistol");
            gun.GetComponent<Carryable>().SetInventorySpace(0);
            carriedItem = 0;
            //inventory.Add(gun);
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnInteract(InputValue value)
        {
            // If the interaction is starting
            if (value.Get<float>() > 0)
            {
                InteractableComponent ic = GetNearestInteractable();
                if(ic != null && ic.status == InteractionStatus.FREE)
                {
                    CmdStartInteraction(ic.netIdentity);
                }
            }
            else
            {
                CmdEndInteraction();
            }
        }

        public void OnPoint(InputValue value)
        {
            mousePos = value.Get<Vector2>();
            mouseAim = true;
            rotationTimer = rotationTime;
        }

        public void OnThrow(InputValue value)
        {
            if(value.Get<float>() < 1f) return;

            if(HeldItem() != null)
            {
                CmdThrowItem(aimTarget);
            }

            else
            {
                CmdPickupItem();
            }
        }

        public void OnScroll(InputValue value)
        {
            if(value.Get<Vector2>().y >= 120)
            {
                CmdSetCarriedItem((carriedItem+1)%4);
            }
            else if(value.Get<Vector2>().y <= -120)
            {
                CmdSetCarriedItem((carriedItem+3)%4);
            }
        }

        public void OnAim(InputValue value)
        {
            if(value.Get<Vector2>().magnitude > 0)
            {
                aimTarget = (new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y));
                mouseAim = false;
                rotationTimer = rotationTime;
            }
        }

        public void OnFire(InputValue value)
        {
            if(status != PlayerStatus.FREE) return;
            if(value.Get<float>() < 1f) return;
            if(reloadTimer == 0)
            {
                if(HeldItem() != null && HeldItem().GetComponent<Carryable>().type == CarryableType.GUN)
                {
                    CameraController.instance.SetScreenShake(1f);
                    CmdShootGun(aimTarget);
                    reloadTimer = 0.3f;
                }
            }
        }

        public void OnItemOne(InputValue value)
        {
            CmdSetCarriedItem(0);
        }

        public void OnItemTwo(InputValue value)
        {
            CmdSetCarriedItem(1);
        }

        public void OnItemThree(InputValue value)
        {
            CmdSetCarriedItem(2);
        }

        public void OnItemFour(InputValue value)
        {
            CmdSetCarriedItem(3);
        }

        [Command]
        void CmdSetCarriedItem(int carriedItem)
        {
            GameObject oldItem = HeldItem();
            if(oldItem != null)
            {
                oldItem.GetComponent<Carryable>().SetState(CarryableState.INVENTORY);
            }

            this.carriedItem = carriedItem;
            GameObject newItem = HeldItem();
            if(newItem != null)
            {
                newItem.GetComponent<Carryable>().SetState(CarryableState.CARRIED);
            }
        }

        [Command]
        void CmdThrowItem(Vector3 aimTarget)
        {
            Carryable item = HeldItem().GetComponent<Carryable>();
            item.SetState(CarryableState.SPAWNED);
            item.GetComponent<Rigidbody>().velocity = body.velocity + Quaternion.LookRotation(aimTarget)*new Vector3(0,0,20);
        }

        [Command]
        void CmdPickupItem()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, overlapRadius);
            foreach(Collider c in hitColliders)
            {
                if(c.attachedRigidbody != null && c.attachedRigidbody.gameObject.GetComponentInParent<Carryable>())
                {
                    Carryable item = c.attachedRigidbody.gameObject.GetComponentInParent<Carryable>();
                    item.SetInventorySpace(carriedItem);
                    item.SetOwnerId(netId);
                    item.SetState(CarryableState.CARRIED);
                    return;
                }
            }
        }

        [Command]
        public void CmdStartInteraction(NetworkIdentity ic)
        {
            status = PlayerStatus.INTERACTING;
            body.velocity = new Vector3(0,0,0);
            ic.GetComponent<InteractableComponent>().Interact(netIdentity);
        }

        [Command]
        public void CmdEndInteraction()
        {
            status = PlayerStatus.FREE;
        }

        [Command]
        void CmdShootGun(Vector3 aimTarget)
        {
            if(reloadTimer==0)
            {
                rotationTimer = rotationTime;
                body.rotation = Quaternion.LookRotation(aimTarget);
                transform.GetComponent<Rigidbody>().AddForce(Quaternion.LookRotation(aimTarget) * new Vector3(0,0,-200));
                GameObject bullet = Instantiate(bulletPrefab, transform.position +new Vector3(0,1,0), Quaternion.LookRotation(aimTarget));
                Physics.IgnoreCollision(bullet.GetComponent<Collider>(), gameObject.GetComponentInChildren<Collider>());
                bullet.GetComponent<Rigidbody>().velocity = Quaternion.LookRotation(aimTarget) * new Vector3(0,0,80);
                reloadTimer = 0.3f;
                NetworkServer.Spawn(bullet);
            }
        }

        // Update is called once per frame
        void Update()
        {
            reloadTimer = Math.Max(0f, reloadTimer - Time.deltaTime);
            rotationTimer = Math.Max(0f, rotationTimer - Time.deltaTime);

            // Handle all movement code
            if(isLocalPlayer)
            {
                InteractableComponent ic = GetNearestInteractable();
                if(ic!=null)
                {
                    ic.interactableDisplay.GetComponent<Animator>().SetBool("open", true);
                    ic.displayHideTimer = 0.05f;
                }
                // If the player can walk around.
                if(status == PlayerStatus.FREE)
                {
                    // Point to the camera
                    if(mouseAim)
                    {
                        Plane plane = new Plane(Vector3.up, 0);

                        float distance;
                        
                        Ray ray = Camera.main.ScreenPointToRay(mousePos);
                        if (plane.Raycast(ray, out distance))
                        {
                            aimTarget = (ray.GetPoint(distance) - transform.position);
                            
                        }
                    }

                    body.velocity = Vector3.SmoothDamp(body.velocity, new Vector3(moveInput.x, 0, moveInput.y)*speed, ref acceleration, smoothTime);

                    if(rotationTimer > 0) 
                    {
                        body.rotation = Quaternion.Lerp(body.rotation, Quaternion.LookRotation(aimTarget), Time.deltaTime * rotationSpeed);
                    }
                    else 
                    {
                        if(body.velocity.magnitude > 0.2)
                        {
                            body.rotation = Quaternion.Lerp(body.rotation, Quaternion.LookRotation(new Vector3(body.velocity.x, 0, body.velocity.z)), Time.deltaTime * rotationSpeed);
                        }
                    }
                }

                else if(status == PlayerStatus.INTERACTING)
                {
                    if(ic != null)
                    {
                        body.velocity = new Vector3(0,0,0);
                        body.rotation = Quaternion.Lerp(body.rotation, Quaternion.LookRotation(ic.GetCenter() - body.position), Time.deltaTime * rotationSpeed);
                    }
                }
            }

            if(HeldItem() != null)
            {
                HeldItem().transform.localPosition = new Vector3(0,1,1);
                HeldItem().transform.localRotation = Quaternion.identity;
            }
        }

        InteractableComponent GetNearestInteractable()
        {
            InteractableComponent nearest = null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5);
            foreach (Collider hitCollider in hitColliders)
            {
                FurnitureController f = hitCollider.GetComponentInParent<FurnitureController>();
                if(f != null)
                {
                    InteractableComponent ic = f.GetComponentInChildren<InteractableComponent>();
                    if(ic != null && hitCollider.GetComponentInParent<PlayerController>() == null)
                    {
                        float playerDistance = ic.GetDistanceToPlayer(netIdentity);
                        if(playerDistance < ic.config.interactionDistance)
                        {
                            if(nearest == null)
                            {
                                nearest = ic;
                            }
                            else
                            {
                                if(playerDistance < nearest.GetDistanceToPlayer(netIdentity))
                                {
                                    nearest = ic;
                                }
                            }
                        }
                    }
                }
                
            }
            return nearest;
        }

        GameObject HeldItem()
        {
            foreach(Carryable c in gameObject.GetComponentsInChildren<Carryable>())
            {
                if(c.inventorySpace == carriedItem)
                {
                    return c.gameObject;
                }
            }
            return null;
        }
    }
}
