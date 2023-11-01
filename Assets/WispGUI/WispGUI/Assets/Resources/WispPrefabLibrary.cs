using UnityEngine;

[CreateAssetMenu(fileName = "Prefab Library", menuName = "Wisp GUI/Prefab Library", order = 2)]

public class WispPrefabLibrary : ScriptableObject {
	
	public string LibraryName = "Default";

    [Space]
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject textMeshPro;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject editBox;
    [SerializeField] private GameObject calandar;
    [SerializeField] private GameObject image;
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject table;
    [SerializeField] private GameObject fileSelector;
	[SerializeField] private GameObject messageBox;
    [SerializeField] private GameObject inputBox;
    [SerializeField] private GameObject scrollView;
    [SerializeField] private GameObject timeLine;
    [SerializeField] private GameObject entityEditor;
    [SerializeField] private GameObject contextMenu;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private GameObject node;
    [SerializeField] private GameObject nodeEditor;
    [SerializeField] private GameObject dialogWindow;
    [SerializeField] private GameObject tooltip;
    [SerializeField] private GameObject checkBox;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject tabView;
    [SerializeField] private GameObject resizingHandle;
    [SerializeField] private GameObject nodeConnector;
    [SerializeField] private GameObject rerouteNode;
    [SerializeField] private GameObject progressBar;
    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject circularSlider;
    [SerializeField] private GameObject titleBar;
    [SerializeField] private GameObject floatingPanel;
    [SerializeField] private GameObject barChart;
    
    [Space]
    [SerializeField] private GameObject backgroundObstructor;
    [SerializeField] private GameObject fxaaFilter;

    public GameObject Canvas { get => canvas; set => canvas = value; }
    public GameObject TextMeshPro { get => textMeshPro; set => textMeshPro = value; }
    public GameObject Button { get => button; set => button = value; }
    public GameObject ButtonPanel { get => buttonPanel; set => buttonPanel = value; }
    public GameObject EditBox { get => editBox; set => editBox = value; }
    public GameObject Image { get => image; set => image = value; }
    public GameObject Calandar { get => calandar; set => calandar = value; }
    public GameObject Grid { get => grid; set => grid = value; }
    public GameObject Table { get => table; set => table = value; }
    public GameObject FileSelector { get => fileSelector; set => fileSelector = value; }
    public GameObject InputBox { get => inputBox; set => inputBox = value; }
    public GameObject MessageBox { get => messageBox; set => messageBox = value; }
    public GameObject ScrollView { get => scrollView; set => scrollView = value; }
    public GameObject TimeLine { get => timeLine; set => timeLine = value; }
    public GameObject EntityEditor { get => entityEditor; set => entityEditor = value; }
    public GameObject ContextMenu { get => contextMenu; set => contextMenu = value; }
    public GameObject BackgroundObstructor { get => backgroundObstructor; set => backgroundObstructor = value; }
    public GameObject LoadingPanel { get => loadingPanel; set => loadingPanel = value; }
    public GameObject LineRenderer { get => lineRenderer; set => lineRenderer = value; }
    public GameObject Node { get => node; set => node = value; }
    public GameObject NodeEditor { get => nodeEditor; set => nodeEditor = value; }
    public GameObject DialogWindow { get => dialogWindow; set => dialogWindow = value; }
    public GameObject Tooltip { get => tooltip; set => tooltip = value; }
    public GameObject CheckBox { get => checkBox; set => checkBox = value; }
    public GameObject Panel { get => panel; set => panel = value; }
    public GameObject TabView { get => tabView; set => tabView = value; }
    public GameObject ResizingHandle { get => resizingHandle; set => resizingHandle = value; }
    public GameObject FxaaFilter { get => fxaaFilter; set => fxaaFilter = value; }
    public GameObject NodeConnector { get => nodeConnector; set => nodeConnector = value; }
    public GameObject RerouteNode { get => rerouteNode; set => rerouteNode = value; }
    public GameObject ProgressBar { get => progressBar; set => progressBar = value; }
    public GameObject Slider { get => slider; set => slider = value; }
    public GameObject CircularSlider { get => circularSlider; set => circularSlider = value; }
    public GameObject TitleBar { get => titleBar; set => titleBar = value; }
    public GameObject FloatingPanel { get => floatingPanel; set => floatingPanel = value; }
    public GameObject BarChart { get => barChart; set => barChart = value; }

    public static WispPrefabLibrary Default
    {
        get
        {
            return Resources.Load<WispPrefabLibrary>("Default Prefab Library");
        } 
    }

}