using System;
using System.Collections.Generic;
using System.IO;
using TinyJson;
using UnityEngine;
using UnityEngine.EventSystems;
using WispExtensions;

public class WispNodeEditor : WispScrollView, IPointerClickHandler
{
    [Header("Node editor")]
    [SerializeField] private string noNodeMessage = "No nodes to display, right click to add nodes.";
    [SerializeField] private bool displayErrorOnLoad = true;
    
    struct WispConnectorPairInfo
    {
        public WispConnectableType type1;
        public string nodeId1;
        public string connectorId1;

        public WispConnectableType type2;
        public string nodeId2;
        public string connectorId2;
    }

    private Dictionary<int, WispNode> nodes = new Dictionary<int, WispNode>();
    private int nodeCounter = 0;

    private Dictionary<int, WispRerouteNode> rerouteNodes = new Dictionary<int, WispRerouteNode>();
    private int rerouteNodesCounter = 0;

    private Dictionary<long, IWispNodeEditorConnectable> connectables = new Dictionary<long, IWispNodeEditorConnectable>();
    private long connectablesCounter = 0;

    private List<WispConnectablePair> pairs = new List<WispConnectablePair>();

    private WispContextMenu menu;
    private TMPro.TextMeshProUGUI noNodeText = null;
    private GameObject antiAliasingFilter = null;

    public override bool Initialize()
    {
        if (isInitialized)
            return true;
        
        base.Initialize();

        WispKeyBoardEventSystem.AddEventOnKey(KeyCode.Delete, invokeOnDeleteKeyPress, this);

        antiAliasingFilter = Instantiate(WispPrefabLibrary.Default.FxaaFilter);
        antiAliasingFilter.transform.SetParent(contentRect);
        antiAliasingFilter.transform.SetAsFirstSibling();
        antiAliasingFilter.GetComponent<RectTransform>().AnchorStyleExpanded(true);

        UpdateNoNodeText();

        isInitialized = true;

        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Open right click menu
            if (menu == null)
            {
                menu =  WispContextMenu.CreateAndOpenAtMousePosition(contentRect);
                menu.SetParent(this, true);
                menu.AddItem("add_node", "Add a new node.", AddNewNodeOnClick);
                menu.AddItem("save_node_graph", "Save node graph to...", SaveNodeGraphOnClick);
                menu.AddItem("load_node_graph", "Load node graph from...", LoadNodeGraphOnClick);
                menu.AddItem("set_canvas_size", "Set canvas size.", OpenSetCanvasSizePopup);
                menu.AddItem("clear", "Delete all nodes.", OpenDeleteNodesDialog);
                menu.AddItem("add_reroute_node", "Add a new reroute node.", AddNewRerouteNodeOnClick);
            }
            else
            {
                menu.gameObject.SetActive(true);
                menu.transform.SetAsLastSibling();
                menu.MyRectTransform.localPosition = contentRect.GetMousePositionInMe();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            
            if (menu != null)
            {
                menu.gameObject.SetActive(false);
            }
        }
    }

    public WispNode AddNewNode(Vector2 ParamPosition, bool ParamHoverMode, int ParamNodeID = -1)
    {
        WispNode node = WispNode.Create(contentRect);
        node.SetParent(this, true);
        node.ParentEditor = this;
        node.MyRectTransform.anchoredPosition = ParamPosition;

        if (ParamHoverMode)
            node.gameObject.AddComponent<WispHoverMode>();

        if (ParamNodeID == -1)
        {
            nodeCounter++;
            nodes.Add(nodeCounter, node);
            node.Index = nodeCounter;
        }
        else
        {
            nodes.Add(ParamNodeID, node);
            node.Index = ParamNodeID;
        }

        UpdateNoNodeText();

        return node;
    }

    public void AddNewNodeOnClick()
    {
        AddNewNode(menu.MyRectTransform.anchoredPosition, true);
        menu.gameObject.SetActive(false);
    }

    public void SaveNodeGraphOnClick()
    {
        WispFileSelector fileSelector = WispFileSelector.OpenAuto("c:/Nodes", "txt", SaveToFile, true, true);
        menu.gameObject.SetActive(false);
    }

    public void LoadNodeGraphOnClick()
    {
        WispFileSelector fileSelector = WispFileSelector.OpenAuto("c:/Nodes", "txt", LoadButtonOnClick, false, false);
        menu.gameObject.SetActive(false);
    }

    public void LoadButtonOnClick()
    {
        LoadFromJson(File.ReadAllText(WispFileSelector.GetLastSelectedFilePath()));
    }

    public void SaveToFile()
    {
        File.WriteAllText(WispFileSelector.GetLastSelectedFilePath(), GetJson());
    }

    public override string GetJson()
    {
        // Nodes
        string nodes = "[";

        foreach(KeyValuePair<int, WispNode> kvp in this.nodes)
        {
            nodes += kvp.Value.GetJson() + ",";
        }

        nodes = nodes.GetStringWithNoCommaAtTheEnd() + "]";

        nodes = nodes.ToBase64();

        // Reroute nodes
        string rerouteNodes = "[";

        foreach(KeyValuePair<int, WispRerouteNode> kvp in this.rerouteNodes)
        {
            rerouteNodes += kvp.Value.GetJson() + ",";
        }

        rerouteNodes = rerouteNodes.GetStringWithNoCommaAtTheEnd() + "]";

        rerouteNodes = rerouteNodes.ToBase64();

        // Pairs
        string pairs = "[";

        foreach(WispConnectablePair pair in this.pairs)
        {
            pairs += pair.GetJson_New() + ",";
        }

        pairs = pairs.GetStringWithNoCommaAtTheEnd() + "]";

        pairs = pairs.ToBase64();

        // Prepare result dictionary
        Dictionary<string, string> result = new Dictionary<string, string>();

        // Build dictionary        
        if (this.nodes.Count > 0)
            result.Add("nodes", nodes);

        if (this.rerouteNodes.Count > 0)
            result.Add("reroute_nodes", rerouteNodes);

        if (this.pairs.Count > 0)
            result.Add("pairs", pairs);

        // Width and Height
        result.Add("w", contentRect.rect.width.ToString());
        result.Add("h", contentRect.rect.height.ToString());

        // Return result dictionary as json
        return result.ToJson();
    }

    public void Clear()
    {
        foreach (KeyValuePair<int, WispNode> kvp in nodes)
        {
            Destroy(kvp.Value.gameObject);
        }

        foreach (KeyValuePair<int, WispRerouteNode> kvp in rerouteNodes)
        {
            Destroy(kvp.Value.gameObject);
        }

        nodes.Clear();
        nodeCounter = 0;
        
        rerouteNodes.Clear();
        rerouteNodesCounter = 0;

        pairs.Clear();

        connectables.Clear();
        connectablesCounter = 0;
    }

    public override bool LoadFromJson(string ParamJson)
    {
        try
        {
            Clear();
        
            Dictionary<string, string> json = ParamJson.FromJson<Dictionary<string, string>>();

            if (json.ContainsKey("nodes"))
            {
                List<string> nodeList = json["nodes"].FromBase64().FromJson<List<string>>();

                foreach(string s in nodeList)
                {
                    AddNewNodeFromJson(s.SurroundWithCurlyBraces());
                }
            }

            if (json.ContainsKey("reroute_nodes"))
            {
                List<string> nodeList = json["reroute_nodes"].FromBase64().FromJson<List<string>>();

                foreach(string s in nodeList)
                {
                    AddNewRerouteNodeFromJson(s.SurroundWithCurlyBraces());
                }
            }

            if (json.ContainsKey("pairs"))
            {
                List<string> pairList = json["pairs"].FromBase64().FromJson<List<string>>();

                foreach(string s in pairList)
                {
                    ConnectPairFromJson(s.SurroundWithCurlyBraces());
                }
            }

            SetDimensions(json["w"].ToFloat(), json["h"].ToFloat());
        }
        catch (System.Exception e)
        {
            if (displayErrorOnLoad)
                WispMessageBox.OpenOneButtonDialog("There was an error loading a node graph from JSON." + Environment.NewLine + "Error message : " + e.Message, "Ok", WispWindow.CloseParentWindow());
            
            Clear();
            return false;
        }

        return true;
    }

    // Must be non Base64, Surrounded with curly braces.
    public WispNode AddNewNodeFromJson(string ParamJson)
    {
        Dictionary<string, string> nodeDetails = ParamJson.FromJson<Dictionary<string, string>>();

        Vector3 pos = new Vector3(nodeDetails["x"].ToFloat(), nodeDetails["y"].ToFloat(), nodeDetails["z"].ToFloat());

        WispNode node = AddNewNode(pos, false, nodeDetails["id"].ToInt());
        node.Label = nodeDetails["label"].FromBase64();
        node.Description = nodeDetails["description"].FromBase64();
        node.MyRectTransform.sizeDelta = new Vector2(nodeDetails["w"].ToFloat(), nodeDetails["h"].ToFloat());
        node.ParentEditor = this;

        if (nodeDetails["id"].ToInt() > nodeCounter)
            nodeCounter = nodeDetails["id"].ToInt();

        node.UpdateConnectorsFromJson(nodeDetails["inputs"].FromBase64(), nodeDetails["outputs"].FromBase64());

        return node;
    }

    // Must be non Base64, Surrounded with curly braces.
    public WispRerouteNode AddNewRerouteNodeFromJson(string ParamJson)
    {
        Dictionary<string, string> nodeDetails = ParamJson.FromJson<Dictionary<string, string>>();

        Vector3 pos = new Vector3(nodeDetails["x"].ToFloat(), nodeDetails["y"].ToFloat(), nodeDetails["z"].ToFloat());

        WispRerouteNode node = AddNewRerouteNode(pos, false, nodeDetails["con_id"].ToLong(), nodeDetails["id"].ToInt());

        if (nodeDetails["id"].ToInt() > rerouteNodesCounter)
            rerouteNodesCounter = nodeDetails["id"].ToInt();

        return node;
    }

    // Must be non Base64, Surrounded with curly braces.
    public void ConnectPairFromJson(string ParamJson)
    {
        Dictionary<string, string> pairInfo = ParamJson.FromJson<Dictionary<string, string>>();
        
        IWispNodeEditorConnectable con_one = connectables[pairInfo["1"].ToLong()];
        IWispNodeEditorConnectable con_two = connectables[pairInfo["2"].ToLong()];

        con_one.ConnectWith(con_two);
    }

    private IWispNodeEditorConnectable GetConnectableFromPairInfo(WispConnectableType ParamType, string ParamID, string ParamConnectorName)
    {
        if (ParamType == WispConnectableType.NodeConnector)
        {
            return nodes[ParamID.ToInt()].GetConnectorByName(ParamConnectorName);
        }
        else if (ParamType == WispConnectableType.RerouteNode)
        {
            print("ID : " + ParamID);
            return rerouteNodes[ParamID.ToInt()];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// ...
    /// </summary>
    public static new WispNodeEditor Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.NodeEditor, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.NodeEditor);
        }

        return go.GetComponent<WispNodeEditor>();
    }

    protected void UpdateNoNodeText()
    {
        if (nodes.Count > 0 || rerouteNodes.Count > 0)
        {
            if (noNodeText != null)
                noNodeText.gameObject.SetActive(false);

            return;
        }

        if (noNodeText == null)
        {
            noNodeText = Instantiate(WispPrefabLibrary.Default.TextMeshPro, contentRect).GetComponent<TMPro.TextMeshProUGUI>();
            
            RectTransform rt = noNodeText.GetComponent<RectTransform>();
            rt.AnchorStyleExpanded(true);
            rt.PivotAround("top-left");

            noNodeText.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
			noNodeText.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
			noNodeText.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
			noNodeText.text = noNodeMessage;
            return;
        }
        else
        {
            noNodeText.gameObject.SetActive(true);
        }
    }

    private void OpenSetCanvasSizePopup()
    {
        menu.Close();
        
        WispEntityInstance canvasSizeEntity = new WispEntityInstance("canvasSizeEntity", "Canvas Size");
        canvasSizeEntity.AddProperty(new WispEntityPropertyText("width", "Width", 1));
        canvasSizeEntity.AddProperty(new WispEntityPropertyText("height", "Height", 1));

        canvasSizeEntity.GetEntityPropertyByName("width").SetValue(contentRect.rect.width.ToString());
        canvasSizeEntity.GetEntityPropertyByName("height").SetValue(contentRect.rect.height.ToString());

        WispDialogWindow window = WispDialogWindow.CreateAndOpen(null);

        WispEntityEditor editor = WispEntityEditor.Create(window.ScrollView.ContentRect);
        editor.SetParent(window.ScrollView, true);
        editor.ColumnCount = 1;
        editor.EnableButtonPanel = false;
        editor.AnchorStyleExpanded(true);
        editor.SetBottom(-64f);
        editor.RenderInstance(canvasSizeEntity);

        window.ButtonPanel.AddButton("ok","Ok", delegate{ DimensionPanelOkButtonOnClick(editor, window); });
        window.ButtonPanel.AddButton("cancel","Cancel", delegate{ DimensionPanelCancelButtonOnClick(window); });

        window.ButtonPanel.GetButton("ok").AssignKeyBoardShortcut(KeyCode.Return);
        window.ButtonPanel.GetButton("ok").AssignKeyBoardShortcut(KeyCode.KeypadEnter);

        window.ButtonPanel.GetButton("cancel").AssignKeyBoardShortcut(KeyCode.Escape);
    }

    private void OpenDeleteNodesDialog()
    {
        menu.Close();
        WispMessageBox.OpenTwoButtonsDialog("Are you sure you want to delete all nodes ? This action is irreversible.", "Yes", Clear, "No", WispVisualComponent.CloseParentWindow, null);
    }

    private void DimensionPanelOkButtonOnClick(WispEntityEditor ParamEditor, WispDialogWindow ParamPopup)
    {
        //...
        ParamPopup.Close();

        //...
        WispEntityInstance instance = ParamEditor.GetInstanceInCurrentState();
        float x = instance.GetEntityPropertyByName("width").GetValue().ToFloat();
        float y = instance.GetEntityPropertyByName("height").GetValue().ToFloat();
        SetDimensions(x,y);
    }

    public void SetDimensions(float ParamWidth, float ParamHeight)
    {
        contentRect.sizeDelta = new Vector2(ParamWidth, ParamHeight);

        // Must do this after changing sizeDelta.
		scrollRect.CalculateLayoutInputHorizontal ();
		scrollRect.CalculateLayoutInputVertical ();
    }

    private void DimensionPanelCancelButtonOnClick(WispDialogWindow ParamPopup)
    {
        ParamPopup.Close();
    }

    private void invokeOnDeleteKeyPress()
    {
        if (WispVisualComponent.FocusedComponent is WispNode)
        {
            WispNode node = (WispNode)WispVisualComponent.FocusedComponent;
            if (node.ParentEditor == this)
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    nodes[node.Index].DeleteAllConnections();
                }
                else
                {
                    DeleteNode(node.Index);
                }
            }
        }
    }

    public void DeleteNode(int ParamNodeID)
    {
        if (nodes.ContainsKey(ParamNodeID))
        {
            nodes[ParamNodeID].DeleteAllConnections();
            Destroy(nodes[ParamNodeID].gameObject);
            nodes.Remove(ParamNodeID);
        }
    }

    public void AddNewRerouteNodeOnClick()
    {
        AddNewRerouteNode(menu.MyRectTransform.anchoredPosition, true, -1);
        menu.gameObject.SetActive(false);
    }

    public WispRerouteNode AddNewRerouteNode(Vector2 ParamPosition, bool ParamHoverMode, long ParamConnectableID, int ParamNodeID = -1)
    {
        WispRerouteNode node = WispRerouteNode.Create(contentRect);
        node.SetParent(this, true);
        node.MyRectTransform.anchoredPosition = ParamPosition;
        node.LineRenderingRectTransform = contentRect;

        if (ParamHoverMode)
        node.gameObject.AddComponent<WispHoverMode>();

        if (ParamNodeID == -1)
        {
            rerouteNodesCounter++;
            rerouteNodes.Add(rerouteNodesCounter, node);
            node.Index = rerouteNodesCounter;
        }
        else
        {
            rerouteNodes.Add(ParamNodeID, node);
            node.Index = ParamNodeID;
        }

        // Register connectable
        if (ParamConnectableID > 0)
            node.CastObject<IWispNodeEditorConnectable>().AssignConnectableID(this, ParamConnectableID);
        else
            node.CastObject<IWispNodeEditorConnectable>().RegisterConnectable(this);
        
        UpdateNoNodeText();

        return node;
    }

    public long RegisterConnectable(IWispNodeEditorConnectable ParamConnectable)
    {
        connectablesCounter++;
        connectables.Add(connectablesCounter, ParamConnectable);

        return connectablesCounter;
    }

    public WispConnectablePair RegisterPair(long ParamConIdOne, long ParamConIdTwo)
    {
        WispConnectablePair pair = new WispConnectablePair();
        pair.one = ParamConIdOne;
        pair.two = ParamConIdTwo;

        pairs.Add(pair);

        return pair;
    }

    public void UpdateConnectablesCounter(long ParamID)
    {
        if (ParamID > connectablesCounter)
        {
            connectablesCounter = ParamID;
        }
    }

    public void ReplaceConnectable(long ParamOldID, long ParamNewID, IWispNodeEditorConnectable ParamConnectable)
    {
        if (connectables.ContainsKey(ParamOldID))
        {
            connectables.Remove(ParamOldID);
        }

        if (connectables.ContainsKey(ParamNewID))
        {
            connectables.Remove(ParamNewID);
        }

        connectables.Add(ParamNewID, ParamConnectable);
    }
}

public interface IWispNodeEditorConnectable
{
    void RegisterPair(IWispNodeEditorConnectable ParamOther, WispLineRenderer ParamLine);
    RectTransform GetRectTransform();
    WispVisualComponent GetParent();
    string GetConnectorName();
    void DisconnectPair(IWispNodeEditorConnectable ParamPair);
    WispConnectableType GetConnectableType();
    string GetPairJson();
    void ConnectWith(IWispNodeEditorConnectable conTwo);
    void RegisterConnectable(WispNodeEditor ParamParentEditor); // Also sets connectableID in IWispNodeEditorConnectable.
    long GetConnectableID();
    void AssignConnectableID(WispNodeEditor ParamParent, long ParamID);
}

public enum WispConnectableType
{
    NodeConnector,
    RerouteNode
}

public struct WispConnectablePair
{
    public long one;
    public long two;

    internal string GetJson_New()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("1", one.ToString());
        result.Add("2", two.ToString());

        return result.ToJson();
    }
}