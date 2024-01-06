using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvents : MonoBehaviour
{
    public void OnOpenPanelClick() 
    {
        Explorer explorer = GetComponent<Explorer>();
        explorer.sidePanelOpened = !explorer.sidePanelOpened;
    }
}
