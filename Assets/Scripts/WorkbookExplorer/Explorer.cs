using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explorer : MonoBehaviour
{
    private void Update()
    {
        SidePanelMovement();
    }

    void SidePanelMovement()
    {
        sidePanel.transform.position = Vector2.MoveTowards(sidePanel.transform.position, sidePanelStates[Convert.ToInt32(sidePanelOpened)].transform.position, 0.1f);
    }

    public GameObject sidePanel;
    public GameObject[] sidePanelStates;

    [HideInInspector]
    public bool sidePanelOpened = false;
}
