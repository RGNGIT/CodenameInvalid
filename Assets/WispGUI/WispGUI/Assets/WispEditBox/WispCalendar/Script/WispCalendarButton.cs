using UnityEngine.UI;

public class WispCalendarButton : WispMonoBehaviour 
{
	// Do not serialize these two public variables, until you are ready to re-assign x and y on every day button in the calenadar prefab
	public int X;
	public int Y;

	private bool activeMonth;

	public bool ActiveMonth {
		get {
			return activeMonth;
		}
		set {
			activeMonth = value;
		}
	}

    private TMPro.TextMeshProUGUI text;

	public TMPro.TextMeshProUGUI Text {
		get {
			return text;
		}
		set {
			text = value;
		}
	}

    private Button buttonComponent;

	public Button ButtonComponent {
		get {
			return buttonComponent;
		}
		set {
			buttonComponent = value;
		}
	}

	public override bool Initialize ()
	{
        if (isInitialized)
            return true;

        base.Initialize ();

		text = GetComponentInChildren<TMPro.TextMeshProUGUI> ();
		buttonComponent = GetComponent<Button> ();

        return true;
    }

	// Use this for initialization
	void Start () {

		Initialize ();

	}

    void Awake ()
    {
        Initialize();
    }
}