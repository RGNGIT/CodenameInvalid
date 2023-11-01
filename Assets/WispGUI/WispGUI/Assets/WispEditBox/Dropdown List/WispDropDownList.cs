using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispDropDownList : WispScrollView {

	private Dictionary<string, WispDropDownListItem> itemsByIdentifier;
    private Dictionary<int, WispDropDownListItem> itemsByIndex;
	private WispEditBox parentEditBox;
	private bool selectOnItemClick = true;
	private bool closeOnItemClick = true;

	public bool SelectOnItemClick {
		get {
			return selectOnItemClick;
		}
		set {
			selectOnItemClick = value;
		}
	}

	public bool CloseOnItemClick {
		get {
			return closeOnItemClick;
		}
		set {
			closeOnItemClick = value;
		}
	}

	public WispEditBox ParentEditBox {
		get {
			return parentEditBox;
		}
		set {
			parentEditBox = value;
		}
	}

    public Dictionary<string, WispDropDownListItem> ItemsByIdentifier { get => itemsByIdentifier; }
    public Dictionary<int, WispDropDownListItem> ItemsByIndex { get => itemsByIndex; }

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
	{
        if (isInitialized)
			return true;

        base.Initialize();

        itemsByIdentifier = new Dictionary<string, WispDropDownListItem> ();
        itemsByIndex = new Dictionary<int, WispDropDownListItem>();

        scrollRect = GetComponent<WispScrollRect> ();
		contentRect = transform.Find ("Viewport").Find ("Content").GetComponent<RectTransform> ();

        WispVisualComponent.AttachCanvas(this, 137, true);

		isInitialized = true;

        return true;
	}

	// Use this for initialization
	void Start ()
    {
        ApplyStyle();
	}

    /// <summary>
    /// Awake
    /// </summary>
    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Add an item in the DropDown List, returns null if an item with the same identifier already exists.
    /// </summary>
    public WispDropDownListItem AddItem (string ParamIdentifier, string ParamText)
	{
        if (itemsByIdentifier.ContainsKey(ParamIdentifier))
        {
            WispVisualComponent.LogError("An item identified as " + ParamIdentifier + " already exists in the DropDown List.");
            return null;
        }

		WispDropDownListItem itemComponent = Instantiate(parentEditBox.DropdownListItemPrefab, contentRect).GetComponent<WispDropDownListItem> ();
        itemComponent.Initialize();
		itemsByIdentifier.Add (ParamIdentifier, itemComponent);

        itemComponent.TextValue = ParamText;

        // float y = itemsByIdentifier.Count * parentEditBox.DropdownListItemPrefab.GetComponent<RectTransform> ().sizeDelta.y;
        // float y = (itemsByIdentifier.Count * parentEditBox.DropdownListItemPrefab.GetComponent<RectTransform> ().rect.height) - style.VerticalPadding;
        float y = (itemsByIdentifier.Count * ParentEditBox.ItemHeight) + style.VerticalPadding;
		itemComponent.Index = itemsByIdentifier.Count;
        itemsByIndex.Add(itemComponent.Index, itemComponent);
		itemComponent.ParentList = this;
        itemComponent.Name = ParamIdentifier;
        
        itemComponent.Width = rectTransform.rect.width - style.HorizontalPadding*2;
        itemComponent.Height = ParentEditBox.ItemHeight;

		itemComponent.MyRectTransform.anchoredPosition3D = new Vector3 (style.HorizontalPadding, y * (-1), 0);
        itemComponent.MyRectTransform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (parentEditBox.GetComponent<RectTransform> ().sizeDelta.x - scrollRect.verticalScrollbar.GetComponent<RectTransform> ().sizeDelta.x, itemComponent.MyRectTransform.sizeDelta.y);
        itemComponent.SetParent(this, true);

		contentRect.sizeDelta = new Vector2 (contentRect.sizeDelta.x, y);

		scrollRect.CalculateLayoutInputHorizontal ();
		scrollRect.CalculateLayoutInputVertical ();

		return itemComponent;
	}

	/// <summary>
	/// Add a new item to the list.
	/// </summary>
	public void Clear ()
	{
		foreach (KeyValuePair<string,WispDropDownListItem> kv in itemsByIdentifier)
        {
			Destroy (kv.Value.gameObject);
		}

		itemsByIdentifier.Clear ();
        itemsByIndex.Clear();
	}

    /*
	/// <summary>
	/// Apply Color and graphics modifications from the style sheet.
	/// </summary>
	public override void ApplyStyle ()
	{
		base.ApplyStyle();
	}
    */

    /// <summary>
    /// Return the index of the selected item.
    /// </summary>
    public int GetSelectedItemIndex ()
	{
		return parentEditBox.SelectedItem.Index;
	}

	/// <summary>
	/// Return the index of the selected item.
	/// </summary>
	public void SetSelectedItemByIndex (int ParamIndex)
	{
        if (itemsByIndex.ContainsKey(ParamIndex))
            parentEditBox.SelectedItem = itemsByIndex[ParamIndex];
        else
        {
            WispVisualComponent.LogError("WispDropDownList : Invalid item index(" + ParamIndex + ") while trying to select item.");

            int defaultToThis = -1;
            foreach(KeyValuePair<int, WispDropDownListItem> kvp in itemsByIndex)
            {
                defaultToThis = kvp.Key;
            }

            if (defaultToThis != -1)
            {
                WispVisualComponent.LogWarning("WispDropDownList : Defaulting to " + defaultToThis);
                parentEditBox.SelectedItem = itemsByIndex[defaultToThis];
            }
        }
    }

    /// <summary>
	/// Update items positions by visibility and search result score.
	/// </summary>
    public override void UpdatePositions()
    {
        foreach (KeyValuePair<int, WispDropDownListItem> kv in itemsByIndex)
        {
            if (kv.Value.IsVisible)
                kv.Value.gameObject.SetActive(true);

            float y = kv.Value.Index * parentEditBox.DropdownListItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
            kv.Value.MyRectTransform.anchoredPosition = new Vector3(0, y * (-1), 0);
        }
    }

    //...
    private void RebuildIndexes(int ParamStart)
    {
        int c = ParamStart;
        
        foreach (KeyValuePair<string, WispDropDownListItem> kv in itemsByIdentifier)
        {
            if (kv.Value.Index == -1)
            {
                kv.Value.Index = c;
            }

            c++;
        }

        itemsByIndex.Clear();

        foreach (KeyValuePair<string, WispDropDownListItem> kv in itemsByIdentifier)
        {
            itemsByIndex.Add(kv.Value.Index, kv.Value);
        }
    }

    // ...
    public void ApplySearchResults(List<int> ParamResults)
    {
        foreach(KeyValuePair<int,WispDropDownListItem> kv in itemsByIndex)
        {
            kv.Value.IsVisible = false;
            kv.Value.gameObject.SetActive(false);
            kv.Value.Index = -1;
        }

        int c = 1;
        foreach(int i in ParamResults)
        {
            if (itemsByIndex[i] != null)
            {
                itemsByIndex[i].IsVisible = true;
                itemsByIndex[i].Index = c;
            }

            c++;
        }

        RebuildIndexes(c);
        UpdatePositions();
    }

    // Swap if item at new index exists.
    private void ChangeItemIndex(int ParamItemIndex, int ParamNewIndex)
    {
        if (itemsByIndex[ParamItemIndex] != null)
        {
            WispDropDownListItem item = itemsByIndex[ParamItemIndex];

            if (/*itemsByIndex[ParamNewIndex] != null*/ itemsByIndex.ContainsKey(ParamNewIndex))
            {
                // Swap
                itemsByIndex[ParamItemIndex] = itemsByIndex[ParamNewIndex];
                itemsByIndex[ParamNewIndex] = item;

                itemsByIndex[ParamItemIndex].Index = ParamItemIndex;
                item.Index = ParamNewIndex;
            }
            else
            {
                // Add item to dictionary with new index
                item.Index = ParamNewIndex;
                itemsByIndex.Remove(ParamItemIndex);
                itemsByIndex.Add(ParamNewIndex, item);
            }
        }
        else
        {
            WispVisualComponent.LogError("No item at index " + ParamItemIndex);
        }
    }

    // ...
    public void SelectUpperItem()
    {
        int currentIndex = 0;

        if (ParentEditBox.SelectedItem != null)
            currentIndex = ParentEditBox.SelectedItem.Index;
        else
            ParentEditBox.SelectedItem = itemsByIndex[1];

        if (itemsByIndex.ContainsKey(currentIndex-1))
        {
            ParentEditBox.SelectedItem = itemsByIndex[currentIndex - 1];
        }
    }

    // ...
    public void SelectLowerItem()
    {
        int currentIndex = 0;

        if (ParentEditBox.SelectedItem != null)
            currentIndex = ParentEditBox.SelectedItem.Index;
        else
            ParentEditBox.SelectedItem = itemsByIndex[1];

        if (itemsByIndex.ContainsKey(currentIndex + 1))
        {
            ParentEditBox.SelectedItem = itemsByIndex[currentIndex + 1];
        }
    }

    public void LoadFromList(List<string> ParamStringList, bool ParamAppendToExisting = false)
    {
        if (!ParamAppendToExisting)
            Clear();

        foreach(string s in ParamStringList)
        {
            AddItem(s, s);
        }
    }

    public void LoadFromDataSet(WispDataSet ParamDataSet, bool ParamAppendToExisting = false)
    {
        if (ParamDataSet is WispMinimalDataSet)
        {
            LoadFromDataSet((WispMinimalDataSet)ParamDataSet, ParamAppendToExisting);
        }
    }

    public void LoadFromDataSet(WispMinimalDataSet ParamDataSet, bool ParamAppendToExisting = false)
    {
        if (ParamDataSet == null)
            return;

        if (!ParamAppendToExisting)
            Clear();

        List<string> keys = ParamDataSet.GetKeys();

        foreach(string key in keys)
        {
            AddItem(key, ParamDataSet.GetSummaryString(key));
        }
    }
}
