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
        float reloadTimer =0;
        float rotationTimer =0;
        [SyncVar] float dodgeRollTimer =0;

        public GameObject carryablePrefab;
        private Vector2 moveInput;
        private Vector3 acceleration = Vector3.zero;
        private Vector2 cameraPosition = Vector2.zero;
        [SyncVar] Vector3 aimTarget;
        [SyncVar] Quaternion rotation = Quaternion.identity;

        Rigidbody body;
        private bool mouseAim = false;
        private Vector2 mousePos = Vector2.zero;
        public GameObject gunPrefab;
        public GameObject bulletPrefab;

        

        //readonly SyncList<GameObject> inventory = new SyncList<GameObject>();
        [SyncVar]
        public int carriedItem = 0;

        // Start is called before the first frame update
        void Start()
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
                CmdSetGun();
            }
        }

        [Command]
        void CmdSetRotation(Quaternion rotation)
        {
            this.rotation = rotation;
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
            if(isLocalPlayer)
            {
                moveInput = value.Get<Vector2>();
            }
        }



        public void OnPoint(InputValue value)
        {
            if(isLocalPlayer)
            {
                mousePos = value.Get<Vector2>();
                mouseAim = true;
                rotationTimer = rotationTime;
            }
        }

        public void OnThrow(InputValue value)
        {
            if(isLocalPlayer)
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
        }

        public void OnScroll(InputValue value)
        {
            if(isLocalPlayer)
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
        }

        public void OnAim(InputValue value)
        {
            if(isLocalPlayer)
            {
                if(value.Get<Vector2>().magnitude > 0)
                {
                    aimTarget = (new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y));
                    mouseAim = false;
                    rotationTimer = rotationTime;
                }

            }
        }

        public void OnFire(InputValue value)
        {
            if(isLocalPlayer)
            {
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
        }

        public void OnDodgeRoll(InputValue value)
        {
            if(isLocalPlayer)
            {
                if(body.velocity.magnitude > 0)
                {
                    CmdDodgeRoll();
                }
            }
        }

        public void CmdDodgeRoll()
        {
            if(body.velocity.magnitude > 0)
            {
                dodgeRollTimer = dodgeRollTime + dodgeRollSlowDown + dodgeRollLag;
            }
        }

        public void OnItemOne(InputValue value)
        {
            if(isLocalPlayer)
            {
                CmdSetCarriedItem(0);
            }
        }

        public void OnItemTwo(InputValue value)
        {
            if(isLocalPlayer)
            {
                CmdSetCarriedItem(1);
            }
        }

        public void OnItemThree(InputValue value)
        {
            if(isLocalPlayer)
            {
                CmdSetCarriedItem(2);
            }
        }

        public void OnItemFour(InputValue value)
        {
            if(isLocalPlayer)
            {
                CmdSetCarriedItem(3);
            }
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

        void OnCollisionEnter(Collision collision)
        {
            if(isLocalPlayer)
            {
                if(dodgeRollTimer > 0)
                {
                    dodgeRollTimer = dodgeRollLag;

                    CameraController.instance.SetScreenShake(0.5f);
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            reloadTimer = Math.Max(0f, reloadTimer - Time.deltaTime);
            rotationTimer = Math.Max(0f, rotationTimer - Time.deltaTime);
            dodgeRollTimer = Math.Max(0f, dodgeRollTimer - Time.deltaTime);

            if(dodgeRollTimer > 0)
            {
                transform.localScale= new Vector3(1,0.5f,1);
                if(dodgeRollTimer > dodgeRollSlowDown + dodgeRollLag)
                {
                    body.velocity = body.velocity.normalized * dodgeRollSpeed;
                }
                else if(dodgeRollTimer > dodgeRollLag)
                {
                    body.velocity = body.velocity.normalized * (dodgeRollSpeed * ((dodgeRollTimer - dodgeRollLag) / dodgeRollSlowDown));
                }
                else
                {
                    body.velocity = Vector3.zero;
                }
            }
            else {
                transform.localScale= new Vector3(1,1,1);

                // Only process inputs for local player.
                if (isLocalPlayer)
                {
                    if(mouseAim)
                    {
                        // Point to the camera
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
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(aimTarget), Time.deltaTime * rotationSpeed);
                        CmdSetRotation(transform.rotation);
                    }
                    else 
                    {
                        if(body.velocity.magnitude > 0.2)
                        {
                            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(body.velocity.x, 0, body.velocity.z)), Time.deltaTime * rotationSpeed);
                            CmdSetRotation(transform.rotation);
                        }
                    }

                } else {
                    transform.rotation = rotation;
                }

                if(HeldItem() != null)
                {
                    HeldItem().transform.localPosition = new Vector3(0,1,1);
                    HeldItem().transform.localRotation = Quaternion.identity;
                }
            }

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
