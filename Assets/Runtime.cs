#nullable enable

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

    public static void SetCurrentUser(User user) 
    {
        settingViaMethod = true;
        currentUser = user;
        settingViaMethod = false;
    }
}

