using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WispExtensions;

public class WispFileSelectorItem : WispVisualComponent, IPointerClickHandler 
{
	private TMPro.TextMeshProUGUI textComponent;
	private WispFileSelector parentFileSelector;
	private Image backgroundImage;
	private Image icon;
	private bool isDirectory = false;

	public string Text 
	{
		get {
			return textComponent.text;
		}
		set {
			textComponent.text = value;
		}
	}

	public WispFileSelector ParentFileSelector 
	{
		get 
		{
			return parentFileSelector;
		}
		set 
		{
			parentFileSelector = value;
			backgroundImage.rectTransform.sizeDelta = new Vector2(parentFileSelector.ViewportWidth(),backgroundImage.rectTransform.sizeDelta.y);
		}
	}

	public bool IsDirectory
	{
		get
		{
			return isDirectory;
		}
		set
		{
			isDirectory = value;
		}
	}

	public void OnPointerClick(PointerEventData ParamPointerEventData)
    {
		if (isDirectory)
			parentFileSelector.DisplayElementsInDirectory(parentFileSelector.CurrentDirectoryPath + "/" + Text);
		else
			parentFileSelector.SelectItem(this);
    }

    void Awake()
    {
        Initialize();
    }

	void Start ()
    {
        ApplyStyle();
	}

	public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();


        // ---------------------------------------------------------------------

		textComponent = transform.Find("FileNameText").GetComponent<TMPro.TextMeshProUGUI> ();
		backgroundImage = GetComponent<Image> ();
		icon = transform.Find("Icon").GetComponent<Image> ();

		// ---------------------------------------------------------------------

        isInitialized = true;

		ApplyStyle();

        return true;
    }

	public override void ApplyStyle ()
	{
		icon.ApplyStyle(style, Opacity, WispSubStyleRule.Icon);
		textComponent.ApplyStyle(style, Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);

		backgroundImage.color = Color.clear;
		backgroundImage.material = null;
	}

	public void SetIcon(Sprite ParamIcon)
	{
		icon.sprite = ParamIcon;
	}

	public void ApplyStyleSelected ()
	{
		icon.ApplyStyle(style, Opacity, WispSubStyleRule.Icon);
		textComponent.ApplyStyle(style, Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);
		backgroundImage.ApplyStyle(style, Opacity, WispSubStyleRule.Widget);
	}

	public void ApplyStyleUnSelected ()
	{
		ApplyStyle();
	}
}