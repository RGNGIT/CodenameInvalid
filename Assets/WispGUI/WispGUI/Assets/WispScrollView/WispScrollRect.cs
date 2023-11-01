using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispScrollRect : ScrollRect
{
    private float hCarretPadding = 0;
    private float vCarretPadding = 0;
    
    private float horizontalScrollbarYOffset = 0;
    private float horizontalScrollbarXPaddings = 0;

    private float verticalScrollbarXOffset = 0;
    private float verticalScrollbarYPaddings = 0;

    public float HorizontalScrollbarYOffset { get => horizontalScrollbarYOffset; set => horizontalScrollbarYOffset = value; }
    public float HorizontalScrollbarXPaddings { get => horizontalScrollbarXPaddings; set => horizontalScrollbarXPaddings = value; }

    public float VerticalScrollbarXOffset { get => verticalScrollbarXOffset; set => verticalScrollbarXOffset = value; }
    public float VerticalScrollbarYPaddings { get => verticalScrollbarYPaddings; set => verticalScrollbarYPaddings = value; }

    public override void SetLayoutHorizontal()
    {
        base.SetLayoutHorizontal();
        // print("SR SetLayoutHorizontal");

        hCarretPadding = horizontalScrollbarXPaddings;
        vCarretPadding = verticalScrollbarYPaddings;

        if (horizontalScrollbar.gameObject.activeInHierarchy)
        {
            vCarretPadding = 20;
        }

        if (verticalScrollbar.gameObject.activeInHierarchy)
        {
            hCarretPadding = 20;
        }

        if (horizontalScrollbar)
        {
            horizontalScrollbar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, horizontalScrollbarYOffset);
            horizontalScrollbar.GetComponent<RectTransform>().SetRight(/*horizontalScrollbarXPaddings + */hCarretPadding);
            horizontalScrollbar.GetComponent<RectTransform>().SetLeft(horizontalScrollbarXPaddings);
        }

        if (verticalScrollbar)
        {
            verticalScrollbar.GetComponent<RectTransform>().anchoredPosition = new Vector2(verticalScrollbarXOffset, 0);
            verticalScrollbar.GetComponent<RectTransform>().SetTop(verticalScrollbarYPaddings);
            verticalScrollbar.GetComponent<RectTransform>().SetBottom(/*verticalScrollbarYPaddings + */vCarretPadding);
        }
    }

    public override void SetLayoutVertical()
    {
        base.SetLayoutVertical();
        // print("SR SetLayoutVertical");

        hCarretPadding = horizontalScrollbarXPaddings;
        vCarretPadding = verticalScrollbarYPaddings;

        if (horizontalScrollbar.gameObject.activeInHierarchy)
        {
            vCarretPadding = 20;
        }

        if (verticalScrollbar.gameObject.activeInHierarchy)
        {
            hCarretPadding = 20;
        }

        if (horizontalScrollbar)
        {
            horizontalScrollbar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, horizontalScrollbarYOffset);
            horizontalScrollbar.GetComponent<RectTransform>().SetRight(/*horizontalScrollbarXPaddings + */hCarretPadding);
            horizontalScrollbar.GetComponent<RectTransform>().SetLeft(horizontalScrollbarXPaddings);
        }

        if (verticalScrollbar)
        {
            verticalScrollbar.GetComponent<RectTransform>().anchoredPosition = new Vector2(verticalScrollbarXOffset, 0);
            verticalScrollbar.GetComponent<RectTransform>().SetTop(verticalScrollbarYPaddings);
            verticalScrollbar.GetComponent<RectTransform>().SetBottom(/*verticalScrollbarYPaddings + */vCarretPadding);
        }
    }
}