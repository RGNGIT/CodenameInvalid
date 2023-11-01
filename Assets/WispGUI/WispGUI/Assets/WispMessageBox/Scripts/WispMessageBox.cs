using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WispExtensions;

public class WispMessageBox : WispWindow 
{
	[Header("Message")]
	[SerializeField] private string message;

	[Header("Button 1")]
	[SerializeField] private bool buttonOneEnabled;
	[SerializeField] private string buttonOneText;
	[SerializeField] private bool buttonOneCloseOnClick = true;

	[Header("Button 2")]
	[SerializeField] private bool buttonTwoEnabled;
	[SerializeField] private string buttonTwoText;
	[SerializeField] private bool buttonTwoCloseOnClick = true;

	[Header("Button 3")]
	[SerializeField] private bool buttonThreeEnabled;
	[SerializeField] private string buttonThreeText;
	[SerializeField] private bool buttonThreeCloseOnClick = true;

	private WispButton buttonOne;
	private WispButton buttonTwo;
	private WispButton buttonThree;
	private TMPro.TextMeshProUGUI messageTextComponent;

    public string Message
    {
        get
        {
            return message;
        }
        set
        {
            message = value;
            messageTextComponent.text = message;

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

		// ---------------------------------------------------------------------
		
		buttonOne = transform.Find("WispButtonOne").GetComponent<WispButton> ();
		buttonTwo = transform.Find("WispButtonTwo").GetComponent<WispButton> ();
		buttonThree = transform.Find("WispButtonThree").GetComponent<WispButton> ();
		messageTextComponent = transform.Find("Message").GetComponent<TMPro.TextMeshProUGUI> ();

		buttonOne.Initialize();
		buttonTwo.Initialize();
		buttonThree.Initialize();

		buttonOne.AddOnClickAction(btnOneOnClick);
		buttonTwo.AddOnClickAction(btnTwoOnClick);
		buttonThree.AddOnClickAction(btnThreeOnClick);
		messageTextComponent.text = message;

        UpdateButtons();
		
		// ---------------------------------------------------------------------

		isInitialized = true;

        return true;
    }

    // Use this for initialization
    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        ApplyStyle();
    }

    // ...
    private byte countEnabledButtons ()
	{
		byte counter = 0;

		if (buttonOneEnabled)
			counter++;

		if (buttonTwoEnabled)
			counter++;

		if (buttonThreeEnabled)
			counter++;

		return counter;
	}

	/// <summary>
	/// Apply Color and graphics modifications from the style sheet.
	/// </summary>
	public override void ApplyStyle ()
	{
        base.ApplyStyle();

        GetComponent<Image> ().ApplyStyle(style, Opacity, subStyleRule);
        messageTextComponent.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
	}

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet.
    /// </summary>
    public override void ApplyStyleInEditor()
    {
        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);

        buttonOne = transform.Find("WispButtonOne").GetComponent<WispButton>();
        buttonTwo = transform.Find("WispButtonTwo").GetComponent<WispButton>();
        buttonThree = transform.Find("WispButtonThree").GetComponent<WispButton>();
        messageTextComponent = transform.Find("Message").GetComponent<TMPro.TextMeshProUGUI>();

        buttonOne.Style = style;
        buttonTwo.Style = style;
        buttonThree.Style = style;

        messageTextComponent.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
    }

    // ...
    public override void Close()
    {
        base.Close();
        
        if (useVisualTransition)
        {
            SetActiveUsingTransition(false, CloseProcedure);
        }
        else
        {
            CloseProcedure();
        }
    }

    private void CloseProcedure()
    {
        if (destroyOnClose)
        {
            Destroy(getBackgroundObstructor());
            buttonOne.UnregisterFromVisualComponentList();
            buttonTwo.UnregisterFromVisualComponentList();
            buttonThree.UnregisterFromVisualComponentList();
            Destroy(gameObject);
        }

        isOpen = false;

        if (enableBackgroundObstructor) getBackgroundObstructor().SetActive(false);
    }

    // ...
    private void btnOneOnClick()
	{
		if (buttonOneCloseOnClick)
			Close();
	}

	// ...
	private void btnTwoOnClick()
	{
		if (buttonTwoCloseOnClick)
			Close();
	}

	// ...
	private void btnThreeOnClick()
	{
		if (buttonThreeCloseOnClick)
			Close();
	}

	// ...
	public override void Open ()
	{
        base.Open();

        buttonOne.BorderRule = WispBorderRule.IfFocused;
        buttonOne.Focus();
        // WispVisualComponent.MakeAvailableForInput(null);
        // IsAvailableForInput = true;
	}

    // ...
    public static WispMessageBox OpenNoButtonDialog(string ParamMessage, Canvas ParamCanvas = null)
    {
        WispMessageBox msgBox = WispMessageBox.Create(WispVisualComponent.GetMainCanvas().transform);
        msgBox.Message = ParamMessage;
        msgBox.buttonOneEnabled = false;
        msgBox.buttonTwoEnabled = false;
        msgBox.buttonThreeEnabled = false;
        msgBox.UpdateButtons();
        
        msgBox.Open();

        return msgBox;
    }

    // ...
    public static WispMessageBox OpenOneButtonDialog(string ParamMessage, string ParamButtonOneText, UnityEngine.Events.UnityAction ParamButtonOneAction, Canvas ParamCanvas = null)
	{
		WispPrefabLibrary library = Resources.Load<WispPrefabLibrary> ("Default Prefab Library");

		Canvas tmpCanvas;
		if (ParamCanvas == null)
		{
			tmpCanvas = WispVisualComponent.GetMainCanvas();
		}
		else
		{
			tmpCanvas = ParamCanvas;
		}

		GameObject go = Instantiate(library.MessageBox, tmpCanvas.transform);
		WispMessageBox msgBox = go.GetComponent<WispMessageBox> ();
		msgBox.Message = ParamMessage;
        msgBox.buttonOneEnabled = true;
        msgBox.buttonTwoEnabled = false;
        msgBox.buttonThreeEnabled = false;
        msgBox.UpdateButtons();

        if (ParamButtonOneAction != null)
            msgBox.buttonOne.AddOnClickAction(ParamButtonOneAction);

        msgBox.buttonOne.SetValue(ParamButtonOneText);
        WispKeyBoardEventSystem.AddEventOnKey(KeyCode.Return, msgBox.runButtonOnClickAction, msgBox.buttonOne);
        WispKeyBoardEventSystem.AddEventOnKey(KeyCode.KeypadEnter, msgBox.runButtonOnClickAction, msgBox.buttonOne);

        msgBox.Open();

        return msgBox;
	}

    private void runButtonOnClickAction()
    {
        buttonOne.Base.onClick.Invoke();
    }

    // ...
    public static WispMessageBox OpenTwoButtonsDialog(string ParamMessage, string ParamButtonOneText, UnityAction ParamButtonOneAction, string ParamButtonTwoText, UnityAction ParamButtonTwoAction, Canvas ParamCanvas = null)
	{
		WispPrefabLibrary library = Resources.Load<WispPrefabLibrary> ("Default Prefab Library");

		Canvas tmpCanvas;
		if (ParamCanvas == null)
		{
			tmpCanvas = WispVisualComponent.GetMainCanvas();
		}
		else
		{
			tmpCanvas = ParamCanvas;
		}

		GameObject go = Instantiate(library.MessageBox, tmpCanvas.transform);
		WispMessageBox msgBox = go.GetComponent<WispMessageBox> ();
		msgBox.Message = ParamMessage;
		msgBox.buttonOneEnabled = true;
		msgBox.buttonTwoEnabled = true;
		msgBox.buttonThreeEnabled = false;
        msgBox.UpdateButtons();
        
        if (ParamButtonOneAction != null)
            msgBox.buttonOne.AddOnClickAction(ParamButtonOneAction);

        if (ParamButtonTwoAction != null)
            msgBox.buttonTwo.AddOnClickAction(ParamButtonTwoAction);

        msgBox.buttonOne.SetValue(ParamButtonOneText);
        msgBox.buttonTwo.SetValue(ParamButtonTwoText);

        msgBox.Open();

        return msgBox;
	}

	// ...
	public static WispMessageBox OpenThreeButtonsDialog(string ParamMessage, string ParamButtonOneText, UnityAction ParamButtonOneAction, string ParamButtonTwoText, UnityAction ParamButtonTwoAction, string ParamButtonThreeText, UnityAction ParamButtonThreeAction, Canvas ParamCanvas = null)
	{
		WispPrefabLibrary library = Resources.Load<WispPrefabLibrary> ("Default Prefab Library");

		Canvas tmpCanvas;
		if (ParamCanvas == null)
		{
			tmpCanvas = WispVisualComponent.GetMainCanvas();
		}
		else
		{
			tmpCanvas = ParamCanvas;
		}

		GameObject go = Instantiate(library.MessageBox, tmpCanvas.transform);
		WispMessageBox msgBox = go.GetComponent<WispMessageBox> ();
		msgBox.message = ParamMessage;
		msgBox.buttonOneEnabled = true;
		msgBox.buttonTwoEnabled = true;
		msgBox.buttonThreeEnabled = true;
        msgBox.UpdateButtons();

        if (ParamButtonOneAction != null)
            msgBox.buttonOne.AddOnClickAction(ParamButtonOneAction);

        if (ParamButtonTwoAction != null)
            msgBox.buttonTwo.AddOnClickAction(ParamButtonTwoAction);

        if (ParamButtonThreeAction != null)
            msgBox.buttonThree.AddOnClickAction(ParamButtonThreeAction);

        msgBox.buttonOne.SetValue(ParamButtonOneText);
        msgBox.buttonTwo.SetValue(ParamButtonTwoText);
        msgBox.buttonThree.SetValue(ParamButtonThreeText);

        msgBox.Open();

        return msgBox;
	}

    public void UpdateButtons()
    {
        byte enabledButtonsCounter = countEnabledButtons();

        if (enabledButtonsCounter == 0)
        {
            buttonOne.gameObject.SetActive(false);
            buttonTwo.gameObject.SetActive(false);
            buttonThree.gameObject.SetActive(false);
        }
        else if (enabledButtonsCounter == 1)
        {
            if (buttonOneEnabled)
            {
                buttonTwo.gameObject.SetActive(false);
                buttonThree.gameObject.SetActive(false);

                buttonOne.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, buttonOne.GetComponent<RectTransform>().anchoredPosition.y);
            }
            else if (buttonTwoEnabled)
            {
                buttonOne.gameObject.SetActive(false);
                buttonThree.gameObject.SetActive(false);

                buttonTwo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, buttonTwo.GetComponent<RectTransform>().anchoredPosition.y);
            }
            else if (buttonThreeEnabled)
            {
                buttonOne.gameObject.SetActive(false);
                buttonTwo.gameObject.SetActive(false);

                buttonThree.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, buttonThree.GetComponent<RectTransform>().anchoredPosition.y);
            }
        }
        else if (enabledButtonsCounter == 2)
        {
            if (!buttonOneEnabled)
            {
                buttonOne.gameObject.SetActive(false);
                buttonTwo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-40, buttonTwo.GetComponent<RectTransform>().anchoredPosition.y);
                buttonThree.GetComponent<RectTransform>().anchoredPosition = new Vector2(+40, buttonThree.GetComponent<RectTransform>().anchoredPosition.y);
            }
            else if (!buttonTwoEnabled)
            {
                buttonTwo.gameObject.SetActive(false);
                buttonOne.GetComponent<RectTransform>().anchoredPosition = new Vector2(-40, buttonOne.GetComponent<RectTransform>().anchoredPosition.y);
                buttonThree.GetComponent<RectTransform>().anchoredPosition = new Vector2(+40, buttonThree.GetComponent<RectTransform>().anchoredPosition.y);
            }
            else if (!buttonThreeEnabled)
            {
                buttonThree.gameObject.SetActive(false);
                buttonOne.GetComponent<RectTransform>().anchoredPosition = new Vector2(-40, buttonOne.GetComponent<RectTransform>().anchoredPosition.y);
                buttonTwo.GetComponent<RectTransform>().anchoredPosition = new Vector2(+40, buttonTwo.GetComponent<RectTransform>().anchoredPosition.y);
            }
        }
        else if (enabledButtonsCounter == 3)
        {
            buttonOne.gameObject.SetActive(true);
            buttonTwo.gameObject.SetActive(true);
            buttonThree.gameObject.SetActive(true);
        }

        buttonOne.SetParent(this, true);
        buttonTwo.SetParent(this, true);
        buttonThree.SetParent(this, true);
    }

    // ...
    public static WispMessageBox Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.MessageBox, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.MessageBox);
        }

        return go.GetComponent<WispMessageBox>();
    }
}
