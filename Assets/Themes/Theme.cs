using UnityEngine;

public interface IThemeInitializer
{
    public Theme Initialize();
}

public class Theme
{
    /// <summary>
    /// Наименование темы
    /// </summary>
    public string Name { get; }

    public Theme(
        string Name, 
        Color Common_BackgroundColor,
        Color Common_DockColor,
        Color TextColor_0,
        Color TextColor_1,
        Color TextColor_2,
        Color IntroTest_TileColor,
        Color Icon_Highlight
        )
    {
        this.Name = Name;
        this.Common_BackgroundColor = Common_BackgroundColor;
        this.Common_DockColor = Common_DockColor;
        this.TextColor_0 = TextColor_0;
        this.TextColor_1 = TextColor_1;
        this.TextColor_2 = TextColor_2;
        this.IntroTest_TileColor = IntroTest_TileColor;
        this.Icon_Highlight = Icon_Highlight;
    }
    /// <summary>
    /// Цвет задника
    /// </summary>
    public Color Common_BackgroundColor { get; }
    /// <summary>
    /// Цвет дока с табами
    /// </summary>
    public Color Common_DockColor { get; }
    /// <summary>
    /// Цвет текста заголовков 0 уровня
    /// </summary>
    public Color TextColor_0 { get; }
    /// <summary>
    /// Цвет текста заголовков 1 уровня
    /// </summary>
    public Color TextColor_1 { get; }
    /// <summary>
    /// Цвет текста заголовков 2 уровня
    /// </summary>
    public Color TextColor_2 { get; }
    /// <summary>
    /// Цвет тайлов во входном тесте
    /// </summary>
    public Color IntroTest_TileColor { get; }
    /// <summary>
    /// Цвет хайлайта иконки
    /// </summary>
    public Color Icon_Highlight { get; }
}