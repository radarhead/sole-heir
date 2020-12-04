using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    [System.Serializable]
    public class ColorPalette {
        public ColorJson dark;
        public ColorJson darkAccent;
        public ColorJson primary;
        public ColorJson lightAccent;
        public ColorJson light;

        public Color Dark(){return dark.GetColor();}
        public Color DarkAccent(){return darkAccent.GetColor();}
        public Color Primary(){return primary.GetColor();}
        public Color LightAccent(){return lightAccent.GetColor();}
        public Color Light(){return light.GetColor();}
    }
}