using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class HelperMethods
    {
        public static Vector3 GetPosition2D(Vector3 oldPos)
        {
            return new Vector3(oldPos.x,0,oldPos.z);
        }

        public static Vector3 GetCenter(GameObject go)
        {
            if(go == null) return Vector3.zero;
            return go.GetComponentInChildren<Collider>().bounds.center;
        }

        public static Bounds LocalBounds(MonoBehaviour mb)
        {
            Bounds b = new Bounds();

            foreach(var r in mb.GetComponentsInChildren<Renderer>())
            {
                b.Encapsulate(r.bounds);
            }

            if(mb.GetComponentsInChildren<ToonOutline>() != null)
            {
                b.Expand(mb.GetComponentInChildren<ToonOutline>().offset * 2);
            }

            return b;
        }

        public static void HideOrShowObject(GameObject go, bool hideShow)
        {
            foreach(var c in go.GetComponentsInChildren<Renderer>()) {c.enabled = hideShow;}
            foreach(var c in go.GetComponentsInChildren<ToonOutline>()) {c.enabled = hideShow;}
        }

        public static RoomGenerator FindCurrentRoom(MonoBehaviour go)
        {
            // Find the current room
            foreach(RoomGenerator roomGenerator in Object.FindObjectsOfType<RoomGenerator>())
            {
                if( go != null &&
                    go.transform.position.x > roomGenerator.bottomLeft.x - roomGenerator.roomSpacing/2 
                    && go.transform.position.x < roomGenerator.topRight.x + roomGenerator.roomSpacing/2
                    && go.transform.position.z > roomGenerator.bottomLeft.z - roomGenerator.roomSpacing/2
                    && go.transform.position.z < roomGenerator.topRight.z + roomGenerator.roomSpacing/2)
                {
                    return roomGenerator;
                }
            }

            return null;
        }   
    }
}