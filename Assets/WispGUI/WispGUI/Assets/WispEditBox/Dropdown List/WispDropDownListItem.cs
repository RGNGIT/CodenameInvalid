using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WispExtensions;

public class WispDropDownListItem : WispVisualComponent 
{
    private WispDropDownList parentList;
    private string textValue;
    private bool isVisible = true;
    TextMeshProUGUI text;

	public WispDropDownList ParentList {
		get {
			return parentList;
		}
		set {
			parentList = value;
		}
	}

	public string TextValue {
		get {
			return text.text;
		}
		set {
			text.text = value;
		}
	}

    public bool IsVisible { get => isVisible; set => isVisible = value; }

    private void Awake()
    {
        Initialize();
    }

    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        isInitialized = true;

        return true;
    }

    // Use this for initialization
    void Start () {

        ApplyStyle();

        if (parentList.SelectOnItemClick)
            this.OnClick(Select);

        if (parentList.CloseOnItemClick)
            this.OnClick(parentList.ParentEditBox.Close);
    }

	// ...
	public void OnClick (UnityEngine.Events.UnityAction ParamAction)
	{
		GetComponent<Button> ().onClick.AddListener (ParamAction);
	}

	public new void Select ()
	{
		parentList.ParentEditBox.SelectedItem = this;
	}

    public override void ApplyStyle()
    {
        base.ApplyStyle();
        
        text.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);

        // if (ParentList.ParentEditBox.SelectedItem == null)
        //     return;

        if (ParentList.ParentEditBox.SelectedItem == this)
        {
            GetComponent<Image>().ApplyStyle_Selected(style, Opacity, subStyleRule);
            text.ApplyStyle_Selected(style, Opacity, WispFontSize.Normal, subStyleRule);
        }
        else
        {
            GetComponent<Image>().ApplyStyle(style, Opacity, subStyleRule);
            text.ApplyStyle(style, Opacity, WispFontSize.Normal, subStyleRule);
        }

        Width = ParentList.MyRectTransform.rect.width - style.HorizontalPadding*2;
        Height = ParentList.ParentEditBox.ItemHeight;

        float y = (Index * parentList.ParentEditBox.DropdownListItemPrefab.GetComponent<RectTransform> ().rect.height) + style.VerticalPadding;
        MyRectTransform.anchoredPosition3D = new Vector3 (style.VerticalPadding, -y, 0);
    }
}