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

    bool isMainFound;
    bool isSettingsFound;
    bool isWorkbookFound;
    bool isPersonalFound;

    private void Awake()
    {
        isMainFound = GameObject.Find("IsMain") != null;
        isSettingsFound = GameObject.Find("IsSettings") != null;
        isWorkbookFound = GameObject.Find("IsWorkbook") != null;
        isPersonalFound = GameObject.Find("IsPersonal") != null;
        ApplyDockIcons();
    }

    void ApplyDockIcons()
    {
        for(int i = 0; i < Icons.Length; i++) 
        {
            Icons[i].texture = Runtime.currentTheme.DockIcons[i];
        }
    }

    private void Update()
    {
        if (isMainFound)
        {
            Texts[0].color = Runtime.currentTheme.Icon_Highlight;
            // Icons[0].color = Runtime.currentTheme.Icon_Highlight;
            Header.text = "Главная";
        }

        if (isWorkbookFound)
        {
            Texts[1].color = Runtime.currentTheme.Icon_Highlight;
            // Icons[1].color = Runtime.currentTheme.Icon_Highlight;
            Header.text = "Учебник";
        }

        if (isPersonalFound)
        {
            Texts[2].color = Runtime.currentTheme.Icon_Highlight;
            // Icons[2].color = Runtime.currentTheme.Icon_Highlight;
            Header.text = "Профиль";
        }

        if (isSettingsFound)
        {
            Texts[3].color = Runtime.currentTheme.Icon_Highlight;
            // Icons[3].color = Runtime.currentTheme.Icon_Highlight;
            Header.text = "Настройки";
        }
    }

    public void ToMainPage() 
    {
        SceneManager.LoadScene((int)Constants.EScene.Main);
    }

    public void ToWorkbookPage()
    {
        SceneManager.LoadScene((int)Constants.EScene.Workbook);
    }

    public void ToPersonalPage()
    {
        SceneManager.LoadScene((int)Constants.EScene.Personal);
    }

    public void ToSettingsPage() 
    {
        SceneManager.LoadScene((int)Constants.EScene.Settings);
    }
}
