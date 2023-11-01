using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WispExtensions;

[ExecuteInEditMode]
public class WispButton : WispVisualComponent
{
    public enum WispButtonIconPlacement { Horizontal, Vertical, Full }

    [Header("Text")]
	[SerializeField] [WispEditorCustomControlName("DEFAULT_FOCUS")] private string defaultText;
    [SerializeField] private bool forceAutoSize = true;

    [Header("Icon")]
    [SerializeField] private bool enableIcon = false;
    
    [ConditionalHideBoolAttribute("enableIcon", true, true)] [SerializeField] private Sprite defaultIcon;
    [ConditionalHideBoolAttribute("enableIcon", true, true)] [SerializeField] private WispButtonIconPlacement iconPlacement;
    [ConditionalHideBoolAttribute("enableIcon", true, true)] [SerializeField] private WispSubStyleRule iconSubStyle = WispSubStyleRule.Icon;
    [ConditionalHideBoolAttribute("enableIcon", true, true)] [SerializeField] [Range(0f, 1f)] private float iconSizeRatio = 0.8f;

    private TMPro.TextMeshProUGUI textComponent;
    private WispImage icon;

    public TMPro.TextMeshProUGUI TextComponent 
    {
		get 
        {
			return transform.Find ("Text").GetComponent<TMPro.TextMeshProUGUI> ();
		}
	}
    
    public string DefaultText
    {
        get
        {
            return defaultText;
        }
    }

    public WispButtonIconPlacement IconPlacement
    {
        get
        {
            return iconPlacement;
        }

        set
        {
            iconPlacement = value;
            UpdatePositions();
        }
    }

    public Button Base
    {
        get
        {
            return GetComponent<Button>();
        }
    }

    public WispImage Icon { get => icon; }
    public bool ForceAutoSize { get => forceAutoSize; set { forceAutoSize = value; ApplyStyle(); } }
    public bool EnableIcon { get => enableIcon; set { enableIcon = value; UpdatePositions(); } }
    public float IconSizeRatio { get => iconSizeRatio; set { iconSizeRatio = value; UpdatePositions(); } }

    void Awake()
    {
        // isWysiwygReady = true;
        Initialize();
    }

    void Start()
    {
		ApplyStyle();
    }

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
	{
		if (isInitialized)
			return true;
		
		base.Initialize();

        // ---------------------------------------------------------------------

        textComponent = transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
        SetValue(defaultText);

        if (enableIcon)
        {
            UpdatePositions();
        }

		// ---------------------------------------------------------------------

		isInitialized = true;

        return true;

	}

    /// <summary>
    /// Change the text of the button.
    /// </summary>
    public override void SetValue(string ParamText)
    {
        textComponent.text = ParamText;
    }

    /// <summary>
    /// Get the text of the button.
    /// </summary>
    public override string GetValue()
    {
        return textComponent.text;
    }

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet.
    /// </summary>
    public override void ApplyStyle ()
	{
        if (CheckIgnoreApplyStyle(false))
            return;
        
        base.ApplyStyle();

        if (isSelected)
        {
            textComponent.ApplyStyle_Selected(style, Opacity, WispFontSize.Normal, subStyleRule);
            GetComponent<Image> ().ApplyStyle_Selected(style, Opacity, subStyleRule);
        }
        else
        {
            textComponent.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
            GetComponent<Image> ().ApplyStyle(style, Opacity, subStyleRule);
        }

        if (forceAutoSize)
            textComponent.enableAutoSizing = true;
        else
        {
            textComponent.enableAutoSizing = false;
            textComponent.fontSize = style.FontSizeNormal;
        }

        Base.colors = style.ButtonColorBlock;
    }

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet in Edit mode.
    /// </summary>
    public override void ApplyStyleInEditor()
    {
        GetComponentInChildren<TMPro.TextMeshProUGUI>().ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
    }

    /// <summary>
    /// What you see is what you get (For edit mode only) 
    /// </summary>
    public override void Wysiwyg()
    {
        if (!Application.isEditor)
            return;

        if (!Application.isPlaying)
        {
            SetValue(DefaultText);
        }

        UpdatePositions();
        ApplyStyle();
    }

    /// <summary>
    /// Define a fucntion to call when the button is pressed.
    /// </summary>
    public void AddOnClickAction(UnityEngine.Events.UnityAction ParamAction)
	{
		GetComponent<Button> ().onClick.AddListener(ParamAction);
	}

    /// <summary>
    /// Set Icon from a sprite.
    /// </summary>
    public void SetIcon(Sprite ParamSprite)
    {
        if (icon != null)
            icon.SetValue(ParamSprite);
    }

    /// <summary>
    /// Create a WispButton.
    /// </summary>
    public static WispButton Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.Button, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.Button);
        }

        return go.GetComponent<WispButton>();
    }

    /// <summary>
    /// Update text and icon placements.
    /// </summary>
    public override void UpdatePositions()
    {
        if (enableIcon)
        {
            if (icon == null)
            {
                Transform t = transform.Find("WispImageIcon");
                
                if (t == null)
                {
                    icon = WispImage.Create(transform);
                    icon.gameObject.name = "WispImageIcon";
                    icon.Initialize();
                    icon.SetParent(this, true);
                    icon.BorderRule = WispBorderRule.Never;
                    icon.transform.SetAsFirstSibling();
                    icon.Base.type = Image.Type.Simple;
                    icon.Base.preserveAspect = true;
                }
                else
                {
                    icon = t.GetComponent<WispImage>();
                    icon.Initialize();
                }
            }

            icon.SetValue(defaultIcon);
            icon.SubStyleRule = iconSubStyle;

            if (iconPlacement == WispButtonIconPlacement.Horizontal)
            {
                // float width_80p = rectTransform.sizeDelta.x * 0.8f;
                // float width_20p = rectTransform.sizeDelta.x * 0.2f;

                // float iconSize = Mathf.Min(width_20p, rectTransform.sizeDelta.y) * iconSizeRatio;

                icon.Initialize();
                // icon.AnchorTo("center-left");
                icon.PivotAround("center-center");
                // icon.MyRectTransform.sizeDelta = new Vector2(iconSize, iconSize); // !! Forces the icon to be a square !!
                // icon.MyRectTransform.anchoredPosition = new Vector2(width_20p/2, 0);
                float r = ((1-iconSizeRatio)/2f)*100f*0.2f;
                icon.MyRectTransform.AnchorToFillPercentage(r, 20-r*2, r, 100-r*2);

                // textComponent.rectTransform.AnchorTo("center-right");
                // textComponent.rectTransform.PivotAround("center-center");
                // textComponent.rectTransform.sizeDelta = new Vector2(width_80p, rectTransform.sizeDelta.y);
                // textComponent.rectTransform.anchoredPosition = new Vector2(-width_80p/2, 0);
                textComponent.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
                textComponent.GetComponent<RectTransform>().AnchorToFillPercentage(20, 80, 0, 100);
            }
            else if (iconPlacement == WispButtonIconPlacement.Vertical)
            {
                // float height_80p = rectTransform.sizeDelta.y * 0.8f;
                // float height_20p = rectTransform.sizeDelta.y * 0.2f;

                // float iconSize = Mathf.Min(rectTransform.sizeDelta.x, height_80p) * iconSizeRatio;

                icon.Initialize();
                // icon.AnchorTo("center-top");
                icon.PivotAround("center-center");
                // icon.MyRectTransform.sizeDelta = new Vector2(iconSize, iconSize); // !! Forces the icon to be a square !!
                // icon.MyRectTransform.anchoredPosition = new Vector2(0, -height_80p/2);
                float r = ((1-iconSizeRatio)/2f)*100f*0.8f;
                icon.MyRectTransform.AnchorToFillPercentage(r, 100-r*2, r, 80-r);

                // textComponent.rectTransform.AnchorTo("center-bottom");
                // textComponent.rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height_20p);
                // textComponent.rectTransform.anchoredPosition = new Vector2(0, height_20p/2);
                textComponent.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
                textComponent.GetComponent<RectTransform>().AnchorToFillPercentage(0,100,80,20);
            }
            else if (iconPlacement == WispButtonIconPlacement.Full)
            {
                icon.AnchorStyleExpanded(true);
                // TextComponent.rectTransform.sizeDelta = new Vector2(0, 0);
                TextComponent.rectTransform.AnchorStyleExpanded(true);

                float h_margin = (icon.MyRectTransform.rect.width * (1-iconSizeRatio)) / 2;
                float v_margin = (icon.MyRectTransform.rect.height * (1-iconSizeRatio)) / 2;

                icon.SetTop(h_margin);
                icon.SetBottom(h_margin);
                icon.SetRight(v_margin);
                icon.SetLeft(v_margin);
            }
        }
        else if (!enableIcon)
        {
            if (icon != null)
            {
                StrategicDestroy(icon.gameObject);
            }

            textComponent.rectTransform.AnchorStyleExpanded(true);
        }
    }

    // Note : User usually just need to press Return/Enter if the button has focus.
    public void AssignKeyBoardShortcut(KeyCode ParamKey, bool ParamRequireButtonFocus = false)
    {
        WispKeyBoardEventSystem.AddEventOnKey(Base.onClick.Invoke, this, true, ParamKey);
    }

    public void RenderOff()
    {
        GetComponent<Image>().enabled = false;
        GetComponent<Button>().enabled = false;

        if (icon != null)
            icon.GetComponent<Image>().enabled = false;
    }

    public void RenderOn()
    {
        GetComponent<Image>().enabled = true;
        GetComponent<Button>().enabled = true;

        if (icon != null)
            icon.GetComponent<Image>().enabled = true;
    }

    public void OnValidate()
	{
        if (CheckIgnoreWysiwigOnValidate())
            return;
        
        RegisterWysiwygRequest();
	}
}