using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class PlayerController : NetworkBehaviour
    {
        private float speed = 10;
        private float smoothTime = 0.05f;
        private Vector3 acceleration = Vector3.zero;
        Rigidbody body;

        // Start is called before the first frame update
        void Start()
        {
            body = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            // Only process inputs for local player.
            if (isLocalPlayer)
            {
                float h = Input.GetAxisRaw("moveX");
                float v = Input.GetAxisRaw("moveY");

                var newVelocity = new Vector3(h, 0, v);
                newVelocity.Normalize();
                //newVelocity.Magnitude(speed);
                body.velocity = Vector3.SmoothDamp(body.velocity, newVelocity*speed, ref acceleration, smoothTime);
            }
        }
    }
}

