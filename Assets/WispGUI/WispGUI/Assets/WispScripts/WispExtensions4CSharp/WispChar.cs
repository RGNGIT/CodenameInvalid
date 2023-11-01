namespace WispExtensions
{
    public static class WispChar
    {
        public static bool IsWispEvaluableStartChar(this char ParamMe)
        {
            if (char.IsLetter(ParamMe))
                return true;

            return false;
        }

        public static bool IsWispEvaluableChar(this char ParamMe)
        {
            if (char.IsLetterOrDigit(ParamMe) || ParamMe == '_' || ParamMe == '.')
                return true;

            return false;
        }
    }
}