using System;
using System.Diagnostics;

public class User
{
    public class Data 
    {
        public int UserKey;
        public string Email;
        public string Verify;
    }

    public Data UserData;
    public string Token;
    public DateTime TTL;
}
