﻿// Makes the VisualComponent follow the mouse until the user clicks somewhere.
// Useful for placing newly created objects by the user.

using UnityEngine;
using WispExtensions;

public class WispHoverMode : MonoBehaviour
{
    RectTransform rt;
    RectTransform parent_rt;
    Vector3 initialMousePosition;
    Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();

        if (transform.parent != null)
        {
            parent_rt = transform.parent.GetComponent<RectTransform>();
        }

        if (rt == null || parent_rt == null)
        {
            Destroy(this); // Can't hover without RectTransforms
        }

        initialMousePosition = Input.mousePosition;
        initialPosition = parent_rt.GetMousePositionInMe();
        rt.anchoredPosition = initialPosition;
    }

    // Update is called once per frame
    void Update()
    {
        rt.localPosition = (Input.mousePosition - initialMousePosition) + initialPosition;

        if (Input.GetMouseButton(0))
        {
            Destroy(this); // Stop hovering when right mouse click
        }
    }
}