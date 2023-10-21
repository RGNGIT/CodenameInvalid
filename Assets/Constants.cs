using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Constants
{
    public static Dictionary<string, EApiResponseStatus> stringResponseStatus = new () 
    {
        { "OK", EApiResponseStatus.OK },
        { "ERROR", EApiResponseStatus.Error }
    };

    public static string ThemesFolder = $@"{Directory.GetCurrentDirectory()}\Assets\Themes\Variants\";

    public static Dictionary<ETheme, Theme> Themes = new() 
    {
        { ETheme.WhiteTheme, new WhiteTheme().Initialize() },
        { ETheme.DarkTheme, new DarkTheme().Initialize() }
    };

    public enum ETheme 
    {
        WhiteTheme = 0,
        DarkTheme = 1
    }

    public enum EApiResponseStatus 
    {
        OK = 1,
        Error = 2
    }

    public enum EAdditiveScene 
    {
        Controls = 3
    }

    public enum EScene 
    {
        Auth = 0,
        Register = 1,
        Main = 2,
        IntroTest = 4,
        Settings = 5,
        Workbook = 6,
        Personal = 7
    }
}

