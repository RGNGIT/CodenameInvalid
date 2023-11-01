using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispScrollView : WispVisualComponent
{
    [Header("Prefabs")]
    [SerializeField] private GameObject blurPanelPrefab;
    [SerializeField] private GameObject loadingPanelPrefab;

    protected RectTransform contentRect;
    protected WispScrollRect scrollRect;
    private IEnumerator scrollCoroutine = null;

    public RectTransform ContentRect { get => contentRect; }
    public ScrollRect ScrollRect { get => scrollRect; }

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

        scrollRect = GetComponent<WispScrollRect>();
        contentRect = transform.Find("Viewport").Find("Content").GetComponent<RectTransform>();

        SetBusyMode(false);

        isInitialized = true;

        return true;
    }

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet.
    /// </summary>
    public override void ApplyStyleInEditor()
    {
        scrollRect = GetComponent<WispScrollRect>();
        
        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);

        scrollRect.horizontalScrollbar.GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
        scrollRect.verticalScrollbar.GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
        scrollRect.horizontalScrollbar.handleRect.GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
        scrollRect.verticalScrollbar.handleRect.GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
    }

    /// <summary>
    /// Apply Color and graphics modifications from the style sheet.
    /// </summary>
    public override void ApplyStyle()
    {
        if (Application.isEditor && !Application.isPlaying)
            return;
        
        if (style == null)
            return;
        
        base.ApplyStyle();

        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);

        scrollRect.horizontalScrollbar.GetComponent<Image>().ApplyStyle_ScrollBar_H(style, Opacity);
        scrollRect.verticalScrollbar.GetComponent<Image>().ApplyStyle_ScrollBar_V(style, Opacity);

        scrollRect.horizontalScrollbar.handleRect.GetComponent<Image>().ApplyStyle_ScrollBar_Handle_H(style, Opacity);
        scrollRect.verticalScrollbar.handleRect.GetComponent<Image>().ApplyStyle_ScrollBar_Handle_V(style, Opacity);

        scrollRect.HorizontalScrollbarYOffset = style.ScrollbarSpacingSettings.horizontalScrollbarYOffset;
        scrollRect.HorizontalScrollbarXPaddings = style.ScrollbarSpacingSettings.horizontalScrollbarXPaddings;
        scrollRect.VerticalScrollbarXOffset = style.ScrollbarSpacingSettings.verticalScrollbarXOffset;
        scrollRect.VerticalScrollbarYPaddings = style.ScrollbarSpacingSettings.verticalScrollbarYPaddings;

        scrollRect.viewport.GetComponent<Image>().sprite = style.ScrollViewMask;
    }

    /// <summary>
    /// A hack to hide this component without disabling it, Whithout disabling renderers and without using canvas groups.
    /// </summary>
    public void PositionAway()
    {
        GetComponent<RectTransform>().localPosition = new Vector3(-4096, -4096, 0);
    }

    public void HorizontalScrollToPosition(float ParamHorizontalPosition)
    {
        float scrollValue = ParamHorizontalPosition / scrollRect.content.rect.width/*GetHeight()*/;
        scrollRect.horizontalScrollbar.value = scrollValue;
    }

    public void VerticalScrollToPosition(Vector3 ParamPosition)
    {
        float scrollValue = 1 + ParamPosition.y / scrollRect.content.rect.height/*GetHeight()*/;
        scrollRect.verticalScrollbar.value = scrollValue;
    }

    public void ScrollToPosition_Async(Vector3 ParamDestination, float ParamDuration)
    {
        if (scrollCoroutine != null)
            StopCoroutine(scrollCoroutine);

        scrollCoroutine = LerpScroll(ParamDestination, ParamDuration);

        if (gameObject.activeInHierarchy)
            StartCoroutine(scrollCoroutine);
    }

    private IEnumerator LerpScroll(Vector3 ParamDestination, float ParamDuration)
    {
        float startValue = scrollRect.verticalScrollbar.value;
        float finalValue = 1 + ParamDestination.y / scrollRect.content.rect.height;
        float timeOfTravel = ParamDuration;
        float currentTime = 0;
        float normalizedValue;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel;
            scrollRect.verticalScrollbar.value = Mathf.Lerp(startValue, finalValue, normalizedValue);
            yield return null;
        }
    }

    public void VerticalScrollToPercentage_Async(float ParamY, float ParamDuration)
    {
        ParamY = Mathf.Clamp(ParamY, 0, 100);
        float scrollBarValue = 1 - (ParamY/100);
        StartCoroutine(LerpVerticalScrollToValue(scrollBarValue, ParamDuration));
    }

    private IEnumerator LerpVerticalScrollToValue(float ParamY, float ParamDuration)
    {
        ParamY = Mathf.Clamp01(ParamY);
        
        float startValue = scrollRect.verticalScrollbar.value;
        float finalValue = ParamY;
        float timeOfTravel = ParamDuration;
        float currentTime = 0;
        float normalizedValue;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel;
            scrollRect.verticalScrollbar.value = Mathf.Lerp(startValue, finalValue, normalizedValue);
            yield return null;
        }
    }

    public void HorizontalScrollToPercentage_Async(float ParamX, float ParamDuration)
    {
        ParamX = Mathf.Clamp(ParamX, 0, 100);
        float scrollBarValue = 1 - (ParamX/100);
        StartCoroutine(LerpHorizontalScrollToValue(scrollBarValue, ParamDuration));
    }

    private IEnumerator LerpHorizontalScrollToValue(float ParamX, float ParamDuration)
    {
        ParamX = Mathf.Clamp01(ParamX);
        
        float startValue = scrollRect.horizontalScrollbar.value;
        float finalValue = ParamX;
        float timeOfTravel = ParamDuration;
        float currentTime = 0;
        float normalizedValue;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel;
            scrollRect.horizontalScrollbar.value = Mathf.Lerp(startValue, finalValue, normalizedValue);
            yield return null;
        }
    }

    // Make RT and contentRect the same size
    public void SyncRectSizes()
    {
        contentRect.sizeDelta = rectTransform.sizeDelta;
    }

    /// <summary>
    /// Make contentRect size so that no scrollbars are visible.
    /// </summary>
    public void MaxContentRectSize()
    {
        contentRect.offsetMax = new Vector2(0, 0);
        contentRect.offsetMin = new Vector2(0, 0);
    }

    // ...
    public static WispScrollView Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.ScrollView, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.ScrollView);
        }

        return go.GetComponent<WispScrollView>();
    }

    // ...
    public void ExpandVertically(float ParamAmount)
    {
		float newHeight = contentRect.sizeDelta.y + ParamAmount;
		contentRect.sizeDelta = new Vector2 (contentRect.sizeDelta.x, newHeight);

		// Must do this at the end.
		scrollRect.CalculateLayoutInputHorizontal ();
		scrollRect.CalculateLayoutInputVertical ();
    }

    // ...
    public void ExpandHorizontally(float ParamAmount)
    {
		float newWidth = contentRect.sizeDelta.x + ParamAmount;
		contentRect.sizeDelta = new Vector2 (newWidth, contentRect.sizeDelta.y);

		// Must do this at the end.
		scrollRect.CalculateLayoutInputHorizontal ();
		scrollRect.CalculateLayoutInputVertical ();
    }
}