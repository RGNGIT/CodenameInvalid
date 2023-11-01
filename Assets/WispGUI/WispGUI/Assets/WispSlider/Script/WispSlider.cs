using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispSlider : WispVisualComponent
{
    private Image background;
    private Image fillArea;
    private Image handle;
    
    public Slider Base
    {
        get
        {
            return GetComponent<Slider>();
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
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
	{
		if (isInitialized)
			return true;
		
		base.Initialize();

		// ---------------------------------------------------------------------

        background = transform.Find("Background").GetComponent<Image>();
        fillArea = transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        handle = transform.Find("Handle Slide Area").Find("Handle").GetComponent<Image>();

		// ---------------------------------------------------------------------

		isInitialized = true;

        return true;

	}

    /// <summary>
    /// Create a WispSlider.
    /// </summary>
    public static WispSlider Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.Slider, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.Slider);
        }

        return go.GetComponent<WispSlider>();
    }

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet.
    /// </summary>
    public override void ApplyStyle ()
	{
        if (CheckIgnoreApplyStyle(false))
            return;
        
        base.ApplyStyle();

        background.ApplyStyle_Inactive(style, Opacity, subStyleRule);
        fillArea.ApplyStyle(style, Opacity, subStyleRule);
        handle.ApplyStyle(style, Opacity, subStyleRule);
    }

    /// <summary>
    /// Change the value of the slider, Clamped between Min and Max.
    /// </summary>
    public override void SetValue(string ParamValue)
    {
        SetValue(ParamValue.ToFloat());
    }

    /// <summary>
    /// Change the value of the slider, Clamped between Min and Max.
    /// </summary>
    public void SetValue(float ParamValue)
    {
        Base.value = ParamValue;
    }

    /// <summary>
    /// Change the fill percentage, 0 to 100.
    /// </summary>
    public void SetPercentageValue(float ParamValue)
    {
        float t = Mathf.Clamp(ParamValue, 0f, 100f) / 100;
        Base.value = Mathf.Lerp(Base.minValue, Base.maxValue, t);
    }

    /// <summary>
    /// Get slider value as string, Clamped between Min and Max.
    /// </summary>
    public override string GetValue()
    {
        return (Base.value).ToString();
    }

    /// <summary>
    /// Get fill percentage value (0 to 1).
    /// </summary>
    public float GetValue01()
    {
        return Mathf.InverseLerp(Base.minValue, Base.maxValue, Base.value);
    }
}