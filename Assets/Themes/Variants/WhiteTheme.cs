using UnityEngine;

public class WhiteTheme : IThemeInitializer
{
    public Theme Initialize() 
    {
        return new Theme(
            "Светлая тема", 
            new Color(248f, 248f, 255f),
            new Color(0, 0, 0),
            Color.blue
            );
    }
}
