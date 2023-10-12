using System.Collections.Generic;

public static class Constants
{
    public static Dictionary<string, int> stringResponseStatus = new () 
    {
        { "OK", 1 },
        { "ERROR", 2 }
    };

    public enum EApiResponseStatus 
    {
        OK = 1,
        Error = 2
    }
}

