using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    [System.Serializable]
    public class ColorJson {
        public int r;
        public int g;
        public int b;

        public Color GetColor()
        {
            return new Color(r/255f,g/255f,b/255f,1);
        }

    }
}