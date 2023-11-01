using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WispExtensions;

public class WispLineDemo : MonoBehaviour
{
    [SerializeField] private RectTransform a;
    [SerializeField] private RectTransform b;
    [SerializeField] private RectTransform canvas;

    private WispLineRenderer line;
    
    void Start()
    {
        line = GetComponent<WispLineRenderer>();

        if (line == null)
            line = gameObject.AddComponent<WispLineRenderer>();

        // Always set the width of the line, default is 0.
        line.Width = 1f;

        // Draw line from object a to object b.
        line.SetStartAndEndPoint(a,b);
    }

    void Update()
    {
        b.anchoredPosition = canvas.GetMousePositionInMe();

        // Update line when objects move.
        line.SetStartAndEndPoint(a,b);
    }
}