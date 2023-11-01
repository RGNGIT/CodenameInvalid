using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispColumn : MonoBehaviour
{	
	protected List<GameObject> items;
	protected RectTransform columnRect;
	protected WispTable parentGrid;
	protected float width;
	protected float xPos = 0;
    // protected float xPosInTable = 0;
    protected bool initialized = false;
	protected float dragX;
	protected float dragY;
	protected float mouseX;
	protected WispTableResizer resizer;
    private float lastClickTime = 0;
	[SerializeField] [ShowOnly] protected string columnID;
    [SerializeField] [ShowOnly] protected string columnName;

    public float Width 
	{
		get 
		{
			return width;
		}
		set 
		{
			if (value < parentGrid.MinimumColumnWidth)
				return;
			
			width = value;

			if (columnRect != null && parentGrid != null) {
				columnRect.sizeDelta = new Vector2 (width, parentGrid.HeaderHeight);
			}

			foreach (GameObject g in items)
			{	
				RectTransform tmp = g.GetComponent<RectTransform> ();
				tmp.sizeDelta  = new Vector2(width,tmp.sizeDelta.y);
			}

			if (resizer != null)
			{
				resizer.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (xPos + width, 0);
			}

			parentGrid.UpdateNoRowPanel();
		}
	}

	public float XPos 
	{
		get 
		{
			return xPos;
		}
		set 
		{
			xPos = value;

			if (columnRect != null) 
			{
				columnRect.anchoredPosition = new Vector2 (xPos + ParentGrid.Style.HorizontalPadding, -ParentGrid.Style.VerticalPadding);
			}

			if (resizer != null) 
			{
				resizer.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (xPos + ParentGrid.Style.HorizontalPadding + width, -ParentGrid.Style.VerticalPadding);
			}
		}
	}

	public List<GameObject> Items 
	{
		get 
		{
			return items;
		}
	}

	public WispTable ParentGrid 
	{
		get 
		{
			return parentGrid;
		}
		set 
		{
			parentGrid = value;
		}
	}

	public string ColumnID 
	{
		get 
		{
			return columnID;
		}
		set 
		{
			columnID = value;
		}
	}

	public string ColumnName 
	{
		get 
		{
			return columnName;
		}
		set 
		{
			columnName = value;
			GetComponentInChildren<TMPro.TextMeshProUGUI> ().text = value;
		}
	}

	public WispTableResizer Resizer 
	{
		get 
		{
			return resizer;
		}
		set 
		{
			resizer = value;
		}
	}

    //...
    void Start()
	{
		Initialize ();
	}

	// ...
	public void Initialize ()
	{
		if (initialized)
			return;

		items = new List<GameObject> ();
		columnRect = GetComponent<RectTransform> ();
		width = parentGrid.ColumnWidth;

		initialized = true;
	}

	// ...
	public void SetPositionForNewItem(GameObject ParamGo)
	{
		RectTransform tmp = ParamGo.GetComponent<RectTransform> ();
		tmp.anchoredPosition  = new Vector2 (0,(parentGrid.GetTotalRowsHeight()*-1)+parentGrid.RowHeight);
		tmp.sizeDelta = new Vector2(width,parentGrid.RowHeight);
	}

	// Called when swaping between columns.
	public void SetPositionForAllItems(float ParamPositionX)
	{
		
		xPos = ParamPositionX;
		foreach (GameObject g in items)
			SetPositionForNewItem (g);

	}

	public GameObject AddItem()
	{
        // Check if rows are available before adding an item in a column
		if (items.Count < parentGrid.RowCount) 
		{
            GameObject item = GameObject.Instantiate(parentGrid.TextCellPrefab, columnRect);

			WispTableCell itemComp = item.GetComponent<WispTableCell> ();
			itemComp.Initialize ();
			itemComp.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI> ().ApplyStyle(parentGrid.Style, parentGrid.Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);
            itemComp.ParentColumn = this;
			itemComp.UpdateStyle();

			SetPositionForNewItem (item);
			items.Add (item);
			parentGrid.GetRow (items.Count-1).RegisterItem (item);
			if (parentGrid.GetRow (items.Count - 1).Items.Count < 2) 
			{
				parentGrid.GetRow (items.Count - 1).RowRect = item.GetComponent<RectTransform> ();
			}
			return item;
		} 
		else 
		{
			return null;
		}

	}
		
	//...
	public void RemoveItems()
	{	foreach (GameObject g in items)
			Destroy (g);
			items.Clear ();
	}
	
	// ...
	public void RemoveItem(int index)
	{
		Destroy (items [index]);
		items.Remove (items [index]);

	}

	//...
	public GameObject GetItem(int ParamRowIndex)
	{
		return items [ParamRowIndex];
	}

	// ...
	public void BeginDrag ()
	{
		if (!parentGrid.EnableColumnDragging)
			return;

		dragX = columnRect.anchoredPosition.x - Input.mousePosition.x;
		mouseX = Input.mousePosition.x;
		parentGrid.BringElementToFront(transform);
	}

	// ...
	public void OnDrag ()
	{
		if (!parentGrid.EnableColumnDragging)
			return;

		columnRect.anchoredPosition = new Vector2 (dragX + Input.mousePosition.x, columnRect.anchoredPosition.y);

		WispColumn tmpColumn;

		if (Input.mousePosition.x > mouseX)
		{
			tmpColumn = parentGrid.GetColumnFromPosition (columnRect.anchoredPosition.x + columnRect.sizeDelta.x);
			mouseX = Input.mousePosition.x - 2;
		}
		else
		{ 
			tmpColumn = parentGrid.GetColumnFromPosition (columnRect.anchoredPosition.x + (columnRect.sizeDelta.x/2));
			mouseX = Input.mousePosition.x + 2;
		}


		if (tmpColumn != null && tmpColumn != this) 
		{
			parentGrid.SwapColumns (tmpColumn, this);
		} 
	}

	// ...
	public void EndDrag ()
	{
		if (!parentGrid.EnableColumnDragging)
			return;

		ReAlignHeaderAndItems ();
	}

	// ...
	public void ReAlignHeaderAndItems()
	{
		// columnRect.anchoredPosition = new Vector2 (xPos + ParentGrid.Style.HorizontalPadding, columnRect.anchoredPosition.y);
		columnRect.anchoredPosition = new Vector2 (xPos + ParentGrid.Style.HorizontalPadding, -ParentGrid.Style.VerticalPadding);

		foreach (GameObject item in items) 
		{
			RectTransform tmpRT = item.GetComponent<RectTransform> ();
			tmpRT.anchoredPosition = new Vector2 (0, tmpRT.anchoredPosition.y);
		}
	}

	// ...
	public void ClearData()
	{
		foreach (GameObject go in items) 
		{
			go.GetComponent<InputField> ().text = "";
		}
	}

    public void OnClick()
    {
        if (!parentGrid.AllowHeaderEditing)
            return;

        // Check if it's a double click
        if (Time.time - lastClickTime < 0.5)
        {
            // Start Edit
            GameObject go = ParentGrid.GetCellEditor();
            go.SetActive(true);
            go.GetComponent<RectTransform>().anchorMin = GetComponent<RectTransform>().anchorMin;
            go.GetComponent<RectTransform>().anchorMax = GetComponent<RectTransform>().anchorMax;
            go.GetComponent<RectTransform>().offsetMin = GetComponent<RectTransform>().offsetMin;
            go.GetComponent<RectTransform>().offsetMax = GetComponent<RectTransform>().offsetMax;
            go.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
            go.GetComponent<RectTransform>().pivot = GetComponent<RectTransform>().pivot;
            go.GetComponent<RectTransform>().anchoredPosition = columnRect.anchoredPosition;

            WispEditBox edt = go.GetComponent<WispEditBox>();
            edt.PickerType = WispEditBox.EditBoxPickerType.None;
            edt.SetValue(transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI> ().text);
            edt.Label = "";
            edt.GetComponent<TMPro.TMP_InputField>().Select();
            edt.Initialize();

            edt.GetComponent<TMPro.TMP_InputField>().textComponent.fontSize = parentGrid.HeaderHeight / 2.5f;
            edt.GetComponent<TMPro.TMP_InputField>().onEndEdit.RemoveAllListeners();
            edt.GetComponent<TMPro.TMP_InputField>().onEndEdit.AddListener(onEndEdit);

            lastClickTime = 0;
        }
        else
        {
            lastClickTime = Time.time;
        }
    }

    private void onEndEdit(string ParamResult)
    {
        transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = ParamResult;
        ParentGrid.CloseCellEditor();
    }
}