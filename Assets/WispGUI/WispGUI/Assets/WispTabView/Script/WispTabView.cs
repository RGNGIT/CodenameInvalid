using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WispExtensions;

public class WispTabView : WispVisualComponent 
{
    [Header("Prefabs")]
    [SerializeField] private GameObject tabButtonPrefab;
    [SerializeField] private GameObject pagePrefab;

    [Header("Tabs")]
    [SerializeField] private float defaultTabSize = 256;
    
    [Header("Tabview Settings")]
    [SerializeField] private string cornerButtontext = "X";

    private Dictionary<string, WispPage> pages = new Dictionary<string, WispPage>();
	private float currentTabSize;
    private RectTransform tabPanelTransform;
    private UnityEvent onCornerButtonClick;
    private WispPage currentShownPage;
    private int currentPageIndex = 1;
    private WispButton cornerButton;

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        // ---------------------------------------------------------------------

        tabPanelTransform = transform.Find("TabButtonPanel").GetComponent<RectTransform>();
        AddCornerButton();

        // ---------------------------------------------------------------------

        isInitialized = true;

        return true;
    }

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        ApplyStyle();
    }

    // ...
    public WispPage AddPage(string ParamID, string ParamLabel,  bool ParamEnableCloseButton = true)
	{
        // If a page with the same ID exists open that page
        if (pages.ContainsKey(ParamID))
        {
            ShowPage(ParamID);
            return null;
        }

        GameObject btnGo = Instantiate (tabButtonPrefab) as GameObject;
		btnGo.transform.SetParent (tabPanelTransform);
		btnGo.name = "Tab( " + ParamLabel + " )";

        WispTabButton tabButton = btnGo.AddComponent<WispTabButton> ();

        WispButton btn = tabButton.GetComponent<WispButton>();
        btn.SetParent(this, true);

        btn.ForceAutoSize = false;
        btn.TextComponent.enableAutoSizing = false;
        btn.TextComponent.fontSize = style.FontSizeNormal;
        btn.TextComponent.overflowMode = TMPro.TextOverflowModes.Ellipsis;
        btn.SetTooltipText(ParamLabel, "");
        btn.TooltipConfiguration.fadeDelay = 0.25f;

        GameObject pageGo = Instantiate(pagePrefab, transform);

        WispPage page = pageGo.GetComponent<WispPage>();
        tabButton.Page = page;

        page.Name = ParamID;
        page.PageID = ParamID;
        page.SetParent(this, true, true);
        page.TabManager = this;
        page.TabButton = tabButton;
        page.AnchorStyleExpanded(true);
        page.SetTop(32);
        page.Index = currentPageIndex;
        currentPageIndex++;
    
        if (ParamEnableCloseButton)
        {
            tabButton.AddCloseButton();
        }

        pages.Add (ParamID, page);

		CalculateTabSize ();

		RectTransform tmpRect = btnGo.GetComponent<RectTransform> ();
		tmpRect.anchorMin = new Vector2 (0, 1);
		tmpRect.anchorMax = new Vector2 (0, 1);
		tmpRect.pivot = new Vector2 (0, 1);
        tmpRect.localPosition = new Vector2(0, 0);
        tmpRect.sizeDelta = new Vector2(defaultTabSize, 32);

		btnGo.GetComponent<WispButton> ().SetValue (ParamLabel);

		ArrangeTabs ();

        ShowPage(page.Name);

        return page;
	}

    public void ShowPage(string ParamID)
    {
        if (pages[ParamID] != null)
        {
            // If not target
            if (currentShownPage != null)
            {
                currentShownPage.Visible = false;
                currentShownPage.IsAvailableForInput = false;
                currentShownPage.TabButton.ChangeState(false);

                pages[ParamID].TabButton.PreviousPage = currentShownPage;
            }

            // If target
            pages[ParamID].Visible = true;
            pages[ParamID].IsAvailableForInput = true;
            pages[ParamID].TabButton.ChangeState(true);

            currentShownPage = pages[ParamID];
        }
        else
        {
            WispVisualComponent.LogError("Invalid WispPage ID.");
            return;
        }
    }

	// ...
	protected void CalculateTabSize ()
	{
		float parentWidth = GetComponent<RectTransform> ().rect.width - style.HorizontalPadding;
		float prefabTabBtnWidth = WispPrefabLibrary.Default.Button.GetComponent<RectTransform> ().rect.width;

		float widthOfAllButtons = prefabTabBtnWidth * pages.Count;

		if (widthOfAllButtons > parentWidth) 
        {
			currentTabSize = parentWidth / pages.Count;
		} 
        else 
        {
			currentTabSize = prefabTabBtnWidth;
		}
	}

	// ...
	protected void ArrangeTabs()
	{
		CalculateTabSize ();

		foreach (KeyValuePair<string, WispPage> kv in pages)
        {
            WispButton btn = kv.Value.TabButton.Button;

            float x = (currentTabSize * (kv.Value.Index - 1)) + style.HorizontalPadding;
            btn.SetPositionAsync(new Vector3(x, 0, 0), 0.05f);
            btn.MyRectTransform.sizeDelta = new Vector2 (currentTabSize, kv.Value.TabButton.Button.MyRectTransform.rect.height);
		}
	}

    public void ClosePage (string ParamPageID)
    {
        if (pages[ParamPageID] == null)
            return;

        // Showing previous tab
        if (pages[ParamPageID].TabButton.PreviousPage != null && pages[ParamPageID].TabManager.currentShownPage == pages[ParamPageID])
            ShowPage(pages[ParamPageID].TabButton.PreviousPage.Name);

        // Removing the page, it's tab button and their game objects
        Destroy(pages[ParamPageID].TabButton.gameObject);
        pages[ParamPageID].OnClose.Invoke();
        Destroy(pages[ParamPageID].gameObject);

        int index = pages[ParamPageID].Index; // Remember the index before removing the page.
        pages.Remove(ParamPageID);

        // Correcting Indexes
        foreach(KeyValuePair<string, WispPage> kv in pages)
        {
            if (kv.Value.Index > index)
            {
                kv.Value.Index--;
            }
        }

        UpdateCurrentPageIndex();
        ArrangeTabs();
    }

    private void UpdateCurrentPageIndex()
    {
        currentPageIndex = 1;

        if (pages.Count > 0)
        {
            foreach (KeyValuePair<string, WispPage> kvp in pages)
            {
                currentPageIndex = Math.Max(currentPageIndex, kvp.Value.Index);
            }
            currentPageIndex++;
        }
    }

	// ...
	private void AddCornerButton ()
	{
        // cornerButton = (Instantiate (WispPrefabLibrary.Default.Button, tabPanelTransform) as GameObject).GetComponent<WispButton>();
        cornerButton = WispButton.Create(tabPanelTransform);
        cornerButton.SetParent(this, true);

        cornerButton.SetValue(cornerButtontext);
        cornerButton.AddOnClickAction(OpenMenu);

        RectTransform tmpRect = cornerButton.GetComponent<RectTransform>();
        tmpRect.anchorMin = new Vector2(1, 1);
        tmpRect.anchorMax = new Vector2(1, 1);
        tmpRect.pivot = new Vector2(1, 1);
        
        UpdateCornerButtonPosition();
	}

    private void UpdateCornerButtonPosition()
    {
        cornerButton.MyRectTransform.anchoredPosition = new Vector2(-style.HorizontalPadding, -style.VerticalPadding);
        cornerButton.MyRectTransform.sizeDelta = new Vector2(32-style.HorizontalPadding*2, 32-style.VerticalPadding*2);
    }

	// ...
	protected void OpenMenu ()
	{
        if (onCornerButtonClick != null)
            onCornerButtonClick.Invoke();
	}

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public void PositionAway(RectTransform ParamTransform)
    {
        ParamTransform.localPosition = new Vector3(-4096, -4096, 0);
    }

    public void AddCornerButtonClickEvent(UnityAction ParamAction)
    {
        if (onCornerButtonClick == null)
            onCornerButtonClick = new UnityEvent();

        onCornerButtonClick.AddListener(ParamAction);
    }

    public override void ApplyStyle()
    {
        base.ApplyStyle();

        tabPanelTransform.GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);

        // currentShownPage.TabButton.ChangeState(true); // Does not work because ApplyStyle() from WispButton will override the color.
        StartCoroutine(UpdateTabButtonsStates()); // <--- Update states one frame after ApplyStyle().
    }

    private IEnumerator UpdateTabButtonsStates()
    {
        yield return new WaitForEndOfFrame();

        foreach(KeyValuePair<string, WispPage> kvp in pages)
        {
            if (kvp.Value == currentShownPage)
            {
                kvp.Value.TabButton.ChangeState(true);
            }
            else
            {
                kvp.Value.TabButton.ChangeState(false);
            }

            kvp.Value.TabButton.Button.Set_Y_Position(-style.VerticalPadding);
            kvp.Value.TabButton.Button.Height = tabPanelTransform.rect.height - style.VerticalPadding*2;
        }

        UpdateCornerButtonPosition();
    }

    public WispPage GetPage(string ParamPageID)
    {
        if (pages.ContainsKey(ParamPageID))
        {
            return pages[ParamPageID];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// ...
    /// </summary>
    /// 
    public static WispTabView Create(Transform ParamTransform)
    {
        GameObject go;

        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.TabView, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.TabView);
        }

        return go.GetComponent<WispTabView>();
    }
}
