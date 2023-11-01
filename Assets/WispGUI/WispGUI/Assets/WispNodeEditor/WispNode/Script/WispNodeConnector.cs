using System;
using System.Collections.Generic;
using TinyJson;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispNodeConnector : WispVisualComponent, IWispNodeEditorConnectable
{
    public enum NodeConnectorType { Input, Output }

    private NodeConnectorType type;
    private WispLineRenderer dragLine;
    private Dictionary<IWispNodeEditorConnectable, WispLineRenderer> pairs = new Dictionary<IWispNodeEditorConnectable, WispLineRenderer>();
    private Vector3 initialMousePosition;
    private Vector3 preDragNodePosition;
    private string connectorName;
    private RectTransform lineRenderingRectTransform;
    private long connectableID = 0;

    public NodeConnectorType Type { get => type; set => type = value; }
    public string ConnectorName { get => connectorName; set => connectorName = value; }
    public RectTransform LineRenderingRectTransform { get => lineRenderingRectTransform; set => lineRenderingRectTransform = value; }

    private static WispNodeConnector currentlyHoveredByMousePointer;
    public static WispNodeConnector CurrentlyHoveredByMousePointer { get => currentlyHoveredByMousePointer; }

    public void onBeginDrag()
    {
        dragLine = WispLineRenderer.Create(WispVisualComponent.GetMainCanvas().transform);
        dragLine.color = colop(Parent.Style.LineColor);
        dragLine.Width = 2f;
        initialMousePosition = Input.mousePosition;
    }

    public void onDrag()
    {
        Vector3 start = GetComponent<RectTransform>().GetMyPositionInAnotherRectTransform(dragLine.rectTransform);
        Vector3 end = start + Input.mousePosition - initialMousePosition;

        dragLine.SetStartAndEndPoint(start, end);
    }

    public void onEndDrag()
    {
        // Connect line if drag ends over another node connector
        if (currentlyHoveredByMousePointer != this && currentlyHoveredByMousePointer != null )
        {
            // Do not connect inputs with inputs, OR outputs with outputs
            if (currentlyHoveredByMousePointer.Type == type)
                return;
            
            // Destroy drag line
            Destroy(dragLine.gameObject);

            if (!pairs.ContainsKey(currentlyHoveredByMousePointer))
            {
                ConnectWith(currentlyHoveredByMousePointer);
            }
        }
        // Connect line if drag ends over a reroute node
        else if (WispRerouteNode.CurrentlyHoveredByMousePointer != null )
        {
            // Destroy drag line
            Destroy(dragLine.gameObject);

            if (!pairs.ContainsKey(WispRerouteNode.CurrentlyHoveredByMousePointer))
            {
                ConnectWith(WispRerouteNode.CurrentlyHoveredByMousePointer);
            }
        }
        else
        {
            if (dragLine != null)
                Destroy(dragLine.gameObject);
        }
    }

    public void onPointEnter()
    {
        currentlyHoveredByMousePointer = this;
        GetComponent<Image>().ApplyStyle_Selected(style, Opacity, subStyleRule);
    }

    public void onPointExit()
    {
        currentlyHoveredByMousePointer = null;
        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
    }

    public bool ConnectWith(IWispNodeEditorConnectable ParamOtherConnector)
    {
        if (pairs.ContainsKey(ParamOtherConnector))
            return false;
        else
        {
            // Connect line
            WispLineRenderer line = WispLineRenderer.Create(lineRenderingRectTransform);
            line.color = colop(Parent.Style.LineColor);
            line.Width = 2f;
            line.rectTransform.SetAsFirstSibling();

            line.SetStartAndEndPoint(rectTransform, ParamOtherConnector.GetRectTransform());

            // Register as pairs
            pairs.Add(ParamOtherConnector, line);
            ParamOtherConnector.RegisterPair(this, line);

            if (Parent.CastObject<WispNode>().ParentEditor != null)
                Parent.CastObject<WispNode>().ParentEditor.RegisterPair(connectableID, ParamOtherConnector.GetConnectableID());

            return true;
        }
    }

    public void UpdateLines()
    {
        foreach (KeyValuePair<IWispNodeEditorConnectable, WispLineRenderer> kvp in pairs)
        {
            kvp.Value.SetStartAndEndPoint(GetComponent<RectTransform>(), kvp.Key.GetRectTransform());
        }
    }

    public override string GetJson()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("name", connectorName);
        result.Add("type", type.ToString());
        result.Add("con_id", connectableID.ToString());

        return result.ToJson();
    }

    public override void ApplyStyle()
    {
        base.ApplyStyle();
        
        GetComponent<Image> ().ApplyStyle(style, Opacity, subStyleRule);

        foreach (KeyValuePair<IWispNodeEditorConnectable, WispLineRenderer> kvp in pairs)
        {
            kvp.Value.color = colop(style.LineColor);
        }
    }

    public void DisconnectAllLines()
    {
        foreach (KeyValuePair<IWispNodeEditorConnectable, WispLineRenderer> kvp in pairs)
        {
            WispLineRenderer tmpLine = kvp.Value;
            kvp.Key.DisconnectPair(this);
            Destroy(tmpLine.gameObject);
        }

        pairs.Clear();
    }

    /// <summary>
    /// Create.
    /// </summary>
    public static WispNodeConnector Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.NodeConnector, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.NodeConnector);
        }

        return go.GetComponent<WispNodeConnector>();
    }

    void OnDestroy()
    {
        DisconnectAllLines();
        OnDestroy_Procedure();
    }

    void IWispNodeEditorConnectable.RegisterPair(IWispNodeEditorConnectable ParamOther, WispLineRenderer ParamLine)
    {
        pairs.Add(ParamOther, ParamLine);
    }

    RectTransform IWispNodeEditorConnectable.GetRectTransform()
    {
        return rectTransform;
    }

    WispVisualComponent IWispNodeEditorConnectable.GetParent()
    {
        return Parent;
    }

    string IWispNodeEditorConnectable.GetConnectorName()
    {
        return connectorName;
    }

    void IWispNodeEditorConnectable.DisconnectPair(IWispNodeEditorConnectable ParamPair)
    {
        pairs.Remove(ParamPair);
    }

    WispConnectableType IWispNodeEditorConnectable.GetConnectableType()
    {
        return WispConnectableType.NodeConnector;
    }

    string IWispNodeEditorConnectable.GetPairJson()
    {
        IWispNodeEditorConnectable me = this as IWispNodeEditorConnectable;
        return "{ \"type\" : " + me.GetConnectableType().ToString().SurroundWithDoubleQuotes() + ", \"node_id\" : " + (me.GetParent() as WispNode).Index.ToString().SurroundWithDoubleQuotes() + ", \"connector_name\" : " + "\"" + me.GetConnectorName() + "\"}";
    }

    void IWispNodeEditorConnectable.ConnectWith(IWispNodeEditorConnectable ParamConnectable)
    {
        ConnectWith(ParamConnectable);
    }

    void IWispNodeEditorConnectable.RegisterConnectable(WispNodeEditor ParamParentEditor)
    {
        connectableID = ParamParentEditor.RegisterConnectable(this);
    }

    long IWispNodeEditorConnectable.GetConnectableID()
    {
        return connectableID;
    }

    void IWispNodeEditorConnectable.AssignConnectableID(WispNodeEditor ParamParent, long ParamID)
    {
        ParamParent.ReplaceConnectable(connectableID, ParamID, this);
        connectableID = ParamID;
        ParamParent.UpdateConnectablesCounter(ParamID);
    }

    public void PrintDebugInfo()
    {
        print("INDX : " + Index + " /// CON_ID : " + connectableID);
    }
}