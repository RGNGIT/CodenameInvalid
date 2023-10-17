using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main_BlockBase : MonoBehaviour
{
    public TMP_Text Header;
    public GameObject Base;

    private void Awake()
    {
        GetComponent<RawImage>().color = Runtime.currentTheme.Main_BlockBase;
        Header.color = Runtime.currentTheme.TextColor_1;
    }
}
