using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispLoadingPanel : WispVisualComponent
{
    [Header("Loading Panel")]
    [SerializeField] private bool forcePanelOnTop = true;
    
    private Image backgroundImage;
    private Image animatedImage;

    public bool ForcePanelOnTop { get => forcePanelOnTop; set => forcePanelOnTop = value; }

    void Awake()
    {
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        ApplyStyle();
    }

    void Update()
    {
        if (forcePanelOnTop)
            transform.SetAsLastSibling();
    }

    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        // ---------------------------------------------------------------------

        backgroundImage = transform.Find("Graphics").Find("Background").GetComponent<Image>();
        animatedImage = transform.Find("Graphics").Find("Animation").GetComponent<Image>();

        // ---------------------------------------------------------------------
        
        GameObject backgroundObstructor = Instantiate(WispPrefabLibrary.Default.BackgroundObstructor, transform);
        backgroundObstructor.GetComponent<RectTransform>().AnchorStyleExpanded(true);
        backgroundObstructor.transform.SetAsFirstSibling();

        // ---------------------------------------------------------------------

        isInitialized = true;

        return true;
    }

    public override void ApplyStyle()
    {
        base.ApplyStyle();

        backgroundImage.ApplyStyle(style, Opacity, subStyleRule);
        animatedImage.ApplyStyle(style, Opacity, WispSubStyleRule.Icon);
    }

    public void SetDimensions(Vector2 ParamDimensions)
    {
        backgroundImage.rectTransform.sizeDelta = ParamDimensions;
        animatedImage.rectTransform.sizeDelta = ParamDimensions;
    }

    /// <summary>
    /// ...
    /// </summary>
    public static WispLoadingPanel Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.LoadingPanel, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.LoadingPanel);
        }

        return go.GetComponent<WispLoadingPanel>();
    }

    // Prevent loading panel of loading panel.
    public override void SetBusyMode(bool ParamState)
    {
        // Do nothing.
    }
}