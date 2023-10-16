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

    private void Awake()
    {
        isMainFound = GameObject.Find("IsMain") != null;
        isSettingsFound = GameObject.Find("IsSettings") != null;
    }

    private void Update()
    {
        if (isMainFound)
        {
            Texts[0].color = Runtime.currentTheme.Icon_Highlight;
            Icons[0].color = Runtime.currentTheme.Icon_Highlight;
            Header.text = "Ãëàâíàÿ";
        }

        if (isSettingsFound)
        {
            Texts[1].color = Runtime.currentTheme.Icon_Highlight;
            Icons[1].color = Runtime.currentTheme.Icon_Highlight;
            Header.text = "Íàñòðîéêè";
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