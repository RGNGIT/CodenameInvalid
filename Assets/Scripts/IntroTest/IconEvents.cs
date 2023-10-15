using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconEvents : MonoBehaviour
{
    private int Id;
    private IntroTestEvents testEvents;

    private void Awake()
    {
        ApplyStyles();
        Id = int.Parse(this.name.Replace("icon", string.Empty));
        testEvents = GameObject.Find("Canvas").GetComponent<IntroTestEvents>();
    }

    void ApplyStyles() 
    {
        this.GetComponent<Image>().color = Runtime.currentTheme.IntroTest_TileColor;
    }

    private void OnMouseDown()
    {
        Debug.Log(Id);
    }
}
