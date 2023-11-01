using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WispRow
{
	private List<GameObject> items;
    private WispTable parentGrid;
    private float height;
    private float yPos;
    private RectTransform rowRect;
    private int index = 0;
    private float dragY;
    private float mouseY;
    private WispTableResizer resizer;
    private string hiddenValue = "";

    public List<GameObject> Items {
		get {
			return items;
		}
	}

	public RectTransform RowRect {
		get {
			return rowRect;
		}
		set {
			rowRect = value;
		}
	}

	public  int Index {
		get {
			return index;
		}
		set {
			index = value;
		}
	}

	public WispTable ParentTable {
		get {
			return parentGrid;
		}
		set {
			parentGrid = value;
		}
	}

	public float Height {
		get {
			return height;
		}
		set {
			if (value < parentGrid.MinimumRowHeight)
				return;
			
			height = value;

			foreach (GameObject go in items) {
				go.GetComponent<RectTransform> ().sizeDelta = new Vector2 (go.GetComponent<RectTransform> ().sizeDelta.x, height);
			}
		}
	}

	public float YPos {
		get {
			return yPos;
		}
		set {
			yPos = value;

			foreach (GameObject go in items) {
			
				go.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (go.GetComponent<RectTransform> ().anchoredPosition.x, yPos);

			}

			if (resizer != null) {
				resizer.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, yPos - height);
			}
		}
	}

	public WispTableResizer Resizer {
		get {
			return resizer;
		}
		set {
			resizer = value;
		}
	}

    public string HiddenValue { get => hiddenValue; set => hiddenValue = value; }

    public void Initialize()
	{
		items = new List<GameObject> ();
		height = parentGrid.RowHeight;
	}

	// Mark an item as being part of this row
	public void RegisterItem (GameObject ParamItem)
	{
		items.Add (ParamItem);
		ParamItem.GetComponent<WispTableCell> ().ParentRow = this;
	}

	// set y position for rows
	public void AlignItems()
	{
		foreach(GameObject g in items)
		{
			RectTransform tmpR  = g.GetComponent<RectTransform> ();
			tmpR.anchoredPosition = new Vector2 (tmpR.anchoredPosition.x,yPos);
		}

	}

	public void BeginDrag()
	{	
		if (!parentGrid.EnableRowDragging)
			return;
		
		dragY = rowRect.anchoredPosition.y - Input.mousePosition.y;
		mouseY = Input.mousePosition.y;

	}

	public void OnDrag()
	{
		if (!parentGrid.EnableRowDragging)
			return;
		
		rowRect.anchoredPosition = new Vector2 (rowRect.anchoredPosition.x, dragY + Input.mousePosition.y);

		WispRow tmpRow;
		if (Input.mousePosition.y < mouseY) {
			tmpRow = parentGrid.GetRowFromPosition (rowRect.anchoredPosition.y - rowRect.sizeDelta.y);
			mouseY = Input.mousePosition.y + 2;
		}
		else {
			tmpRow = parentGrid.GetRowFromPosition (rowRect.anchoredPosition.y - (rowRect.sizeDelta.y/2));
			mouseY = Input.mousePosition.y - 2;
		}
		if (tmpRow != null && tmpRow != this)
			parentGrid.SwapRows (tmpRow, this);

	}

	public void EndDrag()
	{	
		if (!parentGrid.EnableRowDragging)
			return;
		
		AlignItems ();
	}

	public void Select ()
	{
		parentGrid.SelectedRow = this;
	}

    public string[] GetValues()
    {
        string[] result = new string[items.Count];

        int c = 0;
        foreach(GameObject go in items)
        {
            result[c] = go.GetComponent<WispTableCell>().GetValue();
            c++;
        }

        return result;
    }

    public void Remove()
    {
        parentGrid.RemoveRow(this);
    }
}