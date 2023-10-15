using UnityEngine;

public interface IThemeInitializer
{
    public Theme Initialize();
}

public class Theme
{
    public string Name { get; }

    public Theme(
        string Name, 
        Color Common_BackgroundColor, 
        Color IntroTest_TileColor
        )
    {
        this.Name = Name;
        this.Common_BackgroundColor = Common_BackgroundColor;
        this.IntroTest_TileColor = IntroTest_TileColor;
    }

    public Color Common_BackgroundColor { get; }
    public Color IntroTest_TileColor { get; }
}