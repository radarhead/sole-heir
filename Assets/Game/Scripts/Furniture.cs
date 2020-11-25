using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    public class Furniture : MonoBehaviour
    {
        public float width = 0.5f;
        public float height = 0.5f;
        public FurnitureType type = FurnitureType.CENTER;
        public int inventorySize = 0;
    }

    public enum FurnitureType
    {
        CENTER,
        WALL,
        CORNER
    }
}
