using System.Linq;
using System.Collections.Generic;
using SoleHeir.GenerationUtils;
using UnityEngine;

namespace SoleHeir
{
    public class FloorGenerator : MonoBehaviour
    {
        public void Initialize(float xSize, float ySize, float roomSpacing)
        {
            CreateMesh createMesh = gameObject.GetComponent<CreateMesh>();
            createMesh.AddMesh(new Vector2(0,0), new Vector2(xSize,ySize));
        }
    }
}
