using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using WispExtensions;

public class WispTabViewDemo : MonoBehaviour
{
    [Header("FPS")]
    [SerializeField] int fps = 120;

    [Header("Pages")]
    [SerializeField] WispScrollView welcomeScrollView;
    [SerializeField] WispScrollView componentScrollView;
    [SerializeField] RectTransform uiFiltersRt;
    [SerializeField] WispScrollView stylesScrollView;

    [Header("Styles")]
    [SerializeField] WispGuiStyle style1;
    [SerializeField] WispGuiStyle style2;
    [SerializeField] WispGuiStyle style3;
    [SerializeField] WispGuiStyle style4;
    [SerializeField] WispGuiStyle style5;
    [SerializeField] WispGuiStyle style6;
    [SerializeField] WispGuiStyle style7;
    [SerializeField] WispGuiStyle style8;
    [SerializeField] WispGuiStyle style9;
    [SerializeField] WispGuiStyle style10;
    [SerializeField] WispGuiStyle style11;

    private WispTabView tabView;
    private WispMessageBox messageBox;
    
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = fps;
        
        // --------------------------------------------------------------------------------------
        
        tabView = GetComponent<WispTabView>();

        // Welcome Page
        WispPage welcomePage = tabView.AddPage("welcome", "Welcome !", false);
        welcomeScrollView.gameObject.SetActive(true);
        welcomeScrollView.SetParent(welcomePage, true, true);
        welcomeScrollView.AnchorStyleExpanded(true);

        // Component preview page
        WispPage componentPage = tabView.AddPage("component", "Components preview", false);
        componentScrollView.gameObject.SetActive(true);
        componentScrollView.SetParent(componentPage, true, true);
        componentScrollView.AnchorStyleExpanded(true);

        // UI Filters preview page
        WispPage uiFiltersPage = tabView.AddPage("ui_filters", "UI Filters", false);
        uiFiltersRt.SetParent(uiFiltersPage.MyRectTransform);
        uiFiltersRt.AnchorStyleExpanded(true);
        uiFiltersRt.gameObject.SetActive(true);

        // Styles page
        WispPage stylesPage = tabView.AddPage("styles", "Styles", false);
        stylesScrollView.SetParent(stylesPage, true, true);
        stylesScrollView.AnchorStyleExpanded(true);
        stylesScrollView.gameObject.SetActive(true);
        
        // Tab button glow animation
        WispAnimationGlow anim = componentPage.TabButton.gameObject.AddComponent<WispAnimationGlow>();
        anim.Duration = 2f;
        componentPage.TabButton.Button.AddOnClickAction(delegate{Destroy(anim);});

        tabView.ShowPage(welcomePage.PageID);

        tabView.AddCornerButtonClickEvent
        (
            delegate 
            { 
                messageBox = WispMessageBox.OpenTwoButtonsDialog("Would you like to quit this demo ?", "Yes", Quit, "No", CloseMsgBox);
            }
        );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            SetGuiStyle(style1);
        else if (Input.GetKeyDown(KeyCode.F2))
            SetGuiStyle(style2);
        else if (Input.GetKeyDown(KeyCode.F3))
            SetGuiStyle(style3);
        else if (Input.GetKeyDown(KeyCode.F4))
            SetGuiStyle(style4);
        else if (Input.GetKeyDown(KeyCode.F5))
            SetGuiStyle(style5);
        else if (Input.GetKeyDown(KeyCode.F6))
            SetGuiStyle(style6);
        else if (Input.GetKeyDown(KeyCode.F7))
            SetGuiStyle(style7);
        else if (Input.GetKeyDown(KeyCode.F8))
            SetGuiStyle(style8);
        else if (Input.GetKeyDown(KeyCode.F9))
            SetGuiStyle(style9);
        else if (Input.GetKeyDown(KeyCode.F10))
            SetGuiStyle(style10);
        else if (Input.GetKeyDown(KeyCode.F11))
            SetGuiStyle(style11);
    }

    private void Quit()
    {
        #if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
         #else
         Application.Quit();
         #endif
    }

    private void CloseMsgBox()
    {
        messageBox.Close();
    }

    private void SetGuiStyle(WispGuiStyle ParamStyle)
    {
        tabView.Style = ParamStyle;
        WispVisualComponent.ApplyGlobalStyle(ParamStyle);
    }
}