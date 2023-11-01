using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispTableCell : MonoBehaviour 
{
    private float dragY;
    private float mouseY;
    private RectTransform itemRect;
    private WispRow parentRow;
    private WispColumn parentColumn;
    private float height;
    private List<RectTransform> tmpSiblingRects;
    private bool mouseDown = false;
    private bool mouseDrag = false;
    private float lastClickTime = 0;
	private string hiddenValue = "";

    public WispColumn ParentColumn { get => parentColumn; set => parentColumn = value; }
    public WispRow ParentRow { get => parentRow; set => parentRow = value; }
    public string HiddenValue { get => hiddenValue; set => hiddenValue = value; }

    // ...
    public void Initialize ()
	{
		itemRect = GetComponent<RectTransform> ();
		height = itemRect.rect.height;
	}

	// Use this for initialization
	void Start () 
	{
		Initialize ();
	}

	// ...
	public void BeginDrag()
	{
		if (!parentRow.ParentTable.EnableRowDragging)
			return;

		dragY = itemRect.anchoredPosition.y - Input.mousePosition.y;
		mouseY = Input.mousePosition.y;

		// Build sibling list
		tmpSiblingRects = new List<RectTransform> ();

		foreach (GameObject go in parentRow.Items) {
		
			RectTransform tmpRT = go.GetComponent<RectTransform> ();
			tmpSiblingRects.Add (tmpRT);
			parentRow.ParentTable.BringElementToFront(tmpRT);
		
		}

	}

	// ...
	public void OnDrag()
	{
		mouseDrag = true;

		if (!parentRow.ParentTable.EnableRowDragging)
			return;

		// Move the item and its siblings
		foreach (RectTransform rt in tmpSiblingRects) {
		
			rt.anchoredPosition = new Vector2 (rt.anchoredPosition.x, dragY + Input.mousePosition.y);
		
		}

		WispRow tmpRow;

		if (Input.mousePosition.y < mouseY) {
			tmpRow =parentRow.ParentTable.GetRowFromPosition (parentRow.RowRect.anchoredPosition.y );
			mouseY = Input.mousePosition.y + 2;

		}
		else {
			tmpRow =parentRow.ParentTable.GetRowFromPosition (parentRow.RowRect.anchoredPosition.y - (parentRow.RowRect.sizeDelta.y/2));
			mouseY = Input.mousePosition.y - 2;
		}
		if (tmpRow != null && tmpRow != parentRow)
			parentRow.ParentTable.SwapRows (tmpRow, parentRow);

	}
	// ...
	public void EndDrag()
	{
		if (!parentRow.ParentTable.EnableRowDragging)
			return;

		parentRow.AlignItems ();
	}

	// ...
	public void OnMouseDown()
	{
		mouseDown = true;
	}

	// ...
	public void OnMouseUp()
	{
		if (!parentRow.ParentTable.EnableRowSelection)
			return;

		if (mouseDown && !mouseDrag)
			parentRow.Select ();

		mouseDown = false;
		mouseDrag = false;
	}

	// ...
	public void SetValue(string ParamText)
	{
        TMPro.TextMeshProUGUI textcomponent = transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();

        textcomponent.text = ParamText;
		textcomponent.fontSize = parentColumn.ParentGrid.Style.FontSizeNormal;

        if (ParamText == "")
            textcomponent.gameObject.SetActive(false);
        else
            textcomponent.gameObject.SetActive(true);

        parentRow.ParentTable.NotifyObserversOnEdit();
	}

	// ...
	public string GetValue()
	{
		return transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text;
	}

    // ...
    public void OnClick ()
    {
        if (!parentRow.ParentTable.AllowEditing)
            return;
        
        // Check if it's a double click
        if (Time.time - lastClickTime < 0.5)
        {
            // Start Edit
            GameObject go = parentRow.ParentTable.GetCellEditor();
            go.SetActive(true);
            go.GetComponent<RectTransform>().anchorMin = GetComponent<RectTransform>().anchorMin;
            go.GetComponent<RectTransform>().anchorMax = GetComponent<RectTransform>().anchorMax;
            go.GetComponent<RectTransform>().offsetMin = GetComponent<RectTransform>().offsetMin;
            go.GetComponent<RectTransform>().offsetMax = GetComponent<RectTransform>().offsetMax;
            go.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
            go.GetComponent<RectTransform>().pivot = GetComponent<RectTransform>().pivot;
            go.GetComponent<RectTransform> ().anchoredPosition = new Vector2(parentColumn.XPos, GetComponent<RectTransform>().anchoredPosition.y);

            WispEditBox edt = go.GetComponent<WispEditBox>();
            edt.PickerType = WispEditBox.EditBoxPickerType.None;
            edt.SetValue(GetValue());
            edt.Label = "";
            edt.GetComponent<TMPro.TMP_InputField>().Select();
            edt.Initialize();

            edt.GetComponent<TMPro.TMP_InputField>().textComponent.fontSize = transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().fontSize;
            edt.GetComponent<TMPro.TMP_InputField>().onEndEdit.RemoveAllListeners();
            edt.GetComponent<TMPro.TMP_InputField>().onEndEdit.AddListener(onEndEdit);

            lastClickTime = 0;
        }
        else
        {
            lastClickTime = Time.time;
        }
    }

    private void onEndEdit (string ParamResult)
    {
        SetValue(ParamResult);
        parentRow.ParentTable.CloseCellEditor();
    }

	public void UpdateStyle()
	{
		GetComponent<Image>().ApplyStyle(parentColumn.ParentGrid.Style, parentColumn.ParentGrid.Opacity, parentColumn.ParentGrid.CellSubStyle);
		transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().ApplyStyle(parentColumn.ParentGrid.Style, parentColumn.ParentGrid.Opacity, WispFontSize.Normal, parentColumn.ParentGrid.CellSubStyle);
	}
}