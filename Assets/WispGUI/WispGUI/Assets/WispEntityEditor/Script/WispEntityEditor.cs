using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispEntityEditor : WispVisualComponent
{
    [Header("Element rendering")]
    [SerializeField] private int columnCount = 2;
    [SerializeField] private float startingX = 0;
    [SerializeField] private float xMargin = 16;
    [SerializeField] private float startingY = 64;
    [SerializeField] private float componentWidth = 256;
    [SerializeField] private float componentHeight = 32;
    [SerializeField] private float componentVerticalSpacing = 32;
    [SerializeField] private float positioningDuration = 0.5f;

    [Header("Editor options")]
    [SerializeField] private bool enableButtonPanel = true;
    [SerializeField] private bool useEnterButtonForConfirmation = true;

    [Header("Prefab")]
    [SerializeField] private GameObject buttonPanelPrefeb;

    private List<WispVisualComponent> visualComponents;
    private Dictionary<string, WispElement> elements = new Dictionary<string, WispElement>();
    private WispEntityInstance currentInstance;
    private GameObject buttonPanel;
    private Button okButton;
    private Button cancelButton;
    private WispScrollView scrollView;
    private RectTransform contentRect;
    private ScrollRect scrollRect;
    private Dictionary<int, WispVisualComponent> orderedVisualComponents = new Dictionary<int, WispVisualComponent>();
    private int currentFocusIndex = 0;

    public int ColumnCount { get => columnCount; set => columnCount = value; }
    public int VisualComponentCount { get => visualComponents.Count; }
    public float StartingX { get => startingX; set => startingX = value; }
    public float XMargin { get => xMargin; set => xMargin = value; }
    public float StartingY { get => startingY; set => startingY = value; }
    public float ComponentWidth { get => componentWidth; set => componentWidth = value; }
    public float ComponentHeight { get => componentHeight; set => componentHeight = value; }
    public float ComponentVerticalSpacing { get => componentVerticalSpacing; set => componentVerticalSpacing = value; }
    public RectTransform ContentRect { get => contentRect; }
    public WispScrollView ScrollView { get => scrollView; }
    public bool EnableButtonPanel { get => enableButtonPanel; set {enableButtonPanel = value; buttonPanel.SetActive(value);} }


    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        ApplyStyle();
    }

    //...
    public static WispEntityEditor Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.EntityEditor, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.EntityEditor);
        }

        return go.GetComponent<WispEntityEditor>();
    }

    private void OnTabNavigationKeyPress()
    {
        if (WispVisualComponent.FocusedComponent.IsMyParent_Recursive(this))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                FocusPrevious();
            else
                FocusNext();
        }
    }

    // ...
    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        // ---------------------------------------------------------------------

        visualComponents = new List<WispVisualComponent>();

        WispKeyBoardEventSystem.AddEventOnKey(KeyCode.Return, invokeOnOkButtonClick, this);
        WispKeyBoardEventSystem.AddEventOnKey(KeyCode.KeypadEnter, invokeOnOkButtonClick, this);
        WispKeyBoardEventSystem.AddEventOnKey(KeyCode.Tab, OnTabNavigationKeyPress, this);

        buttonPanel = transform.Find("WispButtonPanel").gameObject;

        WispButtonPanel panel = buttonPanel.GetComponent<WispButtonPanel>();
        panel.Initialize();
        panel.SetParent(this, true);

        panel.MyRectTransform.anchorMin = new Vector2(0, 0);
        panel.MyRectTransform.anchorMax = new Vector2(1, 0);
        panel.SetLeft(0);
        panel.SetRight(0);

        panel.AddButton("BtnOk", "Ok", null);
        panel.AddButton("BtnCancel", "Cancel", null);
        okButton = buttonPanel.transform.Find("BtnOk").GetComponent<Button>();
        cancelButton = buttonPanel.transform.Find("BtnCancel").GetComponent<Button>();

        buttonPanel.SetActive(enableButtonPanel);

        scrollView = transform.Find("WispScrollView").GetComponent<WispScrollView>();
        scrollView.Initialize();
        scrollView.SetParent(this, true);

        contentRect = scrollView.ContentRect;
        scrollRect = scrollView.ScrollRect;

        // ---------------------------------------------------------------------

        isInitialized = true;

        return true;
    }

    // ...
    public void Clear()
    {
        foreach (WispVisualComponent c in visualComponents)
        {
            Destroy(c);
        }

        contentRect.sizeDelta = new Vector2(0, 0);

        visualComponents.Clear();
        elements.Clear();
    }

    // ...
    public void RenderInstance(WispEntityInstance ParamEntityInstance)
    {
        currentInstance = ParamEntityInstance;

        WispEntityProperty[] tmpProperties = ParamEntityInstance.GetArrayOfProperties();

        for (int i = 0; i < tmpProperties.Length; i++)
        {
            GameObject go = tmpProperties[i].DrawVisualComponent(this, contentRect, Vector3.zero, tmpProperties[i]);
            WispVisualComponent vc = go.GetComponent<WispVisualComponent>();

            vc.GetComponent<WispElement>().LinkedProperty = tmpProperties[i];
            visualComponents.Add(vc);

            elements.Add(tmpProperties[i].Name, vc.GetComponent<WispElement>());
        }

        if (ParamEntityInstance.DefaultFocusPropertyName != "")
        {
            if (elements.ContainsKey(ParamEntityInstance.DefaultFocusPropertyName))
            {
                elements[ParamEntityInstance.DefaultFocusPropertyName].Focus();
            }
        }
        else
        {
            Focus(1);
        }
        
        StartCoroutine(UpdateVisualComponentsPosition());
    }

    private bool AddElement(string ParamIdentifier, WispElement ParamElement)
    {
        if (elements.ContainsKey(ParamIdentifier))
        {
            WispVisualComponent.LogError("Invalid element identifier.");
            return false;
        }

        elements.Add(ParamIdentifier, ParamElement);

        return true;
    }

    public WispElement GetElement(string ParamIdentifier)
    {
        return elements[ParamIdentifier];
    }

    public T GetElementTyped<T>(string ParamIdentifier)
    {
        return elements[ParamIdentifier].CastObject<T>();
    }

    // ...
    protected void FocusNext()
    {
        if (!orderedVisualComponents.ContainsKey(currentFocusIndex + 1))
            Focus(1);
        else
            Focus(currentFocusIndex + 1);
    }

    // ...
    protected void FocusPrevious()
    {
        if (!orderedVisualComponents.ContainsKey(currentFocusIndex - 1))
            Focus(orderedVisualComponents.Count);
        else
            Focus(currentFocusIndex-1);
    }

    // ...
    private void Focus(int ParamIndex)
    {
        if (!orderedVisualComponents.ContainsKey(ParamIndex))
        {
            LogError("Invalid index while trying to focus a component.");
            return;
        }

        if (orderedVisualComponents.ContainsKey(currentFocusIndex) && orderedVisualComponents[currentFocusIndex] != null)
            orderedVisualComponents[currentFocusIndex].Unfocus();

        orderedVisualComponents[ParamIndex].Focus();
        currentFocusIndex = ParamIndex;
        scrollView.ScrollToPosition_Async(orderedVisualComponents[ParamIndex].MyRectTransform.GetAnchoredPostionIn(contentRect), 0.25f);
    }

    // ...
    public WispVisualComponent GetVisualComponent(string ParamPropertyName)
    {
        foreach (WispVisualComponent component in visualComponents)
        {

            if (component.GetComponent<WispElement>().LinkedProperty.Name == ParamPropertyName)
            {

                return component;

            }

        }

        return null;
    }

    // ...
    public WispVisualComponent GetVisualComponent(int ParamIndex)
    {
        return visualComponents[ParamIndex];
    }

    /// <summary>
    /// Define a fucntion to call when the Ok button is pressed.
    /// </summary>
    public void AddOkOnClickAction(UnityEngine.Events.UnityAction ParamAction)
    {
        if (okButton != null)
            okButton.onClick.AddListener(ParamAction);
    }

    /// <summary>
    /// Define a fucntion to call when the Cancel button is pressed.
    /// </summary>
    public void AddCancelOnClickAction(UnityEngine.Events.UnityAction ParamAction)
    {
        if (cancelButton != null)
            cancelButton.onClick.AddListener(ParamAction);
    }

    public override void UpdatePositions()
    {
        StartCoroutine(UpdateVisualComponentsPosition());
    }

    private IEnumerator UpdateVisualComponentsPosition()
    {
        yield return new WaitForEndOfFrame();

        WispColumnDistanceCalculator distanceCalculator = new WispColumnDistanceCalculator(columnCount, componentWidth, contentRect.rect.width, startingY, startingX, xMargin);

        WispEntityProperty[] tmpProperties = currentInstance.GetArrayOfProperties();

        for (int i = 0; i < tmpProperties.Length; i++)
        {
            if (tmpProperties[i].RequireFullRow)
                distanceCalculator.ForceColumnToFirst(0);

            WispVisualComponent vc = tmpProperties[i].Element;
            Vector3 position = new Vector3(distanceCalculator.GetCurrentX(), distanceCalculator.GetCurrentY() * (-1));

            float width;
            float height;

            if (tmpProperties[i].RequireFullWidth)
            {
                width = contentRect.rect.width - (distanceCalculator.GetColumnXPosition(0) * 2);
            }
            else
            {
                width = componentWidth - XMargin*2;
                width = Mathf.Clamp(width, 0, contentRect.rect.width - xMargin*2);
            }
            
            if (tmpProperties[i].Square)
            {
                height = width;
            }
            else
                height = tmpProperties[i].VerticalHeight;

            vc.MyRectTransform.sizeDelta = new Vector2(width, height);
            vc.ApplyStyle();
            vc.SetPositionAsync(position, positioningDuration);

            distanceCalculator.PushY(height + tmpProperties[i].ExtraHeight);
            distanceCalculator.GetNextColumn();

            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, distanceCalculator.GetMaxY());

            scrollRect.CalculateLayoutInputHorizontal();
            scrollRect.CalculateLayoutInputVertical();
        }

        yield break;
    }

    private void invokeOnOkButtonClick ()
    {
        if (enableButtonPanel && useEnterButtonForConfirmation)
        {
            okButton.onClick.Invoke();
        }
    }

    /// <summary>
    /// When ParamState is TRUE : Blur the view, Display a loading animation and set loading mode to TRUE.
    /// </summary>
    public override void SetBusyMode(bool ParamState)
    {
        scrollView.SetBusyMode(ParamState);

        if (ParamState == true)
        {
            IsAvailableForInput = false;
        }
        else
        {
            if (Parent != null)
                if (Parent.IsAvailableForInput)
                    IsAvailableForInput = true;
        }
    }

    public WispEntityInstance GetInstanceInCurrentState()
    {
        WispEntityInstance instance = new WispEntityInstance(currentInstance.Name, currentInstance.DisplayName);
        instance.ID = currentInstance.ID;

        foreach (KeyValuePair<string, WispElement> kv in elements)
        {
            if (kv.Value is WispElementText)
            {
                WispEntityPropertyText property = new WispEntityPropertyText(kv.Value.LinkedProperty.Name, kv.Value.LinkedProperty.Label, 1);
                property.Value = kv.Value.GetValue();
                property.Element = kv.Value;
                instance.AddProperty(property);
            }
            else if (kv.Value is WispElementDate)
            {
                WispEntityPropertyDate property = new WispEntityPropertyDate(kv.Value.LinkedProperty.Name, kv.Value.LinkedProperty.Label);
                property.Value = kv.Value.GetValue();
                property.Element = kv.Value;
                instance.AddProperty(property);
            }
            else if (kv.Value is WispElementBool)
            {
                WispEntityPropertyBool property = new WispEntityPropertyBool(kv.Value.LinkedProperty.Name, kv.Value.LinkedProperty.Label);
                property.Value = kv.Value.GetValue();
                property.Element = kv.Value;
                instance.AddProperty(property);
            }
            else if (kv.Value is WispElementImage)
            {
                WispEntityPropertyImage property = new WispEntityPropertyImage(kv.Value.LinkedProperty.Name, kv.Value.LinkedProperty.Label);
                property.Value = kv.Value.GetValue();
                property.Element = kv.Value;
                instance.AddProperty(property);
            }
            else if (kv.Value is WispElementSubInstance)
            {
                WispEntityPropertySubInstance property = new WispEntityPropertySubInstance(kv.Value.LinkedProperty.Name, kv.Value.LinkedProperty.Label);
                property.Value = kv.Value.GetValue();
                property.Element = kv.Value;
                instance.AddProperty(property);
            }
            else if (kv.Value is WispElementMultiSubInstance)
            {
                WispElementMultiSubInstance element = kv.Value as WispElementMultiSubInstance;
                WispEntityPropertyMultiSubInstance property = new WispEntityPropertyMultiSubInstance(kv.Value.LinkedProperty.Name, kv.Value.LinkedProperty.Label, null);
                property.Value = kv.Value.GetValue();
                property.TableDataSet = element.InstanceTable.GetDataSet();
                property.Element = kv.Value;
                instance.AddProperty(property);
            }
        }

        return instance;
    }

    internal int PushOrderedVisualComponent(WispVisualComponent ParamVisualComponent)
    {
        if (ParamVisualComponent == null)
            return -1;

        int index = orderedVisualComponents.Count + 1;

        orderedVisualComponents.Add(index, ParamVisualComponent);

        return index;
    }

    internal void SetCurrentFocusIndex(int ParamIndex)
    {
        if (!orderedVisualComponents.ContainsKey(ParamIndex))
        {
            LogError("Invalid index while trying to focus a component in EntityEditor.");
            return;
        }

        currentFocusIndex = ParamIndex;
    }

    public List<string> GetElementKeys()
    {
        List<string> result = new List<string>();

        foreach(KeyValuePair<string, WispElement> kvp in elements)
        {
            result.Add(kvp.Key);
        }

        return result;
    }
}