using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WispExtensions;

public class WispResizingHandle : WispVisualComponent
{
    [Header("Resizing Handle")]
    [SerializeField] private RectTransform target;
    [SerializeField] private WispResizingDirection directionMode = WispResizingDirection.Directional;
    [SerializeField] private float minimumWidth = 64f;
    [SerializeField] private float minimumHeight = 64f;

    [Header("Grid Options")]
    [SerializeField] private bool SnapToGrid = false;
    [ConditionalHideBoolAttribute("SnapToGrid", true, true)] [SerializeField] private int gridX = 32;
    [ConditionalHideBoolAttribute("SnapToGrid", true, true)] [SerializeField] private int gridY = 32;
    [ConditionalHideBoolAttribute("SnapToGrid", true, true)] [SerializeField] private float resizingSpeed = 7.5f;
    
    private Vector3 preDragTargetPosition;
    private Vector3 corner;
    private Vector3 minimumRectPosition;

    private Vector3 preDragMousePosition;
    private Vector2 preDragSize;
    private Vector3 lastOffsetValue = Vector3.zero;
    private UnityAction actionOnDrag = null;
    private UnityAction actionOnEndDrag = null;
    
    private Coroutine smoothResizeCoroutine = null;
    private bool isOnSmoothResize = false;
    private Vector3 rememberStartOffset;

    private RectTransform rootCanvasRT;
    private Vector3 lastValidMousePos;
    private float canvasMinX;
    private float canvasMaxX;
    private float canvasMinY;
    private float canvasMaxY;

    public RectTransform Target { get => target; set => target = value; }
    public UnityAction ActionOnDrag { get => actionOnDrag; set => actionOnDrag = value; }
    public UnityAction ActionOnEndDrag { get => actionOnEndDrag; set => actionOnEndDrag = value; }

    public RawImage Base
    {
        get
        {
            return GetComponent<RawImage>();
        }
    }

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
		ApplyStyle();
    }

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

    public void OnBeginDrag()
    {
        if (target == null)
            return;

        lastOffsetValue = Vector3.zero;

        // Stuff to prevent resizing out of bounds
        rootCanvasRT = GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();
        canvasMinX = rootCanvasRT.position.x - rootCanvasRT.rect.width/2;
        canvasMaxX = rootCanvasRT.position.x + rootCanvasRT.rect.width/2;
        canvasMinY = rootCanvasRT.position.y + rootCanvasRT.rect.height/2;
        canvasMaxY = rootCanvasRT.position.y - rootCanvasRT.rect.height/2;
        lastValidMousePos = Input.mousePosition;

        preDragTargetPosition = target.anchoredPosition3D;
        corner = target.anchoredPosition3D - new Vector3(target.rect.width/2, -target.rect.height/2, 0);
        minimumRectPosition = corner + new Vector3(minimumWidth/2, -minimumHeight/2, 0);

        preDragMousePosition = Input.mousePosition;
        preDragSize = target.sizeDelta;
    }

    public void OnDrag()
    {
        if (target == null)
            return;

        Vector2 v = rootCanvasRT.GetMousePositionInMe();
        Vector3 offset;

        offset = Input.mousePosition - preDragMousePosition;

        if (directionMode == WispResizingDirection.Centered)
            offset *= 2;

        if (SnapToGrid)
        {
            offset = offset.SnapToGrid(gridX, gridY, 1);

            if (offset != lastOffsetValue)
            {
                Vector3 tmp = lastOffsetValue;
                lastOffsetValue = offset;
                
                if (smoothResizeCoroutine != null)
                {
                    if (isOnSmoothResize)
                    {
                        StopCoroutine(smoothResizeCoroutine);
                        isOnSmoothResize = false;

                        smoothResizeCoroutine = StartCoroutine(DoSmoothResize(rememberStartOffset, offset));
                    }
                    else
                    {
                        smoothResizeCoroutine = StartCoroutine(DoSmoothResize(tmp, offset));
                    }
                }
                else
                {
                    smoothResizeCoroutine = StartCoroutine(DoSmoothResize(tmp, offset));
                }
                
            }
        }
        else
        {
            UpdateDimensions(offset);
        }
        

        if (actionOnDrag != null)
            actionOnDrag.Invoke();
    }

    private IEnumerator DoSmoothResize(Vector3 ParamStartOffset, Vector3 ParamEndOffset)
    {
        isOnSmoothResize = true;
        rememberStartOffset = ParamStartOffset; // Useful in case we stop mid resizing, so that we can continue from where we stopped.
        Vector3 current = ParamStartOffset;
        float t = 0;

        while (t < 1)
        {
            t += resizingSpeed * Time.deltaTime;
            current = Vector3.Lerp(ParamStartOffset, ParamEndOffset, t);
            rememberStartOffset = current;
            UpdateDimensions(current);
            yield return new WaitForEndOfFrame();
        }
        
        isOnSmoothResize = false;
        yield return null;
    }

    private void UpdateDimensions(Vector3 ParamOffset)
    {
        target.sizeDelta = new Vector2(preDragSize.x + ParamOffset.x, preDragSize.y - ParamOffset.y);

        if (directionMode == WispResizingDirection.Directional)
            target.anchoredPosition3D = preDragTargetPosition + ParamOffset / 2;

        if (target.rect.width < minimumWidth && target.rect.height < minimumHeight)
        {
            target.SetRectWidth(minimumWidth);
            target.SetRectHeight(minimumHeight);

            if (directionMode == WispResizingDirection.Directional)
                target.anchoredPosition3D = minimumRectPosition;
        }
        else if (target.rect.width < minimumWidth || target.rect.height < minimumHeight)
        {
            if (target.rect.width < minimumWidth)
            {
                target.SetRectWidth(minimumWidth);

                if (directionMode == WispResizingDirection.Directional)
                    target.anchoredPosition3D = new Vector3(minimumRectPosition.x, target.anchoredPosition3D.y, target.anchoredPosition3D.z);
            }

            if (target.rect.height < minimumHeight)
            {
                target.SetRectHeight(minimumHeight);

                if (directionMode == WispResizingDirection.Directional)
                    target.anchoredPosition3D = new Vector3(target.anchoredPosition3D.x, minimumRectPosition.y, target.anchoredPosition3D.z);
            }

        }
        
        if (MyRectTransform.position.x > canvasMaxX)
        {
            float overflow = MyRectTransform.position.x - canvasMaxX;
            target.SetRectWidth(target.rect.width - overflow);
            target.anchoredPosition3D = new Vector3(target.anchoredPosition3D.x - overflow/2, target.anchoredPosition3D.y, target.anchoredPosition3D.z);
        }

        if (MyRectTransform.position.y < canvasMaxY)
        {
            float overflow = MyRectTransform.position.y - canvasMaxY;
            target.SetRectHeight(target.rect.height + overflow);
            target.anchoredPosition3D = new Vector3(target.anchoredPosition3D.x, target.anchoredPosition3D.y - overflow/2, target.anchoredPosition3D.z);
        }
    }

    public void OnEndDrag()
    {
        if (actionOnEndDrag != null)
            actionOnEndDrag.Invoke();
    }

    /// <summary>
    /// Create (For non wisp components).
    /// </summary>
    public static WispResizingHandle Create(Transform ParamTransform, RectTransform ParamTarget, float ParamMinimumWidth, float ParamMinimumHeight)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.ResizingHandle, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.ResizingHandle);
        }

        WispResizingHandle result = go.GetComponent<WispResizingHandle>();
        
        result.target = ParamTarget;
        result.minimumWidth = ParamMinimumWidth;
        result.minimumHeight = ParamMinimumHeight;

        return result;
    }

    /// <summary>
    /// Create.
    /// </summary>
    public static WispResizingHandle Create(WispVisualComponent ParamParent, Vector2 ParamMinimumSize)
    {
        GameObject go;
        go = Instantiate(WispPrefabLibrary.Default.ResizingHandle, ParamParent.MyRectTransform);

        WispResizingHandle result = go.GetComponent<WispResizingHandle>();
        result.SetParent(ParamParent, true);

        result.target = ParamParent.MyRectTransform;
        result.minimumWidth = ParamMinimumSize.x;
        result.minimumHeight = ParamMinimumSize.y;

        return result;
    }

    /// <summary>
    /// Create a WispResizingHandle.
    /// </summary>
    public static WispResizingHandle Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.ResizingHandle, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.ResizingHandle);
        }

        return go.GetComponent<WispResizingHandle>();
    }

    public override void ApplyStyle()
    {
        if (style == null)
            return;
        
        base.ApplyStyle();

        GetComponent<RawImage>().color = colop(style.ResizingHandleColor);
    }
}

public enum WispResizingDirection {Directional, Centered}