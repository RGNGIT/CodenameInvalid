using System.Collections.Generic;
using TinyJson;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WispExtensions;

public class WispTable : WispScrollView, IPointerClickHandler 
{
    [Header("Table Properties")]
	[Space(8)]
	[SerializeField] private bool fillTableWithTestData = false;
	[SerializeField] private string noRowMessage = "This table has no rows to display.";

    [Header("Column Properties")]
	[Space(8)]
	[SerializeField] private float columnWidth = 196;
	[SerializeField] private float headerHeight = 24;
	[SerializeField] private float minimumColumnWidth = 8;
	[SerializeField] private bool enableColumnDragging = true;
	[SerializeField] private bool allowColumnResizing = false;
	[SerializeField] private bool allowHeaderEditing = false;

	[Header("Row Properties")]
	[Space(8)]
	[SerializeField] private float rowHeight = 24;
	[SerializeField] private float minimumRowHeight = 8;
	[SerializeField] private bool enableRowDragging = false;
	[SerializeField] private bool AllowRowResizing = false;
	[SerializeField] private bool enableRowSelection = false;

	[Header("Prefabs")]
	[Space(8)]
	[SerializeField] private GameObject ColumnHeaderPrefab = null;
    [SerializeField] private GameObject textCellPrefab = null;
    [SerializeField] private GameObject textCellEditorPrefab = null;
	[SerializeField] private GameObject resizerPrefab = null;

	[Header("Cell Properties")]
	[Space(8)]
	[SerializeField] private bool allowEditing = true;
	[SerializeField] private WispSubStyleRule cellSubStyle = WispSubStyleRule.Widget;

    private List<WispColumn> columns;
    private List<WispRow> rows;
    private RectTransform resizerRect;
    private int rowIndex = 0;
    private WispRow selectedRow;
    private GameObject currentCellEditor;
    private UnityEvent onRowSelect;
    private Dictionary<string, WispRow> rowsWithUniqueIdentifiers = new Dictionary<string, WispRow>();
    private WispPanel noRowPanel;
	private List<string> jsonExludedColumns = new List<string>();
	private Dictionary<int, int> columnOrderMapping = new Dictionary<int, int>();

    public int ColumnCount
	{
		get { return columns.Count;}
	}

	public int RowCount
	{
		get { return rows.Count;}
	}

	public bool EnableColumnDragging {
		get {
			return enableColumnDragging;
		}
	}

	public bool EnableRowDragging {
		get {
			return enableRowDragging;
		}
	}

	public bool AllowEditing {
		get {
			return allowEditing;
		}
        set
        {
            allowEditing = value;
        }
	}

	public GameObject TextCellEditorPrefab
	{
		get 
		{
			return textCellEditorPrefab;
		}
	}

    public GameObject TextCellPrefab
    {
        get
        {
            return textCellPrefab;
        }
    }

    public float ColumnWidth {
		get {
			return columnWidth;
		}
        set
        {
            foreach (WispColumn col in columns)
            {
                col.Width = value;
            }
        }
	}

	public float HeaderHeight 
	{
		get 
		{
			return headerHeight;
		}
	}

	public float RowHeight
	{
		get
		{
			return rowHeight;
		}
        set
        {
            rowHeight = value;
			
			foreach (WispRow row in rows)
            {
                row.Height = value;
            }
        }

	}

	public float MinimumColumnWidth 
	{
		get 
		{
			return minimumColumnWidth;
		}
		set 
		{
			minimumColumnWidth = value;
		}
	}

	public float MinimumRowHeight 
	{
		get 
		{
			return minimumRowHeight;
		}
		set 
		{
			minimumRowHeight = value;
		}
	}

	public WispRow SelectedRow 
	{
		get 
		{
			return selectedRow;
		}
		set 
		{
            // Change previously selected row style
            if (selectedRow != null) 
			{
				foreach (GameObject go in selectedRow.Items) 
				{
					go.GetComponent<Image> ().ApplyStyle(style, Opacity, WispSubStyleRule.Widget);
                    go.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().ApplyStyle(style, Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);
				}
			}

			selectedRow = value;

            // Call onRowSelect
            if (onRowSelect != null)
                onRowSelect.Invoke();

            if (value == null)
                return;

            // Change newly selected row style
            foreach (GameObject go in value.Items) 
			{
                go.GetComponent<Image> ().ApplyStyle_Selected(style, Opacity, WispSubStyleRule.Widget);
				go.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI> ().ApplyStyle_Selected(style, Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);
			}
		}
	}

	public bool EnableRowSelection 
	{
		get 
		{
			return enableRowSelection;
		}
		set 
		{
			enableRowSelection = value;
		}
	}

    public bool AllowHeaderEditing
    {
        get
        {
            return allowHeaderEditing;
        }

        set
        {
            allowHeaderEditing = value;
        }
    }

    public float ContentWidth
    {
        get
        {
            return contentRect.rect.width;
        }
    }

    public float ContentHeight
    {
        get
        {
            return contentRect.rect.height;
        }
    }

	public WispSubStyleRule CellSubStyle { get {return cellSubStyle;} }

    public string NoRowMessage { get => noRowMessage; set => noRowMessage = value; }

    /// <summary>
    /// Initialize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
	{
		if (isInitialized)
			return true;

        base.Initialize();

		resizerRect = contentRect.Find ("ResizerRect").GetComponent<RectTransform> ();
		columns = new List<WispColumn>();
		rows = new List<WispRow>();

        if (fillTableWithTestData)
        {
            TableTest();
        }

        isInitialized = true;

        return true;
    }

    /// <summary>
    /// ...
    /// </summary>
    public new static WispTable Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.Table, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.Table);
        }

        return go.GetComponent<WispTable>();
    }

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet.
    /// </summary>
    public override void ApplyStyle ()
	{
        base.ApplyStyle();
        
        // Loop through columns, rows and cells.
		foreach (WispColumn col in columns) 
		{
			// Column Header
			col.GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
			col.GetComponentInChildren<TMPro.TextMeshProUGUI> ().ApplyStyle(style, Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);

			// Columns
			col.XPos = col.XPos;

			// Column Items (Cells)
			foreach (GameObject go in col.Items) 
			{
				go.GetComponent<Image>().ApplyStyle(style, Opacity, cellSubStyle);
                go.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().ApplyStyle(style, Opacity, WispFontSize.Normal, cellSubStyle);
            }

			// Column resizers
			if (col.Resizer != null)
				col.Resizer.UpdateStyle();
		}

		// Row resizers
		foreach(WispRow row in rows)
		{
			// Row resizers
			if (row.Resizer != null)
				row.Resizer.UpdateStyle();
		}

		UpdateNoRowPanel();
	}

    /// <summary>
    /// Update the size of the ScrollRect(Viewport) of the grid according to the total width of the columns and the total height of the rows.
    /// </summary>
    public void UpdateScrollRectSize ()
	{
		// Resize the content area to the size of columns.
		// For some reason we need to substract the width of the viewport from the width of the content so that it appears correctly.
		float widthSum;
		widthSum = GetTotalColumnsWidth() - transform.GetComponent<RectTransform> ().rect.width;
		contentRect.sizeDelta = new Vector2 (widthSum, contentRect.sizeDelta.y);

		// Then rows.
		float heightSum = headerHeight + GetTotalRowsHeight();
		contentRect.sizeDelta = new Vector2 (contentRect.sizeDelta.x, heightSum);

		// Must do this at the end.
		scrollRect.CalculateLayoutInputHorizontal ();
		scrollRect.CalculateLayoutInputVertical ();
	}

	/// <summary>
	/// Add a new column to the grid.
	/// </summary>
	public WispColumn AddColumn(string ParamIdentifier, string ParamLabel)
	{	
		if (GetColumnByID (ParamIdentifier) != null)
		{
			LogWarning("Colmun already exist : " + ParamIdentifier);
			return null;
		}
		
		GameObject go = Instantiate (ColumnHeaderPrefab, contentRect);

		go.GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
		go.GetComponentInChildren<TMPro.TextMeshProUGUI> ().ApplyStyle(style, Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);

		WispColumn column = go.GetComponent<WispColumn>();
		RectTransform tmpRT = go.GetComponent<RectTransform> ();
		tmpRT.sizeDelta = new Vector2 (columnWidth, headerHeight);
		column.ParentGrid = this;
		column.ColumnID = ParamIdentifier;
		column.ColumnName = ParamLabel;
		columns.Add (column);
		column.Initialize ();

		float widthSum = 0;

		foreach (WispColumn c in columns) 
		{
			widthSum += c.Width;
		}

		widthSum = widthSum - column.Width;
		
		column.XPos = widthSum + style.HorizontalPadding;
		tmpRT.anchoredPosition = new Vector2 (column.XPos, -style.VerticalPadding);

		if (rows.Count > 0)
		{
			for (int i = 0; i < rows.Count; i++)
				column.AddItem ();
		}
			
		UpdateScrollRectSize ();

		if (allowColumnResizing) 
		{
			GameObject resizer = Instantiate (resizerPrefab, resizerRect);
			RectTransform rt = resizer.GetComponent<RectTransform> ();
			rt.anchoredPosition = new Vector2 (column.XPos + column.Width, 0);
			column.Resizer = resizer.GetComponent<WispTableResizer> ();
			resizer.GetComponent<WispTableResizer> ().TargetColumn = column;
		}

        UpdateNoRowPanel();
        NotifyObserversOnEdit();

        return column;
	}

	/// <summary>
	/// Add a new row to the grid.
	/// </summary>
	public WispRow AddRow(string ParamUniqueIdentifier = "")
	{	
		if (columns.Count == 0)
			return null;

		WispRow row = new WispRow();
		row.ParentTable = this;
		row.Initialize ();
		row.Index = rowIndex;
		rowIndex++;
		row.YPos = -GetTotalRowsHeight() - style.VerticalPadding;
		rows.Add (row);

        if (ParamUniqueIdentifier != "" && ParamUniqueIdentifier != null)
        {
            rowsWithUniqueIdentifiers.Add(ParamUniqueIdentifier, row);
        }

		// Add items only after having added the row to the list of rows
		foreach (WispColumn c in columns) 
		{  
			c.AddItem ();
			if (c.Resizer != null) {
				c.Resizer.GetComponent<RectTransform> ().sizeDelta = new Vector2 (c.Resizer.GetComponent<RectTransform> ().sizeDelta.x, headerHeight + (rows.Count * rowHeight));
			}
		}

		UpdateScrollRectSize ();

		// Resizer
		if (AllowRowResizing) {
			GameObject go = Instantiate (resizerPrefab, resizerRect);
			RectTransform rt = go.GetComponent<RectTransform> ();
			rt.anchorMin = new Vector2 (0, 1);
			rt.anchorMax = new Vector2 (0, 1);
			rt.pivot = new Vector2 (0, 0.5f);
			rt.anchoredPosition = new Vector2 (0, row.YPos - row.Height);
			rt.sizeDelta = new Vector2 (GetTotalColumnsWidth(), rt.sizeDelta.y);
			WispTableResizer tmpResizer = go.GetComponent<WispTableResizer> ();
			tmpResizer.TargetRow = row;
			row.Resizer = tmpResizer;
		}

		BringElementToFront (null);

        UpdateNoRowPanel();
        NotifyObserversOnEdit();

        return row;
	}

    /// <summary>
    /// Add a new row to the grid.
    /// </summary>
    public WispRow AddRowWithValues(params string[] ParamCellValues)
    {
        WispRow row = AddRow();

        if (row == null)
        {
            WispVisualComponent.LogError("Unable to add row, please make sure the table has at least one column.");
            return null;
        }

        for (int i = 0; i < ParamCellValues.Length; i++)
        {
            row.Items[i].GetComponent<WispTableCell>().SetValue(ParamCellValues[i]);
        }

        NotifyObserversOnEdit();

        return row;
    }

	void Start()
	{
        ApplyStyle();
	}

    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Return a column by providing an index.
    /// </summary>
    public WispColumn GetColumn(int ParamIndex)
	{
		return columns [ParamIndex];
	}

	/// <summary>
	/// Return a row by providing an index.
	/// </summary>
	public WispRow GetRow(int ParamIndex)
	{
		return rows [ParamIndex];
	}

	/// <summary>
	/// Return a row by providing an instance of the wanted row.
	/// </summary>
	public WispRow GetRow(WispRow ParamRow)
	{
		int i = rows.IndexOf (ParamRow);
		return rows [i];
	}

	/// <summary>
	/// Return a column at the given X position.
	/// </summary>
	public WispColumn GetColumnFromPosition (float ParamXPosition)
	{
		foreach (WispColumn c in columns) {

			if (ParamXPosition >= c.XPos + ((c.Width*2)/3)  && ParamXPosition <= c.XPos + c.Width)
			{
				return c;
			}
		
		}

		return null;
	}

	/// <summary>
	/// Return a row at the given X position.
	/// </summary>
	public WispRow GetRowFromPosition(float ParamYPosition)
	{

		foreach (WispRow r in rows) 
		{
			if (ParamYPosition <= r.YPos && r.YPos - r.Height <= ParamYPosition) {
				return r;
			}
		}

		return null;
	}

	/// <summary>
	/// Swap the position and the index of two columns.
	/// </summary>
	public void SwapColumns (WispColumn ParamColumnOne, WispColumn ParamColumnTwo)
	{	
		// Arrang columns so that one is on the left and two is on the right
		WispColumn tmpCol;
		if (ParamColumnOne.XPos > ParamColumnTwo.XPos) 
		{
			tmpCol = ParamColumnOne;
			ParamColumnOne = ParamColumnTwo;
			ParamColumnTwo = tmpCol;
		}

		// ...
		int colOneIndex = columns.IndexOf (ParamColumnOne);
		int colTwoIndex = columns.IndexOf (ParamColumnTwo);

		float colOnePos = columns [colOneIndex].XPos;
		float colTwoPos = columns [colTwoIndex].XPos;

		WispColumn tmpColumnOne = columns [colOneIndex];

		// Swap Positions
		columns [colOneIndex].XPos = colOnePos + ParamColumnTwo.Width;
		columns [colOneIndex].ReAlignHeaderAndItems ();

		columns [colTwoIndex].XPos = colOnePos;
		columns [colTwoIndex].ReAlignHeaderAndItems ();

		// Swap in the list
		columns [colOneIndex] = ParamColumnTwo;
		columns [colTwoIndex] = tmpColumnOne;
	}

	/// <summary>
	/// Remove a column from the table.
	/// </summary>
	public void RemoveColumn (WispColumn ParamColumn)
	{
		int colIndex = columns.IndexOf (ParamColumn);
		ParamColumn.RemoveItems ();
		Destroy (ParamColumn.gameObject);
		columns.Remove (ParamColumn);


		float w = 0;
		for (int i = 0; i < colIndex; i++)
			w += columns [i].Width;

		for (int j = colIndex; j < columns.Count; j++) {
			columns [j].XPos = w;
			columns [j].ReAlignHeaderAndItems ();
			w += columns [j].Width;
		}

        UpdateNoRowPanel();
	}

	/// <summary>
	/// Remove a row from the grid.
	/// </summary>
	public void RemoveRow (WispRow ParamRow)
	{
		int rowIndex = ParamRow.Index;

		rows.Remove (ParamRow);

		foreach (WispRow row in rows) {
			if (row.Index > rowIndex)
				row.Index -= 1;
		}

		foreach (WispColumn col in columns)
			col.RemoveItem (rowIndex);
		
		float h = -HeaderHeight;
		for (int i = 0; i < rowIndex; i++) {
			h -= rows [i].Height;
		}

		for (int j = rowIndex; j < rows.Count; j++) {
			rows [j].YPos = h - style.VerticalPadding;
			rows [j].AlignItems ();
			h -= rows [j].Height;
		}

        if (ParamRow == selectedRow)
            SelectedRow = null;

        Destroy(ParamRow.Resizer.gameObject);

        UpdateResizers();

        UpdateNoRowPanel();
    }

	/// <summary>
	/// Swap the position and the index of two rows.
	/// </summary>
	public void SwapRows (WispRow ParamRowOne, WispRow ParamRowTwo)
	{
		// Arrang columns so that one is on the left and two is on the right
		WispRow tmpRowContainer;
		if (ParamRowOne.YPos < ParamRowTwo.YPos) {
			tmpRowContainer = ParamRowOne;
			ParamRowOne = ParamRowTwo;
			ParamRowTwo = tmpRowContainer;
		}

		int rowOneIndex = rows.IndexOf (ParamRowOne);
		int rowTwoIndex = rows.IndexOf (ParamRowTwo);

		float rowOnePos = ParamRowOne.YPos;
		float rowTwoPos = ParamRowTwo.YPos;

		// Swap positions
		WispRow tmpRow = ParamRowOne;

		rows [rowOneIndex].YPos = rowOnePos - ParamRowTwo.Height;
		rows [rowOneIndex].AlignItems ();

		rows [rowTwoIndex].YPos = rowOnePos;
		rows [rowTwoIndex].AlignItems ();

		// Swap in the list
		rows [rowOneIndex] = ParamRowTwo;
		rows [rowTwoIndex] = tmpRow;

		// Swap Index
		int tmpIndex;
		tmpIndex = ParamRowOne.Index;
		ParamRowOne.Index = ParamRowTwo.Index;
		ParamRowTwo.Index = tmpIndex;
	}

	/// <summary>
	/// Return a column by providing its ID.
	/// </summary>
	public WispColumn GetColumnByID(string ParamIdentifier)
	{
		foreach (WispColumn column in columns) 
		{
			if (column.ColumnID == ParamIdentifier)
				return column;
		}
		
		return null;
	}

	/// <summary>
	/// Return a cell by providing the column ID and the index of the row.
	/// </summary>
	public WispTableCell GetCell(string ParamColumnID,int ParamRowIndex)
	{
		foreach (WispColumn column in columns) 
		{
			if (column.ColumnID == ParamColumnID)
				return column.GetItem(ParamRowIndex).GetComponent<WispTableCell> ();
		}
		
		return null;
	}

	/// <summary>
	/// Return a cell by providing the index of the column and the index of the row.
	/// </summary>
	public WispTableCell GetCell(int Col,int Row)
	{
		return columns [Col].GetItem(Row).GetComponent<WispTableCell> ();
	}

	/// <summary>
	/// Return a cell by providing a column instance and a row instance.
	/// </summary>
	public WispTableCell GetCell(WispColumn ParamCol,WispRow ParamRow)
	{
		return columns[columns.IndexOf(ParamCol)].GetItem(rows.IndexOf(ParamRow)).GetComponent<WispTableCell> ();
	}

	/// <summary>
	/// Destroy all columns and rows
	/// </summary>
	public void Clear ()
	{
		foreach (WispColumn c in columns) {
		
			Destroy (c.gameObject);
		
		}

		columns.Clear ();
		rows.Clear ();
	}

	/// <summary>
	/// Destroy all rows
	/// </summary>
	public void ClearRows ()
	{
		foreach(WispRow row in rows)
		{
			Destroy(row.Resizer.gameObject);
			// Destroy(row.RowRect.gameObject);

			foreach(GameObject go in row.Items)
			{
				Destroy(go);
			}
		}

		foreach (WispColumn col in columns)
		{
			col.Items.Clear();
		}
		
		rows.Clear ();
		rowIndex = 0;

		UpdateResizers();
		UpdateNoRowPanel();
	}

	/// <summary>
	/// Clear data from all columns and rows
	/// </summary>
	public void ClearData()
	{
		foreach (WispColumn c in columns) {
			c.ClearData ();
		}
	}

	/// <summary>
	/// Adjust columns width so that all columns are visible in the viewport.
	/// </summary>
	public void AdjustColumnWidthToView ()
	{
		float viewPortWidth = transform.GetComponent<RectTransform> ().rect.width;
		float scalingRatio = viewPortWidth / GetTotalColumnsWidth ();
		float widthSum = 0;

		foreach (WispColumn c in columns) 
		{
			c.Width = c.Width * scalingRatio;
			c.XPos = widthSum + style.HorizontalPadding;
			widthSum += c.Width;
		}

		UpdateScrollRectSize ();
		UpdateResizers ();
        UpdateNoRowPanel();
    }

	/// <summary>
	/// Move a column to position X and all the columns to its right.
	/// </summary>
	public void ShiftColumnPosition (WispColumn ParamColumn, float ParamPosition)
	{
		// Build a list of columns that shall be moved (the ones that are on the right of the concerned column)
		List<WispColumn> tmpColumnList = new List<WispColumn> ();

		foreach (WispColumn col in columns) {
		
			if (col != ParamColumn && col.XPos > ParamColumn.XPos) {
			
				tmpColumnList.Add (col);
			
			}

		}

		// Calcuate shifting amount
		float shift = ParamPosition - ParamColumn.XPos;

		// Move the concerned column
		ParamColumn.XPos = ParamPosition;

		// Move the other columns
		foreach (WispColumn col in tmpColumnList) {

			col.XPos = col.XPos + shift;

		}
	}

	/// <summary>
	/// Move a row to position Y and all the columns to its right.
	/// </summary>
	public void ShiftRowPosition (WispRow ParamRow, float ParamPosition)
	{
		// Build a list of columns that shall be moved (the ones that are on the right of the concerned column)
		List<WispRow> tmpRowList = new List<WispRow> ();

		foreach (WispRow row in rows) {

			if (row != ParamRow && row.YPos < ParamRow.YPos) {

				tmpRowList.Add (row);

			}

		}

		// Calcuate shifting amount
		float shift = ParamPosition - ParamRow.YPos;

		// Move the concerned row
		ParamRow.YPos = ParamPosition;

		// Move the other columns
		foreach (WispRow row in tmpRowList) {

			row.YPos = row.YPos + shift;

		}
	}

	/// <summary>
	/// Return the column at the right of the given column.
	/// </summary>
	public WispColumn GetColumnToTheRight (WispColumn ParamColumn)
	{
		int i = columns.IndexOf(ParamColumn)+1;

		if (i < columns.Count)
			return columns[i];

		return null;
	}

	/// <summary>
	/// Return the row below the given row.
	/// </summary>
	public WispRow GetRowBelowThisRow (WispRow ParamRow)
	{
		int i = rows.IndexOf(ParamRow)+1;

		if (i < rows.Count)
			return rows[i];

		return null;
	}

	/// <summary>
	/// Update the size of the column and row resizers.
	/// </summary>
	public void UpdateResizers ()
	{
		foreach (WispColumn col in columns) {
			if (col.Resizer != null)
				col.Resizer.GetComponent<RectTransform> ().sizeDelta = new Vector2 (col.Resizer.GetComponent<RectTransform> ().sizeDelta.x, GetTotalRowsHeight ());
		}

		foreach (WispRow row in rows) {
			if (row.Resizer != null)
				row.Resizer.GetComponent<RectTransform> ().sizeDelta = new Vector2 (GetTotalColumnsWidth (), row.Resizer.GetComponent<RectTransform> ().sizeDelta.y);
		}
	}

	/// <summary>
	/// Return the width of all the columns combined.
	/// </summary>
	public float GetTotalColumnsWidth ()
	{
		float width = 0;
		foreach (WispColumn col in columns) {
			width += col.Width;
		}
		return width;
	}

	/// <summary>
	/// Return the height of all the rows combined.
	/// </summary>
	public float GetTotalRowsHeight (bool ParamIncludeHeaderHeight = true)
	{
		float height;

		if (ParamIncludeHeaderHeight) {
			height = headerHeight;
		} else {
			height = 0;
		}

		foreach (WispRow row in rows) {
			height += row.Height;
		}

		return height;
	}

	/// <summary>
	/// Return the height of all the rows combined.
	/// </summary>
	public void BringElementToFront (Transform ParamConcernedTransform, bool ParamForceResizersToFront = true)
	{
		if (ParamConcernedTransform != null)
		ParamConcernedTransform.SetAsLastSibling ();

		if (ParamForceResizersToFront)
			resizerRect.SetAsLastSibling ();
	}

	/// <summary>
	/// Return the content of the grid as a json string.
	/// </summary>
	private string GetJsonCompact()
	{
		var columnDictionary = new Dictionary<string, List<string>> ();

		foreach (WispColumn col in columns) {

			if (jsonExludedColumns.Contains(col.ColumnID))
			{
				continue;
			}
			
			List<string> tmpList = new List<string> ();

			// Add Column Name/Label as the first value of the first key
			tmpList.Add (col.ColumnName);

			// Add rows/cells
			foreach (GameObject go in col.Items) {
				tmpList.Add (go.GetComponent<WispTableCell> ().GetValue ());
			}

			columnDictionary.Add (col.ColumnID, tmpList);
		}

		// return Json.Encode(columnDictionary);
		return columnDictionary.ToJson();
	}

    public override string GetJson()
    {
        return GetJsonCompact();
    }

    /// <summary>
    /// Load data into the grid from a json string, the json string must follow the same format as the one returned by GetJson ().
    /// </summary>
    public override bool LoadFromJson (string ParamJson)
	{
		Clear ();

        try
        {
            var dictionary = ParamJson.FromJson<Dictionary<string, List<string>>>();
            int rowCount = 0;

            foreach (KeyValuePair<string, List<string>> col in dictionary)
            {
                var list = col.Value;
                AddColumn(col.Key, list[0]);
                rowCount = list.Count;
            }

            for (int i = 0; i < rowCount - 1; i++)
            {
                AddRow();
            }

            int c = 0;

            foreach (KeyValuePair<string, List<string>> col in dictionary)
            {
                var list = col.Value;
                list.RemoveAt(0);

                foreach (string s in list)
                {
                    GetCell(col.Key, c).SetValue(s);
                    c++;
                }

                c = 0;
            }
        }
        catch (System.Exception e)
        {
            WispVisualComponent.LogError("Error while loading from Json : " + e.Message);
            return false;
        }

		AdjustColumnWidthToView ();

        NotifyObserversOnEdit();

        return true;
    }

    public GameObject GetCellEditor ()
    {
        if (currentCellEditor != null)
            return currentCellEditor;
        else
        {
            currentCellEditor = Instantiate(textCellEditorPrefab, contentRect);
            return currentCellEditor;
        }
    }

    public void CloseCellEditor ()
    {
        GetCellEditor().SetActive(false);
        NotifyObserversOnEdit();
    }

    public void AddOnRowSelectionEvent(UnityAction ParamAction)
    {
        if (onRowSelect == null)
            onRowSelect = new UnityEvent();

        onRowSelect.AddListener(ParamAction);
    }

    public WispDataSetTable GetDataSet()
    {
        WispDataSetTable result = new WispDataSetTable();

        foreach(WispColumn col in columns)
        {
            result.AddColumn(col.ColumnID);
        }

        foreach(WispRow row in rows)
        {
            result.AddRow(row.Index, row.GetValues());
        }

        return result;
    }

    public void LoadFromTableDataSet(WispDataSetTable ParamDataSet)
    {
        Clear();

        List<string> dataSetColumns = ParamDataSet.GetColumns();

        foreach(string col in dataSetColumns)
        {
            AddColumn(col, col);
        }

        List<int> dataSetRows = ParamDataSet.GetRows();

        foreach(int row in dataSetRows)
        {
            WispRow rowObj = AddRow();

            foreach (string col in dataSetColumns)
            {
                string cell = ParamDataSet.GetCell(col, row);
                GetCell(col, rowObj.Index).SetValue(ParamDataSet.GetCell(col, row));
            }
        }

        NotifyObserversOnEdit();
    }

    public WispRow GetRowWithUniqueIdentifier(string ParamRowIdentifier)
    {
        if (rowsWithUniqueIdentifiers.ContainsKey(ParamRowIdentifier))
        {
            return rowsWithUniqueIdentifiers[ParamRowIdentifier];
        }

        return null;
    }

    // Test
    private void TableTest ()
	{
		AddColumn ("1", "Column 1");
		AddColumn ("2", "Column 2");
		AddColumn ("3", "Column 3");
		AddColumn ("4", "Column 4");

		AddRow ();
		AddRow ();
		AddRow ();
		AddRow ();

		AdjustColumnWidthToView ();

	}

    // Test
    private void GridTest()
    {
        AddColumn("1", "");
        AddColumn("2", "");
        AddColumn("3", "");

        AddRowWithValues("1", "2", "3");
        AddRowWithValues("4", "5", "6");
        AddRowWithValues("7", "8", "9");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectedRow = null;
    }

    public WispRow[] GetRows()
    {
        return rows.ToArray();
    }

    public void UpdateNoRowPanel()
    {
        if (ColumnCount == 0)
        {
            if (noRowPanel != null)
                Destroy(noRowPanel.gameObject);

            return;
        }

        if (RowCount > 0)
        {
            if (noRowPanel != null)
                Destroy(noRowPanel.gameObject);

            return;
        }

        if (noRowPanel == null)
        {
            // Panel
			noRowPanel = WispPanel.Create(contentRect);
			noRowPanel.Initialize();
			noRowPanel.SetParent(this, true);
            noRowPanel.AnchorTo("top-left");
            noRowPanel.PivotAround("top-left");
            noRowPanel.Set_X_Position(0);
            noRowPanel.Set_Y_Position(-ColumnHeaderPrefab.GetComponent<RectTransform>().rect.height - style.VerticalPadding);
            noRowPanel.Width = GetTotalColumnsWidth();
            noRowPanel.Height = 32;
			noRowPanel.SetTooltipText("No rows to display.", noRowMessage);
			noRowPanel.SubStyleRule = WispSubStyleRule.Widget;

			// Text
			TMPro.TextMeshProUGUI text = Instantiate(WispPrefabLibrary.Default.TextMeshPro, noRowPanel.transform).GetComponent<TMPro.TextMeshProUGUI>();
			
			WispTextMeshPro wispTMP = text.GetComponent<WispTextMeshPro>();
			wispTMP.Initialize();
			wispTMP.SetParent(this, true);
			wispTMP.AnchorStyleExpanded(true);
			wispTMP.PivotAround("center-center");

			text.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
			text.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
			text.enableWordWrapping = false;
			text.overflowMode = TMPro.TextOverflowModes.Ellipsis;
			text.text = noRowMessage;
            return;
        }
        else
        {
            noRowPanel.MyRectTransform.anchoredPosition = new Vector2(style.HorizontalPadding, -ColumnHeaderPrefab.GetComponent<RectTransform>().rect.height - style.VerticalPadding);
			// noRowPanel.Set_Y_Position(-ColumnHeaderPrefab.GetComponent<RectTransform>().rect.height - style.VerticalPadding);
            noRowPanel.Width = GetTotalColumnsWidth();
            noRowPanel.Height = 32;
            return;
        }
    }

	public void ExcludeColumnFromJsonOutput(string ParamColumnName)
	{
		jsonExludedColumns.Add(ParamColumnName);
	}
}