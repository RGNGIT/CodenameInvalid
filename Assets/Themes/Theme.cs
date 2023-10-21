using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

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

    private Texture2D ImageLoader(string path) 
    {
        Texture2D temp = new(512, 512);
        byte[] imageBytes = File.ReadAllBytes(path);
        temp.LoadImage(imageBytes);
        return temp;
    }

    private void LoadIcons(List<string> iconPaths) 
    {
        iconPaths.ForEach(p => DockIcons.Add(ImageLoader(p)));
    }

    public Theme(
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
        this.Main_BlockBase = Main_BlockBase;
        LoadIcons(iconPaths);
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
    /// <summary>
    /// Цвет базы для блока главного меню
    /// </summary>
    public Color Main_BlockBase { get; }
    /// <summary>
    /// Иконки на док (512x512)
    /// </summary>
    public List<Texture2D> DockIcons { get; } = new();
}