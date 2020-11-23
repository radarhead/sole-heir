using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    public class DoorPrototype : MonoBehaviour
    {


        void Start()
        {
            RoomGenerator generator = gameObject.GetComponentInParent(typeof(RoomGenerator)) as RoomGenerator;
            if(generator != null)
            {
                GameObject fade = transform.Find("Fade").gameObject;
                fade.transform.localScale = new Vector3(-GetWidth(), GetHeight(), 1);
                int divisions = 20;
                for(int i=0; i<divisions; i++)
                {
                    float z = generator.roomSpacing / divisions / 2 * i;
                    float opacity = 1.0f / divisions * (i+1);

                    GameObject newChild = GameObject.Instantiate(fade, transform);
                    //newChild.transform.localScale = fade.transform.localScale;
                    newChild.transform.parent = transform;
                    newChild.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(0.0f, 0.0f, 0.0f, opacity*opacity));
                    newChild.transform.localPosition += new Vector3(0,0,z);
                }

                transform.Find("LeftWall").gameObject.GetComponent<CreateMesh>().AddMesh(new Vector2(0,0), new Vector2(generator.roomSpacing/2,GetHeight()));
                transform.Find("RightWall").gameObject.GetComponent<CreateMesh>().AddMesh(new Vector2(), new Vector2(generator.roomSpacing/2,GetHeight()));
                transform.Find("RightWall").localPosition += new Vector3(GetWidth(),0,0);

            }

            //for(float i=0; i<)
        }
        public List<Vector2> top;

        public float GetWidth()
        {
            return(top[0].x + top[top.Count-1].x);
        }

        public float GetHeight()
        {
            float height = 0;
            foreach (Vector2 item in top)
            {
                if(item.y > height) height = item.y;
            }
            return height;
        }
    }
}


