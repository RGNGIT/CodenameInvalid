using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DabudiTheme : IThemeInitializer
{
    public Theme Initialize() 
    {
        string DockIconsPath = Constants.ThemesFolder + $@"{GetType().Name}\Icons\";

        return new Theme(
            "Дабуди дабудай тема", // Название
            new Color32(236, 250, 253, 0xFF), // Цвет задника
            new Color32(168, 226, 246, 0xFF), // Цвет дока
            new Color32(4, 17, 23, 0xFF), // Цвет главных текстов (заголовки)
            new Color32(4, 17, 23, 0xFF), // Цвет позаголовков
            new Color32(13, 92, 122, 0xFF), // Цвет на иконках
            new Color32(13, 92, 122, 0xFF), // Цвет тайлов из входного теста
            new Color32(21, 148, 191, 0xFF), // Цвет хайлайта иконки
            new Color32(13, 92, 122, 0xFF), // Цвет базы блока главного меню
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
