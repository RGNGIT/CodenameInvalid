using UnityEngine;

namespace WispExtensions
{
    public static class WispColor
    {
        public static Color ColorOpacity(this Color ParamColor, float ParamOpacity)
        {
            return new Color(ParamColor.r, ParamColor.g, ParamColor.b, ParamColor.a * ParamOpacity);
        }

        public static Color RGB_Average(this Color ParamColor)
        {
            float average = (ParamColor.r + ParamColor.g + ParamColor.b) / 3;
            return new Color(average, average, average, ParamColor.a);
        }

        public static Color RandomColor()
        {
            return new Color(Random.value, Random.value, Random.value, 1);
        }

        public static float HighestRgb(this Color ParamColor)
        {
            float max = 0;

            if (ParamColor.r > max)
                max = ParamColor.r;

            if (ParamColor.g > max)
                max = ParamColor.g;

            if (ParamColor.b > max)
                max = ParamColor.b;

            return max;
        }

        public static float MonochromacyRatio(this Color ParamMe)
        {
            // If all components are equal then this color is monochrome.
            if (ParamMe.r == ParamMe.g && ParamMe.g == ParamMe.b)
                return 1;
            
            // If two components are 0 and the third is not, then this color is monochrome.
            if (ParamMe.r > 0 && ParamMe.g == 0 && ParamMe.b == 0)
                return 1;
            else if (ParamMe.r == 0 && ParamMe.g > 0 && ParamMe.b == 0)
                return 1;
            else if (ParamMe.r == 0 && ParamMe.g == 0 && ParamMe.b > 0)
                return 1;

            else
            {
                return 0; // TODO
            }
        }

        public static Color Invert(this Color ParamMe)
        {
            return new Color(1-ParamMe.r, 1-ParamMe.g, 1-ParamMe.b, ParamMe.a);
        }
    }
}