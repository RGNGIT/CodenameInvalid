using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Common_BackgroundColor : MonoBehaviour
{
    private void Awake()
    {
        GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = Runtime.currentTheme.Common_BackgroundColor;
    }
}
