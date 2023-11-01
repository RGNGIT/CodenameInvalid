using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class WispTabButton : MonoBehaviour {

    private WispPage page;
    private WispButton button;
    private WispButton exitButton;
    private WispPage previousPage;
    private WispPage nextPage;

    public WispPage Page { get => page; set => page = value; }
    public WispPage PreviousPage { get => previousPage; set => previousPage = value; }
    public WispPage NextPage { get => nextPage; set => nextPage = value; }
    public WispButton Button { get => button; set => button = value; }
    public WispButton ExitButton { get => exitButton; set => exitButton = value; }

    // Use this for initialization
    void Awake () {
        button = GetComponent<WispButton>();
        button.AddOnClickAction(TabBtnOnClick);
	}

	// onClick Event
	void TabBtnOnClick()
	{
        page.TabManager.ShowPage(page.Name);
	}

	// ...
	public void ChangeState (bool ParamState)
	{
        if (ParamState)
        {
            GetComponent<Image>().ApplyStyle_Selected(page.TabManager.Style, page.TabManager.Opacity, WispSubStyleRule.Widget);
            button.TextComponent.ApplyStyle_Selected(page.TabManager.Style, page.TabManager.Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);
        }
        else
        {
            GetComponent<Image>().ApplyStyle_Inactive(page.TabManager.Style, page.TabManager.Opacity, WispSubStyleRule.Widget);
            button.TextComponent.ApplyStyle(page.TabManager.Style, page.TabManager.Opacity, WispFontSize.Normal, WispSubStyleRule.Widget);
        }
	}

	// ...
	public void CloseMyPage ()
	{
        page.TabManager.ClosePage(page.Name);
    }

    public WispButton AddCloseButton()
    {
        WispButton closeButton = WispButton.Create(transform);
        closeButton.MyRectTransform.sizeDelta = new Vector2(16, 16);
        closeButton.MyRectTransform.anchorMin = new Vector2(1, 1);
        closeButton.MyRectTransform.anchorMax = new Vector2(1, 1);
        closeButton.MyRectTransform.anchoredPosition = new Vector2(-10, -10);
        closeButton.SetValue("x");
        closeButton.AddOnClickAction(CloseMyPage);

        return closeButton;
    }
}