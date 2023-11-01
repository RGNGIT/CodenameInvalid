using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WispExtensions;

public class WispCheckBox : WispVisualComponent, ISelectHandler
{
    [Header("Text")]
    [SerializeField] private string defaultLabel;

    private Toggle toggleComponent;
    private Image checkBoxImage;
    private Image checkMark;
    private TextMeshProUGUI labelComponent;
    private UnityEvent onSelect = new UnityEvent();

    public UnityEvent OnSelect { get => onSelect; }

    public string Label
    {
        set
        {
            labelComponent.text = value;
        }
        get
        {
            return labelComponent.text;
        }
    }

    public string DefaultLabel
    {
        get
        {
            return defaultLabel;
        }
    }

    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        // ---------------------------------------------------------------------

        toggleComponent = GetComponent<Toggle>();
        checkBoxImage = transform.Find("Background").GetComponent<Image>();
        checkMark = checkBoxImage.transform.Find("Checkmark").GetComponent<Image>();
        labelComponent = transform.Find("Label").GetComponent<TextMeshProUGUI>();
        Label = defaultLabel;

        // ---------------------------------------------------------------------

        isInitialized = true;

        return true;
    }

    public override void SetValue(string ParamValue)
    {
        toggleComponent.isOn = ParamValue.ToBool();
    }

    public override string GetValue()
    {
        return toggleComponent.isOn.ToString();
    }

    public Toggle Base
    {
        get
        {
            return GetComponent<Toggle>();
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

    public override void ApplyStyle()
    {
        base.ApplyStyle();

        checkBoxImage.ApplyStyle(style, Opacity, subStyleRule);
        labelComponent.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
        checkMark.ApplyStyle(style, Opacity, WispSubStyleRule.Icon);
    }

    /// <summary>
    /// ...
    /// </summary>
    public static WispCheckBox Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.CheckBox, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.CheckBox);
        }

        return go.GetComponent<WispCheckBox>();
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        onSelect.Invoke();
    }

    /// <summary>
    /// What you see is what you get (For edit mode only) 
    /// </summary>
    public override void Wysiwyg()
    {
        if (!Application.isPlaying)
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = DefaultLabel;
    }
}