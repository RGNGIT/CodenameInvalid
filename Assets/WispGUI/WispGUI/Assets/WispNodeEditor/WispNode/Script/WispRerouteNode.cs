using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;
using TinyJson;

public class WispRerouteNode : WispVisualComponent, IWispNodeEditorConnectable
{
    private WispLineRenderer dragLine;
    private Dictionary<IWispNodeEditorConnectable, WispLineRenderer> pairs = new Dictionary<IWispNodeEditorConnectable, WispLineRenderer>();
    private Vector3 initialMousePosition;
    private Vector3 preDragNodePosition;
    private RectTransform lineRenderingRectTransform;
    private long connectableID = 0;

    public RectTransform LineRenderingRectTransform { get => lineRenderingRectTransform; set => lineRenderingRectTransform = value; }

    private static WispRerouteNode currentlyHoveredByMousePointer;
    public static WispRerouteNode CurrentlyHoveredByMousePointer { get => currentlyHoveredByMousePointer; }

    public void onBeginDrag()
    {
        if (Input.GetMouseButton(1))
        {
            initialMousePosition = Input.mousePosition;
            preDragNodePosition = rectTransform.anchoredPosition3D;
        }
        else
        {
            dragLine = WispLineRenderer.Create(WispVisualComponent.GetMainCanvas().transform);
            dragLine.color = colop(Parent.Style.LineColor);
            dragLine.Width = 2f;
            initialMousePosition = Input.mousePosition;
        }
    }

    public void onDrag()
    {
        if (Input.GetMouseButton(1))
        {
            rectTransform.anchoredPosition3D = preDragNodePosition + Input.mousePosition - initialMousePosition;
            UpdateLines();
        }
        else
        {
            Vector3 start = GetComponent<RectTransform>().GetMyPositionInAnotherRectTransform(dragLine.rectTransform);
            Vector3 end = start + Input.mousePosition - initialMousePosition;

            dragLine.SetStartAndEndPoint(start, end);
        }
    }

    public void onEndDrag()
    {
        if (Input.GetMouseButton(1))
        {
            return;
        }
        
        // Connect line if drag ends over another reroute node
        if (currentlyHoveredByMousePointer != this && currentlyHoveredByMousePointer != null )
        {
            // Destroy drag line
            Destroy(dragLine.gameObject);

            if (!pairs.ContainsKey(currentlyHoveredByMousePointer))
            {
                ConnectWith(currentlyHoveredByMousePointer);
            }
        }
        // Connect line if drag ends over a node connector
        else if (WispNodeConnector.CurrentlyHoveredByMousePointer != null )
        {
            // Destroy drag line
            Destroy(dragLine.gameObject);

            if (!pairs.ContainsKey(WispNodeConnector.CurrentlyHoveredByMousePointer))
            {
                ConnectWith(WispNodeConnector.CurrentlyHoveredByMousePointer);
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

    public bool ConnectWith(IWispNodeEditorConnectable ParamOther)
    {
        if (pairs.ContainsKey(ParamOther))
            return false;
        else
        {
            // Connect line
            WispLineRenderer line = WispLineRenderer.Create(lineRenderingRectTransform);
            line.color = colop(Parent.Style.LineColor);
            line.Width = 2f;
            line.rectTransform.SetAsFirstSibling();

            line.SetStartAndEndPoint(rectTransform, ParamOther.GetRectTransform());

            // Register as pairs
            pairs.Add(ParamOther, line);
            ParamOther.RegisterPair(this, line);

            if (Parent != null)
                Parent.CastObject<WispNodeEditor>().RegisterPair(connectableID, ParamOther.GetConnectableID());

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
    public static WispRerouteNode Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.RerouteNode, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.RerouteNode);
        }

        return go.GetComponent<WispRerouteNode>();
    }

    void OnDestroy()
    {
        DisconnectAllLines();
        OnDestroy_Procedure();
    }

    public override string GetJson()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("id", Index.ToString());
        result.Add("con_id", connectableID.ToString());
        result.Add("x", rectTransform.anchoredPosition3D.x.ToString());
        result.Add("y", rectTransform.anchoredPosition3D.y.ToString());
        result.Add("z", rectTransform.anchoredPosition3D.z.ToString());

        return result.ToJson();
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
        return "";
    }

    void IWispNodeEditorConnectable.DisconnectPair(IWispNodeEditorConnectable ParamPair)
    {
        pairs.Remove(ParamPair);
    }

    WispConnectableType IWispNodeEditorConnectable.GetConnectableType()
    {
        return WispConnectableType.RerouteNode;
    }

    string IWispNodeEditorConnectable.GetPairJson()
    {
        IWispNodeEditorConnectable me = this as IWispNodeEditorConnectable;
        return "{ \"type\" : " + me.GetConnectableType().ToString().SurroundWithDoubleQuotes() + ", \"id\" : " + Index.ToString().SurroundWithDoubleQuotes() + "}";
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