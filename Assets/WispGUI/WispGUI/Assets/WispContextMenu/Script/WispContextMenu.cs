using System.Collections.Generic;
using UnityEngine;
using WispExtensions;

public class WispContextMenu : WispScrollView
{
    [Header("Context Menu Prefabs")]
    [SerializeField] private GameObject itemPrefab;

    [Header("Context Menu Dimensions")]
    [SerializeField] private float itemHeight = 32f;

    private Dictionary<string, WispButton> items = new Dictionary<string, WispButton>();
    private bool destroyOnClose = false;

    public bool DestroyOnClose { get => destroyOnClose; set => destroyOnClose = value; }

    /// <summary>
    /// Create.
    /// </summary>
    public static new WispContextMenu Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.ContextMenu, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.ContextMenu);
        }

        return go.GetComponent<WispContextMenu>();
    }

    /// <summary>
    /// Create.
    /// </summary>
    public static WispContextMenu CreateAndOpenAtMousePosition(Transform ParamTransform, bool ParamDestroyOnClose = false)
    {
        WispContextMenu result = WispContextMenu.Create(ParamTransform);
        result.Open();
        result.rectTransform.localPosition = WispRectTransform.GetMousePositionInRectTransform(result.rectTransform);
        result.transform.SetAsLastSibling();

        result.destroyOnClose = ParamDestroyOnClose;

        return result;
    }

    /// <summary>
    /// Start
    /// </summary>
    void Start()
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
    /// Apply Color and graphics modifications from the style sheet.
    /// </summary>
    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        isInitialized = true;

        return true;
    }

    public WispButton AddItem(string ParamID, string ParamLabel, UnityEngine.Events.UnityAction ParamAction, bool ParamBranch = false)
    {
        if (items.ContainsKey(ParamID))
            return null;

        WispButton item = WispButton.Create(contentRect);
        item.PivotAround("left-top");
        item.AnchorTo("left-top");
        item.Set_X_Position(0);
        item.Set_Y_Position(items.Count * -itemHeight);
        item.SetValue(ParamLabel);
        item.Width = rectTransform.sizeDelta.x;
        item.ForceAutoSize = false;
        item.SetParent(this, true);

        if (ParamAction != null)
        {
            item.AddOnClickAction(ParamAction);
        }

        items.Add(ParamID, item);

        return item;
    }

    public override void ApplyStyle()
    {
        base.ApplyStyle();
        MaxContentRectSize();
    }

    public override void Open()
    {
        base.Open();

        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public override void Close()
    {
        base.Close();

        if (destroyOnClose)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}