using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispLineRenderer : MaskableGraphic
{
    [SerializeField] Vector2 start;
    [SerializeField] Vector2 end;
    [SerializeField] float width = 1f;

    private Vector2 p1;
    private Vector2 p2;
    private Vector2 p3;
    private Vector2 p4;

    private bool useSegmentation = false;
    private float segmentLength = 16;
    private List<Vector2> segmentPoints = new List<Vector2>();

    private bool drawDots = false;
    private List<GameObject> dots = new List<GameObject>();

    public float Width { get => width; set => width = value; }

    /// <summary>
    /// ...
    /// </summary>
    public static WispLineRenderer Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.LineRenderer, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.LineRenderer);
        }

        return go.GetComponent<WispLineRenderer>();
    }

    public void SetStartAndEndPoint(Vector2 ParamStart, Vector2 ParamEnd)
    {
        start = ParamStart;
        end = ParamEnd;

        if (useSegmentation)
        {
            GenerateSegmentationDots();
            
            if (drawDots)
                DrawDots();
        }

        OnRectTransformDimensionsChange();
        SetVerticesDirty();
        SetMaterialDirty();
    }

    private void GenerateSegmentationDots()
    {
        float distance = Vector2.Distance(start, end);

        int segmentCount = 0;

        if (distance <= segmentLength)
        {
            segmentCount = 1;
        }
        else
        {
            segmentCount = Mathf.FloorToInt(distance/segmentLength);
        }

        segmentPoints.Clear();

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (i * segmentLength) / distance; // Lerp ratio
            segmentPoints.Add(Vector2.Lerp(start, end, t));
        }
    }

    public void SetStartAndEndPoint(RectTransform ParamStart, RectTransform ParamEnd)
    {
        SetStartAndEndPoint(switchToRectTransform(ParamStart, GetComponent<RectTransform>()), switchToRectTransform(ParamEnd, GetComponent<RectTransform>()));
    }

    protected override void OnPopulateMesh(VertexHelper ParamVertexHelper)
    {
        if (useSegmentation)
            PopulateWithSegmentedLine(ParamVertexHelper);
        else
            PopulateWithQuad(ParamVertexHelper);
    }

    private void PopulateWithQuad(VertexHelper ParamVertexHelper)
    {
        ParamVertexHelper.Clear();

        var i = ParamVertexHelper.currentVertCount;

        UIVertex vert = new UIVertex();
        vert.color = this.color;

        Vector2 offsetVector = start - end;
        float absoluteAngle = Mathf.Atan2(offsetVector.y, offsetVector.x) * Mathf.Rad2Deg;

        p1 = WispPolarCoordinates.GetPointAroundPoint(start, width, absoluteAngle + 90);
        p2 = WispPolarCoordinates.GetPointAroundPoint(start, width, absoluteAngle - 90);
        p3 = WispPolarCoordinates.GetPointAroundPoint(end, width, absoluteAngle - 90);
        p4 = WispPolarCoordinates.GetPointAroundPoint(end, width, absoluteAngle + 90);

        vert.position = p1;
        ParamVertexHelper.AddVert(vert);

        vert.position = p2;
        ParamVertexHelper.AddVert(vert);

        vert.position = p3;
        ParamVertexHelper.AddVert(vert);

        vert.position = p4;
        ParamVertexHelper.AddVert(vert);

        ParamVertexHelper.AddTriangle(i + 0, i + 2, i + 1);
        ParamVertexHelper.AddTriangle(i + 3, i + 2, i + 0);
    }

    private void PopulateWithSegmentedLine(VertexHelper ParamVertexHelper)
    {
        ParamVertexHelper.Clear();

        var i = ParamVertexHelper.currentVertCount;

        UIVertex vert = new UIVertex();
        // vert.color = this.color;

        Vector2 tail = Vector2.negativeInfinity;

        foreach(Vector2 p in segmentPoints)
        {
            if (tail == Vector2.negativeInfinity) // Is this the first point ?
            {
                tail = p;
                continue;
            }
            
            vert.color = WispColor.RandomColor();
            
            Vector2 offsetVector = tail - p;
            float absoluteAngle = Mathf.Atan2(offsetVector.y, offsetVector.x) * Mathf.Rad2Deg;

            p1 = WispPolarCoordinates.GetPointAroundPoint(tail, width, absoluteAngle + 90);
            p2 = WispPolarCoordinates.GetPointAroundPoint(tail, width, absoluteAngle - 90);
            p3 = WispPolarCoordinates.GetPointAroundPoint(p, width, absoluteAngle - 90);
            p4 = WispPolarCoordinates.GetPointAroundPoint(p, width, absoluteAngle + 90);

            vert.position = p1;
            ParamVertexHelper.AddVert(vert);

            vert.position = p2;
            ParamVertexHelper.AddVert(vert);

            vert.position = p3;
            ParamVertexHelper.AddVert(vert);

            vert.position = p4;
            ParamVertexHelper.AddVert(vert);

            ParamVertexHelper.AddTriangle(i + 0, i + 2, i + 1);
            ParamVertexHelper.AddTriangle(i + 3, i + 2, i + 0);

            i = ParamVertexHelper.currentVertCount;
            tail = p;
        }
    }

    private void DrawDots()
    {
        foreach(GameObject go in dots)
        {
            Destroy(go);
        }

        dots.Clear();

        foreach(Vector2 p in segmentPoints)
        {
            GameObject go = WispVisualComponent.GiveMeLittleRedSquare();
            go.transform.SetParent(transform.parent);
            go.GetComponent<RectTransform>().anchoredPosition = p;
            dots.Add(go);
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
        SetMaterialDirty();
    }

    // From : https://forum.unity.com/threads/find-anchoredposition-of-a-recttransform-relative-to-another-recttransform.330560/
    private Vector2 switchToRectTransform(RectTransform from, RectTransform to)
    {
        Vector2 localPoint;
        Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
        screenP += fromPivotDerivedOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
        Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
        return to.anchoredPosition + localPoint - pivotDerivedOffset;
    }

    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse is over GameObject.");
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on GameObject.");
    }
}