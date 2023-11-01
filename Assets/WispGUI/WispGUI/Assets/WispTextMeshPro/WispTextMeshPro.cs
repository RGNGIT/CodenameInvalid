using System;
using TMPro;
using UnityEngine;
using WispExtensions;

[ExecuteInEditMode]
public class WispTextMeshPro : WispVisualComponent
{

    [Header("Text")]
    [InspectorButton("Adjust size to text", "AdjustSizeToText", isActiveInEditor = true)]
    [SerializeField] private bool overrideFontSize = false;
    
    [ConditionalHideBool("overrideFontSize", true, true)]
    [SerializeField] private float fontSize = 14f;
    
    private TextMeshProUGUI textMesh;

    public TextMeshProUGUI Base
    {
        get
        {
            return textMesh;
        }
    }
    
    void Awake()
    {
        Initialize();
    }

    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        // ---------------------------------------------------------------------

        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.fontSize = fontSize;

        // ---------------------------------------------------------------------

        isInitialized = true;

        return true;
    }
    
    void Start()
    {
        ApplyStyle();
    }

    public override void ApplyStyle()
    {
        if (style == null)
            return;
        
        base.ApplyStyle();

        textMesh.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);

        if (overrideFontSize)
            textMesh.fontSize = fontSize;
    }

    public override string GetValue()
    {
        return textMesh.text;
    }

    public override void SetValue(string ParamValue)
    {
        textMesh.text = ParamValue;
    }

    /// <summary>
    /// Create a WispTextMeshPro.
    /// </summary>
    public static WispTextMeshPro Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.TextMeshPro, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.TextMeshPro);
        }

        return go.GetComponent<WispTextMeshPro>();
    }

    /// <summary>
    /// What you see is what you get (For edit mode only) 
    /// </summary>
    public override void Wysiwyg()
    {
        if (!Application.isEditor)
            return;

        if (!isInitialized)
        {
            LogWarning("Component not initialized.");
            return;
        }

        ApplyStyle();
    }

    void OnValidate()
    {
        if (CheckIgnoreWysiwigOnValidate())
            return;
        
        RegisterWysiwygRequest();
    }

    void AdjustSizeToText()
    {
        TextMeshProUGUI comp = GetComponent<TextMeshProUGUI>();
        
        // Width
        float w = comp.preferredWidth;

        // Height
        float h = comp.preferredHeight;

        GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
    }

    [Obsolete("This method is obsolet due to being in an experimental phase.")]
    public void MakeResponsive()
    {
        AdjustSizeToText();
        textMesh.enableWordWrapping = false;
        textMesh.overflowMode = TextOverflowModes.Ellipsis;
        MyRectTransform.AnchorToFillCurrentPercentage();
    }
}