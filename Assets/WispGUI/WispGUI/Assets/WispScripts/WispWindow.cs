using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using WispExtensions;

public class WispWindow : WispVisualComponent
{
    [Header("Window")]
    [SerializeField] protected bool modal = true;
    [SerializeField] protected bool destroyOnClose = false;
    [SerializeField] protected bool enableBackgroundObstructor = true;
    [SerializeField] private bool centerOnOpen = true;
    [SerializeField] protected bool useVisualTransition = true;
    [SerializeField] protected bool forceCursorVisibility = true;

    private static bool limitWindowsToOne = true;
    private static WispWindow currentlyOpen = null;
    private static WispVisualComponent lastFocusVisualComponent = null;
    private List<int> rememberComponentsAvailableForInput;
    private WispCursorState rememberCursorState;

    protected bool isOpen = false;
	protected GameObject backgroundObstructor;

    public static WispWindow CurrentlyOpen { get => currentlyOpen; }
    public bool IsOpen { get => isOpen; }
    public bool Modal { get => modal; }

    public override bool Initialize()
    {
        base.Initialize();

        WispResizingHandle.Create(this, new Vector2(64f, 64f));
        
        if (WispVisualComponent.GlobalStyle != null)
            style = WispVisualComponent.GlobalStyle;

        return true;
    }

    public override void Open()
    {
        if (limitWindowsToOne && currentlyOpen != null)
        {
            WispVisualComponent.LogError("Unable to open a second window because limitWindowsToOne is set to true.");
            return;
        }

        if (enableBackgroundObstructor)
        {
            getBackgroundObstructor().transform.SetAsLastSibling();
            backgroundObstructor.SetActive(true);

            WispAnimationFloat anim = backgroundObstructor.AddComponent<WispAnimationFloat>();
            anim.FloatStartingValue = 0f;
            anim.FloatFinalValue = 1f;
            anim.Duration = 0.25f;
            anim.FloatPropertyName = "_Blending";
            anim.TargetMaterial = backgroundObstructor.GetComponent<Image>().material;
        }

        transform.SetAsLastSibling();

        if (centerOnOpen)
		{
			MyRectTransform.anchoredPosition3D = new Vector2(0,0);
			MyRectTransform.AnchorTo("center-center");
		}

        // Close other
        CloseCurrentlyOpenedVisualComponent();

        rememberComponentsAvailableForInput = WispVisualComponent.GetWispVisualComponentsAvailableForInput();
        
        // Makes this window the only thing available for input.
        WispVisualComponent.MakeAvailableForInput(null);
        IsAvailableForInput = true;
        
        lastFocusVisualComponent = WispVisualComponent.FocusedComponent;
        WispVisualComponent.FocusedComponent = this;
        currentlyOpen = this;
        isOpen = true;

        if (forceCursorVisibility)
        {
            rememberCursorState = WispCursor.GetCursorState();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (useVisualTransition)
        {
            gameObject.SetActive(false);
            SetActiveUsingTransition(true);    
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public override void Close()
    {
        currentlyOpen = null;
        WispVisualComponent.FocusedComponent = lastFocusVisualComponent;

        if (rememberCursorState != null)
        {
            WispCursor.SetCursorState(rememberCursorState);
            rememberCursorState = null;
        }

        WispAnimationFloat anim = backgroundObstructor.AddComponent<WispAnimationFloat>();
        anim.FloatStartingValue = 1f;
        anim.FloatFinalValue = 0f;
        anim.Duration = 0.25f;
        anim.FloatPropertyName = "_Blending";
        anim.TargetMaterial = backgroundObstructor.GetComponent<Image>().material;
    }

    protected GameObject getBackgroundObstructor ()
	{
		if (backgroundObstructor != null)
			return backgroundObstructor;
		else
		{
			backgroundObstructor = Instantiate(WispPrefabLibrary.Default.BackgroundObstructor, transform.parent);
			return backgroundObstructor;
		}
	}

    public static void RegisterModalWindow()
    {

    }

    // Allows any child button to close it's parent window.
    public static new UnityAction CloseParentWindow()
    {
        return delegate { WispVisualComponent.CloseParentWindow(); };
    }
}