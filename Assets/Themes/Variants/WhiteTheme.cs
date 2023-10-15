using UnityEngine;

public class WhiteTheme : IThemeInitializer
{
    public Theme Initialize() 
    {
        return new Theme(
            "Светлая тема", // Название
            new Color(248f, 248f, 255f), // Цвет задника
            new Color(200f, 200f, 200f), // Цвет дока
            new Color(0, 0, 0), // Цвет главных текстов (заголовки)
            Color.blue // Цвет тайлов из входного теста
            );
    }
}
