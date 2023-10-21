using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static Dictionary<string, EApiResponseStatus> stringResponseStatus = new () 
    {
        { "OK", EApiResponseStatus.OK },
        { "ERROR", EApiResponseStatus.Error }
    };

    public static Dictionary<ETheme, Theme> Themes = new() 
    {
        { ETheme.WhiteTheme, new WhiteTheme().Initialize() }
    };

    public enum ETheme 
    {
        WhiteTheme = 0
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

