using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WispGridCell : WispVisualComponent
{
    [Header("WispGridCell Info")]
    [SerializeField] [ShowOnly] private int cellIndex;
    [SerializeField] [ShowOnly] private int columnIndex = 0;
    [SerializeField] [ShowOnly] private int rowIndex = 0;
    
    private Image image;

    public int CellIndex { get => cellIndex; set => cellIndex = value; }
    public int ColumnIndex { get => columnIndex; set => columnIndex = value; }
    public int RowIndex { get => rowIndex; set => rowIndex = value; }

    void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        if (StyleFollowRule == WispVisualStyleFollowRule.Self)
        {
            ApplyStyle();
        }
    }

    public override bool Initialize()
    {
        base.Initialize();

        image = GetComponent<Image>();

        isInitialized = true;

        return true;
    }

    public override void ApplyStyle()
    {
        base.ApplyStyle();

        image.color = colop(Style.GridColor);
    }
}
