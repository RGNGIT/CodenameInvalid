#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterEvents : MonoBehaviour
{
    public TMP_InputField Surname;
    public TMP_InputField Name;
    public TMP_InputField Patronymic;
    public TMP_InputField Email;
    public TMP_InputField Password;
    public TMP_Text Gender;
    public TMP_InputField Age;
    public TMP_Text info;

    public void Awake()
    {
        info.text = string.Empty;
    }

    public async void RegisterAction() 
    {
        API api = new();
        ApiResponse<User?>? response = await api
        .UserServiceInstanse()
        .Register(Name.text, Surname.text, Patronymic.text, Email.text, Password.text, Gender.text, int.Parse(Age.text));

        if (response != null) 
        {
            if (response.Data != null && Constants.stringResponseStatus[response.Status] == (int)Constants.EApiResponseStatus.OK)
            {
                Debug.Log($"Register successful (Token: \"{response.Data.Token}\")");
                SceneManager.LoadScene(2);
            }
            else if (Constants.stringResponseStatus[response.Status] == (int)Constants.EApiResponseStatus.Error)
            {
                Debug.Log($"Register failed");
                info.text = "Ошибка регистрации";
            }
        }
        else 
        {
            info.text = "Ошибка сервера. Response is NULL";
        }
    }
}
