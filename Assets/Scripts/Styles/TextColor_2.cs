using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextColor_2 : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TMP_Text>().color = Runtime.currentTheme.TextColor_2;
    }
}
