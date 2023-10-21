using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DarkTheme : IThemeInitializer
{
    private string DockIconsPath = Constants.ThemesFolder + $@"DarkTheme\Icons\";

    public Theme Initialize() 
    {
        return new Theme(
            "Темная тема", // Название
            new Color32(10, 10, 10, 0xFF), // Цвет задника
            new Color32(25, 25, 25, 0xFF), // Цвет дока
            new Color32(255, 255, 255, 0xFF), // Цвет главных текстов (заголовки)
            new Color32(255, 255, 255, 0xFF), // Цвет позаголовков
            new Color32(118, 120, 122, 0xFF), // Цвет на иконках
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
