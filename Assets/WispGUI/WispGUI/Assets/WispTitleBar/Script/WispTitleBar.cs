using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WispExtensions;

public class WispTitleBar : WispVisualComponent
{
    [Header("Title Bar")]
    [SerializeField] private RectTransform target = null;
    [SerializeField] private bool matchTargetWidth = true;
    [SerializeField] private string defaultTitle = "Insert Window Title";
    [SerializeField] private TextAlignment titleAlignment = TextAlignment.Left;
    [SerializeField] private bool bringTargetToFrontOnDrag = true;
    
    [Header("Close Button")]
    [SerializeField] private bool enableCloseButton = false;
    [ConditionalHideBoolAttribute("enableCloseButton", true, true)] [SerializeField] private bool destroyOnClose = false;

    [Header("Icon")]
    [SerializeField] private bool enableIcon = false;
    
    [ConditionalHideBoolAttribute("enableIcon", true, true)] [SerializeField] private Sprite defaultIcon;
    [ConditionalHideBoolAttribute("enableIcon", true, true)] [SerializeField] private WispSubStyleRule iconSubStyle = WispSubStyleRule.Icon;
    [ConditionalHideBoolAttribute("enableIcon", true, true)] [SerializeField] [Range(0f, 1f)] private float iconSizeRatio = 0.75f;

    [Header("Grid Options")]
    [SerializeField] private bool SnapToGrid = false;
    [ConditionalHideBoolAttribute("SnapToGrid", true, true)] [SerializeField] private int gridX = 32;
    [ConditionalHideBoolAttribute("SnapToGrid", true, true)] [SerializeField] private int gridY = 32;

    private WispTextMeshPro title;
    private WispImage icon;
    private WispButton exitButton;
 
    private Vector3 startMousePos;
    private Vector3 preDragPos;

    public WispButton ExitButton { get => exitButton; }

    public Image Base
    {
        get
        {
            return GetComponent<Image>();
        }
    }

    public RectTransform Target { get => target; set => target = value; }

    // Awake
    void Awake()
    {
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        ApplyStyle();
    }

    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        // -------------------------------------------------------

        title = GetComponentInChildren<WispTextMeshPro>();
        title.Initialize();
        title.SetValue(defaultTitle);

        icon = transform.Find("Icon").GetComponent<WispImage>();
        icon.Initialize();
        exitButton = transform.Find("BtnExit").GetComponent<WispButton>();

        // -------------------------------------------------------

        if (enableCloseButton && destroyOnClose)
            exitButton.AddOnClickAction(OnExitBtnClick);

        UpdatePositions();

        // -------------------------------------------------------

        isInitialized = true;

        return true;
    }

    public override void UpdatePositions()
    {
        if (target != null)
        {
            transform.SetParent(target);

            if (matchTargetWidth)
            {
                SetRight(0);
                SetLeft(0);
                Set_Y_Position(0);
            }
        }

        if (enableIcon)
        {
            icon.Width = MyRectTransform.rect.height * iconSizeRatio;
            icon.Height = MyRectTransform.rect.height * iconSizeRatio;
            icon.Set_X_Position((MyRectTransform.rect.height - (MyRectTransform.rect.height * iconSizeRatio)) / 2);

            if (defaultIcon != null)
            {
                icon.SetValue(defaultIcon);
            }
        }
        else
        {
            Destroy(icon.gameObject);
        }

        if (enableCloseButton)
        {
            // ...
        }
        else
        {
            Destroy(exitButton.gameObject);
        }


        if (titleAlignment == TextAlignment.Left)
        {
            title.Base.alignment = TextAlignmentOptions.Left;
            
            if (enableIcon)
                title.SetLeft(MyRectTransform.rect.height);
            else
                title.SetLeft(style.ButtonHorizontalSpacing);

            if (enableCloseButton)
                title.SetRight(MyRectTransform.rect.height);
            else
                title.SetRight(style.ButtonHorizontalSpacing);
        }
        else if (titleAlignment == TextAlignment.Center)
        {
            title.Base.alignment = TextAlignmentOptions.Center;
            title.SetLeft(0);
            title.SetRight(0);
        }
        else if (titleAlignment == TextAlignment.Right)
        {
            title.Base.alignment = TextAlignmentOptions.Right;
            title.SetLeft(0);

            if (enableCloseButton)
                title.SetRight(MyRectTransform.rect.height);
            else
                title.SetRight(style.ButtonHorizontalSpacing);
        }
    }

    public override void ApplyStyle()
    {
        if (style == null)
            return;
        
        base.ApplyStyle();

        if (icon)
            icon.SubStyleRule = iconSubStyle;
            
        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
    }

    /// <summary>
    /// ...
    /// </summary>
    public static WispTitleBar Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.TitleBar, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.TitleBar);
        }

        return go.GetComponent<WispTitleBar>();
    }

    public void OnBeginDrag()
    {
        startMousePos = Input.mousePosition;
        preDragPos = target.position;

        if (bringTargetToFrontOnDrag)
        {
            target.SetAsLastSibling();
        }
    }

    public void OnDrag()
    {
        Vector3 offset = Input.mousePosition - startMousePos;

        if (SnapToGrid)
            offset = offset.SnapToGrid(gridX, gridY, 1);

        target.position = preDragPos + offset;
    }

    public void OnEndDrag()
    {

    }

    private void OnExitBtnClick()
    {
        Destroy(target.gameObject);
    }
}