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
        private Vector3 currentRoom;
        private Vector3 targetPosition;
        private Vector3 targetOffset = new Vector3(0,0,-10);
        private Vector3 velocity = Vector3.zero;
        public float bottomY = 0;
        public float bottomYOffset = -1;



        // Start is called before the first frame update
        void Awake()
        {
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
                    currentRoom = (roomGenerator.topRight + roomGenerator.bottomLeft) / 2;
                    bottomY = roomGenerator.bottomLeft.z;
                    targetOffset = offset + new Vector3(0,0, -Vector3.Distance(roomGenerator.bottomLeft, roomGenerator.topRight)/4);
                                
                    transform.rotation = Quaternion.Euler(rotation);
                    targetPosition = ((player.transform.position*2 + currentRoom) / 3) + transform.rotation*targetOffset;
                    targetPosition = new Vector3(targetPosition.x, targetPosition.y, Math.Max(targetPosition.z, bottomY + bottomYOffset));
                    realPosition = Vector3.SmoothDamp(realPosition, targetPosition, ref velocity, smoothTime);
                    transform.position = realPosition + new Vector3(
                        UnityEngine.Random.Range(-screenShake, screenShake),
                        0,
                        UnityEngine.Random.Range(-screenShake, screenShake));
                }
            }
        }

        public void SetScreenShake(float screenShake)
        {
            this.screenShake = screenShakeAmt*screenShake;
        }
    }
}
