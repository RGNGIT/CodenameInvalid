using System.Collections.Generic;
using UnityEngine;

namespace WispExtensions
{
    public static class WispFloat4Unity
    {
        public static Vector2 ToVector2(this float ParamMe)
        {
            return new Vector2(ParamMe, ParamMe);
        }

        public static float SnapToGrid(this float ParamMe, int ParamGridSize)
        {
            return Mathf.RoundToInt( ParamMe/(float)ParamGridSize ) * ParamGridSize;
        }
    }
}