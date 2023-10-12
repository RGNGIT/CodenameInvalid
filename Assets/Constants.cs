using System.Collections.Generic;

public static class Constants
{
    public static Dictionary<string, EApiResponseStatus> stringResponseStatus = new () 
    {
        { "OK", EApiResponseStatus.OK },
        { "ERROR", EApiResponseStatus.Error }
    };

    public enum EApiResponseStatus 
    {
        OK = 1,
        Error = 2
    }
}

