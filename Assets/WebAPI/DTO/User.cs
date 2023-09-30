using System;
using System.Diagnostics;

public abstract class User
{
    private abstract class Data 
    {
        int UserKey;
        string Email;
        string Verify;
    }

    Data UserData;
    string Token;
    DateTime TTL;
}
