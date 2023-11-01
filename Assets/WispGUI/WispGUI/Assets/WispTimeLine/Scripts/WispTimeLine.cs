using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WispExtensions;
using UnityEngine.UI;
using System.Collections;

public class WispTimeLine : WispScrollView, IPointerEnterHandler, IPointerExitHandler
{
    //[Header("Prefabs")] // Don't uncomment this
    [SerializeField] private GameObject eventMarkPrefab = null;

    [Header("Visuals")]
    [SerializeField] private float groupSizeInPixels = 64f;
    [SerializeField] private bool showGroupingGrid = false;
    [SerializeField] private int verticalScatterLevels = 3;

    [Header("Controls")]
    [SerializeField] private bool enableZoom = true;

    [Header("Test")]
    [SerializeField] private bool fillWithTestData = false;

    private List<WispTimeLineEvent> events = new List<WispTimeLineEvent>();
    private Dictionary<int, WispTimeLineEventMark> eventGroups = new Dictionary<int, WispTimeLineEventMark>();
    private Dictionary<long, Image> groupingGridLines = new Dictionary<long, Image>();

    private DateTime displayRangeStartDate;
    private DateTime displayRangeEndDate;
    private double displayRangeStartDateInTicks;
    private double displayRangeEndDateInTicks;
    private double totalRangeInTicks;
    private double totalTimeLineInTicks;
    private bool isMouseInside = false;
    private TMPro.TextMeshProUGUI txtStartDate;
    private TMPro.TextMeshProUGUI txtCurrentDate;
    private TMPro.TextMeshProUGUI txtEndDate;
    private GameObject timeCursor;
    private float cursorHorizontalPositionPercentage;
    private float dragStartPosition;
    private double dragStartTicks;
    private double dragEndTicks;
    private Vector2 localpoint;
    private Vector2 lastLocalPoint;
    private int groupCount = 0;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        ApplyStyle();

        if (fillWithTestData)
            FillWithTestData();
    }

    /// <summary>
    /// Awake
    /// </summary>
    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        if (isMouseInside)
        {
            // Move time cursor
            localpoint = rectTransform.GetMousePositionInMe();

            if (lastLocalPoint != localpoint)
            {
                lastLocalPoint = localpoint;

                timeCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(localpoint.x, 0);
                cursorHorizontalPositionPercentage = (100 * localpoint.x) / contentRect.rect.width;

                // Update current cursor date
                long dateTick = Convert.ToInt64(displayRangeStartDateInTicks + totalRangeInTicks * (cursorHorizontalPositionPercentage / 100)).Clamp(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks);
                txtCurrentDate.text = new DateTime(dateTick).ToShortDateString();
            }

            // Check Zoom
            if (enableZoom)
            {
                int y = Convert.ToInt32(Input.mouseScrollDelta.y);
                if (y != 0)
                {
                    ExpandView(-10 * y);
                    UpdatePositions();
                }
            }

            // Check keyboard zoom
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                ExpandView(10);
                UpdatePositions();
            }
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                ExpandView(-10);
                UpdatePositions();
            }
        }
    }

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        // ------------------------

        txtStartDate = transform.Find("Viewport").Find("TxtStartDate").GetComponent<TMPro.TextMeshProUGUI>();
        txtCurrentDate = transform.Find("Viewport").Find("TxtCurrentDate").GetComponent<TMPro.TextMeshProUGUI>();
        txtEndDate = transform.Find("Viewport").Find("TxtEndDate").GetComponent<TMPro.TextMeshProUGUI>();
        timeCursor = contentRect.Find("TimeCursor").gameObject;

        // ------------------------

        totalTimeLineInTicks = DateTime.MaxValue.Ticks - DateTime.MinValue.Ticks;
        StartCoroutine(lateVisualUpdate());

        // ------------------------

        isInitialized = true;
        return true;
    }

    private IEnumerator lateVisualUpdate()
    {
        yield return new WaitForEndOfFrame();
        UpdatePositions();
        yield return null;
    }

    /// <summary>
    /// Add an event to the Timeline.
    /// </summary>
    public WispTimeLineEvent AddEvent(WispTimeLineEvent ParamEvent)
    {
        events.Add(ParamEvent);

        return ParamEvent;
    }

    /// <summary>
    /// Defines which events can be displayed according to their dates.
    /// </summary>
    public void SetDisplayRange(DateTime ParamStartDate, DateTime ParamEndDate, bool ParamDoVisualUpdate = false)
    {
        displayRangeStartDate = ParamStartDate;
        displayRangeEndDate = ParamEndDate;

        SetDisplayRangeInTicks(ParamStartDate.Ticks, ParamEndDate.Ticks, ParamDoVisualUpdate);

        if (ParamDoVisualUpdate)
            UpdatePositions();
    }

    /// <summary>
    /// Defines which events can be displayed according to their dates.
    /// </summary>
    private void SetDisplayRangeInTicks(double ParamStartDate, double ParamEndDate, bool ParamDoVisualUpdate = false)
    {
        if (ParamStartDate >= ParamEndDate)
            return;

        long ParamStartDateAsLong = Convert.ToInt64(ParamStartDate);
        long ParamEndDateAsLong = Convert.ToInt64(ParamEndDate);

        if (ParamStartDate < DateTime.MinValue.Ticks)
            ParamStartDateAsLong = DateTime.MinValue.Ticks;

        if (ParamEndDate > DateTime.MaxValue.Ticks)
            ParamEndDateAsLong = DateTime.MaxValue.Ticks;

        displayRangeStartDateInTicks = ParamStartDate;
        displayRangeEndDateInTicks = ParamEndDate;

        txtStartDate.text = new DateTime(ParamStartDateAsLong).ToLongDateString();

        txtEndDate.text = new DateTime(ParamEndDateAsLong).ToLongDateString();

        totalRangeInTicks = displayRangeEndDateInTicks - displayRangeStartDateInTicks;

        if (ParamDoVisualUpdate)
        {
            UpdatePositions();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseInside = true;
        // timeCursor.SetActive(true);
        UpdateTimeCursor();
        txtCurrentDate.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseInside = false;
        // timeCursor.SetActive(false);
        UpdateTimeCursor();
        txtCurrentDate.gameObject.SetActive(false);
    }

    public void OnDragStart()
    {
        dragStartPosition = Input.mousePosition.x;
        dragStartTicks = displayRangeStartDateInTicks;
        dragEndTicks = displayRangeEndDateInTicks;
    }

    public void OnDrag()
    {
        double scale = totalRangeInTicks / totalTimeLineInTicks;
        double dragAmount = (Input.mousePosition.x - dragStartPosition)*10000000000000000*scale;
        SetDisplayRangeInTicks(dragStartTicks - dragAmount, dragEndTicks - dragAmount, true);
    }

    // ...
    public new static WispTimeLine Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.TimeLine, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.TimeLine);
        }

        return go.GetComponent<WispTimeLine>();
    }

    public void FillWithTestData()
    {
        SetDisplayRange(new DateTime(1200,1,1), new DateTime(2100, 1, 1), false);

        AddEvent(new WispTimeLineEvent("The Printing press", "Around 1440, Goldsmith and inventor Johannes Gutenberg invented the Printing press.", new DateTime(1440,1,1)));
        AddEvent(new WispTimeLineEvent("The Light bulb", "In 1802, Humphry Davy invented the first electric light.", new DateTime(1802, 1, 1)));
        AddEvent(new WispTimeLineEvent("The Airplane", "On December 17th 1903, Wilbur and Orville Wright made four brief flights at Kitty Hawk with their first powered aircraft.", new DateTime(1903,12,17)));
        AddEvent(new WispTimeLineEvent("The Personal computer", "In 1974, A small firm named MITS made the first personal computer, the Altair, which used Intel Corporation’s 8080 microprocessor.", new DateTime(1974,1,1)));
        AddEvent(new WispTimeLineEvent("The Vaccine", "The smallpox vaccine was invented in 1796 by English physician Edward Jenner.", new DateTime(1796,1,1)));
        AddEvent(new WispTimeLineEvent("The Automobile", "On January 29th 1886, Carl Benz applied for a patent for his “vehicle powered by a gas engine” which may be regarded as the birth certificate of the automobile.", new DateTime(1886,1,29)));
        AddEvent(new WispTimeLineEvent("The Clock", "The first mechanical clocks, were invented in Europe at around the start of the 14th century.", new DateTime(1300,1,1)));
        AddEvent(new WispTimeLineEvent("The Telephone", "Both Alexander Graham Bell and Elisha Gray submitted independent patent applications concerning telephones to the patent office in Washington on February 14th 1876.", new DateTime(1876,2,14)));
        AddEvent(new WispTimeLineEvent("The Refrigeration system", "In 1834 American inventor Jacob Perkins, built the world's first working vapor-compression refrigeration system.", new DateTime(1834, 1, 1)));
        AddEvent(new WispTimeLineEvent("The Camera", "Johann Zahn designed the first camera in 1685. But the first photograph was clicked by Joseph Nicephore Niepce in the year 1814. It was thousands of years back that an Iraqi scientist Ibn Al Haytham made a mention of this kind of a device in his book, Book of Optics in 1021.", new DateTime(1658, 1, 1)));
        AddEvent(new WispTimeLineEvent("The Transistor", "The first transistor was successfully demonstrated on December 23rd 1947,The three individuals credited with the invention of the transistor were William Shockley, John Bardeen and Walter Brattain.", new DateTime(1947, 12, 23)));
        AddEvent(new WispTimeLineEvent("The Liquid-fueled rocket", "Robert Hutchings Goddard is considered the father of modern rocket propulsion.", new DateTime(1926, 3, 16)));
        AddEvent(new WispTimeLineEvent("The Artificial satellite", "The first artificial satellite was Sputnik 1, launched by the Soviet Union on 4 October 1957 under the Sputnik program, with Sergei Korolev as chief designer.", new DateTime(1957, 10, 4)));
        AddEvent(new WispTimeLineEvent("The Vacuum tube", "In 1904, British engineer John Ambrose Fleming invents and patents the thermionic valve, the first vacuum tube. With this advance, the age of modern wireless electronics is born.", new DateTime(1904, 11, 16)));
        AddEvent(new WispTimeLineEvent("The Wristwatch", "In response to a commission from the Queen of Naples June 8th 1810, Breguet conceived and made the first wristwatch ever known.", new DateTime(1810, 6, 8)));
        AddEvent(new WispTimeLineEvent("The Smartphone", "The first commercially available device that could be properly referred to as a \"smartphone\" began as a prototype called \"Angler\" developed by Canova in 1992 while at IBM.", new DateTime(1992, 11, 1)));
        AddEvent(new WispTimeLineEvent("The Digital watch", "The first digital electronic watch, a Pulsar LED prototype in 1970, was developed jointly by Hamilton Watch Company and Electro-Data, founded by George H. Thiess.", new DateTime(1970, 5, 5)));
    }

    private void ExpandView(float ParamExpansionRatio)
    {
        float ratio = contentRect.rect.width.GetPercentage(ParamExpansionRatio);

        if (ParamExpansionRatio < 0 && contentRect.rect.width - ratio < rectTransform.rect.width)
        {
            ratio = rectTransform.rect.width - contentRect.rect.width;
        }

        contentRect.TuneRight(ratio);
        HorizontalScrollToPosition((cursorHorizontalPositionPercentage/100)*contentRect.rect.width);

        GenerateGroups();
    }

    public override void UpdatePositions()
    {
        GenerateGroups();
        RenderGroups();
        GenerateGridLines();
    }

    private void GenerateGroups()
    {
        ClearGroups();

        groupCount = Mathf.FloorToInt(contentRect.rect.width / groupSizeInPixels);

        if (groupCount > 0)
        {
            long tickPerGroup = Convert.ToInt64(totalRangeInTicks / groupCount);

            for (int i = 0; i < groupCount; i++)
            {
                WispTimeLineEventMark mark = Instantiate(eventMarkPrefab, ContentRect).GetComponent<WispTimeLineEventMark>();
                mark.BaseTick = (i * tickPerGroup) + Convert.ToInt64(displayRangeStartDateInTicks);
                mark.MaxTick = ((i+1) * tickPerGroup) + Convert.ToInt64(displayRangeStartDateInTicks);
                mark.SetLabel(i.ToString());
                mark.SetParent(this, true);
                eventGroups.Add(i, mark);
            }

            foreach(WispTimeLineEvent evnt in events)
            {
                for (int i = 0; i < groupCount; i++)
                {
                    if (evnt.StartingDateInTicks >= eventGroups[i].BaseTick && evnt.StartingDateInTicks <= eventGroups[i].MaxTick)
                    {
                        eventGroups[i].RegisterEvent(evnt);
                        break;
                    }
                }
            }
        }
    }

    private void GenerateGridLines()
    {
        foreach (KeyValuePair<long, Image> kvp in groupingGridLines)
        {
            Destroy(kvp.Value.gameObject);
        }

        groupingGridLines.Clear();

        if (!showGroupingGrid)
            return;

        double tickPerPixel = totalRangeInTicks / contentRect.rect.width;

        for (int i = 1; i < groupCount + 1; i++)
        {
            Image img = Instantiate(timeCursor, contentRect).GetComponent<Image>();
            img.color = new Color(0, 0, 1, 0.1f);
            img.gameObject.name = "GGL";
            img.GetComponent<RectTransform>().anchorMin = new Vector2(0,0);
            img.GetComponent<RectTransform>().anchorMax = new Vector2(0,1);
            img.GetComponent<RectTransform>().anchoredPosition = new Vector2(groupSizeInPixels * i, 0);
            groupingGridLines.Add(i * Convert.ToInt64(tickPerPixel), img);
        }
    }

    private void ClearGroups()
    {
        foreach(KeyValuePair<int, WispTimeLineEventMark> kvp in eventGroups)
        {
            Destroy(kvp.Value.gameObject);
        }

        eventGroups.Clear();
    }

    private void RenderGroups()
    {
        // Prepare some variables for vertical scatter.
        if (verticalScatterLevels <= 0)
            verticalScatterLevels = 1;
        
        float[] levels = new float[verticalScatterLevels];
        float h = rectTransform.rect.height.GetPercentage(80);
        float levelSize = h / verticalScatterLevels;

        for(int i = 0; i < levels.Length; i++)
        {
            levels[i] = (i * levelSize) - (h/2) + rectTransform.rect.height.GetPercentage(20);
        }

        //---------------------------------------------------------------------
        
        int c = 0;
        foreach(KeyValuePair<int, WispTimeLineEventMark> kvp in eventGroups)
        {
            if (kvp.Value.EventCount > 0)
            {
                kvp.Value.gameObject.SetActive(true);
                kvp.Value.UpdateTooltipAndLabel();
                kvp.Value.GetComponent<WispImage>().Set_X_Position((kvp.Key * groupSizeInPixels) + (groupSizeInPixels / 2));
                kvp.Value.GetComponent<WispImage>().Set_Y_Position(levels[c]);
                c++;

                if (c == levels.Length)
                    c = 0;
            }
            else
            {
                kvp.Value.gameObject.SetActive(false);
            }
        }
    }

    public override void ApplyStyle()
    {
        base.ApplyStyle();

        txtStartDate.ApplyStyle(style, Opacity, WispFontSize.Quiet, WispSubStyleRule.Widget);
        txtCurrentDate.ApplyStyle(style, Opacity, WispFontSize.Quiet, WispSubStyleRule.Widget);
        txtEndDate.ApplyStyle(style, Opacity, WispFontSize.Quiet, WispSubStyleRule.Widget);

        UpdateTimeCursor();
    }

    private void UpdateTimeCursor()
    {
        if (isMouseInside)
            timeCursor.GetComponent<Image>().color = colop(style.LineColor);
        else
            timeCursor.GetComponent<Image>().color = Color.clear;
    }
}