using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    public class ColorManager : MonoBehaviour {

        public static ColorManager instance = null;

        public List<ColorPalette> palettes;
        void Awake()
        {
            if(instance == null)
            {
                palettes = JsonUtility.FromJson<ColorPalettesJson>(Resources.Load<TextAsset>("Colors").text).palettes;
                instance = this;
            }
            else if(instance != this)
            {
                Destroy(gameObject);
            }
        }

        public ColorPalette RandomPalette()
        {
            return palettes[UnityEngine.Random.Range(0, palettes.Count)];
        }
    }
}