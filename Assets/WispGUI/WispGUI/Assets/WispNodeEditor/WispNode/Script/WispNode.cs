using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TinyJson;
using WispExtensions;
using System;

public class WispNode : WispVisualComponent
{
    [Header("Node settings")]
    [SerializeField] private string label = "Default Label";
    [SerializeField] private string description = "No description.";
    [SerializeField] private List<string> defaultInputs;
    [SerializeField] private List<string> defaultOutputs;

    [Header("Node prefabs")]
    [SerializeField] private GameObject connector;

    private WispButton labelButton;
    private WispScrollView descriptionScrollView;
    private TMPro.TextMeshProUGUI descriptionText;

    private WispNodeEditor parentEditor;
    private Dictionary<string, WispNodeConnector> inputs = new Dictionary<string, WispNodeConnector>();
    private Dictionary<string, WispNodeConnector> outputs = new Dictionary<string, WispNodeConnector>();
    private Vector3 preDragMousePosition;
    private Vector3 preDragNodePosition;
    private float lastClickTime = 0;
    private WispEditBox textEditor;
    private WispResizingHandle handle;

    public string Label
    {
        get => label;
        set { label = value; labelButton.SetValue(value); }
    }

    public string Description
    {
        get => description;
        set { description = value; descriptionText.text = value; }
    }

    public WispNodeEditor ParentEditor 
    { 
        get => parentEditor;
        
        set
        {
            parentEditor = value;
            UpdateLineRenderingRectTransform();
            UpdateConnectableIDs();
        }
    }

    private void UpdateLineRenderingRectTransform()
    {
        foreach (KeyValuePair<string, WispNodeConnector> kvp in inputs)
        {
            kvp.Value.LineRenderingRectTransform = parentEditor.ContentRect;
        }

        foreach (KeyValuePair<string, WispNodeConnector> kvp in outputs)
        {
            kvp.Value.LineRenderingRectTransform = parentEditor.ContentRect;
        }
    }

    private void UpdateConnectableIDs()
    {
        foreach (KeyValuePair<string, WispNodeConnector> kvp in inputs)
        {
            kvp.Value.CastObject<IWispNodeEditorConnectable>().RegisterConnectable(parentEditor);
        }

        foreach (KeyValuePair<string, WispNodeConnector> kvp in outputs)
        {
            kvp.Value.CastObject<IWispNodeEditorConnectable>().RegisterConnectable(parentEditor);
        }
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

        labelButton = transform.Find("Label").GetComponent<WispButton>();

        descriptionScrollView = transform.Find("Description").GetComponent<WispScrollView>();
        descriptionScrollView.Initialize();
        descriptionScrollView.SetParent(this, true);

        descriptionText = descriptionScrollView.ContentRect.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();

        WispMouseClickHandler mouseClickHandler = gameObject.AddComponent<WispMouseClickHandler>();
        mouseClickHandler.OnDoubleClick.AddListener(delegate { Edit(descriptionScrollView); });

        labelButton.Initialize();
        labelButton.SetValue(label);
        labelButton.SetParent(this, true);
        labelButton.AddOnClickAction(LabelButtonOnClick);

        descriptionText.text = description;

        AddConnectors(defaultInputs, defaultOutputs);
        UpdatePositions();

        handle = WispResizingHandle.Create(rectTransform, rectTransform, 128f, 96f);
        handle.ActionOnDrag = UpdatePositions;
        handle.ActionOnEndDrag = UpdateBorders;
        
        isInitialized = true;

        return true;
    }

    /// <summary>
    /// Create.
    /// </summary>
    public static WispNode Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.Node, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.Node);
        }

        return go.GetComponent<WispNode>();
    }

    public void ClearConnectors()
    {
        foreach (KeyValuePair<string, WispNodeConnector> kvp in inputs)
        {
            Destroy(kvp.Value.gameObject);
        }

        inputs.Clear();

        foreach (KeyValuePair<string, WispNodeConnector> kvp in outputs)
        {
            Destroy(kvp.Value.gameObject);
        }

        outputs.Clear();
    }

    public void AddConnectors(List<string> ParamInputs, List<string> ParamOutputs)
    {
        foreach (string s in ParamInputs)
        {
            WispNodeConnector con = (Instantiate(connector, transform) as GameObject).GetComponent<WispNodeConnector>();
            con.Type = WispNodeConnector.NodeConnectorType.Input;
            con.SetParent(this, true);
            con.ConnectorName = s;
            
            if (parentEditor != null)
            {
                con.LineRenderingRectTransform = parentEditor.ContentRect;
                con.CastObject<IWispNodeEditorConnectable>().RegisterConnectable(parentEditor);
            }

            inputs.Add(s, con);
        }

        foreach (string s in ParamOutputs)
        {
            WispNodeConnector con = (Instantiate(connector, transform) as GameObject).GetComponent<WispNodeConnector>();
            con.Type = WispNodeConnector.NodeConnectorType.Output;
            con.SetParent(this, true);
            con.ConnectorName = s;
            
            if (parentEditor != null)
            {
                con.LineRenderingRectTransform = parentEditor.ContentRect;
                con.CastObject<IWispNodeEditorConnectable>().RegisterConnectable(parentEditor);
            }

            outputs.Add(s, con);
        }

        UpdatePositions();
    }

    public WispNodeConnector GetConnectorByName(string ParamConnectorName)
    {
        if (inputs.ContainsKey(ParamConnectorName))
            return inputs[ParamConnectorName];
        else if (outputs.ContainsKey(ParamConnectorName))
            return outputs[ParamConnectorName];
        else
            return null;
    }

    public override void UpdatePositions()
    {
        base.UpdatePositions();

        int i = 1;
        float inputOffset = rectTransform.rect.height / (inputs.Count + 1);

        foreach (KeyValuePair<string, WispNodeConnector> kvp in inputs)
        {
            RectTransform rt = kvp.Value.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(0, -1 * inputOffset * i);
            i++;
        }

        i = 1;
        float outputOffset = rectTransform.rect.height / (outputs.Count + 1);

        foreach (KeyValuePair<string, WispNodeConnector> kvp in outputs)
        {
            RectTransform rt = kvp.Value.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(0, -1 * outputOffset * i);
            i++;
        }

        UpdateLines();
    }

    public override string GetJson()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("id", Index.ToString());
        
        result.Add("x", rectTransform.anchoredPosition3D.x.ToString());
        result.Add("y", rectTransform.anchoredPosition3D.y.ToString());
        result.Add("z", rectTransform.anchoredPosition3D.z.ToString());
        
        result.Add("w", rectTransform.sizeDelta.x.ToString());
        result.Add("h", rectTransform.sizeDelta.y.ToString());
        
        result.Add("label", label.ToBase64());
        result.Add("description", description.ToBase64());

        // Connectors
        // 1 : Inputs
        string inputs = "[";

        foreach(KeyValuePair<string, WispNodeConnector> kvp in this.inputs)
        {
            inputs += kvp.Value.GetJson() + ",";
        }

        inputs = inputs.GetStringWithNoCommaAtTheEnd() + "]";

        inputs = inputs.ToBase64();

        // 1 : Outputs
        string outputs = "[";

        foreach(KeyValuePair<string, WispNodeConnector> kvp in this.outputs)
        {
            outputs += kvp.Value.GetJson() + ",";
        }

        outputs = outputs.GetStringWithNoCommaAtTheEnd() + "]";

        outputs = outputs.ToBase64();

        if (this.inputs.Count > 0)
            result.Add("inputs", inputs);

        if (this.outputs.Count > 0)
            result.Add("outputs", outputs);

        return result.ToJson();
    }

    // Must be non Base64, Surrounded with bars.
    public void UpdateConnectorsFromJson(string ParamJsonInputs, string ParamJsonOutputs)
    {
        ClearConnectors();
        
        List<string> inputsList = ParamJsonInputs.FromJson<List<string>>();
        List<string> tmpInputs = new List<string>();

        foreach(string s in inputsList)
        {
            Dictionary<string, string> inputDetails = s.SurroundWithCurlyBraces().FromJson<Dictionary<string, string>>();
            tmpInputs.Add(inputDetails["name"]);
        }

        List<string> outputsList = ParamJsonOutputs.FromJson<List<string>>();
        List<string> tmpOutputs = new List<string>();

        foreach(string s in outputsList)
        {
            Dictionary<string, string> outputDetails = s.SurroundWithCurlyBraces().FromJson<Dictionary<string, string>>();
            tmpOutputs.Add(outputDetails["name"]);
        }

        AddConnectors(tmpInputs, tmpOutputs);

        // Then we assign connectorIDs
        // 1 : Inputs
        foreach(string s in inputsList)
        {
            Dictionary<string, string> inputDetails = s.SurroundWithCurlyBraces().FromJson<Dictionary<string, string>>();
            GetConnectorByName(inputDetails["name"]).CastObject<IWispNodeEditorConnectable>().AssignConnectableID(ParentEditor, inputDetails["con_id"].ToLong());
        }

        // 2 : Outputs
        foreach(string s in outputsList)
        {
            Dictionary<string, string> outputDetails = s.SurroundWithCurlyBraces().FromJson<Dictionary<string, string>>();
            GetConnectorByName(outputDetails["name"]).CastObject<IWispNodeEditorConnectable>().AssignConnectableID(ParentEditor, outputDetails["con_id"].ToLong());
        }
    }

    public void OnBeginDrag()
    {
        preDragMousePosition = Input.mousePosition;
        preDragNodePosition = rectTransform.anchoredPosition3D;
    }

    public void OnDrag()
    {
        rectTransform.anchoredPosition3D = preDragNodePosition + Input.mousePosition - preDragMousePosition;

        UpdateLines();
    }

    public void OnClick()
    {
        Focus();
    }

    public void UpdateLines()
    {
        foreach (KeyValuePair<string, WispNodeConnector> kvp in inputs)
        {
            kvp.Value.UpdateLines();
        }

        foreach (KeyValuePair<string, WispNodeConnector> kvp in outputs)
        {
            kvp.Value.UpdateLines();
        }
    }

    private void LabelButtonOnClick()
    {
        // Check if it's a double click
        if (Time.time - lastClickTime < 0.5)
        {
            // Start Edit
            Edit(labelButton);

            lastClickTime = 0;
        }
        else
        {
            lastClickTime = Time.time;
        }
    }

    private void descriptionOnDoubleClick()
    {
        Edit(descriptionScrollView);
    }

    private void Edit(WispVisualComponent ParamEditable)
    {
        WispEditBox edt = WispEditBox.Create(rectTransform);
        edt.PickerType = WispEditBox.EditBoxPickerType.None;

        if (ParamEditable is WispButton)
            edt.SetValue(label);
        else if (ParamEditable is WispScrollView)
            edt.SetValue(description);
        else
            edt.SetValue("");

        edt.Label = "";
        edt.Base.Select();
        edt.BorderRule = WispBorderRule.Never;
        edt.Initialize();

        textEditor = edt;

        edt.MyRectTransform.SetAnchorSettings(ParamEditable.MyRectTransform.GetAnchorSettings());
        edt.MyRectTransform.SetGeometricSettings(ParamEditable.MyRectTransform.GetGeometricSettings());

        edt.Base.onEndEdit.RemoveAllListeners();

        if (ParamEditable == labelButton)
        {
            edt.Base.textComponent.fontSize = labelButton.TextComponent.fontSize;
            edt.Base.onEndEdit.AddListener(onLabelEndEdit);
        }
        else if (ParamEditable == descriptionScrollView)
        {
            edt.Base.textComponent.fontSize = descriptionText.fontSize;
            edt.Base.onEndEdit.AddListener(onDescriptionEndEdit);
        }
    }

    private void onLabelEndEdit(string ParamValue)
    {
        Label = ParamValue;
        Destroy(textEditor.gameObject);
    }

    private void onDescriptionEndEdit(string ParamValue)
    {
        Description = ParamValue;
        Destroy(textEditor.gameObject);
    }

    // public override string GetJson()
    // {
    //     Dictionary<string, string> nodeData = new Dictionary<string, string>();

    //     string connectors = "[";

    //     foreach (KeyValuePair<string, WispNodeConnector> kvp in inputs)
    //     {
    //         connectors += kvp.Value.GetJson() + ",";
    //     }

    //     foreach (KeyValuePair<string, WispNodeConnector> kvp in outputs)
    //     {
    //         connectors += kvp.Value.GetJson();
    //     }

    //     connectors = connectors.GetStringWithNoCommaAtTheEnd() + "]";

    //     nodeData.Add("x", rectTransform.anchoredPosition3D.x.ToString());
    //     nodeData.Add("y", rectTransform.anchoredPosition3D.y.ToString());
    //     nodeData.Add("z", rectTransform.anchoredPosition3D.z.ToString());
    //     nodeData.Add("w", rectTransform.sizeDelta.x.ToString());
    //     nodeData.Add("h", rectTransform.sizeDelta.y.ToString());
    //     nodeData.Add("label", label);
    //     nodeData.Add("description", description);
    //     nodeData.Add("connectors", connectors);

    //     return nodeData.ToJson();
    // }

    public override void ApplyStyle()
    {
        base.ApplyStyle();

		// descriptionText.color = colop(style.TextColor);
        // descriptionText.fontSize = style.TextSize;
        descriptionText.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);

        foreach (KeyValuePair<string, WispNodeConnector> kvp in inputs)
        {
            kvp.Value.ApplyStyle();
        }

        foreach (KeyValuePair<string, WispNodeConnector> kvp in outputs)
        {
            kvp.Value.ApplyStyle();
        }

        // GetComponent<Image> ().color = colop(style.BackgroundColor);
		// GetComponent<Image> ().sprite = style.BorderAndFillSprite;
		// GetComponent<Image> ().material = style.BackgroundMaterial;
        // GetComponent<Image> ().type = Image.Type.Tiled;
        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
    }

    public void DeleteAllConnections()
    {
        foreach (KeyValuePair<string, WispNodeConnector> kvp in inputs)
        {
            kvp.Value.DisconnectAllLines();
        }

        foreach (KeyValuePair<string, WispNodeConnector> kvp in outputs)
        {
            kvp.Value.DisconnectAllLines();
        }
    }
}