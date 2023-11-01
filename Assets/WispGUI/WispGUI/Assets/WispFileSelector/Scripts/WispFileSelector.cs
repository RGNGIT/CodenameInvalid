using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using WispExtensions;

public class WispFileSelector : WispWindow
{
	[SerializeField] private Sprite directoryIcon = null;
	[SerializeField] private Sprite fileIcon = null;

	[Header("Prefabs")]
	[SerializeField] private GameObject itemPrefab = null;

    [Header("Text")]
    [SerializeField] private string title;

	[Header("File selector")]
    [SerializeField] private bool closeOnStart;
	[SerializeField] private bool saveMode = false;
	[SerializeField] private bool allowOverWriteOnSave = false;

	private TMPro.TextMeshProUGUI titleText;
	private WispButton previousDirectoryButton;
	// private WispButton nextDirectoryButton;
	private WispButton okButton;
	private WispButton cancelButton;
	private WispEditBox edtCurrentDirectory;
	private WispEditBox edtCurrentFile;
	private WispEditBox edtExtensionFilter;
	private WispScrollView fileView;
	private List<WispFileSelectorItem> items;
	private string currentDirectoryPath;
	private WispFileSelectorItem currentSelectedItem;
	private UnityAction actionOnConfirm = null;

	private static string lastSelectedFilePath;

    public const string PlayerPrefsLastDirectoryKeyName = "WispFileSelectorLastDirectory";

	public string Title
	{
		get
		{
			return title;
		}

		set
		{
			title = value;
		}
	}

	public String CurrentDirectoryPath
	{
		get
		{
			return currentDirectoryPath;
		}
	}

    public bool SaveMode
    {
        get => saveMode;

        set
        {
            saveMode = value;

            if (saveMode)
                edtCurrentFile.ReadOnly = false;
            else
                edtCurrentFile.ReadOnly = true;
        }
    }

    public WispScrollView FileView { get => fileView; }

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
    {
        if (isInitialized)
            return true;
        
		base.Initialize();

        // ---------------------------------------------------------------------

        titleText = transform.Find("Title").GetComponent<TMPro.TextMeshProUGUI> ();
		previousDirectoryButton = transform.Find("WispButtonPreviousDirectory").GetComponent<WispButton> ();
		// nextDirectoryButton = transform.Find("WispButtonNextDirectory").GetComponent<WispButton> ();
		okButton = transform.Find("WispButtonOK").GetComponent<WispButton> ();
		cancelButton = transform.Find("WispButtonCancel").GetComponent<WispButton> ();
		edtCurrentDirectory = transform.Find("WispEditBoxDirectory").GetComponent<WispEditBox> ();
		edtCurrentFile = transform.Find("WispEditBoxFile").GetComponent<WispEditBox> ();
		edtExtensionFilter = transform.Find("WispEditBoxExtensionFilter").GetComponent<WispEditBox> ();
		fileView = transform.Find ("FileView").GetComponent<WispScrollView>();

        previousDirectoryButton.Initialize();
		// nextDirectoryButton.Initialize();
		okButton.Initialize();
		cancelButton.Initialize();
		edtCurrentDirectory.Initialize();
		edtCurrentFile.Initialize();

        edtExtensionFilter.Initialize();
        edtExtensionFilter.PickerType = WispEditBox.EditBoxPickerType.List;

        previousDirectoryButton.SetParent(this, true);
        // nextDirectoryButton.SetParent(this, true);
        okButton.SetParent(this, true);
        cancelButton.SetParent(this, true);
        edtCurrentDirectory.SetParent(this, true);
        edtCurrentFile.SetParent(this, true);
        edtExtensionFilter.SetParent(this, true);
		fileView.SetParent(this,true);

		okButton.AddOnClickAction(btnOkOnClick);
		cancelButton.AddOnClickAction(btnCancelOnClick);
		previousDirectoryButton.AddOnClickAction(previousDirectory);

		WispKeyBoardEventSystem.AddEventOnKey(btnOkOnClick, okButton, false, KeyCode.Return, KeyCode.KeypadEnter);
		WispKeyBoardEventSystem.AddEventOnKey(btnCancelOnClick, cancelButton, false, KeyCode.Escape);
		WispKeyBoardEventSystem.AddEventOnKey(previousDirectory, previousDirectoryButton, false, KeyCode.Backspace);

		items = new List<WispFileSelectorItem> ();

		if (saveMode)
			edtCurrentFile.ReadOnly = false;

		if (closeOnStart)
			// Close();
			CloseProcedure();
		else
			isOpen = true;

		// ---------------------------------------------------------------------

		isInitialized = true;
        return true;
    }

    // ...
    void Start()
    {
        ApplyStyle();
		fileView.VerticalScrollToPosition(Vector3.zero);
    }

    // ...
    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet.
    /// </summary>
    public override void ApplyStyle ()
	{
        base.ApplyStyle();

		GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);

		titleText.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
	}

    public override void ApplyStyleInEditor()
    {
        titleText = transform.Find("Title").GetComponent<TMPro.TextMeshProUGUI>();
        previousDirectoryButton = transform.Find("WispButtonPreviousDirectory").GetComponent<WispButton>();
        // nextDirectoryButton = transform.Find("WispButtonNextDirectory").GetComponent<WispButton>();
        okButton = transform.Find("WispButtonOK").GetComponent<WispButton>();
        cancelButton = transform.Find("WispButtonCancel").GetComponent<WispButton>();
        edtCurrentDirectory = transform.Find("WispEditBoxDirectory").GetComponent<WispEditBox>();
        edtCurrentFile = transform.Find("WispEditBoxFile").GetComponent<WispEditBox>();
        edtExtensionFilter = transform.Find("WispEditBoxExtensionFilter").GetComponent<WispEditBox>();
        WispScrollView scrollView = transform.Find("FileView").GetComponent<WispScrollView>();

		GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);

        previousDirectoryButton.Style = style;
        // nextDirectoryButton.Style = style;
        okButton.Style = style;
        cancelButton.Style = style;
        edtCurrentDirectory.Style = style;
        edtCurrentFile.Style = style;
        edtExtensionFilter.Style = style;
        scrollView.Style = style;
    }

    /// <summary>
    /// ...
    /// </summary>
    public void Clear ()
	{
		foreach (WispFileSelectorItem item in items)
		{
			Destroy(item.gameObject);
		}

		items.Clear();
	}

	/// <summary>
	/// ...
	/// </summary>
	public void DisplayElementsInDirectory (string ParamDirectoryPath)
	{
		if (!isOpen)
		{
			print("File Selector Not Open !");
			return;
		}
		
		if (!Directory.Exists(ParamDirectoryPath))
		{
			throw new WispInvalidDirectoryException(ParamDirectoryPath);
		}

		string[] directoryPaths;
		string[] filePaths;

		try 
		{
			directoryPaths = Directory.GetDirectories(ParamDirectoryPath);
			filePaths = Directory.GetFiles(ParamDirectoryPath);
		}
		catch (Exception e) 
		{
			throw e;
		}

		Clear();
		currentDirectoryPath = ParamDirectoryPath;
		edtCurrentDirectory.SetValue(ParamDirectoryPath);

		char[] delimiterChars = { '/', '\\'};

		foreach (string s in directoryPaths)
		{
			WispFileSelectorItem item = addItem();
			string[] words = s.Split(delimiterChars);
			item.Text = words[words.Length-1];
			item.SetIcon(directoryIcon);
			item.IsDirectory =  true;
		}

		foreach (string s in filePaths)
		{
			WispFileSelectorItem item = addItem();
			item.Text = Path.GetFileName(s);
			item.SetIcon(fileIcon);
		}
	}

	// ...
	private WispFileSelectorItem addItem()
	{
		WispFileSelectorItem item = Instantiate(itemPrefab, fileView.ContentRect).GetComponent<WispFileSelectorItem> ();
		item.Initialize();

		items.Add (item);

		float y = style.VerticalPadding + items.Count * itemPrefab.GetComponent<RectTransform> ().sizeDelta.y + (style.ButtonVerticalSpacing*(items.Count-1));
		item.Index = items.Count;
		item.ParentFileSelector = this;

		item.MyRectTransform.anchoredPosition3D = new Vector3 (style.HorizontalPadding, y * (-1), 0);
        item.SetParent(fileView, true);

		fileView.ScrollRect.content.sizeDelta = new Vector2 (fileView.ScrollRect.content.sizeDelta.x, y);

		fileView.ScrollRect.CalculateLayoutInputHorizontal ();
		fileView.ScrollRect.CalculateLayoutInputVertical ();

		return item;
	}

	// ...
	public void SelectItem (WispFileSelectorItem ParamItem)
	{
		if (currentSelectedItem != null)
		{
			currentSelectedItem.ApplyStyleUnSelected();
		}

		if (saveMode && !allowOverWriteOnSave)
		{
			// Do Nothing
		}
		else
		{
			ParamItem.ApplyStyleSelected();
			currentSelectedItem = ParamItem;
			edtCurrentFile.SetValue(ParamItem.Text);
			lastSelectedFilePath = currentDirectoryPath + "/" + ParamItem.Text;
		}
	}

	// ...
	public float ViewportWidth()
	{
		return fileView.ScrollRect.GetComponent<RectTransform> ().rect.width;
	}

	// ...
	private class WispInvalidDirectoryException : Exception
	{
		public WispInvalidDirectoryException()
		{

		}

		public WispInvalidDirectoryException(string ParamDirectory) : base(String.Format("Invalid Directory : {0}", ParamDirectory))
		{

		}
	}

	// ...
	private void previousDirectory()
	{
		DirectoryInfo dirInfo = Directory.GetParent(currentDirectoryPath);
		
		if (dirInfo != null)
			DisplayElementsInDirectory(dirInfo.FullName);
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
            Destroy(gameObject);
        }

        isOpen = false;

        titleText.gameObject.SetActive(false);
        previousDirectoryButton.gameObject.SetActive(false);
        // nextDirectoryButton.gameObject.SetActive(false);
        okButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        edtCurrentDirectory.gameObject.SetActive(false);
        edtCurrentFile.gameObject.SetActive(false);
        edtExtensionFilter.gameObject.SetActive(false);
        fileView.gameObject.SetActive(false);
        GetComponent<Image>().enabled = false;

        if (enableBackgroundObstructor) getBackgroundObstructor().SetActive(false);
    }

    public void Open(string ParamDirectoryPath, UnityEngine.Events.UnityAction ParamAction)
	{
		base.Open();
		
		isOpen = true;
		
		titleText.gameObject.SetActive(true);
		previousDirectoryButton.gameObject.SetActive(true);
		okButton.gameObject.SetActive(true);
		cancelButton.gameObject.SetActive(true);
		edtCurrentDirectory.gameObject.SetActive(true);
		edtCurrentFile.gameObject.SetActive(true);
		edtExtensionFilter.gameObject.SetActive(true);
		fileView.gameObject.SetActive(true);
		GetComponent<Image> ().enabled = true;

		actionOnConfirm = ParamAction;

        string path = "";
        if (ParamDirectoryPath == "")
        {
            path = GetLastOrDefaultDirectoryPath();
        }
        else
        {
            path = ParamDirectoryPath;
        }

        DisplayElementsInDirectory(path);
	}

	private void btnOkOnClick()
	{
        lastSelectedFilePath = currentDirectoryPath + "/" + edtCurrentFile.GetValue();

        if (saveMode)
        {
			if (edtExtensionFilter.Base.text != "")
			{
				string ext = Path.GetExtension(lastSelectedFilePath);

				// This makes sure that the file name has no duplicate extension at the end.
				if ("." + edtExtensionFilter.Base.text != ext)
                	lastSelectedFilePath = lastSelectedFilePath + "." + edtExtensionFilter.Base.text;
			}

        }

        PlayerPrefs.SetString(PlayerPrefsLastDirectoryKeyName, currentDirectoryPath);

		if (actionOnConfirm != null)
			actionOnConfirm.Invoke();

        Close();
	}

	private void btnCancelOnClick()
	{
		lastSelectedFilePath = null;
		Close();
	}

	public static WispFileSelector OpenAuto(string ParamDirectoryPath, string ParamDefaultExtension, UnityEngine.Events.UnityAction ParamAction, bool ParamSaveMode, bool ParamAllowOverWrite, Canvas ParamCanvas = null)
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

		GameObject go = Instantiate(library.FileSelector, tmpCanvas.transform);
        WispFileSelector selector = go.GetComponent<WispFileSelector> ();
		selector.closeOnStart = false;
		selector.destroyOnClose = true;
		selector.allowOverWriteOnSave = ParamAllowOverWrite;
		selector.SaveMode = ParamSaveMode;
        selector.edtExtensionFilter.SetValue(ParamDefaultExtension);
		selector.Open(ParamDirectoryPath, ParamAction);

        return selector;
	}

	static public string GetLastSelectedFilePath()
	{
		return lastSelectedFilePath;
	}

    public void SetExtensionList(List<string> ParamExtensions)
    {
        edtExtensionFilter.Clear();
        edtExtensionFilter.DropDownList.Clear();

        foreach(string s in ParamExtensions)
        {
            edtExtensionFilter.DropDownList.AddItem(s, s);
        }

        if (ParamExtensions.Count > 1)
            edtExtensionFilter.DropDownList.SetSelectedItemByIndex(0);
    }

    public static string GetLastOrDefaultDirectoryPath()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsLastDirectoryKeyName))
            return PlayerPrefs.GetString(PlayerPrefsLastDirectoryKeyName);
        else
            return Directory.GetCurrentDirectory();
    }

    // ...
    public static WispFileSelector Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.FileSelector, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.FileSelector);
        }

        return go.GetComponent<WispFileSelector>();
    }
}