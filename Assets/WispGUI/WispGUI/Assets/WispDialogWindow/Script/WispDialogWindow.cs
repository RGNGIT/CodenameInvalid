using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispDialogWindow : WispWindow
{
    private WispScrollView scrollView;
    private WispButtonPanel buttonPanel;

    public WispScrollView ScrollView { get => scrollView; }
    public WispButtonPanel ButtonPanel { get => buttonPanel; }
    public bool DestroyOnClose { get => destroyOnClose; set => destroyOnClose = value; }

    // ...
    void Awake()
    {
        Initialize();
    }

    // ...
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

        scrollView = transform.Find("WispScrollView").GetComponent<WispScrollView>();
        buttonPanel = transform.Find("WispButtonPanel").GetComponent<WispButtonPanel>();

        scrollView.Initialize();
        buttonPanel.Initialize();

        scrollView.SetParent(this, true);
        buttonPanel.SetParent(this, true);

        isInitialized = true;

        return isInitialized;
    }

    // ...
    public static WispDialogWindow Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.DialogWindow, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.DialogWindow);
        }

        return go.GetComponent<WispDialogWindow>();
    }

    // ...
    public override void ApplyStyle()
    {
        base.ApplyStyle();

        GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
    }

    // ...
    public override void Open()
    {
        base.Open();
    }

    // ...
    public override void Close()
    {
        base.Close();

        if (useVisualTransition)
        {
            SetActiveUsingTransition(false, CloseProcedure);
        }
        else
        {
            CloseProcedure();
        }
    }

    private void CloseProcedure()
    {
        if (destroyOnClose)
        {
            Destroy(getBackgroundObstructor());
            Destroy(gameObject);
        }

        isOpen = false;

        if (enableBackgroundObstructor)
            getBackgroundObstructor().SetActive(false);
    }

    // ...
    public static WispDialogWindow CreateAndOpen(Transform ParamTransform = null)
    {
        if (ParamTransform == null)
        {
            WispDialogWindow popupView = Create(WispVisualComponent.GetMainCanvas().transform);
            popupView.Open();
            return popupView;
        }
        else
        {
            WispDialogWindow popupView = Create(ParamTransform);
            popupView.Open();
            return popupView;
        }
    }
}