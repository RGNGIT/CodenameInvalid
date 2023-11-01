using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WispSelectAndFocusDemo : MonoBehaviour
{
    public WispButton textButton;
    public WispButton textButtonIcon;
    public WispImage image;
    
    // Start is called before the first frame update
    void Start()
    {
        textButton.AddOnClickAction(ToggleTextButtonSelectionMode);
        textButtonIcon.AddOnClickAction(ToggleTextButtonIconSelectionMode);
        image.GetComponent<WispMouseClickHandler>().OnClick.AddListener(ToggleImageSelectionMode);
    }

    private void ToggleTextButtonSelectionMode()
    {
        if (textButton.IsSelected)
        {
            textButton.Unselect();
        }
        else
        {
            textButton.Select();
        }
    }

    private void ToggleTextButtonIconSelectionMode()
    {
        if (textButtonIcon.IsSelected)
        {
            textButtonIcon.Unselect();
        }
        else
        {
            textButtonIcon.Select();
        }
    }

    private void ToggleImageSelectionMode()
    {
        if (image.IsSelected)
        {
            image.Unselect();
        }
        else
        {
            image.Select();
        }
    }
}