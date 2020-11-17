using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

namespace SoleHeir
{
    public class PlayerController : NetworkBehaviour
    {
        public GameObject bulletPrefab;
        private float speed = 10;
        private float smoothTime = 0.05f;
        private Vector2 moveInput;
        private Vector3 acceleration = Vector3.zero;
        private Vector2 cameraPosition = Vector2.zero;
        [SyncVar]
        private Vector3 aimTarget;
        Rigidbody body;
        private bool mouseAim = false;
        public GameObject heldItem;
        public GameObject gunPrefab;

        // Start is called before the first frame update
        void Start()
        {
            body = GetComponent<Rigidbody>();
            UnityEngine.Object[] spawners = GameObject.FindGameObjectsWithTag("SpawnLocation");
            if(spawners.Length > 0)
            {
                GameObject spawner = spawners[UnityEngine.Random.Range(0, spawners.Length)] as GameObject;
                this.transform.position = spawner.transform.position + new Vector3(1,0,1);
            }
            heldItem = Instantiate(gunPrefab, transform);
            NetworkServer.Spawn(heldItem);
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
                // Point to the camera
                Plane plane = new Plane(Vector3.up, 0);

                float distance;
                
                Ray ray = Camera.main.ScreenPointToRay(value.Get<Vector2>());
                if (plane.Raycast(ray, out distance))
                {
                    aimTarget = (ray.GetPoint(distance) - transform.position);
                    
                }
                mouseAim = true;
            }

        }

        public void OnAim(InputValue value)
        {
            if(isLocalPlayer)
            {
                aimTarget = new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y);
                mouseAim = false;
            }
            
        }

        public void OnFire(InputValue value)
        {
            gameObject.GetComponentInChildren<GunController>().CmdInteract();
        }

        [Command]
        void CmdFire()
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            NetworkServer.Spawn(bullet);
            bullet.GetComponent<Rigidbody>().velocity = new Vector3(10, 0, 0);
        }

        // Update is called once per frame
        void Update()
        {
            // Only process inputs for local player.
            if (isLocalPlayer)
            {
                body.velocity = Vector3.SmoothDamp(body.velocity, new Vector3(moveInput.x, 0, moveInput.y)*speed, ref acceleration, smoothTime);
            }

            heldItem.transform.LookAt(aimTarget + transform.position);
        }
    }
}
