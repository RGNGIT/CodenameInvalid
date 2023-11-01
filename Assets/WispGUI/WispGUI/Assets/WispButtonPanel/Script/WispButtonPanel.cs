using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispButtonPanel : WispVisualComponent
{
    [Header("Dimensions")]
    [SerializeField] private float maxButtonWidth = 128f;
    [SerializeField] private float buttonMargin = 8f;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject buttonPrefab;

    private Dictionary<string, WispButton> buttons = new Dictionary<string, WispButton>();
    private Coroutine updateButtonPositionCoroutine = null;

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        // ---------------------------------------------------------------------

        isInitialized = true;

        return true;

    }

    // Start is called before the first frame update
    void Start()
    {
        ApplyStyle();
    }

    // ...
    void Awake()
    {
        Initialize();
    }

    //...
    public static WispButtonPanel Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.ButtonPanel, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.ButtonPanel);
        }

        return go.GetComponent<WispButtonPanel>();
    }

    // ...
    public WispButton AddButton(string ParamID, string ParamLabel, UnityEngine.Events.UnityAction ParamAction, Sprite ParamIcon = null)
    {
        if (buttons.ContainsKey(ParamID))
            return null;

        GameObject go = Instantiate(buttonPrefab, transform);
        // RectTransform rect = go.GetComponent<RectTransform>();

        go.name = ParamID;

        WispButton btn = go.GetComponent<WispButton>();
        btn.Initialize();
        btn.Name = ParamID;
        btn.Index = buttons.Count + 1;
        btn.SetValue(ParamLabel);
        btn.Set_Y_Position(0);
        btn.AnchorTo("left-center");
        btn.PivotAround("center-center");
        btn.ForceAutoSize = false;
        // btn.MyRectTransform.anchorMin = new Vector2(0, 0.5f);
        // btn.MyRectTransform.anchorMax = new Vector2(0, 0.5f);
        btn.SetParent(this, true);

        if (ParamIcon != null)
        {
            btn.IconPlacement = WispButton.WispButtonIconPlacement.Horizontal;
            btn.Icon.SetValue(ParamIcon);
        }
        
        if (ParamAction != null) { btn.AddOnClickAction(ParamAction); }

        buttons.Add(ParamID, btn);
        // UpdateButtonPosition();

        if (updateButtonPositionCoroutine == null)
        {
            updateButtonPositionCoroutine = StartCoroutine(UpdateButtonsPositionsNextFrame());
        }

        return btn;
    }

    public void Clear()
    {
        foreach (KeyValuePair<string,WispButton> kv in buttons)
        {
            Destroy(kv.Value.gameObject);
        }

        buttons.Clear();
    }

    IEnumerator UpdateButtonsPositionsNextFrame()
    {
        yield return new WaitForEndOfFrame();
        UpdateButtonPosition();
        updateButtonPositionCoroutine = null;
        yield return null;
    }

    // ...
    private void UpdateButtonPosition()
    {
        float buttonSpace = MyRectTransform.rect.width / buttons.Count;
        float buttonWidth = buttonSpace - buttonMargin*2;

        if (buttonWidth > maxButtonWidth)
            buttonWidth = maxButtonWidth;

        int c = 0;

        foreach(KeyValuePair<string, WispButton> kvp in buttons)
        {
            kvp.Value.Set_X_Position(buttonSpace/2 + buttonSpace*c);
            kvp.Value.Width = buttonWidth;
            c++;
        }
    }

    public override void ApplyStyle()
    {
        if (style == null)
            return;
        
        base.ApplyStyle();

        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
    }

    public WispButton GetButton(string ParamID)
    {
        if (buttons.ContainsKey(ParamID))
        {
            return buttons[ParamID];
        }
        else
        {
            LogError("Button not found : " + ParamID);
            return null;
        }
    }
}