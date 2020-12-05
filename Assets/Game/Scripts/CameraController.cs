using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance = null;
        public float screenShakeVelocity;
        public float screenShakeAmt;
        private float screenShake = 0f;
        public float smoothTime = 0.15f;
        private Vector3 realPosition = Vector3.zero;
        public Vector3 offset;
        public Vector3 rotation;
        private Vector3 currentRoomCenter;
        private Vector3 targetPosition;
        private Vector3 targetOffset = new Vector3(0,0,-10);
        private Vector3 velocity = Vector3.zero;
        public float bottomY = 0;
        public float bottomYOffset = -1;

        Camera camera;


        // Start is called before the first frame update
        void Awake()
        {
            camera = GetComponent<Camera>();
            if(instance == null)
            {
                instance = this;
            }
            else if(instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.rotation = Quaternion.Euler(rotation);

            screenShake = Mathf.Max(0f, screenShake - Time.deltaTime*screenShakeVelocity);

            // Find the local player
            GameObject player = null;
            foreach (GameObject thisPlayer in GameObject.FindGameObjectsWithTag("Player"))
            {
                if(thisPlayer.GetComponent<PlayerController>().isLocalPlayer)
                {
                    player = thisPlayer;
                }
            }
            if(player!=null)
            {
                RoomGenerator roomGenerator = player.GetComponent<AnonymousComponent>().currentRoom;
                if(roomGenerator != null)
                {
                    Vector3 oldPosition = transform.position;

                    currentRoomCenter = (roomGenerator.topRight + roomGenerator.bottomLeft) / 2;
                    targetOffset = offset + new Vector3(0,0, -Vector3.Distance(roomGenerator.bottomLeft, roomGenerator.topRight)/4);
                    targetPosition = ((player.transform.position*2 + currentRoomCenter) / 3) + transform.rotation*targetOffset;

                    

                                





                    bottomY = roomGenerator.bottomLeft.z;
                    Ray r = camera.ViewportPointToRay(new Vector2(0,0));
                    Plane bottomPlane = new Plane(Vector3.up, Vector3.zero);
                    float enter = 0f;
                    if(bottomPlane.Raycast(r, out enter))
                    {
                        bottomYOffset = (r.origin*enter).z;
                    }

                    targetPosition = new Vector3(targetPosition.x, targetPosition.y, Math.Max(targetPosition.z, bottomY - roomGenerator.roomSpacing/2));
                    realPosition = Vector3.SmoothDamp(realPosition, targetPosition, ref velocity, smoothTime);
                    transform.position = realPosition + new Vector3(
                        UnityEngine.Random.Range(-screenShake, screenShake),
                        0,
                        UnityEngine.Random.Range(-screenShake, screenShake));
                    //transform.LookAt(player.transform, Vector3.up);
                }
            }
        }

        public void SetScreenShake(float screenShake)
        {
            this.screenShake = screenShakeAmt*screenShake;
        }
    }
}
