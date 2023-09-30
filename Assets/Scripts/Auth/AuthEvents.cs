using TMPro;
using UnityEngine;

public class AuthEvents : MonoBehaviour
{
    public TMP_InputField login;
    public TMP_InputField password;

    public void Login() 
    {
        if(!string.IsNullOrEmpty(login.text) && !string.IsNullOrEmpty(password.text)) 
        {
            
        }
    }
}
