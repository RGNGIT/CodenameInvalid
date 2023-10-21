using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WhiteTheme : IThemeInitializer
{
    private string DockIconsPath = Constants.ThemesFolder + $@"WhiteTheme\Icons\";

    public Theme Initialize() 
    {
        return new Theme(
            "Светлая тема", // Название
            new Color32(248, 248, 255, 0xFF), // Цвет задника
            new Color32(255, 255, 255, 0xFF), // Цвет дока
            new Color32(0, 0, 0, 0xFF), // Цвет главных текстов (заголовки)
            new Color32(64, 64, 64, 0xFF), // Цвет позаголовков
            new Color32(128, 128, 128, 0xFF), // Цвет на иконках
            Color.blue, // Цвет тайлов из входного теста
            Color.blue, // Цвет хайлайта иконки
            new Color32(0, 0, 128, 0x0F), // Цвет базы блока главного меню
            new List<string>
            {
                DockIconsPath + "home.png",
                DockIconsPath + "book-bookmark.png",
                DockIconsPath + "user.png",
                DockIconsPath + "settings.png"
            }
            );
    }
}
