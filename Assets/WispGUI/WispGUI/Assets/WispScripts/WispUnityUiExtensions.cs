using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace WispExtensions
{
    public static class WispUnityUiExtensions
    {
        // Image        
        public static void ApplyStyle(this Image ParamMe, WispGuiStyle ParamStyle, float ParamOpacity, WispSubStyleRule ParamSubStyleRule = WispSubStyleRule.None)
        {
            WispSubStyleBlock ssb = ParamStyle.GetSubStyle(ParamSubStyleRule);
            ParamMe.color = ssb.activeBackgroundColor.ColorOpacity(ParamOpacity);
            ParamMe.material = ssb.material;

            if (ParamSubStyleRule == WispSubStyleRule.Icon)
                ParamMe.type = Image.Type.Simple;
            else
                ParamMe.type = ssb.spriteDrawMode;

            ParamMe.pixelsPerUnitMultiplier = ssb.pixelsPerUnitMultiplier;
            
            if (ParamSubStyleRule != WispSubStyleRule.Icon && ParamSubStyleRule != WispSubStyleRule.Picture)
                ParamMe.sprite = ssb.graphics;
        }

        public static void ApplyStyle_Inactive(this Image ParamMe, WispGuiStyle ParamStyle, float ParamOpacity, WispSubStyleRule ParamSubStyleRule = WispSubStyleRule.None)
        {
            WispSubStyleBlock ssb = ParamStyle.GetSubStyle(ParamSubStyleRule);
            ParamMe.color = ssb.inactiveBackgroundColor.ColorOpacity(ParamOpacity);
            ParamMe.material = ssb.material;
            
            if (ParamSubStyleRule == WispSubStyleRule.Icon)
                ParamMe.type = Image.Type.Simple;
            else
                ParamMe.type = ssb.spriteDrawMode;

            ParamMe.pixelsPerUnitMultiplier = ssb.pixelsPerUnitMultiplier;
            
            if (ParamSubStyleRule != WispSubStyleRule.Icon && ParamSubStyleRule != WispSubStyleRule.Picture)
                ParamMe.sprite = ParamStyle.GetSubStyle(ParamSubStyleRule).graphics;
        }

        public static void ApplyStyle_Selected(this Image ParamMe, WispGuiStyle ParamStyle, float ParamOpacity, WispSubStyleRule ParamSubStyleRule = WispSubStyleRule.None)
        {
            WispSubStyleBlock ssb = ParamStyle.GetSubStyle(ParamSubStyleRule);
            ParamMe.color = ssb.selectedBackgroundColor.ColorOpacity(ParamOpacity);
            ParamMe.material = ssb.material;
            
            if (ParamSubStyleRule == WispSubStyleRule.Icon)
                ParamMe.type = Image.Type.Simple;
            else
                ParamMe.type = ssb.spriteDrawMode;

            ParamMe.pixelsPerUnitMultiplier = ssb.pixelsPerUnitMultiplier;
            
            if (ParamSubStyleRule != WispSubStyleRule.Icon && ParamSubStyleRule != WispSubStyleRule.Picture)
                ParamMe.sprite = ParamStyle.GetSubStyle(ParamSubStyleRule).graphics;
        }

        // Scrollbars
        public static void ApplyStyle_ScrollBar_V(this Image ParamMe, WispGuiStyle ParamStyle, float ParamOpacity)
        {
            ParamMe.color = ParamStyle.ScrollbarBackgroundColor.ColorOpacity(ParamOpacity);
            ParamMe.type = ParamStyle.ScrollbarDrawMode;
            ParamMe.sprite = ParamStyle.verticalScrollBar;
        }

        public static void ApplyStyle_ScrollBar_Handle_V(this Image ParamMe, WispGuiStyle ParamStyle, float ParamOpacity)
        {
            ParamMe.color = ParamStyle.ScrollbarHandleColor.ColorOpacity(ParamOpacity);
            ParamMe.type = ParamStyle.ScrollbarDrawMode;
            ParamMe.sprite = ParamStyle.verticalScrollBarHandle;
        }

        public static void ApplyStyle_ScrollBar_H(this Image ParamMe, WispGuiStyle ParamStyle, float ParamOpacity)
        {
            ParamMe.color = ParamStyle.ScrollbarBackgroundColor.ColorOpacity(ParamOpacity);
            ParamMe.type = ParamStyle.ScrollbarDrawMode;
            ParamMe.sprite = ParamStyle.horizontalScrollBar;
        }

        public static void ApplyStyle_ScrollBar_Handle_H(this Image ParamMe, WispGuiStyle ParamStyle, float ParamOpacity)
        {
            ParamMe.color = ParamStyle.ScrollbarHandleColor.ColorOpacity(ParamOpacity);
            ParamMe.type = ParamStyle.ScrollbarDrawMode;
            ParamMe.sprite = ParamStyle.horizontalScrollBarHandle;
        }

        // TextMeshProUGUI
        public static void ApplyStyle(this TextMeshProUGUI ParamMe, WispGuiStyle ParamStyle, float ParamOpacity, WispFontSize ParamFontSize, WispSubStyleRule ParamSubStyleRule = WispSubStyleRule.None)
        {
            ParamMe.color = ParamStyle.GetSubStyle(ParamSubStyleRule).activeColor.ColorOpacity(ParamOpacity);
            ParamMe.font = ParamStyle.Font;
            ParamMe.fontSize = ParamStyle.GetFontSize(ParamFontSize);
        }

        public static void ApplyStyle_Inactive(this TextMeshProUGUI ParamMe, WispGuiStyle ParamStyle, float ParamOpacity, WispFontSize ParamFontSize, WispSubStyleRule ParamSubStyleRule = WispSubStyleRule.None)
        {
            ParamMe.color = ParamStyle.GetSubStyle(ParamSubStyleRule).inactiveColor.ColorOpacity(ParamOpacity);
            ParamMe.font = ParamStyle.Font;
            ParamMe.fontSize = ParamStyle.GetFontSize(ParamFontSize);
        }

        public static void ApplyStyle_Selected(this TextMeshProUGUI ParamMe, WispGuiStyle ParamStyle, float ParamOpacity, WispFontSize ParamFontSize, WispSubStyleRule ParamSubStyleRule = WispSubStyleRule.None)
        {
            ParamMe.color = ParamStyle.GetSubStyle(ParamSubStyleRule).selectedColor.ColorOpacity(ParamOpacity);
            ParamMe.font = ParamStyle.Font;
            ParamMe.fontSize = ParamStyle.GetFontSize(ParamFontSize);
        }

        // TMP_Text
        public static void ApplyStyle(this TMP_Text ParamMe, WispGuiStyle ParamStyle, float ParamOpacity, WispFontSize ParamFontSize, WispSubStyleRule ParamSubStyleRule = WispSubStyleRule.None)
        {
            ParamMe.color = ParamStyle.GetSubStyle(ParamSubStyleRule).activeColor.ColorOpacity(ParamOpacity);
            ParamMe.font = ParamStyle.Font;
            ParamMe.fontSize = ParamStyle.GetFontSize(ParamFontSize);
        }
    } 
}