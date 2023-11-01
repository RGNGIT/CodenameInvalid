using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispProgressBar : WispVisualComponent
{
    [Header("Progress Bar")]
    [SerializeField] private float initialValue = 100f;
    [SerializeField] private WispFillDirection fillDirection = WispFillDirection.RightToLeft;
    [SerializeField] private bool animatedFill = true;
    [ConditionalHideBoolAttribute("animatedFill", true, true)] [SerializeField] private float fillSpeed = 50; // Percent per second.
    [SerializeField] private float margin = 4f;
    [SerializeField] private bool showTextValue = true;
    [ConditionalHideBoolAttribute("showTextValue", true, true)] [SerializeField] private int decimals = 2;
    [ConditionalHideBoolAttribute("showTextValue", true, true)] [SerializeField] private bool centerTextInProgress = true;
    
    private Image bar = null;
    private float currentValue = 100f; // 0 to 100.
    private float visualValue = 0f; // Used for animation.
    private Coroutine sync = null;
    private WispTextMeshPro text = null;

    public float FillSpeed { get => fillSpeed; set => fillSpeed = value; }
    public float InitialValue { get => initialValue; set => initialValue = value; }
    public bool ShowTextValue { get => showTextValue; set { showTextValue = value; text.gameObject.SetActive(value); } }

    void Awake()
    {
        Initialize();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ApplyStyle();
        SetValue(initialValue);
    }

    /// <summary>
    /// Initialize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
	{
		if (isInitialized)
			return true;
		
		base.Initialize();

        // ---------------------------------------------------------------------

        bar = transform.Find("Padding").Find("Bar").GetComponent<Image>();

        text = bar.transform.Find("Text").GetComponent<WispTextMeshPro>();
        text.Initialize();
        text.SetParent(this, true);

        if (!centerTextInProgress)
        {
            text.transform.SetParent(transform);
            text.GetComponent<RectTransform>().AnchorStyleExpanded(true);
        }

        if (!showTextValue)
            text.gameObject.SetActive(false);

        margin = 0; // TODO : Maybe remove the margin/padding system later ?
        bar.GetComponent<RectTransform>().AnchorStyleExpanded(margin);

		// ---------------------------------------------------------------------

		isInitialized = true;

        return true;
	}

    public override void ApplyStyle()
    {
        if (CheckIgnoreApplyStyle(false))
            return;
        
        base.ApplyStyle();

        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
        bar.color = style.ProgressColor.ColorOpacity(Opacity);
        bar.type = style.GetSubStyle(subStyleRule).spriteDrawMode;
        bar.sprite = style.GetSubStyle(subStyleRule).graphics;

		// GetComponent<Image>().pixelsPerUnitMultiplier = 1f;
    }

    public override string GetValue()
    {
        return currentValue.ToString();
    }

    public override void SetValue(string ParamValue)
    {
        SetValue(ParamValue.ToFloat());
        UpdateBarVisualValue();
    }

    public void SetValue(float ParamValue)
    {
        currentValue = Mathf.Clamp(ParamValue, 0, 100);
        UpdateBarVisualValue();
    }

    private void UpdateBarVisualValue()
    {
        if (animatedFill)
        {
            if (sync != null)
            StopCoroutine(sync);

            if (gameObject.activeInHierarchy)
                sync = StartCoroutine(Sync());
        }
        else
        {
            SetBarVisualValue(currentValue);
        }
    }

    private IEnumerator Sync()
    {
        float step = 0;

        while (visualValue != currentValue)
        {
            step = fillSpeed * Time.deltaTime;

            if (visualValue < currentValue)
            {
                float result = visualValue + step;

                if (result > currentValue)
                    result = currentValue;

                SetBarVisualValue(result);
            }
            else
            {
                float result = visualValue - step;

                if (result < currentValue)
                    result = currentValue;

                SetBarVisualValue(result);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void SetBarVisualValue(float ParamValue)
    {
        ParamValue = Mathf.Clamp(ParamValue, 0, 100);
        visualValue = ParamValue;
        // bar.GetComponent<RectTransform>().AnchorStyleExpanded(margin);

        if (showTextValue)
        {
            text.SetValue(visualValue.ToString("n" + Mathf.Clamp(decimals,0,8).ToString()) + "%");
        }

        if (fillDirection == WispFillDirection.RightToLeft)
        {
            // float barWidth = rectTransform.rect.width - margin*2;
            // float step = barWidth/100;
            // bar.GetComponent<RectTransform>().SetLeft(barWidth - step*ParamValue + margin);
            bar.GetComponent<RectTransform>().AnchorToFillPercentageHorizontally(100-ParamValue, ParamValue);
        }
        else if (fillDirection == WispFillDirection.LeftToRight)
        {
            // float barWidth = rectTransform.rect.width - margin*2;
            // float step = barWidth/100;
            // bar.GetComponent<RectTransform>().SetRight(barWidth - step*ParamValue + margin);
            bar.GetComponent<RectTransform>().AnchorToFillPercentageHorizontally(0f, ParamValue);
        }
        else if (fillDirection == WispFillDirection.TopToBottom)
        {
            // float barHeight = rectTransform.rect.height - margin*2;
            // float step = barHeight/100;
            // bar.GetComponent<RectTransform>().SetBottom(barHeight - step*ParamValue + margin);
            bar.GetComponent<RectTransform>().AnchorToFillPercentageVertically(0f, ParamValue);
        }
        else if (fillDirection == WispFillDirection.BottomToTop)
        {
            // float barHeight = rectTransform.rect.height - margin*2;
            // float step = barHeight/100;
            // bar.GetComponent<RectTransform>().SetTop(barHeight - step*ParamValue + margin);
            bar.GetComponent<RectTransform>().AnchorToFillPercentageVertically(100-ParamValue, ParamValue);
        }
    }

    /// <summary>
    /// Create a WispProgressBar.
    /// </summary>
    public static WispProgressBar Create(Transform ParamTransform)
    {
        GameObject go;

        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.ProgressBar, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.ProgressBar);
        }

        return go.GetComponent<WispProgressBar>();
    }
}

public enum WispFillDirection { RightToLeft, LeftToRight, TopToBottom, BottomToTop }