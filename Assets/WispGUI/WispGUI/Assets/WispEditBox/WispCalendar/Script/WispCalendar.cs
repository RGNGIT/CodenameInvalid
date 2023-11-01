using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WispExtensions;
using TMPro;
using System.Collections;

public class WispCalendar : WispVisualComponent 
{
	public enum WispDay {Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday}

    [Header("Calendar")]
    [SerializeField] private WispDay startingDay;
    [SerializeField] private bool bringToFront = true;

    [Header("Dimensions")]    
    [SerializeField] private int dayTextSize = 10;
    [SerializeField] private int currentMonthTextSize = 12;
    [SerializeField] private int dayOfTheWeekTextSize = 8;
    // [SerializeField] private float separationHeight = 16;
    
	[Header("Transition")]
    [SerializeField] private float transitionSpeed = 5f;

	// Variables for day of the week ordering.
	protected WispDay[] weekDays = new WispDay[7];
	protected List<String> orderedWeek = new List<string>();

	private List<WispCalendarButton> dayButtons;
	private RectTransform dayButtonsParentRT;
	private RectTransform dayOfWeekLabelsParentRT;
    private WispButton NextMonthButton;
    private TMPro.TextMeshProUGUI NextMonthButtonText;
    private WispButton PreviousMonthButton;
    private TMPro.TextMeshProUGUI PreviousMonthButtonText;
    private TMPro.TextMeshProUGUI selectedMonthText;
    private DateTime currentMonth;
    private DateTime selectedDate;
    private WispCalendarButton currentlySelectedDayButton;
	private TMPro.TextMeshProUGUI[] dowTexts = new TMPro.TextMeshProUGUI[7];
    private UnityEvent onDateChanged = new UnityEvent();
    private WispEditBox parentEditBox;
	private RectTransform initialTransitionButtonsParentRT;
	private RectTransform arrivingTransitionButtonsParentRT;
	private Dictionary<Vector2, RectTransform> initialTransitionButtons = new Dictionary<Vector2, RectTransform>();
	private Dictionary<Vector2, RectTransform> arrivingTransitionButtons = new Dictionary<Vector2, RectTransform>();
	private bool isCalendarTransitioning = false;
	private Image transitionMaskImage;

	public DateTime SelectedDate {
		get {
			return selectedDate;
		}
	}

    public WispDay StartingDay { get => startingDay; set => startingDay = value; }
    public UnityEvent OnDateChanged { get => onDateChanged; }
    public WispEditBox ParentEditBox { get => parentEditBox; set => parentEditBox = value; }

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
	{
        if (isInitialized)
            return true;

        base.Initialize();

        // ---------------------------------------------------------------------

        currentMonth = DateTime.Now;
		selectedDate = currentMonth;

		NextMonthButton = transform.Find("BtnNext").GetComponent<WispButton> ();
		NextMonthButton.AddOnClickAction (NextMonth);
		NextMonthButton.SetParent(this, true);

		PreviousMonthButton = transform.Find ("BtnPrevious").GetComponent<WispButton> ();
		PreviousMonthButton.AddOnClickAction (PreviousMonth);
		PreviousMonthButton.SetParent(this, true);

		NextMonthButtonText = NextMonthButton.GetComponentInChildren<TMPro.TextMeshProUGUI> ();
		PreviousMonthButtonText = PreviousMonthButton.GetComponentInChildren<TMPro.TextMeshProUGUI> ();

		selectedMonthText = transform.Find ("DateText").GetComponent<TMPro.TextMeshProUGUI> ();
		
		dayButtonsParentRT = transform.Find ("DayButtons").GetComponent<RectTransform>();
		dayOfWeekLabelsParentRT = transform.Find("DayOfWeekLabels").GetComponent<RectTransform>();

		// Days of the week
		dowTexts[0] = dayOfWeekLabelsParentRT.Find ("DOW 1").GetComponent<TMPro.TextMeshProUGUI> ();
		dowTexts[1] = dayOfWeekLabelsParentRT.Find ("DOW 2").GetComponent<TMPro.TextMeshProUGUI> ();
		dowTexts[2] = dayOfWeekLabelsParentRT.Find ("DOW 3").GetComponent<TMPro.TextMeshProUGUI> ();
		dowTexts[3] = dayOfWeekLabelsParentRT.Find ("DOW 4").GetComponent<TMPro.TextMeshProUGUI> ();
		dowTexts[4] = dayOfWeekLabelsParentRT.Find ("DOW 5").GetComponent<TMPro.TextMeshProUGUI> ();
		dowTexts[5] = dayOfWeekLabelsParentRT.Find ("DOW 6").GetComponent<TMPro.TextMeshProUGUI> ();
		dowTexts[6] = dayOfWeekLabelsParentRT.Find ("DOW 7").GetComponent<TMPro.TextMeshProUGUI> ();

		// Transition buttons
		initialTransitionButtonsParentRT = transform.Find("TransitionButtons").Find("Initial").GetComponent<RectTransform>();
		arrivingTransitionButtonsParentRT = transform.Find("TransitionButtons").Find("Arriving").GetComponent<RectTransform>();
		transitionMaskImage = transform.Find("TransitionButtons").GetComponent<Image>();

		// Days buttons
		WispCalendarButton[] tmpBtnArray = GetComponentsInChildren<WispCalendarButton> ();

		dayButtons = new List<WispCalendarButton> ();

		int i = 0;
		for (i = 0; i < tmpBtnArray.Length; i++) {

			tmpBtnArray [i].Initialize ();

			dayButtons.Add (tmpBtnArray [i]);

			tmpBtnArray [i].ButtonComponent.onClick.AddListener (DayButtonOnClick);
		}

		DrawCalandarElements ();
		GenerateInitialTransitionButtons();
		GenerateArrivingTransitionButtons();
		UpdateDays (currentMonth.Year, currentMonth.Month);
		SelectToday ();
		UpdateSelectedDay(selectedDate);
		PaintButtons ();
		UpdateDaysOfTheWeekLabels ();

		if (bringToFront)
			WispVisualComponent.AttachCanvas(this, 137, true);

		isInitialized = true;

        return true;
	}

    void Start ()
    {
        ApplyStyle();
	}

    void Awake()
    {
		Initialize();
    }

    /// <summary>
    /// Oredering week days according to startingDay.
    /// </summary>
    protected int OrderWeek(DayOfWeek ParamDayOfWeek)
	{	
		int x = 0;
		
		foreach (WispDay day in Enum.GetValues(typeof(WispDay)))
		{
			weekDays [x] = day;

			x++;
		}

		int i, y = 0;
		int j = (int)startingDay;

		for (i = 0; i < 7; i++) {
			if (j <= 6)
            {

				orderedWeek.Add( weekDays [j].ToString());
				j++;
			} 
			else
            {
				orderedWeek.Add( weekDays [y].ToString());y++;
			}
		}

		return orderedWeek.IndexOf (ParamDayOfWeek.ToString());
	}

	/// <summary>
	/// Apply Color and graphics modifications from the style sheet.
	/// </summary>
	public override void ApplyStyle ()
	{
		if (style == null)
		{
			LogError("This calendar has no style sheet and will not be rendered correctly ---> " + gameObject.name);
            return;
		}
		
		base.ApplyStyle();
        DrawCalandarElements ();
		PaintButtons ();

		GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
		
		transitionMaskImage.ApplyStyle(style, Opacity, subStyleRule);
		transitionMaskImage.material = null;
	}

	// ...
	protected void UpdateSelectedDay (DateTime ParamDate)
	{
        selectedDate = ParamDate;
	}

	// ...
	protected void UpdateCurrentMonthLabel (DateTime ParamDate)
	{
		selectedMonthText.text = ParamDate.ToString ("MMMM", CultureInfo.InvariantCulture) + " " + ParamDate.Year.ToString();
	}

	// ...
	protected void SelectToday ()
	{
		string today = DateTime.Now.Day.ToString ();

		foreach(WispCalendarButton b in dayButtons)
		{	

			if (b.Text.text == today && b.ActiveMonth)
            {
				currentlySelectedDayButton = b;
				break;
			}

		}
	}

	// ...
	protected void UpdateDays(int ParamYear, int ParamMonth)
	{
		int year = ParamYear;
		int month = ParamMonth;
		int[,] tmpCalendar = new int[6,7];
		bool[,] tmpCalendarDaysOfActiveMonth = new bool[6,7];
		int max = DateTime.DaysInMonth (year, month);
		DateTime date = new DateTime (year, month, 1);
		DayOfWeek firstday = date.DayOfWeek; 
		int i = OrderWeek (firstday);

		int c = 1;
		int j = i;
		int x = 0;
		int y;
	
		// -------------------------------------------------------------------------
		for (y = 0; y < 6; y++) 
		{
			for (x = i; x < 7; x++) 
			{
				tmpCalendar [y, x] = c;
				tmpCalendarDaysOfActiveMonth [y, x] = true;
				c++;

				if (c > max) 
				{
					break;
				}
			}

			i = 0;

			if (c > max) 
			{
				break;
			}
		}

		// -------------------------------------------------------------------------
		c=1;
		int x1,y1;
		for (y1 = y; y1 < 6; y1++) 
		{
			for (x1 = x+1; x1 < 7; x1++) 
			{
				tmpCalendar [y1, x1] = c;
				tmpCalendarDaysOfActiveMonth [y1, x1] = false;
				c++;
			}
			x = -1;
		}

		// -------------------------------------------------------------------------
		int counter = 15;
		if (month != 1)
		{
			counter = DateTime.DaysInMonth (year, month-1);
		} 
		else
		{
			counter = DateTime.DaysInMonth (year, 12);
		}

		for (y = j - 1; y >= 0; y--)
		{
			tmpCalendar [0, y] = counter;
			tmpCalendarDaysOfActiveMonth [0, y] = false;
			counter--;
		}

		// -------------------------------------------------------------------------
		foreach(WispCalendarButton b in dayButtons)
		{	
			b.Text.text = tmpCalendar [b.Y-1, b.X-1].ToString();
			b.ActiveMonth = tmpCalendarDaysOfActiveMonth [b.Y-1, b.X-1];

			arrivingTransitionButtons[new Vector2(b.X, b.Y)].GetComponentInChildren<TextMeshProUGUI>().text = tmpCalendar [b.Y-1, b.X-1].ToString();
		}

		// -------------------------------------------------------------------------
		UpdateCurrentMonthLabel(date);
	}

	// ...
	protected void NextMonth ()
	{
		if (isCalendarTransitioning)
			return;
		
		currentMonth = currentMonth.AddMonths (1);
		CopyDaysToInitialTransitionButtons();
		UpdateDays (currentMonth.Year, currentMonth.Month);
		PaintButtons (false);
		StartCoroutine(PerformSlideTransitionToNextMonth());
	}

	// ...
	protected void PreviousMonth ()
	{
		if (isCalendarTransitioning)
			return;
		
		currentMonth = currentMonth.AddMonths (-1);
		CopyDaysToInitialTransitionButtons();
		UpdateDays (currentMonth.Year, currentMonth.Month);
		PaintButtons (false);
		StartCoroutine(PerformSlideTransitionToPreviousMonth());
	}

	// ...
	protected void PaintButtons (bool ParamUpdateInitialTransitionButtons = true)
	{
		if (style == null)
		{
			LogError("This calendar has no style sheet and will not be rendered correctly ---> " + gameObject.name);
            return;
		}
		
		foreach(WispCalendarButton b in dayButtons)
		{	
			Vector2 key = new Vector2(b.X, b.Y);
			
			if (b.ActiveMonth)
			{
				b.GetComponent<Image> ().ApplyStyle(style, Opacity, WispSubStyleRule.Widget);
				b.Text.ApplyStyle(style, Opacity, WispFontSize.Quiet, WispSubStyleRule.Widget);
				
				arrivingTransitionButtons[key].GetComponent<Image> ().ApplyStyle(style, Opacity, WispSubStyleRule.Widget);
				arrivingTransitionButtons[key].GetComponentInChildren<TMPro.TextMeshProUGUI> ().ApplyStyle(style, Opacity, WispFontSize.Quiet, WispSubStyleRule.Widget);
			}
			else 
			{
				b.GetComponent<Image> ().ApplyStyle_Inactive(style, Opacity, WispSubStyleRule.Widget);
				b.Text.ApplyStyle_Inactive(style, Opacity, WispFontSize.Quiet, WispSubStyleRule.Widget);

				arrivingTransitionButtons[key].GetComponent<Image> ().ApplyStyle_Inactive(style, Opacity, WispSubStyleRule.Widget);
				arrivingTransitionButtons[key].GetComponentInChildren<TMPro.TextMeshProUGUI> ().ApplyStyle_Inactive(style, Opacity, WispFontSize.Quiet, WispSubStyleRule.Widget);
			}
		}

		if (currentlySelectedDayButton != null && currentMonth.Month == selectedDate.Month) {

			currentlySelectedDayButton.GetComponent<Image> ().ApplyStyle_Selected(style, Opacity, WispSubStyleRule.Widget);
			currentlySelectedDayButton.Text.ApplyStyle_Selected(style, Opacity, WispFontSize.Quiet, WispSubStyleRule.Widget);
		}
	}

	// ...
	protected void DrawCalandarElements ()
	{
		if (style == null)
		{
			LogError("This calendar has no style sheet and will not be rendered correctly ---> " + gameObject.name);
            return;
		}

		float H_SPACING = (float)style.ButtonHorizontalSpacing;
		float V_SPACING = (float)style.ButtonVerticalSpacing;
		float H_PADDING = (float)style.HorizontalPadding;
		float V_PADDING = (float)style.VerticalPadding;

		// Previous month button
		PreviousMonthButton.MyRectTransform.AnchorToFillPercentage(0f,10f,0,10f);
		PreviousMonthButton.MyRectTransform.SetLeft(H_PADDING);
		PreviousMonthButton.MyRectTransform.SetTop(V_PADDING);
		// PreviousMonthButtonText.ApplyStyle(style, Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);
		// PreviousMonthButton.GetComponent<Image>().ApplyStyle(style, Opacity, WispSubStyleRule.Widget);
		
		// Next month button
		NextMonthButton.MyRectTransform.AnchorToFillPercentage(90f,10f,0f,10f);
		NextMonthButton.MyRectTransform.SetRight(H_PADDING);
		NextMonthButton.MyRectTransform.SetTop(V_PADDING);
		// NextMonthButtonText.ApplyStyle(style, Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);
		// NextMonthButton.GetComponent<Image>().ApplyStyle(style, Opacity, WispSubStyleRule.Widget);

		// Current month and year text
		selectedMonthText.GetComponent<RectTransform> ().AnchorToFillPercentage(10f,80f,0f,10f);
		selectedMonthText.GetComponent<RectTransform> ().SetTop(V_PADDING);
		selectedMonthText.fontSize = currentMonthTextSize;
		selectedMonthText.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);

		// Day of week labels
		dayOfWeekLabelsParentRT.AnchorToFillPercentage(0f,100f, 10f, 10f);

		// Day of the week labels
		float PERCENT_PER_TEXT_H = (100f / 7f);
		float PERCENT_PER_TEXT_V = (100f / 6f);

		// First day label
		dowTexts[0].GetComponent<RectTransform>().AnchorToFillPercentage(0, PERCENT_PER_TEXT_H, 0, 100);
		dowTexts[0].GetComponent<RectTransform>().SetLeft(H_PADDING);

		// Last day label
		dowTexts[6].GetComponent<RectTransform>().AnchorToFillPercentage(PERCENT_PER_TEXT_H*6, PERCENT_PER_TEXT_H, 0, 100);
		dowTexts[6].GetComponent<RectTransform>().SetRight(H_PADDING);
		
		// Day label from 2 to 6
		for (int i = 1; i < 6; i++)
		{
			dowTexts[i].GetComponent<RectTransform>().AnchorToFillPercentage(PERCENT_PER_TEXT_H*i, PERCENT_PER_TEXT_H, 0, 100);
		}

		for (int i = 0; i < 7; i++)
		{
			dowTexts[i].fontSize = dayOfTheWeekTextSize;
			dowTexts[i].ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
		}
		
		// Position day buttons
		dayButtonsParentRT.AnchorToFillPercentage(0f,100f,20f,80f);
		dayButtonsParentRT.SetRight(H_PADDING);
		dayButtonsParentRT.SetLeft(H_PADDING);
		dayButtonsParentRT.SetBottom(V_PADDING);

		foreach(WispCalendarButton b in dayButtons)
		{	
			RectTransform brt = b.GetComponent<RectTransform> ();
			brt.AnchorToFillPercentage((b.X-1) * PERCENT_PER_TEXT_H, PERCENT_PER_TEXT_H, (b.Y-1) * PERCENT_PER_TEXT_V, PERCENT_PER_TEXT_V);
			
			if (b.X == 1)
				brt.SetRight(H_SPACING / 2);
			else if (b.X == 7)
				brt.SetLeft(H_SPACING / 2);
			else
			{
				brt.SetRight(H_SPACING / 2);
				brt.SetLeft(H_SPACING / 2);
			}

			if (b.Y == 1)
				brt.SetTop(V_SPACING / 2);
			else if (b.Y == 6)
				brt.SetBottom(V_SPACING / 2);
			else
			{
				brt.SetTop(V_SPACING / 2);
				brt.SetBottom(V_SPACING / 2);
			}

			b.Text.fontSize = dayTextSize;
			b.GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
		}

		// Transition buttons
		// GenerateInitialTransitionButtons(H_PADDING, V_PADDING);
		// GenerateArrivingTransitionButtons(H_PADDING, V_PADDING);
	}

	private void GenerateInitialTransitionButtons()
	{
		float H_SPACING = (float)style.ButtonHorizontalSpacing;
		float V_SPACING = (float)style.ButtonVerticalSpacing;
		float H_PADDING = (float)style.HorizontalPadding;
		float V_PADDING = (float)style.VerticalPadding;
		
		initialTransitionButtonsParentRT.AnchorToFillPercentage(0f,100f,20f,80f);
		initialTransitionButtonsParentRT.SetRight(H_PADDING);
		initialTransitionButtonsParentRT.SetLeft(H_PADDING);
		initialTransitionButtonsParentRT.SetBottom(V_PADDING);

		float PERCENT_PER_TEXT_H = (100f / 7f);
		float PERCENT_PER_TEXT_V = (100f / 6f);
		
		for (int i = 1; i < 8; i++)
		{
			for (int j = 1; j < 7; j++)
			{
				RectTransform rt = (new GameObject("FakeButton", typeof(RectTransform))).GetComponent<RectTransform>();
				rt.gameObject.AddComponent<Image>();
				rt.SetParent(initialTransitionButtonsParentRT);
				rt.AnchorToFillPercentage((i-1) * PERCENT_PER_TEXT_H, PERCENT_PER_TEXT_H, (j-1) * PERCENT_PER_TEXT_V, PERCENT_PER_TEXT_V);
				initialTransitionButtons.Add(new Vector2(i,j), rt);

				// TODO : Spacing CONST H
				if (i == 1)
					rt.SetRight(H_SPACING / 2);
				else if (i == 7)
					rt.SetLeft(H_SPACING / 2);
				else
				{
					rt.SetRight(H_SPACING / 2);
					rt.SetLeft(H_SPACING / 2);
				}

				// TODO : Spacing CONST V
				if (j == 1)
					rt.SetTop(V_SPACING / 2);
				else if (j == 6)
					rt.SetBottom(V_SPACING / 2);
				else
				{
					rt.SetTop(V_SPACING / 2);
					rt.SetBottom(V_SPACING / 2);
				}

				RectTransform text_rt = (new GameObject("Text", typeof(RectTransform))).GetComponent<RectTransform>();
				text_rt.SetParent(rt);
				text_rt.AnchorStyleExpanded(true);
				TMPro.TextMeshProUGUI textComp = text_rt.gameObject.AddComponent<TMPro.TextMeshProUGUI>();
				textComp.fontSize = dayTextSize;
				textComp.text = "Init";
				textComp.alignment = TextAlignmentOptions.Center;
			}
		}

		initialTransitionButtonsParentRT.gameObject.SetActive(false);
	}

	private void GenerateArrivingTransitionButtons()
	{
		float H_SPACING = (float)style.ButtonHorizontalSpacing;
		float V_SPACING = (float)style.ButtonVerticalSpacing;
		float H_PADDING = (float)style.HorizontalPadding;
		float V_PADDING = (float)style.VerticalPadding;
		
		arrivingTransitionButtonsParentRT.AnchorToFillPercentage(0f,100f,20f,80f);
		arrivingTransitionButtonsParentRT.SetRight(H_PADDING);
		arrivingTransitionButtonsParentRT.SetLeft(H_PADDING);
		arrivingTransitionButtonsParentRT.SetBottom(V_PADDING);

		float PERCENT_PER_TEXT_H = (100f / 7f);
		float PERCENT_PER_TEXT_V = (100f / 6f);
		
		for (int i = 1; i < 8; i++)
		{
			for (int j = 1; j < 7; j++)
			{
				RectTransform rt = (new GameObject("FakeButton", typeof(RectTransform))).GetComponent<RectTransform>();
				rt.gameObject.AddComponent<Image>();
				rt.SetParent(arrivingTransitionButtonsParentRT);
				rt.AnchorToFillPercentage((i-1) * PERCENT_PER_TEXT_H, PERCENT_PER_TEXT_H, (j-1) * PERCENT_PER_TEXT_V, PERCENT_PER_TEXT_V);
				arrivingTransitionButtons.Add(new Vector2(i,j), rt);

				// TODO : Spacing CONST H
				if (i == 1)
					rt.SetRight(H_SPACING / 2);
				else if (i == 7)
					rt.SetLeft(H_SPACING / 2);
				else
				{
					rt.SetRight(H_SPACING / 2);
					rt.SetLeft(H_SPACING / 2);
				}

				// TODO : Spacing CONST V
				if (j == 1)
					rt.SetTop(V_SPACING / 2);
				else if (j == 6)
					rt.SetBottom(V_SPACING / 2);
				else
				{
					rt.SetTop(V_SPACING / 2);
					rt.SetBottom(V_SPACING / 2);
				}

				RectTransform text_rt = (new GameObject("Text", typeof(RectTransform))).GetComponent<RectTransform>();
				text_rt.SetParent(rt);
				text_rt.AnchorStyleExpanded(true);
				TMPro.TextMeshProUGUI textComp = text_rt.gameObject.AddComponent<TMPro.TextMeshProUGUI>();
				textComp.fontSize = dayTextSize;
				textComp.text = "Arr";
				textComp.alignment = TextAlignmentOptions.Center;
			}
		}

		arrivingTransitionButtonsParentRT.gameObject.SetActive(false);
	}

	// ...
	protected void UpdateDaysOfTheWeekLabels ()
	{
		WispDay[] dayArray = new WispDay[7];
		int c = 0;

		foreach (WispDay day in Enum.GetValues(typeof(WispDay)))
		{
			dayArray [c] = day;
			c++;
		}

		for (int i = 0; i < 7; i++)
		{
			dowTexts[i].text = orderedWeek [i].Substring(0,3);
		}
	}

	// ...
	protected void DayButtonOnClick ()
	{
		WispCalendarButton tmpBtn = EventSystem.current.currentSelectedGameObject.GetComponent<WispCalendarButton> ();

		if (tmpBtn.ActiveMonth) {
		
			currentlySelectedDayButton = tmpBtn;
            DateTime newDate = new DateTime(currentMonth.Year, currentMonth.Month, Int32.Parse(currentlySelectedDayButton.Text.text));

            if (newDate == selectedDate)
                Close();

            UpdateSelectedDay(newDate);

            if (onDateChanged != null)
            {
                onDateChanged.Invoke();
            }

		}

		PaintButtons ();
	}

    /// <summary>
    /// ...
    /// </summary>
    public static WispCalendar Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.Calandar, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.Calandar);
        }

        return go.GetComponent<WispCalendar>();
    }

    public override void Close()
    {
        base.Close();

        if (parentEditBox != null)
            parentEditBox.Close();
    }

    public override string GetValue()
    {
        if (selectedDate == null)
			return "";
		else
			return SelectedDate.ToShortDateString();
    }

	private IEnumerator PerformSlideTransitionToNextMonth()
	{
		isCalendarTransitioning = true;

		dayButtonsParentRT.gameObject.SetActive(false);
		initialTransitionButtonsParentRT.gameObject.SetActive(true);
		arrivingTransitionButtonsParentRT.gameObject.SetActive(true);

		Vector3 initialPos_1 = initialTransitionButtonsParentRT.position;
		Vector3 destinationPos_1 = initialTransitionButtonsParentRT.position + new Vector3(initialTransitionButtonsParentRT.rect.width + 8, 0, 0);

		Vector3 destinationPos_2 = arrivingTransitionButtonsParentRT.position;
		Vector3 initialPos_2 = arrivingTransitionButtonsParentRT.position - new Vector3(arrivingTransitionButtonsParentRT.rect.width + 8, 0, 0);
		arrivingTransitionButtonsParentRT.position = initialPos_2;

		float t = 0;

		while(t<1)
		{
			t += transitionSpeed * Time.deltaTime;
			initialTransitionButtonsParentRT.position = Vector3.Lerp(initialPos_1, destinationPos_1, t);
			arrivingTransitionButtonsParentRT.position = Vector3.Lerp(initialPos_2, destinationPos_2, t);
			yield return new WaitForEndOfFrame();
		}

		dayButtonsParentRT.gameObject.SetActive(true);

		initialTransitionButtonsParentRT.position = initialPos_1;
		initialTransitionButtonsParentRT.gameObject.SetActive(false);

		arrivingTransitionButtonsParentRT.position = initialPos_1;
		arrivingTransitionButtonsParentRT.gameObject.SetActive(false);

		isCalendarTransitioning = false;
		
		yield return null;
	}

	private IEnumerator PerformSlideTransitionToPreviousMonth()
	{
		isCalendarTransitioning = true;

		dayButtonsParentRT.gameObject.SetActive(false);
		initialTransitionButtonsParentRT.gameObject.SetActive(true);
		arrivingTransitionButtonsParentRT.gameObject.SetActive(true);

		Vector3 initialPos_1 = initialTransitionButtonsParentRT.position;
		Vector3 destinationPos_1 = initialTransitionButtonsParentRT.position - new Vector3(initialTransitionButtonsParentRT.rect.width + 8, 0, 0);

		Vector3 destinationPos_2 = arrivingTransitionButtonsParentRT.position;
		Vector3 initialPos_2 = arrivingTransitionButtonsParentRT.position + new Vector3(arrivingTransitionButtonsParentRT.rect.width + 8, 0, 0);
		arrivingTransitionButtonsParentRT.position = initialPos_2;

		float t = 0;

		while(t<1)
		{
			t += transitionSpeed * Time.deltaTime;
			initialTransitionButtonsParentRT.position = Vector3.Lerp(initialPos_1, destinationPos_1, t);
			arrivingTransitionButtonsParentRT.position = Vector3.Lerp(initialPos_2, destinationPos_2, t);
			yield return new WaitForEndOfFrame();
		}

		dayButtonsParentRT.gameObject.SetActive(true);

		initialTransitionButtonsParentRT.position = initialPos_1;
		initialTransitionButtonsParentRT.gameObject.SetActive(false);

		arrivingTransitionButtonsParentRT.position = initialPos_1;
		arrivingTransitionButtonsParentRT.gameObject.SetActive(false);

		isCalendarTransitioning = false;
		
		yield return null;
	}

	private void CopyDaysToInitialTransitionButtons()
	{
		foreach(WispCalendarButton b in dayButtons)
		{	
			Vector2 key = new Vector2(b.X, b.Y);
			initialTransitionButtons[key].GetComponentInChildren<TextMeshProUGUI>().text = b.Text.text;

			if (b.ActiveMonth)
			{
				initialTransitionButtons[key].GetComponent<Image> ().ApplyStyle(style, Opacity, WispSubStyleRule.Widget);
				initialTransitionButtons[key].GetComponentInChildren<TMPro.TextMeshProUGUI> ().ApplyStyle(style, Opacity, WispFontSize.Quiet, WispSubStyleRule.Widget);
			}
			else
			{
				initialTransitionButtons[key].GetComponent<Image> ().ApplyStyle_Inactive(style, Opacity, WispSubStyleRule.Widget);
				initialTransitionButtons[key].GetComponentInChildren<TMPro.TextMeshProUGUI> ().ApplyStyle_Inactive(style, Opacity, WispFontSize.Quiet, WispSubStyleRule.Widget);
			}
		}
	}
}