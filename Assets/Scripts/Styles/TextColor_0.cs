using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextColor_0 : MonoBehaviour
{
    private void Awake()
    {
        TMP_Text currentText = GetComponent<TMP_Text>();

        currentText.color = Runtime.currentTheme.TextColor_0;
    }
}
