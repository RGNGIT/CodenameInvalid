using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WispExtensions;

public class WispVisualComponent : WispMonoBehaviour
{
    [Serializable]
    private class WispVisualComponentInfoContainer
    {
        public int id;
        public string name;
        public string go_name;
        public int parent_id;
        public string parent_name;
        public string parent_go_name;
    }
    
    public enum WispVisualStyleFollowRule { Self, Parent, Global }
    public enum WispBorderRule { Never, IfFocused, Always }

    [Header("Info")]
    [SerializeField] [ShowOnly] private int id = 0;
    [SerializeField] [ShowOnly] private int depthInHierarchy = 1;
    [SerializeField] [ShowOnly] protected bool isAvailableForInput = true;

    [Header("Style")]
    [SerializeField] protected WispGuiStyle style;
    [SerializeField] protected WispSubStyleRule subStyleRule;
    [SerializeField] private WispVisualStyleFollowRule styleFollowRule = WispVisualStyleFollowRule.Global;
    [SerializeField] private WispVisualComponent parent;
    [SerializeField] private WispBorderRule borderRule = WispBorderRule.Never;
    [SerializeField] [Range(0f, 1f)] private float opacity = 1.0f;

    [Header("Tooltip")]
    [SerializeField] private bool enableTooltip = false;
    [ConditionalHideBoolAttribute("enableTooltip", true, true)] [SerializeField] private WispTooltipConfiguration tooltipConfiguration;

    private int index = 0;
    private string value;
    private bool isMoving = false;
    private bool isRotating = false;
    private RawImage borders = null;
    private GameObject busyModeGameObject;
    private WispLoadingPanel loadingPanel;
    private bool busyMode = false;
    private Vector2 lastAnchorMinState;
    private Vector2 lastAnchorMaxState;
    private Vector2 lastPivotState;
    private List<WispVisualComponent> observers = new List<WispVisualComponent>();
    private UnityEvent onEdit;
    private bool blockObserverNotification = false;
    private bool isTransitioning = false;

    protected bool isSelected = false;
    protected RectTransform rectTransform;
    protected List<WispVisualComponent> children = new List<WispVisualComponent>();
    
    // protected bool isWysiwygReady = false; // Components that implements Wysiwyg should have this set to TRUE on awake.

    static private Dictionary<int, WispVisualComponent> globalVisualComponents;
    static private int currentID = 1;
    static private Canvas mainCanvas;
    static private WispGuiStyle globalStyle;
    static private WispVisualComponent focusedComponent;
    static private WispVisualComponent previouslyFocusedComponent;
    static protected WispVisualComponent currentlyOpenedVisualComponent;
    static private WispTooltip tooltip;
    static private WispVisualComponent currentTooltipOwner = null;
    static private WispVisualComponent lastSelectedInHierarchy = null;
    static private List<WispVisualComponent> wysiwygWaitingList = new List<WispVisualComponent>();
    static private List<WispVisualComponent> applyStyleWaitingList = new List<WispVisualComponent>();

    public static WispVisualComponent LastSelectedInHierarchy
    {
        get
        {
            return lastSelectedInHierarchy;
        }

        set
        {
            lastSelectedInHierarchy = value;
        }
    }

    public string Name
    {
        get => name;

        set
        {
            name = value;
        }
    }

    /// <summary>
    /// A generic integer value used for indexing multiple instances of the visual component.
    /// </summary>
    public int Index { get => index; set => index = value; }

    public RectTransform MyRectTransform 
    { 
        get
        {
            if (rectTransform != null)    
                return rectTransform;
            else
            {
                rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }
    }

    public WispVisualStyleFollowRule StyleFollowRule { get => styleFollowRule; set => styleFollowRule = value; }
    public bool BusyMode { get => busyMode; }
    public WispVisualComponent Parent { get => parent; }

    public virtual bool IsAvailableForInput
    {
        get
        {
            return isAvailableForInput && !busyMode;
        }

        set
        {
            isAvailableForInput = value;

            if (children == null)
            {
                WispVisualComponent.LogError("Null children list in component : " + gameObject.name);
                return;
            }

            foreach (WispVisualComponent child in children)
            {
                child.IsAvailableForInput = value;
            }
        }
    }

    public WispGuiStyle Style
    {
        get
        {
            return style;
        }
        set
        {
            if (value == null)
                LogWarning("Setting style to null. id : " + id + " name : " + name);
            
            style = value;
            if (Application.isEditor)
            {
                if (Application.isPlaying)
                {
                    ApplyStyle();
                }
                else
                {
                    ApplyStyleInEditor();
                }
            }
            else
            {
                ApplyStyle();
            }
        }
    }

    public static WispGuiStyle GlobalStyle
    {
        get
        {
            return globalStyle;
        }
    }

    public float Width
    {
        get
        {
            return rectTransform.rect.width;
        }
        set
        {
            rectTransform.sizeDelta = new Vector2(value, rectTransform.sizeDelta.y);
        }
    }

    public float Height
    {
        get
        {
            return rectTransform.rect.height;
        }
        set
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, value);
        }
    }

    public float Y
    {
        get
        {
            return rectTransform.anchoredPosition.y;
        }
    }

    public static WispVisualComponent FocusedComponent
    {
        get => focusedComponent;

        set
        {
            if (focusedComponent != null)
            {
                previouslyFocusedComponent = focusedComponent;
            }

            focusedComponent = value;

            if (previouslyFocusedComponent != null)
                previouslyFocusedComponent.UpdateBorders();
        }
    }

    public bool IsMoving { get => isMoving; }
    public int Id { get => id; }
    public int DepthInHierarchy { get => depthInHierarchy; }

    public WispBorderRule BorderRule 
    { 
        get
        {
            return borderRule;
        } 
        set
        {
            borderRule = value;
            UpdateBorders();
        }  
    }

    public float Opacity { get => opacity; set { opacity = value; ApplyStyle(); } }

    public bool BlockObserverNotification { get => blockObserverNotification; set => blockObserverNotification = value; }

    public bool EnableTooltip
    {
        get => enableTooltip;

        set
        {
            enableTooltip = value;

            if (enableTooltip)
            {
                if (GetComponent<WispTooltipTrigger>() == null)
                {
                    gameObject.AddComponent<WispTooltipTrigger>();
                    GetComponent<WispTooltipTrigger>().Delay = tooltipConfiguration.delay;
                }
            }
            else
            {
                if (GetComponent<WispTooltipTrigger>() != null)
                {
                    Destroy(GetComponent<WispTooltipTrigger>());
                }
            }
        }
    }

    public WispTooltipConfiguration TooltipConfiguration { get => tooltipConfiguration; }
    public WispSubStyleRule SubStyleRule { get => subStyleRule; set => subStyleRule = value; }
    public bool IsSelected { get => isSelected; }

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
    {
        base.Initialize();

        rectTransform = GetComponent<RectTransform>();

        if (id == 0)
        {
            id = currentID;
            currentID++;
        }
        else
        {
            id = currentID;
            currentID++;
            
            /*
            LogError("ID : " + id);

            bool r = false;
            foreach(KeyValuePair<int, WispVisualComponent> kvp in globalVisualComponents)
            {
                if (kvp.Value == this)
                {
                    r = true;
                    break;
                }
            }
            
            LogError("R : " + r);
            */
        }

        RegisterIntoVisualComponentList();

        if (enableTooltip)
        {
            gameObject.AddComponent<WispTooltipTrigger>();
            GetComponent<WispTooltipTrigger>().Delay = tooltipConfiguration.delay;
        }

        if (parent != null)
        {
            SetParent(parent, true);
        }

        return true;
    }

    private void RegisterIntoVisualComponentList()
    {
        if (globalVisualComponents == null)
        {
            globalVisualComponents = new Dictionary<int, WispVisualComponent>();
        }

        if (!globalVisualComponents.ContainsKey(id))
        {
            globalVisualComponents.Add(id, this);
        }
        else
        {
            if (globalVisualComponents[id] == this)
                WispVisualComponent.LogWarning("Trying to register WispVisualComponent a second time, Register operation was skipped.");
            else
                WispVisualComponent.LogError("Trying to register WispVisualComponent with an already existing ID, Register operation was unsuccesful.");
        }
    }

    /// <summary>
    /// Use this after destroying GameObjects that might have children inactive in hierarchy.
    /// </summary>
    public static void FlushNullVisualComponents()
    {
        foreach (KeyValuePair<int, WispVisualComponent> kvp in globalVisualComponents)
        {
            if (kvp.Value == null)
            {
                globalVisualComponents.Remove(kvp.Key);
            }
        }
    }

    // A Hack to make OnDestroy() inheritable.
    protected void OnDestroy_Procedure()
    {
        if (currentTooltipOwner == this)
            HideTooltip();

        if (parent != null)
            parent.UnregisterChild(this);

        UnregisterFromVisualComponentList();
    }

    /// <summary>
    /// ...
    /// </summary>
    void OnDestroy()
    {
        OnDestroy_Procedure();
    }

    void OnDisable()
    {
        if (currentTooltipOwner == this)
            HideTooltip();
    }

    internal void UnregisterFromVisualComponentList()
    {
        if (globalVisualComponents == null)
            return;

        globalVisualComponents.Remove(id);
    }

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    void Awake()
    {
        Initialize();
    }

    public virtual string GetValue()
    {
        return value;
    }

    public virtual void SetValue(string ParamValue)
    {
        value = ParamValue;
    }

    /// <summary>
    /// Set X Position.
    /// </summary>
    public void Set_X_Position(float ParamX)
    {
        RectTransform tmpRect = GetComponent<RectTransform>();

        if (tmpRect == null)
            return;

        tmpRect.anchoredPosition = new Vector3(ParamX, tmpRect.anchoredPosition.y);
    }

    /// <summary>
    /// Set Y Position.
    /// </summary>
    public void Set_Y_Position(float ParamY)
    {
        RectTransform tmpRect = GetComponent<RectTransform>();

        if (tmpRect == null)
            return;

        tmpRect.anchoredPosition = new Vector3(tmpRect.anchoredPosition.x, ParamY);
    }

    public virtual void Focus()
    {
        if (focusedComponent != null && focusedComponent != this)
            focusedComponent.Unfocus();

        focusedComponent = this;

        UpdateBorders();
    }

    public virtual void Unfocus()
    {
        focusedComponent = null;
        UpdateBorders();

        if (this is WispWindow)
        {
            // Do nothing
        }
        else
        {
            Close();
        }
    }

    /// <summary>
    /// <para>Set anchor position.</para>
    /// <para>Example : AnchorTo("left-top"); to anchor to the Top Left of the parent RectTransform.</para>
    /// <para>Another Example : AnchorTo("center-center"); to anchor to the Center of the parent RectTransform.</para>
    /// <para>X options : right,center and left.</para>
    /// <para>Y options : top,center and Bottom.</para>
    /// </summary>
    public void AnchorTo(string ParamAnchor)
    {
        string[] anchors = ParamAnchor.Split('-');

        if (anchors.Length != 2)
        {
            WispVisualComponent.LogError("Invalid anchoring parameters.");
            return;
        }

        float x = 0.5f;
        float y = 0.5f;

        if (anchors[0] == "right")
            x = 1f;
        else if (anchors[0] == "center")
            x = 0.5f;
        else if (anchors[0] == "left")
            x = 0f;
        else if (anchors[0] == "top")
            y = 1f;
        else if (anchors[0] == "center")
            y = 0.5f;
        else if (anchors[0] == "bottom")
            y = 0f;
        else
        {
            WispVisualComponent.LogError("Invalid anchoring parameters.");
            return;
        }

        if (anchors[1] == "top")
            y = 1f;
        else if (anchors[1] == "center")
            y = 0.5f;
        else if (anchors[1] == "bottom")
            y = 0f;
        else if (anchors[1] == "right")
            x = 1f;
        else if (anchors[1] == "center")
            x = 0.5f;
        else if (anchors[1] == "left")
            x = 0f;
        else
        {
            WispVisualComponent.LogError("Invalid anchoring parameters.");
            return;
        }

        GetComponent<RectTransform>().anchorMin = new Vector2(x, y);
        GetComponent<RectTransform>().anchorMax = new Vector2(x, y);
    }

    /// <summary>
    /// <para>Set pivot position.</para>
    /// <para>Example : AnchorTo("left-top"); to anchor to the Top Left of the parent RectTransform.</para>
    /// <para>Another Example : AnchorTo("center-center"); to anchor to the Center of the parent RectTransform.</para>
    /// <para>X options : right,center and left.</para>
    /// <para>Y options : top,center and Bottom.</para>
    /// </summary>
    public void PivotAround(string ParamPivot)
    {
        rectTransform.PivotAround(ParamPivot);
    }

    /// <summary>
    /// Define a fucntion to call when the button is pressed.
    /// </summary>
    public void AnchorToCenter()
    {
        GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
    }

    /// <summary>
    /// Define a fucntion to call when the button is pressed.
    /// </summary>
    public void AnchorStyleExpanded(bool ParamMaximize = false)
    {
        GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
        GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
        GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

        if (ParamMaximize)
        {
            SetRight(0);
            SetLeft(0);
            SetTop(0);
            SetBottom(0);
        }
    }

    /// <summary>
    /// Define a fucntion to call when the button is pressed.
    /// </summary>
    public virtual void ApplyStyle()
    {
        ApplyStyleToChildren();
        UpdateBorders();
    }

    /// <summary>
    /// Define a fucntion to call when the button is pressed.
    /// </summary>
    public virtual void ApplyStyleInEditor()
    {

    }

    /// <summary>
    /// Apply a style to all visual componnts.
    /// </summary>
    public static void ApplyGlobalStyle(WispGuiStyle ParamStyle)
    {
        globalStyle = ParamStyle;

        if (globalVisualComponents != null)
        {
            foreach (KeyValuePair<int, WispVisualComponent> kp in globalVisualComponents)
            {
                if (kp.Value.styleFollowRule == WispVisualStyleFollowRule.Global)
                    kp.Value.Style = ParamStyle;
            }
        }
    }

    /// <summary>
    /// Define a fucntion to call when the button is pressed.
    /// </summary>
    public static Canvas GetMainCanvas()
    {
        if (mainCanvas == null)
        {
            // Get canvas
            Canvas[] canvasArray = GameObject.FindObjectsOfType<Canvas>();

            if (canvasArray.Length > 0)
            {
                mainCanvas = canvasArray[0];
                return canvasArray[0];
            }
            else
            {
                return null;
            }
        }
        else
        {
            return mainCanvas;
        }
    }

    /// <summary>
    /// Use SetParent instead.
    /// </summary>
    public void RegisterChild(WispVisualComponent ParamChild)
    {
        if (ParamChild.Parent != null && ParamChild.Parent != this)
        {
            // print("Unregistring old parent. This : " + this.gameObject.name + " ||| Parent : " + parent?.gameObject.name + " ||| Child : " + ParamChild);
            // parent.UnregisterChild(this); // Fixed in 1.6.3
            ParamChild.Parent.UnregisterChild(this);
        }

        ParamChild.parent = this;

        if (!children.Contains(ParamChild))
            children.Add(ParamChild);
    }

    /// <summary>
    /// Don't use this function.
    /// </summary>
    public void UnregisterChild(WispVisualComponent ParamChild)
    {
        ParamChild.parent = null;

        if (!children.Contains(ParamChild))
        {
            children.Remove(ParamChild);
            print("Unregistring child from : " + name);
        }
    }

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet to children.
    /// </summary>
    public void ApplyStyleToChildren()
    {
        if (children == null)
            return;

        // Prepare a garbage list for possible null children.
        List<WispVisualComponent> garbage = null;

        foreach (WispVisualComponent child in children)
        {
            if (child.StyleFollowRule == WispVisualComponent.WispVisualStyleFollowRule.Parent)
            {
                if (child == null)
                {
                    // LogError("Null child while applying style. Parent Id : " + id + " name : " + name);

                    if (garbage == null)
                        garbage = new List<WispVisualComponent>();

                    garbage.Add(child);

                    continue;
                }
                
                child.style = style;
                child.opacity = opacity;
                child.isSelected = isSelected;
                child.ApplyStyle();
            }
        }

        // Clear garbage
        if (garbage != null)
        {
            foreach(WispVisualComponent vc in garbage)
            {
                children.Remove(vc);
                // print("Removing child due to NULL from : " + name);
            }
        }
    }

    /// <summary>
    /// What you see is what you get (For edit mode only) 
    /// </summary>
    public virtual void Wysiwyg()
    {

    }

    public void SetLeft(float ParamLeft)
    {
        rectTransform.offsetMin = new Vector2(ParamLeft, rectTransform.offsetMin.y);
    }

    public void SetRight(float ParamRight)
    {
        rectTransform.offsetMax = new Vector2(-ParamRight, rectTransform.offsetMax.y);
    }

    public void SetTop(float ParamTop)
    {
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -ParamTop);
    }

    public void SetBottom(float ParamBottom)
    {
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, ParamBottom);
    }

    public void TuneLeft(float ParamAmount)
    {
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x + ParamAmount, rectTransform.offsetMin.y);
    }

    public void TuneRight(float ParamAmount)
    {
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x + ParamAmount, rectTransform.offsetMax.y);
    }

    public void TuneTop(float ParamAmount)
    {
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, rectTransform.offsetMax.y + ParamAmount);
    }

    public void TuneBottom(float ParamAmount)
    {
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, rectTransform.offsetMin.y + ParamAmount);
    }

    /// <summary>
    /// Define a parent and chose if to follow style and be it's child in the hierarchy.
    /// </summary>
    public void SetParent(WispVisualComponent ParamParent, bool ParamFollowParentStyle, bool ParamParentInHierarchy = false)
    {
        if (ParamParent == null)
        {
            WispVisualComponent.LogError("Null parent while SetParent().");
            return;
        }

        ParamParent.RegisterChild(this);

        if (ParamFollowParentStyle)
        {
            StyleFollowRule = WispVisualStyleFollowRule.Parent;
            style = ParamParent.Style;

            if (isInitialized && ParamParent.IsInitialized)
                ApplyStyle();
        }

        if (ParamParentInHierarchy)
        {
            MyRectTransform.SetParent(ParamParent.MyRectTransform);
        }

        IsAvailableForInput = ParamParent.IsAvailableForInput;

        incrementDepthInHierarchy();
    }

    private void incrementDepthInHierarchy()
    {
        depthInHierarchy = Parent.depthInHierarchy + 1;

        foreach (WispVisualComponent child in children)
        {
            child.incrementDepthInHierarchy();
        }
    }

    public void AnchorAsBottomPanel(bool ParamMaximize)
    {
        MyRectTransform.anchorMin = new Vector2(0, 0);
        MyRectTransform.anchorMax = new Vector2(1, 0);

        if (ParamMaximize)
        {
            SetLeft(0);
            SetRight(0);
        }
    }

    public void AnchorAsTopPanel()
    {
        MyRectTransform.anchorMin = new Vector2(0, 1);
        MyRectTransform.anchorMax = new Vector2(1, 1);
        SetLeft(0);
        SetRight(0);
    }

    public virtual void UpdatePositions()
    {

    }

    /// <summary>
    /// The object will be seen moving to ParamDestination for ParamDuration seconds.
    /// </summary>
    public void SetPositionAsync(Vector3 ParamDestination, float ParamDuration, WispPositionReference ParamRef = WispPositionReference.Anchored)
    {
        if (isMoving)
            return;

        StartCoroutine(LerpPosition(ParamDestination, ParamDuration, ParamRef));
    }

    private IEnumerator LerpPosition(Vector3 ParamDestination, float ParamDuration, WispPositionReference ParamRef)
    {
        isMoving = true;

        Vector3 startPosition;
        if (ParamRef == WispPositionReference.World)
            startPosition = rectTransform.position;
        else // Default is Anchored
            startPosition = rectTransform.anchoredPosition3D;

        float timeOfTravel = ParamDuration;
        float currentTime = 0;
        float normalizedValue;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel;

            if (ParamRef == WispPositionReference.World)
                rectTransform.position = Vector3.Lerp(startPosition, ParamDestination, normalizedValue);
            else if (ParamRef == WispPositionReference.Anchored)
                rectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, ParamDestination, normalizedValue);

            yield return null;
        }

        isMoving = false;
    }

    /// <summary>
    /// The object will be seen rotating to ParamRotation for ParamDuration seconds.
    /// </summary>
    public void SetRotationAsync(Quaternion ParamRotation, float ParamDuration, WispRotationReference ParamRef = WispRotationReference.World)
    {
        if (isRotating)
            return;

        StartCoroutine(LerpRotation(ParamRotation, ParamDuration, ParamRef));
    }

    private IEnumerator LerpRotation(Quaternion ParamRotation, float ParamDuration, WispRotationReference ParamRef)
    {
        isRotating = true;

        Quaternion startRotation;
        if (ParamRef == WispRotationReference.Local)
            startRotation = rectTransform.localRotation;
        else // Default is world rotaion
            startRotation = rectTransform.rotation;

        float timeOfRotation = ParamDuration;
        float currentTime = 0;
        float normalizedValue;

        while (currentTime <= timeOfRotation)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfRotation;
            
            if (ParamRef == WispRotationReference.World)
                rectTransform.rotation = Quaternion.Lerp(startRotation, ParamRotation, normalizedValue);
            else if (ParamRef == WispRotationReference.Local)
                rectTransform.localRotation = Quaternion.Lerp(startRotation, ParamRotation, normalizedValue);

            yield return null;
        }

        isRotating = false;
    }

    public static void Log(string ParamMessage)
    {
        print("WispGUI : " + ParamMessage);
        WispDebugTools.AddNewLogEntry(ParamMessage, WispDebugTools.WispLogEntry.WispLogEntryType.Basic);
    }

    public static void LogFeedback(string ParamMessage)
    {
        print("<color=#7777FF>" + "WispGUI : " + ParamMessage + "</color>");
        WispDebugTools.AddNewLogEntry(ParamMessage, WispDebugTools.WispLogEntry.WispLogEntryType.Feedback);
    }

    public static void LogError(string ParamMessage)
    {
        print("<color=red>" + "WispGUI Error : " + ParamMessage + "</color>");
        WispDebugTools.AddNewLogEntry(ParamMessage, WispDebugTools.WispLogEntry.WispLogEntryType.Error);
    }

    public static void LogWarning(string ParamMessage)
    {
        print("<color=yellow>" + "WispGUI Warning : " + ParamMessage + "</color>");
        WispDebugTools.AddNewLogEntry(ParamMessage, WispDebugTools.WispLogEntry.WispLogEntryType.Warning);
    }

    public static string GetVisualComponentListAsJson()
    {
        string final = "";

        foreach (KeyValuePair<int, WispVisualComponent> kv in globalVisualComponents)
        {
            int parent_id = 0;
            string parent_name = "";
            string parent_go_name = "";

            if (kv.Value == null)
            {
                WispVisualComponent.LogError("Destroyed visual component is still registred and will be ignored during GetVisualComponentListAsJson() : " + kv.Value.GetType().ToString());
                continue;
            }

            if (kv.Value.Parent != null)
            {
                parent_id = kv.Value.Parent.id;
                parent_name = kv.Value.Parent.Name;
                parent_go_name = kv.Value.parent.gameObject.name;
            }
            else
            {
                parent_name = null;
                parent_go_name = null;
            }

            WispVisualComponentInfoContainer container = new WispVisualComponentInfoContainer();
            container.id = kv.Value.id;
            container.name = kv.Value.name;
            container.go_name = kv.Value.gameObject.name;
            container.parent_id = parent_id;
            container.parent_name = parent_name;
            container.parent_go_name = parent_go_name;

            final += JsonUtility.ToJson(container) + ",";
        }

        return "[" + final.Substring(0, final.Length - 1) + "]";
    }

    public bool IsMyParent_Recursive(WispVisualComponent ParamParent)
    {
        if (parent == null)
            return false;

        if (parent == ParamParent)
            return true;

        return parent.IsMyParent_Recursive(ParamParent);
    }

    public WispVisualComponent GetHighestParent()
    {
        if (parent == null)
            return null;

        return getHighestParentRecursive(this);
    }

    private WispVisualComponent getHighestParentRecursive(WispVisualComponent ParamStarter)
    {
        if (parent == null)
            return this;

        return getHighestParentRecursive(ParamStarter);
    }

    public WispVisualComponent GetHighestParentAvailableForInput()
    {
        if (parent == null || !isAvailableForInput)
            return null;

        return GetHighestParentAvailableForInputRecursive(this);
    }

    private WispVisualComponent GetHighestParentAvailableForInputRecursive(WispVisualComponent ParamLastFound)
    {
        if (parent == null)
        {
            if (isAvailableForInput)
                return this;
            else
                return ParamLastFound;
        }

        if (isAvailableForInput)
            return getHighestParentRecursive(this);
        else
            return ParamLastFound;
    }

    public static List<int> GetWispVisualComponentsAvailableForInput()
    {
        List<int> result = new List<int>();

        foreach (KeyValuePair<int, WispVisualComponent> kv in globalVisualComponents)
        {
            if (kv.Value == null)
            {
                WispVisualComponent.LogError("Null component at " + kv.Key);
                continue;
            }

            if (kv.Value.isActiveAndEnabled)
                result.Add(kv.Key);
        }

        return result;
    }

    // Makes ParamVisualComponents available for input.
    // Also makes everything else not available for input.
    public static void MakeAvailableForInput(List<int> ParamVisualComponents)
    {
        foreach(KeyValuePair<int, WispVisualComponent> kv in globalVisualComponents)
        {
            kv.Value.isAvailableForInput = false;
        }

        if (ParamVisualComponents == null)
            return;

        foreach (int id in ParamVisualComponents)
        {
            if (globalVisualComponents.ContainsKey(id))
            {
                globalVisualComponents[id].isAvailableForInput = true; 
            }
        }
    }

    public List<WispVisualComponent> GetChildrenRecursively()
    {
        List<WispVisualComponent> result = new List<WispVisualComponent>();

        foreach(WispVisualComponent vc in children)
        {
            result.Add(vc);
            vc.GetChildrenRecursive(result);
        }

        return result;
    }

    public List<int> GetChildrenRecursivelyAsId()
    {
        List<int> result = new List<int>();

        foreach (WispVisualComponent vc in children)
        {
            result.Add(vc.id);
            vc.GetChildrenRecursiveAsId(result);
        }

        return result;
    }

    private void GetChildrenRecursive(List<WispVisualComponent> ParamResult)
    {
        foreach (WispVisualComponent vc in children)
        {
            ParamResult.Add(vc);
            vc.GetChildrenRecursive(ParamResult);
        }
    }

    private void GetChildrenRecursiveAsId(List<int> ParamResult)
    {
        foreach (WispVisualComponent vc in children)
        {
            ParamResult.Add(vc.id);
            vc.GetChildrenRecursiveAsId(ParamResult);
        }
    }

    protected virtual void UpdateBorders()
    {
        // #if UNITY_EDITOR
        //     return;
        // #endif

        if (!Application.isPlaying)
            return;
        
        if (borderRule == WispBorderRule.Never)
        {
            if (borders == null)
                return;
            else if (borders != null)
            {
                Destroy(borders.gameObject);
                return;
            }
        }

        if (style == null || rectTransform == null)
        {
            WispVisualComponent.LogError("Unable to generate borders due to null dependency.");
        }

        if (style.EnableBorders)
        {
            if (borders != null)
            {
                // #if !UNITY_EDITOR
                    Destroy(borders.gameObject);
                // #endif
            }

            if (WispVisualComponent.focusedComponent == this)
                borders = WispTextureTools.GenerateRectTransformBorders(rectTransform, style.BorderWidth, colop(style.SelectedBorderColor), style.BorderType);
            else
            {
                if (borderRule == WispBorderRule.Always)
                borders = WispTextureTools.GenerateRectTransformBorders(rectTransform, style.BorderWidth, colop(style.BorderColor), style.BorderType);
            }
        }
        else
        {
            if (borders != null)
                Destroy(borders.gameObject);
        }
    }

    // col : color, op : opacity
    protected Color colop (Color ParamColor)
    {
        return ParamColor.ColorOpacity(opacity);
    }

    /// <summary>
    /// When ParemState is TRUE : Blur the background, Display a loading animation and set loading mode to TRUE.
    /// </summary>
    public virtual void SetBusyMode(bool ParamState)
    {
        if (ParamState)
        {
            GetBusyModeGameObject().SetActive(true);
            busyMode = true;
            loadingPanel.ApplyStyle();
            IsAvailableForInput = false;

            // ReorderBusyModeGameObjects();
        }
        else
        {
            GetBusyModeGameObject().SetActive(false);
            busyMode = false;

            if (Parent != null)
                if (Parent.IsAvailableForInput)
                    IsAvailableForInput = true;
        }
    }

    protected GameObject GetBusyModeGameObject()
    {
        if (busyModeGameObject != null)
        {
            return busyModeGameObject;
        }

        // busyModeGameObject = Instantiate(WispPrefabLibrary.Default.BackgroundObstructor, this.transform);
        busyModeGameObject = WispLoadingPanel.Create(transform).gameObject;
        // loadingPanel = Instantiate(WispPrefabLibrary.Default.LoadingPanel, busyModeGameObject.transform).GetComponent<WispLoadingPanel>();
        loadingPanel = busyModeGameObject.GetComponent<WispLoadingPanel>();
        loadingPanel.Initialize();
        loadingPanel.SetParent(this, true);

        // if (loadingPanel.Height > rectTransform.rect.height || loadingPanel.Width > rectTransform.rect.width)
        //     loadingPanel.SetDimensions(Mathf.Min(rectTransform.rect.height, rectTransform.rect.width).ToVector2());

        return busyModeGameObject;
    }

    /// <summary>
    /// Bring the background obtructor and the loading panel to the front of view.
    /// </summary>
    protected void ReorderBusyModeGameObjects()
    {
        busyModeGameObject.transform.SetAsLastSibling();
        loadingPanel.transform.SetAsLastSibling();
    }

    /// <summary>
    /// ...
    /// </summary>
    public void SaveAnchoringState()
    {
        lastAnchorMinState = rectTransform.anchorMin;
        lastAnchorMaxState = rectTransform.anchorMax;
    }

    /// <summary>
    /// ...
    /// </summary>
    public void SavePivotingState()
    {
        lastPivotState = rectTransform.pivot;
    }

    /// <summary>
    /// ...
    /// </summary>
    public void LoadAnchoringState()
    {
        if (lastAnchorMinState != null)
            rectTransform.anchorMin = lastAnchorMinState;

        if (lastAnchorMaxState != null)
            rectTransform.anchorMax = lastAnchorMaxState;
    }

    /// <summary>
    /// ...
    /// </summary>
    public void LoadPivotingState()
    {
        if (lastPivotState != null)
            rectTransform.pivot = lastPivotState;
    }

    /// <summary>
    /// ...
    /// </summary>
    public virtual string GetJson()
    {
        return "";
    }

    /// <summary>
    /// ...
    /// </summary>
    public virtual bool LoadFromJson(string ParamJson)
    {
        return true;
    }

    public static WispVisualComponent GetComponentById(int ParamId)
    {
        if (globalVisualComponents.ContainsKey(ParamId))
        {
            return globalVisualComponents[ParamId];
        }

        return null;
    }

    // !!! Slow !!!
    public static WispVisualComponent GetComponentByName(string ParamName)
    {
        foreach (KeyValuePair<int, WispVisualComponent> kvp in globalVisualComponents)
        {
            if (kvp.Value.name == ParamName)
                return kvp.Value;
        }

        return null;
    }

    public void AddOnEditAction(UnityAction ParamAction)
    {
        if (onEdit == null)
            onEdit = new UnityEvent();

        onEdit.AddListener(ParamAction);
    }

    public static void CloseCurrentlyOpenedVisualComponent()
    {
        if (currentlyOpenedVisualComponent != null)
            currentlyOpenedVisualComponent.Close();
    }

    public virtual void Open()
    {
        CloseCurrentlyOpenedVisualComponent();
        currentlyOpenedVisualComponent = this;
    }

    public virtual void Close()
    {
        currentlyOpenedVisualComponent = null;
    }

    #region Observer pattern

    public void RegisterObserver(WispVisualComponent ParamObserver)
    {
        observers.Add(ParamObserver);
    }

    public void NotifyObserversOnEdit()
    {
        if (blockObserverNotification)
            return;

        if (onEdit != null)
            onEdit.Invoke();
    }

    #endregion

    public void SetTooltipText(string ParamTtile, string ParamContent)
    {
        EnableTooltip = true;

        tooltipConfiguration.title = ParamTtile;
        tooltipConfiguration.content = ParamContent;
    }

    public void ShowTooltip()
    {
        currentTooltipOwner = this;

        if (tooltip == null)
        {
            tooltip = WispTooltip.Create(WispVisualComponent.GetMainCanvas().transform);
            tooltip.gameObject.AddComponent<WispHoverModePermanent>();
        }

        tooltip.gameObject.SetActive(true);

        tooltip.SetBothTitleAndContent(tooltipConfiguration.title, tooltipConfiguration.content);
        tooltip.Style = style;
        tooltip.UpdatePositions();

        WispAnimationFade animation = tooltip.gameObject.GetComponent<WispAnimationFade>();
        if (animation == null)
        {
            animation = tooltip.gameObject.AddComponent<WispAnimationFade>();
            animation.Duration = tooltipConfiguration.fadeDelay;
        }
        else
        {
            animation.RestartAnimation();
        }

        Cursor.visible = false;
    }

    internal void HideTooltip()
    {
        if (tooltip == null)
            return;

        tooltip.gameObject.SetActive(false);

        Cursor.visible = true;
    }

    public void AutoSetParents(bool ParamFollowParentStyle)
    {
        foreach(Transform child in transform)
        {
            WispVisualComponent vc = child.GetComponent<WispVisualComponent>();

            if (vc != null)
            {
                vc.SetParent(this, true);
                WispVisualComponent.LogFeedback("Parent updated for  : " + vc.name);
                
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(vc);
                UnityEditor.EditorUtility.SetDirty(vc.gameObject);
                #endif
            }

            /*
            if (child.childCount > 0)
            {
                if (vc != null)
                {
                    vc.AutoSetParents(ParamFollowParentStyle);
                }
                else
                {
                    WispVisualComponent.AutoSetParents_Recursive(child, this, ParamFollowParentStyle);
                }
            }
            */
        }
    }

    private static void AutoSetParents_Recursive(Transform ParamTransform, WispVisualComponent ParamParent, bool ParamFollowParentStyle)
    {
        foreach(Transform child in ParamTransform)
        {
            WispVisualComponent vc = child.GetComponent<WispVisualComponent>();

            if (vc != null)
            {
                vc.SetParent(ParamParent, true);
                WispVisualComponent.LogFeedback("Parent updated for  : " + vc.name);
            }

            if (child.childCount > 0)
            {
                if (vc != null)
                {
                    vc.AutoSetParents(ParamFollowParentStyle);
                }
                else
                {
                    WispVisualComponent.AutoSetParents_Recursive(child, ParamParent, ParamFollowParentStyle);
                }
            }
        }
    }

    public static Canvas AttachCanvas(WispVisualComponent ParamVisualComponent, int ParamOrder, bool ParamAddGraphicRayCaster)
    {
        Canvas result = ParamVisualComponent.gameObject.AddComponent<Canvas>();
        result.overrideSorting = true;
        result.sortingOrder = ParamOrder;

        if (ParamAddGraphicRayCaster)
        {
            ParamVisualComponent.gameObject.AddComponent<GraphicRaycaster>();
        }

        return result;
    }

    // Allows any child button to close it's parent window.
    public static void CloseParentWindow()
    {
        WispWindow window = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponentInParent<WispWindow>();

        if (window != null)
            window.Close();
    }

    public static List<int> GetVcIDList()
    {
        List<int> result = new List<int>();

        foreach(KeyValuePair<int, WispVisualComponent> kvp in globalVisualComponents)
        {
            result.Add(kvp.Key);
        }

        return result;
    }

    public string GetParentName()
    {
        if (parent == null)
            return "";
        else
            return parent.name;
    }

    public string GetChildCount()
    {
        return children.Count.ToString();
    }

    public List<string> GetChildrenNames()
    {
        List<string> result = new List<string>();

        foreach(WispVisualComponent vc in children)
        {
            result.Add(vc.name);
        }

        return result;
    }

    public static GameObject GiveMeLittleRedSquare()
    {
        return Instantiate(Resources.Load<GameObject>("RedSquare"));
    }

    // Use default transition (Fade) to transition between two components.
    public bool TransitionTo(WispVisualComponent ParamOther)
    {
        if (isTransitioning)
        {
            LogError("Unable to perform transition because component is already transitioning.");
            return false;
        }

        if (ParamOther.isTransitioning)
        {
            LogError("Unable to perform transition because the other component is already transitioning.");
            return false;
        }
        
        isTransitioning = true;
        WispAnimationFade animation_me = gameObject.AddComponent<WispAnimationFade>();
        animation_me.Duration = 0.5f;
        animation_me.AlphaStartingValue = 1;
        animation_me.AlphaFinalValue = 0;
        animation_me.OnEnd.AddListener( delegate { isTransitioning = false; gameObject.SetActive(false); } );
        animation_me.StartAnimation();

        ParamOther.gameObject.SetActive(true);
        ParamOther.isTransitioning = true;
        WispAnimationFade animation_other = ParamOther.gameObject.AddComponent<WispAnimationFade>();
        animation_other.Duration = 0.5f;
        animation_other.AlphaStartingValue = 0;
        animation_other.AlphaFinalValue = 1;
        animation_other.OnEnd.AddListener( delegate { ParamOther.isTransitioning = false; } );
        animation_other.StartAnimation();

        return true;
    }

    public bool SetActiveUsingTransition(bool ParamState, UnityAction ParamActionAtEnd = null)
    {
        if (ParamState && gameObject.activeInHierarchy)
        {
            LogWarning("Component is already in the target state : " + ParamState);
            return true;
        }
        
        if (isTransitioning)
        {
            LogError("Unable to perform transition because component is already transitioning.");
            return false;
        }

        gameObject.SetActive(true);
        isTransitioning = true;

        WispAnimationFade animation_me = gameObject.AddComponent<WispAnimationFade>();
        animation_me.Duration = 0.5f;

        if (ParamState)
        {
            animation_me.AlphaStartingValue = 0;
            animation_me.AlphaFinalValue = 1;
            animation_me.OnEnd.AddListener( delegate { isTransitioning = false; } );

        }
        else
        {
            animation_me.AlphaStartingValue = 1;
            animation_me.AlphaFinalValue = 0;
            animation_me.OnEnd.AddListener( delegate { isTransitioning = false; gameObject.SetActive(false); } );
        }

        if (ParamActionAtEnd != null)
        {
            animation_me.OnEnd.AddListener(ParamActionAtEnd);
        }

        animation_me.StartAnimation();

        return true;
    }

    public virtual void Select()
    {
        isSelected = true;
        ApplyStyle();
    }

    public virtual void Unselect()
    {
        isSelected = false;
        ApplyStyle();
    }

    virtual protected void onFocus()
    {
        WispVisualComponent.FocusedComponent = this;
        UpdateBorders();
    }

    virtual protected void onFocusLost()
    {
        WispVisualComponent.FocusedComponent = null;
        UpdateBorders();
    }

    public static void StrategicDestroy(GameObject ParamGameObject)
    {
        #if UNITY_EDITOR
            if (Application.isEditor && !Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall+=()=>
                {
                    DestroyImmediate(ParamGameObject);
                };
            }
            else
            {
                Destroy(ParamGameObject);
            }
        #endif
    }

    public static void RebuildGlobalComponentList()
    {
        if (globalVisualComponents == null)
        {
            LogError("No Global Component List");
            return;
        }
        
        List<WispVisualComponent> tmpComps = new List<WispVisualComponent>();
        
        foreach(KeyValuePair<int, WispVisualComponent> kvp in globalVisualComponents)
        {
            kvp.Value.id = 0;
            tmpComps.Add(kvp.Value);
        }

        globalVisualComponents.Clear();

        currentID = 1;
        
        foreach(WispVisualComponent c in tmpComps)
        {
            c.id = currentID;
            currentID++;
            c.RegisterIntoVisualComponentList();
        }
    }

    public static bool ProcessWysiwygWaitingList()
    {
        if (wysiwygWaitingList.Count == 0)
        {
            return false;
        }

        foreach(WispVisualComponent vc in wysiwygWaitingList)
        {
            // vc?.Wysiwyg(); // This did not work correctly :( We will stick with normal null check for now.

            if (vc == null)
            {
                // print("NULL");
            }
            else
            {
                vc.Wysiwyg();
            }
        }

        wysiwygWaitingList.Clear();
        return true;
    }

    public void RegisterWysiwygRequest()
    {
        if (wysiwygWaitingList.Contains(this))
            return;

        wysiwygWaitingList.Add(this);
    }

    protected bool CheckIgnoreWysiwigOnValidate()
    {
        if (gameObject.scene == null || !isInitialized /*|| StyleFollowRule == WispVisualStyleFollowRule.Parent*/)
            return true; // Ignore the OnValidate event

        return false; // Don't ignore this OnValidate, Wysiwig must be performed
    }

    public static bool ProcessApplyStyleWaitingList()
    {
        if (applyStyleWaitingList.Count == 0)
        {
            return false;
        }

        foreach(WispVisualComponent vc in applyStyleWaitingList)
        {
            // print("S PROCESS : " + vc.name);
            vc?.ApplyStyle();
        }

        applyStyleWaitingList.Clear();
        return true;
    }

    public void RegisterApplyStyleRequest()
    {
        if (applyStyleWaitingList.Contains(this))
            return;

        applyStyleWaitingList.Add(this);
    }

    protected bool CheckIgnoreApplyStyle(bool ParamIsWysiwygReady)
    {
        if (style == null)
            return true;

        if (!Application.isPlaying && ParamIsWysiwygReady)
            return true;

        return false;
    }
}

[Serializable]
public class WispTooltipConfiguration
{
    public string title = "";
    public string content = "";
    public float delay = 0.25f;
    public float fadeDelay = 0.25f;
}

public enum WispSubStyleRule { None, Background, Widget, Container, Picture, Icon, TitleBar }
public enum WispFontSize { Normal, Header, Label, Quiet }
public enum WispPositionReference { World, Anchored }
public enum WispRotationReference { World, Local }