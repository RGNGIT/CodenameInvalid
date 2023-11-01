using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WispExtensions;

public class WispButtonOpenTabDemo : MonoBehaviour
{
    [SerializeField] private WispTabView tabView;

    private WispInputResult inputResult;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<WispButton>().AddOnClickAction(OpenTab);
    }

    private void OpenTab()
    {
        WispPage testPage = tabView.GetPage("more");

        if (testPage != null)
        {
            tabView.ShowPage("more");
            return;
        }
        
        WispPage page = tabView.AddPage("more", "More...", true);
        page.SetBusyMode(true);

        StartCoroutine(DrawComponents(page));
    }

    private IEnumerator DrawComponents(WispPage ParamPage)
    {
        WispScrollView scrollView = WispScrollView.Create(ParamPage.transform);
        scrollView.AnchorStyleExpanded(true);
        scrollView.SetParent(ParamPage, true);
        scrollView.ContentRect.SetSizeDeltaHeight(1024);

        WispGrid grid = WispGrid.Create(scrollView.ContentRect);
        grid.AnchorTo("center-top");
        grid.SetParent(scrollView, true);
        grid.MyRectTransform.sizeDelta = new Vector2(256,256);
        grid.Set_Y_Position(-160);
        grid.SetDimensions(2,2);
        grid.AutoFit();

        WispImage image_One = WispImage.Create(grid.GetCell(0).MyRectTransform);
        image_One.SubStyleRule = WispSubStyleRule.Picture;
        image_One.SetParent(grid, true);
        image_One.MyRectTransform.AnchorStyleExpanded(4);
        image_One.SetValue("https://picsum.photos/128");

        WispImage image_Two = WispImage.Create(grid.GetCell(1).MyRectTransform);
        image_Two.SubStyleRule = WispSubStyleRule.Picture;
        image_Two.SetParent(grid, true);
        image_Two.MyRectTransform.AnchorStyleExpanded(4);
        image_Two.SetValue("https://picsum.photos/128");

        WispImage image_Three = WispImage.Create(grid.GetCell(2).MyRectTransform);
        image_Three.SubStyleRule = WispSubStyleRule.Picture;
        image_Three.SetParent(grid, true);
        image_Three.MyRectTransform.AnchorStyleExpanded(4);
        image_Three.SetValue("https://picsum.photos/128");

        WispImage image_Four = WispImage.Create(grid.GetCell(3).MyRectTransform);
        image_Four.SubStyleRule = WispSubStyleRule.Picture;
        image_Four.SetParent(grid, true);
        image_Four.MyRectTransform.AnchorStyleExpanded(4);
        image_Four.SetValue("https://picsum.photos/128");

        WispTextMeshPro gridText = WispTextMeshPro.Create(scrollView.ContentRect);
        gridText.SetParent(scrollView, true);
        gridText.AnchorTo("center-top");
        gridText.Set_Y_Position(-256-64);
        gridText.Width = 256f;
        gridText.Base.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
        gridText.Base.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
        gridText.SetValue("The above grid displays 4 random pictures from the internet.");

        WispPanel panel = WispPanel.Create(scrollView.ContentRect);
        panel.SetParent(scrollView, true);
        panel.AnchorTo("center-top");
        panel.PivotAround("center-center");
        panel.Set_Y_Position(-512);
        panel.Width = 256f;
        panel.Height = 256f;

        WispButton button = WispButton.Create(panel.transform);
        button.SetParent(panel, true);
        button.AnchorTo("center-center");
        button.PivotAround("center-center");
        button.MyRectTransform.anchoredPosition = new Vector2(0,0);
        button.SetValue("Set Text...");
        button.AddOnClickAction(delegate{ OpenInputBox(button); });

        WispTextMeshPro inputBoxText = WispTextMeshPro.Create(scrollView.ContentRect);
        inputBoxText.SetParent(scrollView, true);
        inputBoxText.AnchorTo("center-top");
        inputBoxText.Set_Y_Position(-512-128-32);
        inputBoxText.Width = 256f;
        inputBoxText.Base.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
        inputBoxText.Base.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
        inputBoxText.SetValue("Click the above button to change it's text using an Input Box.");

        yield return new WaitForSeconds(3f);

        ParamPage.SetBusyMode(false);
    }

    private void OpenInputBox(WispButton ParamTarget)
    {
        inputResult = WispInputBox.OpenInputDialog("Enter the new text...", delegate{ ChangeText(ParamTarget); });
    }

    private void ChangeText(WispButton ParamTarget)
    {
        ParamTarget.SetValue(inputResult.Result);
    }
}