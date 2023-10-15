#nullable enable
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthEvents : MonoBehaviour
{
    public TMP_InputField login;
    public TMP_InputField password;
    public TMP_Text info;

    public void Awake()
    {
        info.text = string.Empty;
    }

    public void ToRegisterPage() 
    {
        SceneManager.LoadScene((int)Constants.EScene.Register);
    }

    public async void Login() 
    {
        if(!string.IsNullOrEmpty(login.text) && !string.IsNullOrEmpty(password.text)) 
        {
            API api = new();
            ApiResponse<User?>? response = await api
            .UserServiceInstanse()
            .Login(login.text, password.text);

            if (response != null)
            {
                if (response.Data != null && Constants.stringResponseStatus[response.Status] == Constants.EApiResponseStatus.OK)
                {
                    Debug.Log($"Login successful (Token: \"{response.Data.Token}\")");
                    Runtime.SetCurrentUser(response.Data);
                    SceneManager.LoadScene((int)Constants.EScene.Main);
                }
                else if (Constants.stringResponseStatus[response.Status] == Constants.EApiResponseStatus.Error)
                {
                    Debug.Log($"Login failed");
                    info.text = "Ошибка логина";
                }
            }
            else
            {
                info.text = "Ошибка сервера. Response is NULL";
            }
        }
        else
        {
            info.text = "Не введен логин/пароль";
        }
    }
}
