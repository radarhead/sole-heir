using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes2D;

namespace SoleHeir
{
    [ExecuteAlways] public class CircleRotationHandler : MonoBehaviour
    {
        public float circleAmount = 0;

        
        void Update()
        {
            circleAmount = Mathf.Clamp(circleAmount, 0, 1);
            Shapes2D.Shape s = GetComponent<Shapes2D.Shape>();
            if (s != null)
            {
                s.settings.startAngle = 0;
                s.settings.endAngle = Mathf.Max(0.00001f, 360 * circleAmount);

            }
        }
    }
}