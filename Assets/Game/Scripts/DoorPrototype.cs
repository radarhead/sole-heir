using System.Collections;
using System.Collections.Generic;
using SoleHeir.GenerationUtils;
using UnityEngine;

namespace SoleHeir
{
    public class DoorPrototype : MonoBehaviour
    {
        public List<Vector2> outline;
        public DoorType doorType;

        public void Initialize(float roomSpacing)
        {
            for(int i=1; i<outline.Count; i++)
            {
                transform.Find("Wall").gameObject.GetComponent<CreateMesh>().AddMesh(outline[i], outline[i-1], roomSpacing/2);
            }

            transform.Find("Door Spacing").localPosition = new Vector3(GetWidth()/2,1.5f,0);
            transform.Find("Door Spacing").localScale = new Vector3(GetWidth(),3,2);
        }
        

        public float GetWidth()
        {
            return Mathf.Abs(outline[0].x + outline[outline.Count-1].x);
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


