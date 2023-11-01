using System;
using System.Collections.Generic;
using UnityEngine;

namespace WispExtensions
{
    public static class WispLong4Unity
    {
        public static Vector2 ToVector2(this long ParamMe)
        {
            return new Vector2(ParamMe, ParamMe);
        }
    }
}