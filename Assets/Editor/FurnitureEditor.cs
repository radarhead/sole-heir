using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoleHeir
{
    [CustomEditor(typeof (Furniture)), CanEditMultipleObjects]
    public class FurnitureEditor : Editor
    {
        protected virtual void OnSceneGUI()
        {
            Furniture t = target as Furniture;
            Vector3 pos = t.transform.position;
            
            Handles
                .DrawSolidRectangleWithOutline(new Vector3[] {
                    new Vector3(pos.x, pos.y, pos.z),
                    new Vector3(pos.x, pos.y, pos.z - t.height),
                    new Vector3(pos.x + t.width, pos.y, pos.z - t.height),
                    new Vector3(pos.x + t.width, pos.y, pos.z)
                },
                new Color(0.5f, 0.5f, 0.5f, 0.1f),
                new Color(0, 0, 0, 1));
            
            if(t.type != FurnitureType.CENTER)
            {
                Handles
                .DrawSolidRectangleWithOutline(new Vector3[] {
                    new Vector3(pos.x , pos.y + 10, pos.z),
                    new Vector3(pos.x + t.width, pos.y + 10, pos.z),
                    new Vector3(pos.x + t.width, pos.y, pos.z),
                    new Vector3(pos.x , pos.y, pos.z)
                },
                new Color(0.5f, 0.5f, 0.5f, 0.1f),
                new Color(0, 0, 0, 1));
            }

            if(t.type == FurnitureType.CORNER)
            {
                Handles
                .DrawSolidRectangleWithOutline(new Vector3[] {
                    new Vector3(pos.x, pos.y + 10, pos.z),
                    new Vector3(pos.x, pos.y + 10, pos.z - t.height),
                    new Vector3(pos.x, pos.y, pos.z - t.height),
                    new Vector3(pos.x, pos.y, pos.z)
                },
                new Color(0.5f, 0.5f, 0.5f, 0.1f),
                new Color(0, 0, 0, 1));
            }

            t.width =
                Handles
                    .ScaleValueHandle(t.width,
                    new Vector3(t.width,0,0),
                    Quaternion.identity,
                    1.0f,
                    Handles.CubeHandleCap,
                    1.0f);
            t.height =
                Handles
                    .ScaleValueHandle(t.height,
                    new Vector3(0,0,-t.height),
                    Quaternion.identity,
                    1.0f,
                    Handles.CubeHandleCap,
                    1.0f);
        }
    }
}
