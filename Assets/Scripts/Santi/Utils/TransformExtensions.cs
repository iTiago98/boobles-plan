using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Santi.Utils
{
    public static class TransformExtensions
    {
        public static void SetXPosition(this Transform t, float newValue)
        {
            t.position = new Vector3(newValue, t.position.y, t.position.z);
        }

        public static void SetYPosition(this Transform t, float newYValue)
        {
            t.position = new Vector3(t.position.x, newYValue, t.position.z);
        }

        public static void SetZPosition(this Transform t, float newZValue)
        {
            t.position = new Vector3(t.position.x, t.position.y, newZValue);
        }
    }
}
