using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using System;

namespace SoleHeir
{
    public class PlayerController : NetworkBehaviour, KillableInterface
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
        [SyncVar] public Quaternion rotation = Quaternion.identity;
        [SyncVar] public PlayerStatus status;

        Rigidbody body;
        private bool mouseAim = false;
        private Vector2 mousePos = Vector2.zero;
        public GameObject gunPrefab;
        public GameObject bulletPrefab;
        public InventoryComponent inventory;
        public AnonymousComponent anonymousComponent;

        public PlayerIdentity playerIdentity;


        [SyncVar] public int carriedItem = 0;

        // Start is called before the first frame update
        void Start()
        {
            anonymousComponent = GetComponent<AnonymousComponent>();
            playerIdentity = GetComponent<PlayerIdentity>();
            if (isServer)
            {
                playerIdentity.Create("Player");
            }
            transform.parent = PlayerManager.instance.transform;

            body = GetComponent<Rigidbody>();

            if (isLocalPlayer)
            {
                GetComponent<PlayerInput>().enabled = true;
                CmdSetGun();
            }

            inventory = GetComponent<InventoryComponent>();
        }

        [Command]
        void CmdSetGun()
        {
            GameObject gun = Instantiate(carryablePrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(gun);
            gun.GetComponent<Carryable>().SetType(CarryableType.GUN);
            gun.GetComponent<Carryable>().SetEntityName("Pistol");
            gun.GetComponent<Carryable>().AddToInventory(GetComponent<InventoryComponent>(), 0);
            carriedItem = 0;

            GameObject kit = Instantiate(carryablePrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(kit);
            kit.GetComponent<Carryable>().SetType(CarryableType.KIT);
            kit.GetComponent<Carryable>().SetEntityName("Kit A");
            kit.GetComponent<Carryable>().AddToInventory(GetComponent<InventoryComponent>(), 1);
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnInteract(InputValue value)
        {
            if (!IsAlive()) return;
            // If the interaction is starting
            if (value.Get<float>() > 0)
            {
                InteractableComponent ic = GetNearestInteractable();
                GameObject k = GetNearestKitInteractable();

                if (k != null)
                {
                    CmdStartKit();
                }
                else if (ic != null && ic.status == InteractionStatus.FREE)
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
            if (!IsAlive()) return;
            if (value.Get<float>() < 1f) return;

            if (HeldItem() != null)
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
            if (value.Get<Vector2>().y >= 120)
            {
                CmdSetCarriedItem((carriedItem + 3) % 4);
            }
            else if (value.Get<Vector2>().y <= -120)
            {
                CmdSetCarriedItem((carriedItem + 1) % 4);
            }
        }

        public void OnAim(InputValue value)
        {
            if (value.Get<Vector2>().magnitude > 0)
            {
                aimTarget = (new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y));
                mouseAim = false;
                rotationTimer = rotationTime;
            }
        }

        public void OnFire(InputValue value)
        {
            if (status != PlayerStatus.FREE) return;
            if (value.Get<float>() < 1f) return;

            if (reloadTimer == 0)
            {
                Carryable heldItem = HeldItem();
                if (heldItem != null && heldItem.type == CarryableType.GUN)
                {
                    CameraController.instance.SetScreenShake(1f);
                    CmdShootGun();
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
            if(status != PlayerStatus.FREE) return;

            Carryable oldItem = HeldItem();

            if (oldItem != null && oldItem.GetComponent<Carryable>().type == CarryableType.BODY)
            {
                return;
            }
            this.carriedItem = carriedItem;
        }

        [Command]
        void CmdThrowItem(Vector3 aimTarget)
        {
            if(status != PlayerStatus.FREE) return;
            Carryable item = HeldItem();
            item.Spawn(Quaternion.LookRotation(aimTarget) * new Vector3(0, 0, 80));
        }

        [Command]
        void CmdPickupItem()
        {
            if(status != PlayerStatus.FREE) return;
            Carryable nc = GetNearestCarryable();
            if (nc != null)
            {
                nc.AddToInventory(GetComponent<InventoryComponent>(), carriedItem);
            }
        }

        [Command]
        public void CmdStartInteraction(NetworkIdentity ic)
        {
            if(status != PlayerStatus.FREE) return;
            status = PlayerStatus.INTERACTING;
            body.velocity = new Vector3(0, 0, 0);
            ic.GetComponent<InteractableComponent>().Interact(this);
        }

        [Command]
        public void CmdStartKit()
        {
            if(status != PlayerStatus.FREE) return;
            status = PlayerStatus.KIT_INTERACTING;
            body.velocity = new Vector3(0, 0, 0);
        }

        [Command]
        public void CmdEndInteraction()
        {
            status = PlayerStatus.FREE;
        }

        [Command]
        void CmdShootGun()
        {
            if (reloadTimer == 0)
            {
                transform.GetComponent<Rigidbody>().AddForce(Quaternion.LookRotation(aimTarget) * new Vector3(0, 0, -200));
                GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0, 1, 0), body.rotation);
                BulletController controller = bullet.GetComponentInChildren<BulletController>();
                GunConfig config = HeldItem().GetComponentInChildren<GunConfig>();
                controller.damage = config.damage;
                Physics.IgnoreCollision(bullet.GetComponent<Collider>(), gameObject.GetComponentInChildren<Collider>());
                bullet.GetComponent<Rigidbody>().velocity = body.rotation * new Vector3(0, 0, 80);
                bullet.GetComponent<BulletController>().attacker = playerIdentity.id;
                reloadTimer = 0.3f;
                NetworkServer.Spawn(bullet);
            }
        }

        // Update is called once per frame
        void Update()
        {
            reloadTimer = Math.Max(0f, reloadTimer - Time.deltaTime);
            rotationTimer = Math.Max(0f, rotationTimer - Time.deltaTime);
            bool alive = IsAlive();



            if (!alive)
            {
                transform.Find("Capsule").gameObject.layer = 12;
                if (!isLocalPlayer)
                {
                    foreach (Renderer r in transform.Find("Capsule").gameObject.GetComponentsInChildren<Renderer>())
                    {
                        r.enabled = false;
                    }
                }
            }

            // Handle all movement code
            if (isLocalPlayer)
            {
                InteractableComponent ic = GetNearestInteractable();
                GameObject k = GetNearestKitInteractable();
                PlayerController nearSabotage = GetNearestSabotageable();
                Carryable nc = GetNearestCarryable();
                Carryable heldItem = HeldItem();

                if (k != null)
                {
                    GetOrAddUiController(k).CanKit();
                }
                else if (ic != null)
                {
                    GetOrAddUiController(ic.gameObject).CanInteract();
                }
                if (nearSabotage != null)
                {
                    if(nearSabotage.isLocalPlayer)
                    {
                        GetOrAddUiController(k.gameObject).CanSabotage();
                    }
                    else
                    {
                        GetOrAddUiController(nearSabotage.gameObject).CanSabotage();
                    }
                }

                if (nc != null && heldItem == null)
                {
                    GetOrAddUiController(nc.gameObject).CanPickUp();
                }
                // If the player can walk around.
                if (status == PlayerStatus.FREE)
                {
                    // Point to the camera
                    if (mouseAim)
                    {
                        Plane plane = new Plane(Vector3.up, 0);

                        float distance;

                        Ray ray = Camera.main.ScreenPointToRay(mousePos);
                        if (plane.Raycast(ray, out distance))
                        {
                            aimTarget = (ray.GetPoint(distance) - transform.position);

                        }
                    }

                    float speedMod = 1;
                    if (heldItem != null && heldItem.GetComponent<Carryable>().type == CarryableType.BODY) speedMod = 0.3f;
                    Vector3 newVelocity = Vector3.SmoothDamp(body.velocity, new Vector3(moveInput.x, 0, moveInput.y) * speed * speedMod, ref acceleration, smoothTime);
                    body.velocity = new Vector3(newVelocity.x, body.velocity.y, newVelocity.z);
                    if (rotationTimer > 0)
                    {
                        rotation = Quaternion.Lerp(rotation, Quaternion.LookRotation(aimTarget), Time.deltaTime * rotationSpeed);
                    }
                    else
                    {
                        if (body.velocity.magnitude > 0.2)
                        {
                            rotation = Quaternion.Lerp(rotation, Quaternion.LookRotation(HelperMethods.GetPosition2D(body.velocity)), Time.deltaTime * rotationSpeed);
                        }
                    }
                }

                else if (status == PlayerStatus.INTERACTING)
                {
                    if (ic != null)
                    {
                        Vector3 stoppedVelocity = new Vector3(0, body.velocity.y, 0);
                        body.velocity = Vector3.Lerp(body.velocity, stoppedVelocity, Time.deltaTime*100);
                        rotation = Quaternion.Lerp(rotation, Quaternion.LookRotation(HelperMethods.GetPosition2D(ic.GetCenter() - body.position)), Time.deltaTime * rotationSpeed);
                    }
                }

                else if (status == PlayerStatus.KIT_INTERACTING)
                {
                    if (k != null)
                    {
                        Vector3 stoppedVelocity = new Vector3(0, body.velocity.y, 0);
                        body.velocity = Vector3.Lerp(body.velocity, stoppedVelocity, Time.deltaTime*100);
                        rotation = Quaternion.Lerp(rotation, Quaternion.LookRotation(HelperMethods.GetCenter(k) - body.position), Time.deltaTime * rotationSpeed);
                    }
                }
            }

            if (isServer)
            {
                if (status == PlayerStatus.KIT_INTERACTING)
                {
                    GameObject k = GetNearestKitInteractable();
                    if (k != null)
                    {
                        foreach(Rigidbody r in k.GetComponentsInParent<Rigidbody>())
                        {
                            Vector3 stoppedVelocity = new Vector3(0, r.velocity.y, 0);
                            Vector3 stoppedAngular = new Vector3(r.angularVelocity.x, 0, r.angularVelocity.z);
                            r.velocity = Vector3.Lerp(r.velocity, stoppedVelocity, Time.deltaTime*100);
                            r.angularVelocity = Vector3.Lerp(r.angularVelocity, stoppedAngular, Time.deltaTime*100);
                        }
                    }
                    else
                    {
                        status = PlayerStatus.FREE;
                    }
                }
            }



            if (HeldItem() != null)
            {
                HeldItem().transform.localPosition = new Vector3(0, 1, 1);
                HeldItem().transform.localRotation = Quaternion.identity;
            }
        }

        void FixedUpdate()
        {
            body.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }

        public void KillMe()
        {
            transform.Find("Capsule").gameObject.layer = 12;
            foreach (Carryable c in GetComponentsInChildren<Carryable>())
            {
                c.GetComponent<Rigidbody>().position = body.position;
                c.Spawn(body.velocity + Quaternion.LookRotation(aimTarget) * new Vector3(0, 5, 2));
            }
        }

        public bool IsAlive()
        {
            return GetComponent<KillableComponent>().alive;
        }



        InteractableComponent GetNearestInteractable()
        {
            InteractableComponent nearest = null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f);
            foreach (Collider hitCollider in hitColliders)
            {
                InteractableComponent ic = hitCollider.GetComponentInParent<InteractableComponent>();
                if (ic != null && ic.CanInteract(this))
                {
                    float playerDistance = GetColliderDistance(ic.gameObject, this.gameObject);
                    if (nearest == null)
                    {
                        nearest = ic;
                    }
                    else
                    {
                        if (playerDistance < GetColliderDistance(nearest.gameObject, this.gameObject))
                        {
                            nearest = ic;
                        }
                    }
                }
            }
            return nearest;
        }

        GameObject GetNearestKitInteractable()
        {
            if (HeldItem() == null) return null;
            IKitAction kc = HeldItem().GetComponentInChildren<IKitAction>();
            if (kc == null) return null;
            if (HeldItem().GetBool((int)ParentBools.Used)) return null;
            GameObject nearest = null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f);
            foreach (Collider hitCollider in hitColliders)
            {
                GameObject usable = kc.CanUse(hitCollider.gameObject);
                if (usable != null)
                {
                    float playerDistance = GetColliderDistance(usable, this.gameObject);
                    if (nearest == null)
                    {
                        nearest = usable;
                    }
                    else
                    {
                        if (playerDistance < GetColliderDistance(usable, this.gameObject))
                        {
                            nearest = usable;
                        }
                    }
                }
            }
            return nearest;
        }



        PlayerController GetNearestSabotageable()
        {
            PlayerController nearest = null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3);
            foreach (Collider hitCollider in hitColliders)
            {
                PlayerController ic = hitCollider.GetComponentInParent<PlayerController>();
                if (ic != null && ic.status == PlayerStatus.KIT_INTERACTING && !ic.HeldItem().GetBool((int)ParentBools.Used))
                {
                    float playerDistance = GetColliderDistance(ic.gameObject, this.gameObject);
                    if (nearest == null)
                    {
                        nearest = ic;
                    }
                    else
                    {
                        if (playerDistance < GetColliderDistance(nearest.gameObject, this.gameObject))
                        {
                            nearest = ic;
                        }
                    }
                }
            }
            return nearest;
        }

        Carryable GetNearestCarryable()
        {
            Carryable nearest = null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f);
            foreach (Collider hitCollider in hitColliders)
            {
                Carryable ic = hitCollider.GetComponentInParent<Carryable>();
                if (ic != null && ic.CanPickup(this))
                {
                    float playerDistance = GetColliderDistance(ic.gameObject, this.gameObject);
                    if (nearest == null)
                    {
                        nearest = ic;
                    }
                    else
                    {
                        if (playerDistance < GetColliderDistance(nearest.gameObject, this.gameObject))
                        {
                            nearest = ic;
                        }
                    }
                }
            }
            return nearest;
        }

        public Carryable HeldItem()
        {
            return GetComponent<InventoryComponent>().Get(carriedItem);
        }

        public static float GetColliderDistance(GameObject a, GameObject b)
        {
            Vector3 pointA = a.GetComponentInChildren<Collider>().bounds.center;
            Vector3 pointB = b.GetComponentInChildren<Collider>().bounds.center;
            return (pointA - pointB).magnitude;
        }

        public EntityUIController GetOrAddUiController(GameObject g)
        {
            foreach (EntityUIController eic in g.GetComponentsInChildren<EntityUIController>())
            {
                return eic;
            }
            return g.AddComponent<EntityUIController>();
        }
    }
}
