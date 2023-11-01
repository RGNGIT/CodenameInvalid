using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispFloatingPanelDemo : MonoBehaviour
{
    private WispTitleBar bar;
    
    void Start()
    {
        bar = transform.Find("WispTitleBar").GetComponent<WispTitleBar>();
        
        // Assigning a method to call when the close button is pressed
        bar.ExitButton.AddOnClickAction(OnCloseButtonClick);
    }

    // Method to call when the Close button is pressed
    private void OnCloseButtonClick()
    {
        if (bar.Parent.Opacity > 0.5f)
        {
            bar.Parent.Opacity = 0.5f;
        }
        else
        {
            Destroy(bar.Target.gameObject);
        }
    }
}