using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RegisterEvents : MonoBehaviour
{
    public TMP_InputField Surname;
    public TMP_InputField Name;
    public TMP_InputField Patronymic;
    public TMP_InputField Email;
    public TMP_InputField Password;
    public TMP_Text Gender;
    public TMP_InputField Age;

    public async void RegisterAction() 
    {
        API api = new();
        await api.UserServiceInstanse().Register(Name.text, Surname.text, Patronymic.text, Email.text, Password.text, Gender.text, int.Parse(Age.text));
    }
}
