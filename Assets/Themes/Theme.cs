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
        Color Common_DockColor,
        Color TextColor_0,
        Color IntroTest_TileColor
        )
    {
        this.Name = Name;
        this.Common_BackgroundColor = Common_BackgroundColor;
        this.Common_DockColor = Common_DockColor;
        this.TextColor_0 = TextColor_0;
        this.IntroTest_TileColor = IntroTest_TileColor;
    }

    public Color Common_BackgroundColor { get; }
    public Color Common_DockColor { get; }
    public Color TextColor_0 { get; }
    public Color IntroTest_TileColor { get; }
}