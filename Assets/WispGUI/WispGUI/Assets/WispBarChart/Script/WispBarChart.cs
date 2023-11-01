using System.Collections.Generic;
using UnityEngine;
using System;
using WispExtensions;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class WispBarChart : WispScrollView
{
    const int MAX_GRID_PARTS = 256;
    const float RIGHT_PADDING = 16f;
    const float GRID_LINE_WIDTH = 3f;

    [Header("Bar Chart")]
    [SerializeField] private float initialSpacing = 32f;
    [SerializeField] private float barThickness = 16f;
    // [SerializeField] private float barLength = 128f;
    [SerializeField] [Range(0f, 1f)] private float labelToBarRatio = 0.25f;
    [SerializeField] private float barSpacing = 16f;
    
    [Header("Grid")]
    [SerializeField] private GameObject gridLinePrefab = null;

    private List<WispProgressBar> bars = new List<WispProgressBar>();
    private List<Image> verticalLines = new List<Image>();
    private Image topLine;
    private Image bottomLine;
    // private WispScrollView scrollView;
    
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

		isInitialized = true;

        return true;

	}

    /// <summary>
    /// Create a WispBarChart.
    /// </summary>
    public static new WispBarChart Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.BarChart, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.BarChart);
        }

        return go.GetComponent<WispBarChart>();
    }

    private void DrawLines(float ParamMinValue, float ParamMaxValue, uint ParamPartCount, float ParamLineLength, string ParamTopText)
    {
        foreach(Image img in verticalLines)
        {
            Destroy(img.gameObject);
        }

        verticalLines.Clear();

        // ----------------------------------------------

        // ParamPartCount = Math.Clamp(ParamPartCount, 1, MAX_GRID_PARTS); // Math.Clamp stopped working on Unity 2020.3

        if (ParamPartCount < 1)
            ParamPartCount = 1;

        if (ParamPartCount > MAX_GRID_PARTS)
            ParamPartCount = MAX_GRID_PARTS;

        // ----------------------------------------------

        float valuePerPart = (ParamMaxValue - ParamMinValue) / ParamPartCount;
        // float lengthOfPart = barLength / ParamPartCount;
        float lengthOfPart = (1-labelToBarRatio) / ParamPartCount;
        
        // ----------------------------------------------

        for (int i = 0; i <= ParamPartCount; i++)
        {
            RectTransform rt_local = Instantiate(gridLinePrefab).GetComponent<RectTransform>();
            rt_local.SetParent(contentRect);
            rt_local.AnchorTo("center-top");
            rt_local.PivotAround("center-top");
            rt_local.AnchorAtPercentageHorizontally(labelToBarRatio + i*lengthOfPart);
            rt_local.anchoredPosition = new Vector2(0, -initialSpacing);
            rt_local.sizeDelta = new Vector2(GRID_LINE_WIDTH, ParamLineLength);
            rt_local.SetAsFirstSibling();

            Image img = rt_local.GetComponent<Image>();
            img.color = new Color(1,1,1,0.1f);
            verticalLines.Add(img);

            WispTextMeshPro text = WispTextMeshPro.Create(rt_local);
            text.SetParent(this, true);
            text.PivotAround("right-bottom");
            text.AnchorTo("center-top");
            text.Base.alignment = TextAlignmentOptions.Right;
            text.Height = 12f;
            text.MyRectTransform.anchoredPosition = new Vector2(0, 4f);
            float v = ParamMinValue + (i * valuePerPart);
            text.SetValue(v.ToString("n0"));
            // text.MakeResponsive();
        }

        // ----------------------------------------------

        // Top line
        if (topLine != null)
            Destroy(topLine.gameObject);

        RectTransform rt = Instantiate(gridLinePrefab).GetComponent<RectTransform>();
        rt.SetParent(contentRect);
        rt.AnchorTo("center-top");
        rt.PivotAround("left-center");
        rt.sizeDelta = new Vector2(0f, GRID_LINE_WIDTH);
        rt.AnchorToFillPercentageHorizontally(labelToBarRatio*100, (1-labelToBarRatio)*100);
        rt.anchoredPosition = new Vector2(0, -initialSpacing);

        topLine = rt.GetComponent<Image>();
        topLine.color = new Color(1,1,1,0.1f);

        WispTextMeshPro topText = WispTextMeshPro.Create(rt);
        topText.SetParent(this, true);
        topText.AnchorTo("center-top");
        topText.PivotAround("center-bottom");
        topText.Base.alignment = TextAlignmentOptions.Center;
        topText.Base.overflowMode = TextOverflowModes.Ellipsis;
        topText.MyRectTransform.anchoredPosition = new Vector2(0, 20f);
        topText.Height = 16f;
        topText.SetValue(ParamTopText);
        topText.MyRectTransform.AnchorToStretchHorizontally();
        topText.MyRectTransform.SetRight(0f);
        topText.MyRectTransform.SetLeft(0f);
        // topText.MakeResponsive();

        // Bottom line
        if (bottomLine != null)
            Destroy(bottomLine.gameObject);

        rt = Instantiate(gridLinePrefab).GetComponent<RectTransform>();
        rt.SetParent(contentRect);
        rt.AnchorTo("center-top");
        rt.PivotAround("left-center");
        rt.sizeDelta = new Vector2(0f, GRID_LINE_WIDTH);
        rt.AnchorToFillPercentageHorizontally(labelToBarRatio*100, (1-labelToBarRatio)*100);
        rt.anchoredPosition = new Vector2(0, -initialSpacing-ParamLineLength);

        bottomLine = rt.GetComponent<Image>();
        bottomLine.color = new Color(1,1,1,0.1f);

        // Right margin
        ContentRect.SetRight(RIGHT_PADDING);
    }

    // Scale label example : Damage per player. 
    public void DrawChart(Dictionary<string, float> ParamLabelsAndValues, float ParamMinLabelValue, float ParamMaxLabelValue, uint ParamSegmentCount, string ParamScaleLabel)
    {
        Clear();
        
        // Reset content size
        ContentRect.sizeDelta = new Vector2(ContentRect.sizeDelta.x, 0);

        // Initial spacing
        float y = barThickness/2;
        ExpandVertically(y);

        // Init some variables
        float currentMaxY = 0f;
        float maxValue = ParamLabelsAndValues.Values.Max();

        // Draw bars
        foreach(KeyValuePair<string, float> kvp in ParamLabelsAndValues)
        {
            WispProgressBar bar = WispProgressBar.Create(ContentRect);
            bars.Add(bar);
            bar.SetParent(this, true);

            bar.ShowTextValue = false;
            bar.PivotAround("left-center");
            bar.AnchorTo("center-top");
            bar.MyRectTransform.AnchorToFillPercentageHorizontally(labelToBarRatio*100, (1-labelToBarRatio)*100);
            bar.MyRectTransform.anchoredPosition = new Vector2(0f, -y-initialSpacing);
            bar.Height = barThickness;
            // bar.Width = barLength;
            bar.FillSpeed = 200;

            WispTextMeshPro text = WispTextMeshPro.Create(bar.MyRectTransform);
            text.SetParent(bar, true);
            text.PivotAround("right-center");
            text.AnchorTo("left-center");
            text.Base.alignment = TextAlignmentOptions.Right;
            text.MyRectTransform.anchoredPosition = new Vector2(-16f, 0);
            text.SetValue(kvp.Key);
            // text.MakeResponsive();
            
            y += barThickness + barSpacing;
            currentMaxY = y;
            ExpandVertically(barThickness + barSpacing);

            float ratio = kvp.Value / maxValue;
            bar.InitialValue = ratio * 100f;
        }

        DrawLines(ParamMinLabelValue, ParamMaxLabelValue, ParamSegmentCount, currentMaxY, ParamScaleLabel);

        ExpandVertically(initialSpacing + GRID_LINE_WIDTH*2);
    }

    public void Clear()
    {
        foreach(WispProgressBar bar in bars)
        {
            Destroy(bar.gameObject);
        }

        bars.Clear();
    }
}