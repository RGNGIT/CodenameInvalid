using UnityEngine;

public class WhiteTheme : IThemeInitializer
{
    public Theme Initialize() 
    {
        return new Theme(
            "WhiteTheme", 
            new Color(248f, 248f, 255f), 
            Color.blue
            );
    }
}
