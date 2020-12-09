using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;

namespace SoleHeir
{
    public class MinimapDoorwayController : MonoBehaviour
    {
        private Shapes2D.Shape shape;

        void Start()
        {
            shape = GetComponentInChildren<Shapes2D.Shape>();
        }
        void Update()
        {
            shape.settings.fillColor = Shader.GetGlobalColor("_PaletteLightColor");
        }
    }
}