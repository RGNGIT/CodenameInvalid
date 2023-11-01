using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using WispExtensions;
using TMPro;

public class WispCircularSlider : WispVisualComponent
{
    const float TAU = Mathf.PI*2;
    const float MIN = 2.355f;
    // const float MIN = 0.5652f;
    const float MAX = 4.71f;
    // const float MAX = 5.70852f;

    [Header("Slider settings")]
    [Range(0f, 1f)]
    public float fillAmount;

    public UnityEvent OnValueChanged { get {return onValueChanged;} }

    private Image shape;
    private Image fill;
    private RectTransform handle;
    private float handleY;
    private float lastMouseX;
    private float lastFillAmount = -1f;
    private TextMeshProUGUI percentage;
    private UnityEvent onValueChanged;

    void Awake()
    {
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

        shape = GetComponent<Image>();
        fill = transform.Find("Fill").GetComponent<Image>();

        handle = transform.Find("Handle").GetComponent<RectTransform>();
        handleY = handle.anchoredPosition.y;

        percentage = transform.Find("Percentage").GetComponent<TextMeshProUGUI>();

        onValueChanged = new UnityEvent();

		// ---------------------------------------------------------------------

		isInitialized = true;

        return true;
	}
    
    public override void UpdatePositions()
    {
        float floatRange = Mathf.Lerp(0f, MAX, fillAmount);
        float t = TAU-(floatRange+MIN);

        float x = Mathf.Cos(t) * handleY;
        float y = Mathf.Sin(t) * handleY;
        handle.anchoredPosition = new Vector2(x,y);

        // fill.fillAmount = Mathf.Lerp(0.125f, 0.875f, fillAmount);
        fill.fillAmount = Mathf.Lerp(0.09f, 0.909f, fillAmount);
        shape.fillAmount = 1 - fill.fillAmount;

        percentage.text = Mathf.FloorToInt(fillAmount*100).ToString() + "%";

        if (onValueChanged != null)
            onValueChanged.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
        if (lastFillAmount != fillAmount)
        {
            UpdatePositions();
            lastFillAmount = fillAmount;
        }
        #endif
    }

    public void OnBeginDrag()
    {
        lastMouseX = Input.mousePosition.x;
    }

    public void OnDrag()
    {
        float amount = Input.mousePosition.x - lastMouseX;
        fillAmount = Mathf.Clamp01(fillAmount + (amount/100));
        lastMouseX = Input.mousePosition.x;
        UpdatePositions();
    }

    /// <summary>
    /// Create a WispCircularSlider.
    /// </summary>
    public static WispCircularSlider Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.CircularSlider, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.CircularSlider);
        }

        return go.GetComponent<WispCircularSlider>();
    }

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet.
    /// </summary>
    public override void ApplyStyle ()
	{
        if (CheckIgnoreApplyStyle(false))
            return;
        
        base.ApplyStyle();

        shape.color = style.GetSubStyle(subStyleRule).activeColor.ColorOpacity(Opacity);
        fill.color = style.GetSubStyle(subStyleRule).selectedBackgroundColor.ColorOpacity(Opacity);
        percentage.color = style.GetSubStyle(subStyleRule).activeColor.ColorOpacity(Opacity);
        handle.GetComponent<Image>().color = style.GetSubStyle(subStyleRule).activeColor.ColorOpacity(Opacity);
    }

    public Image Base
    {
        get
        {
            return GetComponent<Image>();
        }
    }

    /// <summary>
    /// Change the fill percentage, 0 to 100.
    /// </summary>
    public override void SetValue(string ParamValue)
    {
        SetValue(ParamValue.ToFloat());
    }

    /// <summary>
    /// Change the fill percentage, 0 to 100.
    /// </summary>
    public void SetValue(float ParamValue)
    {
        fillAmount = Mathf.Clamp(ParamValue, 0f, 100f) / 100;
        UpdatePositions();
    }

    /// <summary>
    /// Get fill percentage value as string.
    /// </summary>
    public override string GetValue()
    {
        return (fillAmount*100).ToString();
    }

    /// <summary>
    /// Get fill percentage value (0 to 1).
    /// </summary>
    public float GetValue01()
    {
        return Mathf.Clamp01(fillAmount);
    }
}