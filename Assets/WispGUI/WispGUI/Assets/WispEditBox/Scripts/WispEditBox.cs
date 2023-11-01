using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WispExtensions;

public class WispEditBox : WispVisualComponent 
{
	public enum EditBoxPickerType {None, DatePicker, List}

	[Header("Style")]
	[SerializeField] private Sprite statusSignSprite = null;
    [SerializeField] private Sprite pickerOpenerSprite = null;

    [Header("Type")]
    [SerializeField] private EditBoxPickerType pickerType;

	[ConditionalHidePickerTypeAttribute("pickerType", true, WispEditBox.EditBoxPickerType.List)]
	[SerializeField] private DropDownListDefaultItems defaultItems = null;

	[ConditionalHidePickerTypeAttribute("pickerType", true, WispEditBox.EditBoxPickerType.List)]
	[SerializeField] private bool selectOnItemClick = true;

	[ConditionalHidePickerTypeAttribute("pickerType", true, WispEditBox.EditBoxPickerType.List)]
	[SerializeField] private bool closeOnItemClick = true;

    [ConditionalHidePickerTypeAttribute("pickerType", true, WispEditBox.EditBoxPickerType.List)]
    [SerializeField] private bool searchMode = true;

    [ConditionalHidePickerTypeAttribute("pickerType", true, WispEditBox.EditBoxPickerType.List)]
    [SerializeField] private float itemHeight = 32f;

    [ConditionalHidePickerTypeAttribute("pickerType", true, WispEditBox.EditBoxPickerType.DatePicker)]
	[SerializeField] private WispCalendar.WispDay startingDayOfTheWeek = WispCalendar.WispDay.Sunday;

	[Header("Label")]
    [SerializeField] private bool displayLabel = true;

    [SerializeField] [ConditionalHideBoolAttribute("displayLabel", true, true)] private string defaultLabel = "";

	[Header("Text")]
	[SerializeField] private string defaultText = "";
	[SerializeField] private bool readOnly = false;

	[Header("Prefabs")]
	[SerializeField] private GameObject calendarPrefab = null;
	[SerializeField] private GameObject dropdownListPrefab = null;
	[SerializeField] private GameObject dropdownListItemPrefab = null;

	public GameObject DropdownListPrefab 
    {
		get 
        {
			return dropdownListPrefab;
		}
	}

	public GameObject DropdownListItemPrefab 
    {
		get 
        {
			return dropdownListItemPrefab;
		}
	}

	public WispDropDownList DropDownList 
    {
		get 
        {
			return dropDownList;
		}
	}

	public string HiddenValue 
    {
		get 
        {
			return hiddenValue;
		}
		set 
        {
			hiddenValue = value;
		}
	}

	public string Label
	{
		get { return transform.Find ("Label").GetComponent<TMPro.TMP_Text> ().text; }
		set { transform.Find ("Label").GetComponent<TMPro.TMP_Text> ().text = value; }
	}

    public bool DisplayLabel
    {
        get => displayLabel;

        set
        {
            displayLabel = value;
            if (displayLabel)
            {
                label.SetActive(true);
            }
            else
            {
                label.SetActive(false);
            }
        }
    }

    public WispDropDownListItem SelectedItem 
    {
		get 
        {
			return selectedItem;
		}
		set 
        {
            ignoreOnValueChangedFrame = Time.frameCount;

            if (value == null)
            {
                selectedItem = null;
                SetValue("");
                hiddenValue = "";
                Close();
                return;
            }

            WispDropDownListItem item = selectedItem;

            selectedItem = value;
			SetValue (value.TextValue);
			hiddenValue = value.Name;

            dropDownList.ScrollToPosition_Async(value.MyRectTransform.anchoredPosition, 0.1f);

            selectedItem.ApplyStyle();

            if (item != null)
                item.ApplyStyle();

            if (onItemSelectionChanged != null)
                onItemSelectionChanged.Invoke();  
        }
	}

	public bool ReadOnly 
    {
		get
		{
			return inputField.readOnly;
		}
		set 
		{
            inputField.readOnly = value;
		}
	}

    public EditBoxPickerType PickerType
    {
        get => pickerType;

        set
        {
            UpdatePicker(value);
        }
    }

    public TMP_InputField Base
    {
        get
        {
            return GetComponent<TMP_InputField>();
        }
    }

    public float ItemHeight { get => itemHeight; }

    private WispButton pickerButton;
    private GameObject datePicker;
    private WispDropDownList dropDownList;
    private string hiddenValue;
    private WispDropDownListItem selectedItem = null;
    private GameObject statusSign;
    private TMPro.TMP_InputField inputField;
    private UnityAction onItemSelectionChanged;
    private GameObject label;
    private UnityEvent onPickerOpen;
    private IEnumerator searchCoroutine = null;
    private int ignoreOnValueChangedFrame;
    private UnityAction onEnterAction;

    private void UpdatePicker(EditBoxPickerType ParamType, bool ParamForceUpdate = false)
    {
        if (ParamType == pickerType && !ParamForceUpdate)
            return;

        pickerType = ParamType;

        if (PickerType == EditBoxPickerType.None)
        {
            if (dropDownList != null)
            {
                Destroy(dropDownList);
            }

            if (datePicker != null)
            {
                Destroy(datePicker);
            }

            pickerButton.RenderOff();

            if (onEnterAction != null)
            {
                WispKeyBoardEventSystem.AddEventOnKey(KeyCode.Return, onEnterAction, this);
                WispKeyBoardEventSystem.AddEventOnKey(KeyCode.KeypadEnter, onEnterAction, this);
            }
        }
        else if (PickerType == EditBoxPickerType.DatePicker)
        {
            if (dropDownList != null)
            {
                Destroy(dropDownList);
            }

            pickerButton.Base.onClick.RemoveAllListeners();
            pickerButton.gameObject.SetActive(true);

            if (GetComponentInChildren<WispCalendar>(true) != null)
            {
                Destroy(GetComponentInChildren<WispCalendar>(true).gameObject);
            }

            datePicker = GameObject.Instantiate(calendarPrefab, transform);
            datePicker.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -32);
            datePicker.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, datePicker.GetComponent<RectTransform>().sizeDelta.y);
            datePicker.SetActive(false);

            pickerButton.RenderOn();
            pickerButton.AddOnClickAction(toggleDatePicker);

            WispCalendar tmpCalandar = datePicker.GetComponent<WispCalendar>();
            tmpCalandar.SetParent(this, true);
            tmpCalandar.ParentEditBox = this;

            tmpCalandar.MyRectTransform.AnchorTo("left-bottom");
            tmpCalandar.MyRectTransform.PivotAround("left-top");
            tmpCalandar.MyRectTransform.AnchorToFillPercentageHorizontally(0f,100f);
            tmpCalandar.MyRectTransform.SetRight(0f);
            tmpCalandar.MyRectTransform.SetLeft(0f);
            tmpCalandar.MyRectTransform.anchoredPosition3D = Vector3.zero;

            tmpCalandar.OnDateChanged.AddListener(delegate { inputField.text = tmpCalandar.SelectedDate.ToShortDateString(); });
            tmpCalandar.StartingDay = startingDayOfTheWeek;

            SetValue(tmpCalandar.SelectedDate.ToShortDateString());
        }
        else if (PickerType == EditBoxPickerType.List)
        {
            if (datePicker != null)
            {
                Destroy(datePicker);
            }

            pickerButton.Base.onClick.RemoveAllListeners();
            pickerButton.gameObject.SetActive(true);

            if (GetComponentInChildren<WispDropDownList>(true) != null)
            {
                Destroy(GetComponentInChildren<WispDropDownList>(true).gameObject);
            }

            dropDownList = GameObject.Instantiate(dropdownListPrefab, transform).GetComponent<WispDropDownList>(); ;

            dropDownList.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -32);
            dropDownList.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, dropDownList.GetComponent<RectTransform>().sizeDelta.y);

            WispDropDownList tmpList = dropDownList.GetComponent<WispDropDownList>();
            tmpList.SetParent(this, true);

            tmpList.ParentEditBox = this;
            tmpList.SelectOnItemClick = selectOnItemClick;
            tmpList.CloseOnItemClick = closeOnItemClick;

            pickerButton.RenderOn();
            pickerButton.AddOnClickAction(toggleDropDownList);

            if (defaultItems.defaultItemsList != null)
            {
                foreach (string s in defaultItems.defaultItemsList)
                {
                    dropDownList.GetComponent<WispDropDownList>().AddItem(s, s);
                }
            }

            if (defaultItems.defaultSelection >= 0)
            {
                dropDownList.GetComponent<WispDropDownList>().SetSelectedItemByIndex(defaultItems.defaultSelection + 1); // Add one because DropDownList index is 1 based, and DefaultItems are 0 based.
            }

            WispKeyBoardEventSystem.AddEventOnKey(KeyCode.UpArrow, DropDownListGoUp, dropDownList);
            WispKeyBoardEventSystem.AddEventOnKey(KeyCode.DownArrow, DropDownListGoDown, dropDownList);
            WispKeyBoardEventSystem.AddEventOnKey(KeyCode.Return, Close, dropDownList);
            WispKeyBoardEventSystem.AddEventOnKey(KeyCode.KeypadEnter, Close, dropDownList);
            
            dropDownList.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Awake
    /// </summary>
    void Awake()
    {
        Initialize();
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

        inputField = GetComponent<TMPro.TMP_InputField>();

        statusSign = transform.Find ("StatusSign").gameObject;
		statusSign.GetComponent<Image> ().sprite = statusSignSprite;
		statusSign.SetActive(false);

        label = transform.Find("Label").gameObject;
        Label = defaultLabel;
        if (!displayLabel)
        {
            label.SetActive(false);
        }

		inputField.text = defaultText;
        inputField.readOnly = readOnly;
        inputField.onSelect.AddListener(delegate { onFocus(); });
        inputField.onDeselect.AddListener(delegate { onFocusLost(); });

        if (searchMode)
        {
            inputField.onValueChanged.AddListener(delegate { searchDropDownList(); });
        }

		// ---------------------------------------------------------------------

		pickerButton = transform.Find ("PickerOpener").GetComponent<WispButton> ();
        pickerButton.Initialize();
        pickerButton.IconPlacement = WispButton.WispButtonIconPlacement.Full;
        pickerButton.SetIcon(pickerOpenerSprite);
        pickerButton.SetParent(this, true);

        // ---------------------------------------------------------------------

        UpdatePicker(pickerType, true);

		// ---------------------------------------------------------------------------------------------------------------------------
		
		// ApplyStyle();

        // -----------------------
        if (inputField.text != null)
		    isInitialized = true;

        return true;
	}

    void Start()
    {
        ApplyStyle();
    }

    /// <summary>
    /// ...
    /// </summary>
    public static WispEditBox Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.EditBox, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.EditBox);
        }

        return go.GetComponent<WispEditBox>();
    }

    /// <summary>
    /// Return the text in the editbox, or the hidden value of the selected item in case of pickerType being List.
    /// </summary>
    public override string GetValue()
	{
		if (pickerType == EditBoxPickerType.None) {

			return inputField.text;

		} else if (pickerType == EditBoxPickerType.DatePicker) {

            DateTime date;

            try
            {
                date = DateTime.Parse(inputField.text);
            }
            catch (FormatException e)
            {
                print(e.Message);
                return "";
            }
            
			return date.Year + "-" + date.Month + "-" + date.Day;

		} else if (pickerType == EditBoxPickerType.List) {

			return hiddenValue;

		}
			
		return "";
	}

	/// <summary>
	/// Enable or disable Status sign/indicator.
	/// </summary>
	public void EnableStatusSign(bool ParamState)
	{
		statusSign.SetActive (ParamState);
	}

    // ...
    private void toggleDatePicker ()
	{
		if (datePicker.activeInHierarchy) {
            closeDatePicker();
		} else {
            openDatePicker();
		}
	}

    public void AddOnPickerOpenAction(UnityAction ParamAction)
    {
        if (onPickerOpen == null)
            onPickerOpen = new UnityEvent();

        onPickerOpen.AddListener(ParamAction);
    }

    // ...
    private void toggleDropDownList ()
	{
        if (dropDownList.gameObject.activeInHierarchy) {
			dropDownList.gameObject.SetActive (false);
			currentlyOpenedVisualComponent = null;
		} else {
            OpenDropDownList();
		}
	}

    private void OpenDropDownList()
    {
        if (currentlyOpenedVisualComponent != null)
			currentlyOpenedVisualComponent.Close();
			
		dropDownList.gameObject.SetActive(true);
		transform.SetAsLastSibling();
		currentlyOpenedVisualComponent = this;

        if (onPickerOpen != null)
            onPickerOpen.Invoke();
    }

    public override void Open()
    {
        base.Open();

        if (pickerType == EditBoxPickerType.DatePicker)
        {
            openDatePicker();
        }
        else if (pickerType == EditBoxPickerType.List)
        {
            OpenDropDownList();
        }
    }

    public override void Close()
    {
        base.Close();

        if (pickerType == EditBoxPickerType.DatePicker)
        {
            closeDatePicker();
        }
        else if (pickerType == EditBoxPickerType.List)
        {
            closeDropDownList();
        }
    }

    // ...
    private void closeDropDownList ()
	{
		dropDownList.gameObject.SetActive (false);
	}

    // ...
    private void closeDatePicker ()
	{
        datePicker.SetActive(false);
        currentlyOpenedVisualComponent = null;
    }

    //...
    private void openDatePicker()
    {
        if (currentlyOpenedVisualComponent != null)
            currentlyOpenedVisualComponent.Close();

        datePicker.SetActive(true);

        currentlyOpenedVisualComponent = this;

        if (onPickerOpen != null)
            onPickerOpen.Invoke();

        datePicker.GetComponent<WispCalendar>().Style = style;
    }

	/// <summary>
	/// Assign an action to clicking on the Calendar/List open button.
	/// </summary>
	private void OpenButtonOnClick (UnityEngine.Events.UnityAction ParamAction)
	{
		pickerButton.AddOnClickAction(ParamAction);
	}

    /// <summary>
    /// Specify event when an item has been selected.
    /// </summary>
    public void OnItemSelectionChanged(UnityAction ParamAction)
    {
        onItemSelectionChanged += ParamAction;
    }

    /// <summary>
    /// This is what happens by default when you click on an item in the list.
    /// </summary>
    public void listItemOnClick (WispDropDownListItem ParamItem)
	{
        ignoreOnValueChangedFrame = Time.frameCount;
        inputField.text = ParamItem.TextValue;
		hiddenValue = ParamItem.Name;
		closeDropDownList ();
	}

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet in edit mode.
    /// </summary>
    public override void ApplyStyleInEditor()
    {
        TMPro.TMP_InputField tmpInputField = GetComponent<TMPro.TMP_InputField>();

        transform.Find("Label").GetComponent<TMPro.TMP_Text>().ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
        tmpInputField.selectionColor = colop(style.GetSubStyle(WispSubStyleRule.Container).selectedColor);
        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);

        if (tmpInputField.textComponent != null)
            tmpInputField.textComponent.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);

        Button tmpPickerButton = transform.Find("PickerOpener").GetComponent<Button>();

        tmpPickerButton.GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);

        transform.Find("Label").gameObject.SetActive(displayLabel);

        ApplyStyleToChildren();
    }

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet.
    /// </summary>
    public override void ApplyStyle ()
	{
        if (CheckIgnoreApplyStyle(false))
            return;
        
        base.ApplyStyle();

        label.GetComponent<TMPro.TMP_Text>().ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
        inputField.selectionColor = colop(style.GetSubStyle(subStyleRule).selectedBackgroundColor);
        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);

        if (inputField.textComponent != null)
        {
            inputField.textComponent.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
            inputField.placeholder.GetComponent<TMPro.TextMeshProUGUI>().ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
        }

        if (pickerButton != null)
            pickerButton.GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
    }

	/// <summary>
	/// Set the text of the EditBox.
	/// </summary>
	public override void SetValue(string ParamValue)
	{
        ignoreOnValueChangedFrame = Time.frameCount;
        inputField.text = ParamValue;
	}

    /// <summary>
    /// Focus.
    /// </summary>
    public override void Focus()
    {
        base.Focus();
        inputField.Select();
        inputField.ActivateInputField();
    }

    public override void Unfocus()
    {
        base.Unfocus();
        inputField.DeactivateInputField();
    }

    // ...
    private IEnumerator async_select()
    {
        yield return new WaitForEndOfFrame();
        inputField.Select();
        inputField.ActivateInputField();
        WispVisualComponent.FocusedComponent = this;
        yield break;
    }

    /// <summary>
    /// New line.
    /// </summary>
    public void AppendNewLine(string ParamText)
    {
        if (inputField.text.Length == 0)
        {
            inputField.text = ParamText + Environment.NewLine;
            return;
        }

        if (inputField.text.EndsWith(Environment.NewLine))
        {
            inputField.text += ParamText + Environment.NewLine;
        }
        else
        {
            inputField.text += Environment.NewLine + ParamText + Environment.NewLine;
        }
    }

    /// <summary>
    /// Clear.
    /// </summary>
    public void Clear()
    {
        if (pickerType == EditBoxPickerType.None || pickerType == EditBoxPickerType.DatePicker)
            SetValue("");
        else if (pickerType == EditBoxPickerType.List)
            SelectedItem = null;
    }

    // ...
    private void searchDropDownList()
    {
        if (!searchMode)
            return;

        if (ignoreOnValueChangedFrame == Time.frameCount || pickerType != EditBoxPickerType.List)
            return;

        if (searchCoroutine != null)
            StopCoroutine(searchCoroutine);

        searchCoroutine = searchDropDownListItems_Async(inputField.text);
        StartCoroutine(searchCoroutine);
    }

    // ...
    private IEnumerator searchDropDownListItems_Async(string ParamKeyWord)
    {
        OpenDropDownList();

        if (ParamKeyWord == "")
            yield return null;

        WispDropDownList list = dropDownList.GetComponent<WispDropDownList>();

        if (list == null)
            yield return null;

        Dictionary<int, WispDropDownListItem> ParamDictionary = dropDownList.GetComponent<WispDropDownList>().ItemsByIndex;

        // Highest priority for stings that starts with ParamKeyWord
        var queryResult = from instance in ParamDictionary where instance.Value.TextValue.StartsWith(ParamKeyWord) select instance;

        List<int> searchResult = new List<int>();
        Dictionary<int, int> finalResultDictionary = new Dictionary<int, int>();

        if (queryResult.Count() > 0)
        {
            foreach (var instance in queryResult)
                finalResultDictionary.Add(instance.Key, WispString.LevenshteinDistance(instance.Value.TextValue, ParamKeyWord));

        }

        // Second priority for strings that contains ParamKeyWord
        queryResult = from instance in ParamDictionary where instance.Value.TextValue.Contains(ParamKeyWord) select instance;

        if (queryResult.Count() > 0)
        {
            foreach (var instance in queryResult)
                if (!finalResultDictionary.ContainsKey(instance.Key))
                    finalResultDictionary.Add(instance.Key, WispString.LevenshteinDistance(instance.Value.TextValue, ParamKeyWord));
        }

        // Third priority for strings with the lowest Levenshtein distance to ParamKeyWord
        Dictionary<int, int> distances = new Dictionary<int, int>();

        foreach (KeyValuePair<int, WispDropDownListItem> kv in ParamDictionary)
        {
            distances.Add(kv.Key, WispString.LevenshteinDistance(kv.Value.TextValue, ParamKeyWord));
        }

        List<int> sortedDistances = distances.OrderBy(kp => kp.Value).Select(kp => kp.Key).ToList();

        int c = 0;

        foreach (int i in sortedDistances)
        {
            if (!finalResultDictionary.ContainsKey(i))
                finalResultDictionary.Add(i, distances[i]);

            c++;

            if (c == 4)
                break;
        }

        List<int> results = finalResultDictionary.OrderBy(kp => kp.Value).Select(kp => kp.Key).ToList();

        dropDownList.GetComponent<WispDropDownList>().ApplySearchResults(results);

        yield return null;
    }

    private void DropDownListGoUp()
    {
        dropDownList.GetComponent<WispDropDownList> ().SelectUpperItem();
    }

    private void DropDownListGoDown ()
    {
        dropDownList.GetComponent<WispDropDownList>().SelectLowerItem();
    }

    public void SetOnEnterAction(UnityAction ParamAction)
    {
        onEnterAction = ParamAction;
    }
}