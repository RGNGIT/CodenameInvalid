using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

[ExecuteInEditMode]
public class WispPanel : WispVisualComponent
{
    private Image image;

    public Image Base
    {
        get
        {
            return GetComponent<Image>();
        }
    }
    
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

        image = GetComponent<Image>();

        isInitialized = true;

        return true;
    }

    public override void ApplyStyle()
    {
        if (style == null)
            return;
        
        base.ApplyStyle();

        image.ApplyStyle(style, Opacity, subStyleRule);
    }

    public override void ApplyStyleInEditor()
    {
        ApplyStyle();
    }

    /// <summary>
    /// ...
    /// </summary>
    public static WispPanel Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.Panel, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.Panel);
        }

        return go.GetComponent<WispPanel>();
    }

    void OnValidate()
    {
        if (CheckIgnoreWysiwigOnValidate())
            return;

        ApplyStyle();
    }
}