using System.Collections.Generic;

namespace WispExtensions
{
    public static class WispFloat
    {
        public static float GetPercentage(this float ParamMe, float ParamPercentage)
        {
            return ParamMe * (ParamPercentage / 100);
        }

        public static float ChangeByPercentage(this float ParamMe, float ParamPercentage)
        {
            return ParamMe + (ParamMe * (ParamPercentage / 100));
        }

        public static bool DeltaCheck(this float ParamMe, float ParamOther, float ParamThreshold)
        {
            if (ParamMe >= ParamOther - ParamThreshold && ParamMe <= ParamOther + ParamThreshold)
            {
                return false;
            }

            return true;
        }
        
        public static bool IsBetween(this float ParamMe, float ParamFloor, float ParamCeil)
        {
            if (ParamMe >= ParamFloor && ParamMe <= ParamCeil)
            {
                return true;
            }

            return false;
        }
    }
}