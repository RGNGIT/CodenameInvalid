using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Themes
{
    internal class AdaptiveTheme : Theme
    {
        public AdaptiveTheme(
            string Name,
            Color Common_BackgroundColor,
            Color Common_DockColor,
            Color TextColor_0,
            Color TextColor_1,
            Color TextColor_2,
            Color IntroTest_TileColor,
            Color Icon_Highlight,
            Color Main_BlockBase,
            List<string> iconPaths
            ) : base(
                Name,
                Common_BackgroundColor, 
                Common_DockColor, 
                TextColor_0, TextColor_1, 
                TextColor_2, IntroTest_TileColor, 
                Icon_Highlight, 
                Main_BlockBase,
                iconPaths
                )
        {

        }
    }
}
