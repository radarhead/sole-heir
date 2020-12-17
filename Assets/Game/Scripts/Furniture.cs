using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    public class Furniture : MonoBehaviour
    {
        public FurnitureType type = FurnitureType.CENTER;
        public float rarity = 1;

        public bool onePerRoom = false;
        public Vector3 offset = new Vector3();
        void Awake()
        {
            gameObject.layer = 9;
            
            transform.parent = FurnitureManager.instance.transform;
        }
    }


    public enum FurnitureType
    {
        CENTER,
        WALL,
        CORNER
    }
}
