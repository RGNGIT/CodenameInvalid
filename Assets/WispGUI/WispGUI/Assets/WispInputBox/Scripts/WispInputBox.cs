using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispInputBox : WispWindow
{
    [Header("Message")]
    [SerializeField] private string label;

    [Header("Input box")]
    [SerializeField] private bool closeOnConfirm = true;
    [SerializeField] private bool closeOnCancel = true;

    private WispButton buttonOk;
    private WispButton buttonCancel;
    private WispEditBox editBox;
    private WispInputResult resultContainer;

    /// <summary>
    /// Initiaize internal variables, A single call of this methode is required.
    /// </summary>
    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        // ---------------------------------------------------------------------

        buttonOk = transform.Find("WispButtonOk").GetComponent<WispButton>();
        buttonCancel = transform.Find("WispButtonCancel").GetComponent<WispButton>();
        editBox = transform.Find("WispEditBox").GetComponent<WispEditBox>();

        buttonOk.Initialize();
        buttonCancel.Initialize();
        editBox.Initialize();

        buttonOk.AddOnClickAction(btnOkOnClick);
        buttonOk.AssignKeyBoardShortcut(KeyCode.Return, true);
        buttonOk.AssignKeyBoardShortcut(KeyCode.KeypadEnter, true);

        buttonCancel.AddOnClickAction(btnCancelOnClick);
        buttonCancel.AssignKeyBoardShortcut(KeyCode.Escape, true);
        editBox.Label = label;

        buttonOk.SetParent(this, true);
        buttonCancel.SetParent(this, true);
        editBox.SetParent(this, true);

        // ---------------------------------------------------------------------

        isInitialized = true;

        return true;
    }

    // ...
    void Awake()
    {
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        ApplyStyle();
    }

    public override void ApplyStyle()
    {
        base.ApplyStyle();

        GetComponent<Image> ().ApplyStyle(style, Opacity, subStyleRule);
    }

    // ...
    private void btnOkOnClick()
    {
        resultContainer.Result = editBox.GetValue();
        if (closeOnConfirm)
            Close();
    }

    // ...
    private void btnCancelOnClick()
    {
        if (closeOnCancel)
            Close();
    }

    // ...
    public override void Open()
    {
        base.Open();

        if (enableBackgroundObstructor)
        {
            getBackgroundObstructor().transform.SetAsLastSibling();
            backgroundObstructor.SetActive(true);
        }

        transform.SetAsLastSibling();

        editBox.Focus();
    }

    // ...
    public override void Close()
    {
        base.Close();

        gameObject.SetActive(false);

        if (destroyOnClose)
        {
            Destroy(getBackgroundObstructor());
            Destroy(gameObject);
        }

        isOpen = false;

        if (enableBackgroundObstructor) getBackgroundObstructor().SetActive(false);
    }

    // ...
    public static WispInputResult OpenInputDialog(string ParamMessage, UnityEngine.Events.UnityAction ParamOkAction, Canvas ParamCanvas = null)
    {
        WispPrefabLibrary library = Resources.Load<WispPrefabLibrary>("Default Prefab Library");

        Canvas tmpCanvas;
        if (ParamCanvas == null)
        {
            tmpCanvas = WispVisualComponent.GetMainCanvas();
        }
        else
        {
            tmpCanvas = ParamCanvas;
        }

        GameObject go = Instantiate(library.InputBox, tmpCanvas.transform);
        WispInputBox box = go.GetComponent<WispInputBox>();
        box.Initialize();
        box.editBox.Label = ParamMessage;


        if (ParamOkAction != null)
            box.buttonOk.AddOnClickAction(ParamOkAction);

        box.Open();

        WispInputResult result = new WispInputResult();
        result.Source = box;

        box.resultContainer = result;  

        return result;
    }

    // ...
    public static WispInputBox Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.InputBox, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.InputBox);
        }

        return go.GetComponent<WispInputBox>();
    }
}

public class WispInputResult
{
    private WispInputBox source;
    private string result;

    public WispInputBox Source { get => source; set => source = value; }
    public string Result { get => result; set => result = value; }
}