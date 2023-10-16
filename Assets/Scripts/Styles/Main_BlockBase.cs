using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main_BlockBase : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<RawImage>().color = Runtime.currentTheme.Main_BlockBase;
    }
}
