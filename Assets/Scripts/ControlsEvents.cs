using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlsEvents : MonoBehaviour
{
    public TMP_Text[] Texts;
    public RawImage[] Icons;

    public TMP_Text Header;

    GameObject isMainFound;
    GameObject isSettingsFound;

    private void Awake()
    {
        isMainFound = GameObject.Find("IsMain");
        isSettingsFound = GameObject.Find("IsSettings");
    }

    private void Update()
    {
        if (isMainFound != null)
        {
            Texts[0].color = Runtime.currentTheme.Icon_Highlight;
            Icons[0].color = Runtime.currentTheme.Icon_Highlight;
            Header.text = "Главная";
        }

        if (isSettingsFound != null)
        {
            Texts[1].color = Runtime.currentTheme.Icon_Highlight;
            Icons[1].color = Runtime.currentTheme.Icon_Highlight;
            Header.text = "Настройки";
        }
    }

    public void ToMainPage() 
    {
        SceneManager.LoadScene((int)Constants.EScene.Main);
    }

    public void ToSettingsPage() 
    {
        SceneManager.LoadScene((int)Constants.EScene.Settings);
    }
}
