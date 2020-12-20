using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

namespace SoleHeir
{
    public class PlayerController : NetworkBehaviour, KillableInterface
    {
        // Config
        public float speed;
        public float smoothTime;
        public float rotationTime;
        public float rotationSpeed;
        public float airTime = 0.1f;
        private float airTimer;
        private float jumpTimer;
        // Timers
        float rotationTimer = 0;
        private bool bizzareGrounded;
        private bool bizzareGroundedCheck { get { if (bizzareGrounded) { bizzareGrounded = false; return true; } return false; } set { bizzareGrounded = value; } }
        [SyncVar] public bool holdingFire = false;


        public GameObject carryablePrefab;
        private Vector2 moveInput;
        private Vector3 acceleration = Vector3.zero;
        public Vector3 aimTarget;
        [SyncVar] public Quaternion rotation = Quaternion.identity;
        [SyncVar] public PlayerStatus status;

        Rigidbody body;
        private bool mouseAim = false;
        private Vector2 mousePos = Vector2.zero;
        public Inventory inventory;
        public Killable killable;
        public int identity { get { return killable.playerIdentity; } }
        [SyncVar] public int carriedItem = 0;

        // Nearest objects
        public GameObject nearestKitInteractable;
        public KitController nearestSabotageable;
        public Interactable nearestInteractable;
        public Carryable nearestCarryable;

        // Start is called before the first frame update
        void Start()
        {
            transform.parent = PlayerManager.instance.transform;

            body = GetComponent<Rigidbody>();
            killable = GetComponent<Killable>();

            if (isLocalPlayer)
            {
                GetComponent<PlayerInput>().enabled = true;
            }

            if (isServer)
            {
                var list = Object.FindObjectsOfType<NetworkStartPosition>();
                transform.position = list[Random.Range(0, list.Length)].transform.position;
            }
            inventory = GetComponent<Inventory>();
        }

        void OnCollisionStay(Collision collision)
        {
            foreach (var contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5)
                {
                    bizzareGrounded = true;
                }
            }
        }

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnSabotage(InputValue value)
        {
            CmdSabotage();
        }

        public void OnInteract(InputValue value)
        {
            if (!IsAlive()) return;
            // If the interaction is starting
            if (value.Get<float>() > 0)
            {
                if (nearestInteractable != null)
                {
                    CmdStartInteraction();
                }
                else if (nearestKitInteractable != null)
                {
                    CmdStartKit();
                }
            }
            else
            {
                CmdEndInteraction();
            }
        }

        public void OnPlayerMenu(InputValue value)
        {
            if (value.Get<float>() > 0)
            {
                CmdOpenPlayerMenu();
            }
            else
            {
                CmdClosePlayerMenu();
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

        public void OnJump(InputValue value)
        {
            if (!IsAlive()) return;
            if (status != PlayerStatus.FREE) return;
            if (value.Get<float>() < 1f) return;

            if (airTimer > 0 && jumpTimer == 0)
            {
                airTimer = -1;
                jumpTimer = airTime;
                body.velocity = new Vector3(body.velocity.x, 5, body.velocity.z);
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
                //aimTarget = (new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y));
                mouseAim = false;
                rotationTimer = rotationTime;
            }
        }

        public void OnFire(InputValue value)
        {
            CmdSetHoldingFire(value.Get<float>() > 0.5f);
        }

        [Command]
        void CmdSetHoldingFire(bool holding)
        {
            if (status == PlayerStatus.FREE)
            {
            if (holdingFire == false && holding == true && HeldItem() != null && HeldItem().TryGetComponent<ICarryableShoot>(out var shoot))
            {
                shoot.Shoot();
            }
            this.holdingFire = holding;
            }
            else this.holdingFire = false;
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
        void CmdSabotage()
        {
            if (nearestSabotageable != null)
            {
                nearestSabotageable.sabotaged = true;
            }
        }

        [Command]
        void CmdSetCarriedItem(int carriedItem)
        {
            if (status != PlayerStatus.FREE) return;

            Carryable oldItem = HeldItem();

            if (oldItem != null && oldItem.GetComponent<CorpseController>())
            {
                return;
            }
            this.carriedItem = carriedItem;
        }

        [Command]
        void CmdThrowItem(Vector3 aimTarget)
        {
            if (status != PlayerStatus.FREE) return;
            Carryable item = HeldItem();
            item.Spawn(rotation * new Vector3(0, 0, 80));
        }

        [Command]
        void CmdPickupItem()
        {
            if (status != PlayerStatus.FREE) return;
            Carryable nc = GetNearestCarryable();
            if (nc != null)
            {
                nc.AddToInventory(GetComponent<Inventory>(), carriedItem);
            }
        }

        [Command]
        public void CmdStartInteraction()
        {
            if (status != PlayerStatus.FREE) return;
            if (nearestInteractable == null) return;
            status = PlayerStatus.INTERACTING;
            nearestInteractable.Interact(this);
        }

        [Command]
        public void CmdStartKit()
        {
            if (status != PlayerStatus.FREE) return;
            if (nearestKitInteractable == null) return;
            status = PlayerStatus.KIT_INTERACTING;
            HeldItem().GetComponent<KitController>().Interact(nearestKitInteractable);
        }

        [Command]
        public void CmdEndInteraction()
        {
            status = PlayerStatus.FREE;
        }

        [Command]
        public void CmdOpenPlayerMenu()
        {
            if (status == PlayerStatus.FREE)
                status = PlayerStatus.PLAYER_LIST;
        }
        [Command]
        public void CmdClosePlayerMenu()
        {
            if (status == PlayerStatus.PLAYER_LIST)
                status = PlayerStatus.FREE;
        }




        // Update is called once per frame
        void Update()
        {
            rotationTimer = Mathf.Max(0f, rotationTimer - Time.deltaTime);
            bool alive = IsAlive();

            KitController kc = HeldItem()?.GetComponent<KitController>();

            nearestKitInteractable = GetNearestOfType<MonoBehaviour>(1.5f, c => kc?.action?.CanUse(c?.gameObject)==true)?.gameObject;
            nearestInteractable = GetNearestOfType<Interactable>(1.5f, c => c.CanInteract(this));
            nearestSabotageable = GetNearestOfType<PlayerController>(1.5f, c => c.HeldItem()?.GetComponent<KitController>()?.PreparedToSabotage() == true)?.HeldItem()?.GetComponent<KitController>();
            nearestCarryable = GetNearestOfType<Carryable>(1.5f, c => c.inventory == null);

            if(nearestInteractable?.gameObject == nearestKitInteractable?.gameObject) nearestInteractable = null;


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

            Carryable heldItem = HeldItem();

            GameObject kitTarget=null;
            if(nearestKitInteractable!=null)
            {
                kitTarget = HeldItem()?.GetComponent<KitController>()?.target;
            }

            // Handle all movement code
            if (isLocalPlayer)
            {
                if (nearestInteractable != null)
                {
                    GetOrAddUiController(nearestInteractable.gameObject).CanInteract();
                }
                else if (kitTarget != null)
                {
                    GetOrAddUiController(kitTarget).CanKit();
                }
                else if(nearestKitInteractable!=null)
                {
                    GetOrAddUiController(nearestKitInteractable).CanKit();
                }


                if (nearestSabotageable != null)
                {
                    if(nearestSabotageable?.carryable?.inventory?.GetComponent<PlayerController>()?.isLocalPlayer == true)
                    {
                        if(kitTarget!=null) GetOrAddUiController(kitTarget.gameObject).CanSabotage();
                        else if (nearestKitInteractable!=null) GetOrAddUiController(nearestKitInteractable.gameObject).CanSabotage();
                    }
                    else
                    {
                        GetOrAddUiController(nearestSabotageable.gameObject).CanSabotage();
                    }
                }

                if (nearestCarryable != null && heldItem == null)
                {
                    GetOrAddUiController(nearestCarryable.gameObject).CanPickUp();
                }
                // If the player can walk around.
                if (status == PlayerStatus.FREE)
                {
                    // Point to the camera
                    if (mouseAim)
                    {
                        Plane plane = new Plane(Vector3.up, transform.position.y + 1.5f);

                        float distance;

                        Ray ray = Camera.main.ScreenPointToRay(mousePos);
                        if (Physics.Raycast(ray, out var hit, 100f, LayerMask.GetMask("Players", "Furniture")))
                        {
                            aimTarget = hit.point;
                        }
                        else
                        {
                            plane.Raycast(ray, out float enter);
                            aimTarget = HelperMethods.GetPosition2D(ray.GetPoint(enter));
                        }
                    }

                    float speedMod = 1;
                    if (heldItem != null && heldItem.TryGetComponent<Rigidbody>(out var heldBody))
                    {
                        speedMod = (body.mass) / (body.mass + heldBody.mass);
                    }
                    Vector3 newVelocity = Vector3.SmoothDamp(body.velocity, new Vector3(moveInput.x, 0, moveInput.y) * speed * speedMod, ref acceleration, smoothTime);
                    body.velocity = new Vector3(newVelocity.x, body.velocity.y, newVelocity.z);
                    if (rotationTimer > 0)
                    {
                        rotation = Quaternion.Lerp(rotation, Quaternion.LookRotation(HelperMethods.GetPosition2D(aimTarget - body.position)), Time.deltaTime * rotationSpeed);
                    }
                    else
                    {
                        if (body.velocity.magnitude > 0.2)
                        {
                            rotation = Quaternion.Lerp(rotation, Quaternion.LookRotation(HelperMethods.GetPosition2D(body.velocity)), Time.deltaTime * rotationSpeed);
                        }
                    }
                }

                else if (status == PlayerStatus.INTERACTING || status == PlayerStatus.KIT_INTERACTING)
                {
                    Vector3 stoppedVelocity = new Vector3(0, body.velocity.y, 0);
                    body.velocity = Vector3.Lerp(body.velocity, stoppedVelocity, Time.deltaTime * 100);
                }
            }

            if (isServer)
            {
                if (status == PlayerStatus.KIT_INTERACTING)
                {
                    if (kitTarget != null)
                    {
                        if (kitTarget.TryGetComponent<Rigidbody>(out var r))
                        {
                            Vector3 stoppedVelocity = new Vector3(0, r.velocity.y, 0);
                            Vector3 stoppedAngular = new Vector3(r.angularVelocity.x, 0, r.angularVelocity.z);
                            r.velocity = Vector3.Lerp(r.velocity, stoppedVelocity, Time.deltaTime * 100);
                            r.angularVelocity = Vector3.Lerp(r.angularVelocity, stoppedAngular, Time.deltaTime * 100);
                        }
                    }
                    else
                    {
                        status = PlayerStatus.FREE;
                    }
                }
            }

            

            if (bizzareGroundedCheck)
            {
                airTimer = airTime;
            }
            else
            {
                airTimer = Mathf.Max(0, airTimer - Time.deltaTime);
            }
            jumpTimer = Mathf.Max(0, jumpTimer - Time.deltaTime);
        }

        void FixedUpdate()
        {


            if (HeldItem() != null)
            {
                HeldItem().transform.position = transform.position + rotation * new Vector3(0, 1.1f, 1);
                HeldItem().transform.rotation = transform.rotation;
            }

            body.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }

        public void KillMe()
        {
            transform.Find("Capsule").gameObject.layer = 12;
            foreach (Carryable c in GetComponentsInChildren<Carryable>())
            {
                c.GetComponent<Rigidbody>().position = body.position;
                c.Spawn(body.velocity + rotation * new Vector3(0, 5, 2));
            }
        }



        public bool IsAlive()
        {
            return GetComponent<Killable>().alive;
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

        public T GetNearestOfType<T>(float distance) where T : MonoBehaviour
        {
            return GetNearestOfType<T>(distance, c => true);
        }

        public T GetNearestOfType<T>(float distance, System.Func<T, bool> validate) where T : MonoBehaviour
        {
            // Set this to be in front of player.
            Vector3 startLocation = transform.position + rotation*Vector3.forward;
            Collider[] hitColliders = Physics.OverlapSphere(startLocation, distance);
            T nearest = null;
            float nearestDistance = distance;
            foreach (Collider collider in hitColliders)
            {
                float thisDistance;
                if (collider.attachedRigidbody != null &&
                    collider.attachedRigidbody.TryGetComponent<T>(out T component) &&
                    (thisDistance = Vector3.Distance(startLocation, collider.ClosestPoint(startLocation))) < nearestDistance &&
                    validate(component)
                )
                {
                    nearest = component;
                    nearestDistance = thisDistance;
                }
            }
            return nearest;
        }

        public Carryable HeldItem()
        {
            return GetComponent<Inventory>().Get(carriedItem);
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
