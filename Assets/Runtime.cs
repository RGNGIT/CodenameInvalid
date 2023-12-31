﻿#nullable enable

public static class Runtime
{
    private static bool settingViaMethod = false;

    public static User? currentUser {
        get 
        {
            return currentUser;
        }
        set
        {
            if(settingViaMethod)
                currentUser = value;
        }
    }

    public static Theme currentTheme = Constants.Themes[Constants.ETheme.WhiteTheme];

    public static void SetCurrentUser(User user) 
    {
        settingViaMethod = true;
        currentUser = user;
        settingViaMethod = false;
    }
}

