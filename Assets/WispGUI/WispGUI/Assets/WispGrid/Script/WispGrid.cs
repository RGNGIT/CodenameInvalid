using System.Collections.Generic;
using UnityEngine;
using WispExtensions;

public class WispGrid : WispScrollView
{
    [Header("Cell settings")]
    [SerializeField] private float cellWidth = 128f;
    [SerializeField] private float cellHeight = 128f;
    [SerializeField] private GameObject cellPrefab;

    private int columnCount = 0;
    private int rowCount = 0;
    private Dictionary<int, WispGridCell> cells = new Dictionary<int, WispGridCell>();

    public int CellCount
    {
        get
        {
            return cells.Count;
        }
    }

    public float CellWidth { get => cellWidth; set => cellWidth = value; }
    public float CellHeight { get => cellHeight; set => cellHeight = value; }

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        ApplyStyle();
    }

    public override bool Initialize()
    {
        base.Initialize();

        isInitialized = true;

        return true;
    }

    //...
    public void SetDimensions(int ParamColumns, int ParamRows)
    {
        if (ParamColumns < 0 || ParamRows < 0)
            return;

        if (ParamColumns > columnCount)
        {
            // Add Columns
            while (columnCount < ParamColumns)
            {
                AddColumn();
            }
        }
        else if (ParamColumns < columnCount)
        {
            // Remove Columns
            //columnCount = ParamColumns;
        }

        if (ParamRows > rowCount)
        {
            // Add Rows
            while (rowCount < ParamRows)
            {
                AddRow();
            }
        }
        else if (ParamRows < rowCount)
        {
            // Remove Rows
            //rowCount = ParamRows;
        }

        contentRect.sizeDelta = new Vector2((cellWidth*columnCount/2), (cellHeight*rowCount/2));

        // Must do this at the end.
        scrollRect.CalculateLayoutInputHorizontal();
        scrollRect.CalculateLayoutInputVertical();
    }

    public void AddColumn()
    {
        for (int i = 0; i < rowCount; i++)
        {
            GameObject go = Instantiate(cellPrefab, contentRect);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(columnCount * cellWidth, i * cellHeight * -1);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2 (cellWidth, cellHeight);
            WispGridCell cell = go.GetComponent<WispGridCell>();
            cell.ColumnIndex = columnCount + 1;
            cell.RowIndex = i+1;
            cell.CellIndex = cells.Count;
            cell.SetParent(this, true);
            cells.Add(cells.Count, cell);
        }

        columnCount++;
    }

    public void AddRow()
    {
        for (int i = 0; i < columnCount; i++)
        {
            GameObject go = Instantiate(cellPrefab, contentRect);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * cellWidth, rowCount * cellHeight * -1);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth, cellHeight);
            WispGridCell cell = go.GetComponent<WispGridCell>();
            cell.ColumnIndex = i+1;
            cell.RowIndex = rowCount + 1;
            cell.CellIndex = cells.Count;
            cell.SetParent(this, true);
            cells.Add(cells.Count, cell);
        }

        rowCount++;
    }

    public WispGridCell GetCell(int ParamCellIndex)
    {
        return cells[ParamCellIndex];
    }

    public override void ApplyStyle()
    {
        base.ApplyStyle();
    }

    /// <summary>
    /// ...
    /// </summary>
    public new static WispGrid Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.Grid, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.Grid);
        }

        return go.GetComponent<WispGrid>();
    }

    public void AutoFit()
    {
        if (columnCount == 0 || rowCount == 0)
            return;

        cellWidth = MyRectTransform.rect.width / columnCount;
        cellHeight = MyRectTransform.rect.height / rowCount;
        
        MaxContentRectSize();
        UpdatePositions();
    }

    public override void UpdatePositions()
    {
        foreach(KeyValuePair<int, WispGridCell> kvp in cells)
        {
            kvp.Value.MyRectTransform.anchoredPosition = new Vector2((kvp.Value.ColumnIndex-1) * cellWidth, (kvp.Value.RowIndex-1) * cellHeight * (-1));
            kvp.Value.MyRectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);
        }
    }
}