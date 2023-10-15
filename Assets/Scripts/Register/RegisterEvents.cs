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

        bool checkInputs() 
        {
            return
                !string.IsNullOrEmpty(Name.text) &&
                !string.IsNullOrEmpty(Surname.text) &&
                !string.IsNullOrEmpty(Patronymic.text) &&
                !string.IsNullOrEmpty(Email.text) &&
                !string.IsNullOrEmpty(Password.text) &&
                !string.IsNullOrEmpty(Gender.text) &&
                !string.IsNullOrEmpty(Age.text);
        }

        if(checkInputs())
        {
            if (response != null)
            {
                if (response.Data != null && Constants.stringResponseStatus[response.Status] == Constants.EApiResponseStatus.OK)
                {
                    Debug.Log($"Register successful (Token: \"{response.Data.Token}\")");
                    Runtime.SetCurrentUser(response.Data);
                    SceneManager.LoadScene((int)Constants.EScene.IntroTest);
                }
                else if (Constants.stringResponseStatus[response.Status] == Constants.EApiResponseStatus.Error)
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
        else 
        {
            info.text = "Не заполнены одно или несколько полей";
        }
    }
}
