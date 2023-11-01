using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispTooltip : WispVisualComponent
{
    private TextMeshProUGUI contentTextComponent;
    private TextMeshProUGUI titleTextComponent;

    const float widthMargin = 8f;
    const float heightMargin = 8f;
    const float minWidth = 192f;

    #region Prevent tooltip of tooltip
    public new void SetTooltipText(string ParamTtile, string ParamContent)
    {

    }

    internal new void ShowTooltip()
    {

    }

    internal new void HideTooltip()
    {

    }

    public new bool EnableTooltip
    {
        get { return false; }
        set { }
    }
    #endregion

    public string Content
    {
        get
        {
            return contentTextComponent.text;
        }

        set
        {
            contentTextComponent.text = value;
            UpdatePositions();
        }
    }

    public string Title
    {
        get
        {
            return titleTextComponent.text;
        }

        set
        {
            titleTextComponent.text = value;
            UpdatePositions();
        }
    }

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        ApplyStyle();
    }

    /// <summary>
    /// Initialize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        subStyleRule = WispSubStyleRule.Container;

        titleTextComponent = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        titleTextComponent.autoSizeTextContainer = true;

        contentTextComponent = transform.Find("Content").GetComponent<TextMeshProUGUI>();

        isInitialized = true;

        return true;
    }

    /// <summary>
    /// Create.
    /// </summary>
    public static WispTooltip Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.Tooltip, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.Tooltip);
        }

        return go.GetComponent<WispTooltip>();
    }

    public override void ApplyStyle()
    {
        base.ApplyStyle();

        titleTextComponent.ApplyStyle(style, Opacity, WispFontSize.Header, subStyleRule);
        contentTextComponent.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
    }

    public override void UpdatePositions()
    {
        // Pass 1 -------------------------------------------------------------------------------------------
        
        // Width
        float w = Mathf.Max(titleTextComponent.preferredWidth + (widthMargin*2), minWidth + (widthMargin*2));

        // Height
        float h = 0;

        if (contentTextComponent.text == "")
        {
            h = style.FontSizeHeader /*+ (heightMargin*2)*/;
        }
        else
        {
            h = titleTextComponent.preferredHeight + contentTextComponent.preferredHeight + (heightMargin*2);
        }

        MyRectTransform.sizeDelta = new Vector2(w, h + (heightMargin*2));
        titleTextComponent.rectTransform.sizeDelta = new Vector2(w, h);
        contentTextComponent.rectTransform.sizeDelta = new Vector2(w - (widthMargin*2), h/* + (heightMargin*2)*/);

        // Pass 2 -------------------------------------------------------------------------------------------
        
        // Height
        if (contentTextComponent.text == "")
        {
            h = titleTextComponent.preferredHeight /*+ (heightMargin*2)*/;
        }
        else
        {
            h = titleTextComponent.preferredHeight + contentTextComponent.preferredHeight + (heightMargin*2);
        }

        MyRectTransform.sizeDelta = new Vector2(w, h + (heightMargin*2));
        titleTextComponent.rectTransform.sizeDelta = new Vector2(w, h);
        contentTextComponent.rectTransform.sizeDelta = new Vector2(w - (widthMargin*2), h/* + (heightMargin*2)*/);

        // Positions
        titleTextComponent.rectTransform.anchoredPosition = new Vector2(widthMargin, heightMargin*(-1));
        contentTextComponent.rectTransform.anchoredPosition = new Vector2(widthMargin, ((heightMargin*-2) - titleTextComponent.preferredHeight));
    }

    // This does not automaticly update width and height, you must call UpdatePositions() after calling this method.
    public void SetBothTitleAndContent(string ParamTitle, string ParamContent)
    {
        titleTextComponent.text = ParamTitle;
        contentTextComponent.text = ParamContent;
    }
}