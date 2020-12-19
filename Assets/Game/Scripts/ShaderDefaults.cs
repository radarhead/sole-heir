using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{
    [ExecuteInEditMode]
    public class ShaderDefaults : MonoBehaviour
    {
        void Awake()
        {
            ColorPalette palette = JsonUtility.FromJson<ColorPalettesJson>(Resources.Load<TextAsset>("Colors").text).palettes[0];
            Shader.SetGlobalColor("_PaletteDarkColor", palette.Dark());
            Shader.SetGlobalColor("_PaletteDarkAccentColor", palette.DarkAccent());
            Shader.SetGlobalColor("_PalettePrimaryColor", palette.Primary());
            Shader.SetGlobalColor("_PaletteLightAccentColor", palette.LightAccent());
            Shader.SetGlobalColor("_PaletteLightColor", palette.Light());
            Shader.SetGlobalVector("_TopRight", new Vector3(100000,100000, 100000));
            Shader.SetGlobalVector("_BottomLeft", new Vector3(-100000, -100000, -100000));
            Shader.SetGlobalInt("_LightsOut", 0);
        }
    }
}
