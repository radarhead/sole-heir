using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    public class DoorPrototype : MonoBehaviour
    {
        public List<Vector2> outline;

        void Start()
        {
            RoomGenerator generator = gameObject.GetComponentInParent(typeof(RoomGenerator)) as RoomGenerator;
            if(generator != null)
            {
                for(int i=1; i<outline.Count; i++)
                {
                    transform.Find("Wall").gameObject.GetComponent<CreateMesh>().AddMesh(outline[i], outline[i-1], generator.roomSpacing/2);
                }
            }

            //for(float i=0; i<)
        }
        

        public float GetWidth()
        {
            return(outline[0].x + outline[outline.Count-1].x);
        }

        public float GetHeight()
        {
            float height = 0;
            foreach (Vector2 item in outline)
            {
                if(item.y > height) height = item.y;
            }
            return height;
        }
    }
}


