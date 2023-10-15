using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Common_DockColor : MonoBehaviour
{
    private void Awake()
    {
        this.GetComponent<RawImage>().color = Runtime.currentTheme.Common_DockColor;
    }
}
