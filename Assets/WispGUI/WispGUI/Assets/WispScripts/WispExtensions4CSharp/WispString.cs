using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WispExtensions
{
    public static class WispString
    {
        public static char DoubleQuote { get { return '"'; } }
        public static char BackSlash { get { return '\\'; } }

        public static string GetStringWithNoCommaAtTheEnd(this string ParamMe)
        {
            if (ParamMe.Substring(ParamMe.Length - 1) == ",")
            {
                return ParamMe.TrimEnd(',');
            }

            return ParamMe;
        }

        public static string Unescape_Regex(this string ParamMe)
        {
            return Regex.Unescape(ParamMe);
        }

        public static string SurroundWithDoubleQuotes(this string ParamMe)
        {
            return DoubleQuote + ParamMe + DoubleQuote;
        }

        public static string SurroundWithCurlyBraces(this string ParamMe)
        {
            return "{" + ParamMe + "}";
        }

        public static string SurroundWithBars(this string ParamMe)
        {
            return "[" + ParamMe + "]";
        }

        public static int ToInt(this string ParamMe)
        {
            int result = 0;

            if (int.TryParse(ParamMe, out result))
                return result;
            else
                return 0;
        }

        public static long ToLong(this string ParamMe)
        {
            long result = 0;

            if (long.TryParse(ParamMe, out result))
                return result;
            else
                return 0;
        }

        public static float ToFloat(this string ParamMe)
        {
            float result = 0;

            if (float.TryParse(ParamMe, out result))
                return result;
            else
                return 0f;
        }

        public static string GetDigitsOnly(this string ParamMe)
        {
            if (ParamMe == "" || ParamMe == null)
                return "";

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < ParamMe.Length; i++)
            {
                if (Char.IsDigit(ParamMe[i]))
                    stringBuilder.Append(ParamMe[i]);
            }

            return stringBuilder.ToString();
        }

        public static bool ToBool(this string ParamMe)
        {
            if (ParamMe == "")
                return false;

            if (ParamMe == "true" || ParamMe == "yes")
            {
                return true;
            }

            if (ParamMe == "false" || ParamMe == "no")
            {
                return true;
            }

            if (ParamMe.GetDigitsOnly().Length > 0)
            {
                if (ParamMe.GetDigitsOnly().ToInt() > 0)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Compute the distance between two strings.
        /// From : https://www.csharpstar.com/csharp-string-distance-algorithm/
        /// </summary>
        public static int LevenshteinDistance(string ParamStringOne, string ParamStringTwo)
        {
            int n = ParamStringOne.Length;
            int m = ParamStringTwo.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (ParamStringTwo[j - 1] == ParamStringOne[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static int GetLevensteinDistance(this string ParamMe, string ParamProbe)
        {
            return WispString.LevenshteinDistance(ParamMe, ParamProbe);
        }

        public static bool IsSemiColonTerminated(this string ParamMe)
        {
            if (ParamMe.Length > 0)
            {
                if (ParamMe.Substring(ParamMe.Length - 1) == ";")
                    return true;
            }

            return false;
        }

        public static string UrlEncode(this string ParamMe)
        {
            return System.Net.WebUtility.UrlEncode(ParamMe);
        }

        public static string ToBase64(this string ParamMe)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(ParamMe);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string ParamMe)
        {
            byte[] bytes = Convert.FromBase64String(ParamMe);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string FromBase64_NoFail(this string ParamMe)
        {
            byte[] bytes;

            try
            {
                bytes = Convert.FromBase64String(ParamMe);
            }
            catch (Exception)
            {
                bytes = Encoding.UTF8.GetBytes("");
            }

            return Encoding.UTF8.GetString(bytes);
        }
    }
}