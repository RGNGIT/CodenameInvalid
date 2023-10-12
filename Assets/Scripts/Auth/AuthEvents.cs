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
                if (response.Data != null && Constants.stringResponseStatus[response.Status] == (int)Constants.EApiResponseStatus.OK)
                {
                    Debug.Log($"Login successful (Token: \"{response.Data.Token}\")");
                    SceneManager.LoadScene(2);
                }
                else if (Constants.stringResponseStatus[response.Status] == (int)Constants.EApiResponseStatus.Error)
                {
                    Debug.Log($"Login failed");
                    info.text = "������ ������";
                }
            }
            else
            {
                info.text = "������ �������. Response is NULL";
            }
        }
        else
        {
            info.text = "�� ������ �����/������";
        }
    }
}
