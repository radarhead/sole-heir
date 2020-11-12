using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    public class TransparencyComponent : MonoBehaviour
    {
        public bool enabled = true;
        private float alpha = 1.0f;
        public float velocity = 3.0f;
        public RoomGenerator currentRoom;
        private bool isRoom = false;

        // Start is called before the first frame update
        void Start()
        {
            // Try to get the room generator component
            currentRoom = gameObject.GetComponent<RoomGenerator>();
            if(currentRoom != null)
            {
                isRoom = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Find the current room
            if(!isRoom)
            {
                foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
                {
                    RoomGenerator roomGenerator = room.GetComponent<RoomGenerator>();
                    if(transform.position.x > roomGenerator.bottomLeft.x 
                        && transform.position.x < roomGenerator.topRight.x
                        && transform.position.z > roomGenerator.bottomLeft.z
                        && transform.position.z < roomGenerator.topRight.z)
                    {
                        currentRoom = roomGenerator;
                    }
                }
            }

            if(enabled)
            {
                alpha += velocity * Time.deltaTime;
                if(alpha>1.0f) alpha = 1.0f;
            }
            else
            {
                alpha -= velocity * Time.deltaTime;
                if(alpha<0.0f) alpha = 0.0f;
            }

            SetTargetTransparent(gameObject, alpha);
        }

        void SetTargetTransparent(GameObject target, float transparency)
        {
            Component[] a = target.GetComponentsInChildren(typeof(Renderer));
            foreach (Component b in a)
            {
                Renderer c = (Renderer)b;
                c.material.color= new Color(c.material.color.r,c.material.color.g,c.material.color.b,transparency);
            }
        }
    }

}
