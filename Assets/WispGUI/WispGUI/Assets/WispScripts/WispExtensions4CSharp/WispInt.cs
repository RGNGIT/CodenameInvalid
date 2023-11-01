using System;
using System.Collections.Generic;

namespace WispExtensions
{
    public static class WispInt
    {
        // public static long GetPercentage(this long ParamMe, float ParamPercentage)
        // {
        //     return Convert.ToInt64(ParamMe * (ParamPercentage / 100));
        // }

        // public static long ChangeByPercentage(this long ParamMe, float ParamPercentage)
        // {
        //     return ParamMe + Convert.ToInt64((ParamMe * (ParamPercentage / 100)));
        // }

        // public static long Clamp(this long ParamMe, long ParamMin, long ParamMax)
        // {
        //     if (ParamMe < ParamMin) { return ParamMin; }
        //     if (ParamMe > ParamMax) { return ParamMax; }
        //     return ParamMe;
        // }

        public static bool IsOdd(this int ParamMe)
        {
            if(ParamMe%2 == 0)
                return false;

            return true;
        }

        public static bool IsEven(this int ParamMe)
        {
            if(ParamMe%2 == 0)
                return true;

            return false;
        }
    }
}