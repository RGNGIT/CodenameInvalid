using TMPro;
using UnityEngine;

public class AuthEvents : MonoBehaviour
{
    public TMP_InputField login;
    public TMP_InputField password;
    public TMP_Text info;

    public void Awake()
    {
        info.text = string.Empty;
    }

    public void Login() 
    {
        if(!string.IsNullOrEmpty(login.text) && !string.IsNullOrEmpty(password.text)) 
        {
            
        }
        else
        {
            info.text = "Не введен логин/пароль";
        }
    }
}
