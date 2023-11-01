using UnityEngine;
using UnityEngine.Events;

public class WispPage : WispVisualComponent
{
    private WispTabView tabManager;
    private WispTabButton tabButton;
    private bool lastBlurPanelState;
    private UnityEvent onClose;
    private string pageID = null;

    public WispTabView TabManager { get => tabManager; set => tabManager = value; }
    public WispTabButton TabButton { get => tabButton; set => tabButton = value; }
    public UnityEvent OnClose { get => onClose; }

    public string PageID 
    { 
        get => pageID;
        
        set
        {
            if (pageID == null)
            {
                pageID = value;
            }
        }
    }

    public bool Visible 
    {
        set
        {
            if (value == true)
            {
                GetComponent<CanvasGroup>().alpha = 1;
                GetComponent<CanvasGroup>().interactable = true;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                GetComponent<CanvasGroup>().alpha = 0;
                GetComponent<CanvasGroup>().interactable = false;
                GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
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

        onClose = new UnityEvent();

        // ---------------------------------------------------------------------

        isInitialized = true;

        return true;
	}

    void Awake()
    {
        Initialize();
    }

    void Start ()
    {
        ApplyStyle();
    }
}