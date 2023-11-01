using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Style", menuName = "Wisp GUI/Style", order = 1)]

public class WispGuiStyle : ScriptableObject 
{
	public WispSubStyleBlock GetSubStyle(WispSubStyleRule ParamSubStyleRule)
	{
		switch (ParamSubStyleRule)
		{
			case WispSubStyleRule.Background :
				return background;
			case WispSubStyleRule.Widget :
				return widget;
			case WispSubStyleRule.Container :
				return container;
			case WispSubStyleRule.Picture :
				return picture;
			case WispSubStyleRule.Icon :
				return icon;
			case WispSubStyleRule.TitleBar :
				return titleBar;
			
			default:
				return none;
		}
	}
	
	[Header("General")]
	public string styleName = "Default";

	[Header("Font")]
	public TMPro.TMP_FontAsset Font;
	public float FontSizeNormal = 12;
	public float FontSizeHeader = 18;
	public float FontSizeLabel = 14;
	public float FontSizeQuiet = 10;
	
	[Header("Line renderer")]
	public Color LineColor = Color.black;
	
	[Header("Scrollbars")]
	public Sprite horizontalScrollBar;
	public Sprite horizontalScrollBarHandle;
	public Sprite verticalScrollBar;
	public Sprite verticalScrollBarHandle;
	public Color ScrollbarBackgroundColor = Color.grey;
	public Color ScrollbarHandleColor = Color.white;
	public Image.Type ScrollbarDrawMode = Image.Type.Sliced;
	public Sprite ScrollViewMask;
	public WispScrollbarSpacingSettings ScrollbarSpacingSettings;

	[Header("Color")]

    [Space(8)]
    public Color ImageColor = Color.black;
    public Color ProgressColor = Color.black;

    public float GetFontSize(WispFontSize ParamFontSize)
    {
        switch (ParamFontSize)
		{
			case WispFontSize.Normal :
				return FontSizeNormal;

			case WispFontSize.Header :
				return FontSizeHeader;

			case WispFontSize.Label :
				return FontSizeLabel;

			case WispFontSize.Quiet :
				return FontSizeQuiet;

			default:
				return FontSizeNormal;
		}
    }

	[Space(8)]
	public Color ResizingBarActiveColor = new Color(1,1,1,0.5f);
	public Color ResizingBarInactiveColor = Color.clear;

	[Space(8)]
    public Color ResizingHandleColor = Color.black;

	[Header("Button Colors")]
	[SerializeField] protected ColorBlock buttonColorBlock = ColorBlock.defaultColorBlock;

	public ColorBlock ButtonColorBlock
    {
        get
        {
            return buttonColorBlock;
        }
        set
        {
            buttonColorBlock = value;
        }
    }

    [Header("Grid")]
    [SerializeField] protected Color gridColor = Color.grey;

    public Color GridColor
    {
        get
        {
            return gridColor;
        }
        set
        {
            gridColor = value;
        }
    }

	[Header("Spacing")]
	[SerializeField] public int ButtonHorizontalSpacing = 4;
	[SerializeField] public int ButtonVerticalSpacing = 4;
	[SerializeField] public int HorizontalPadding = 4;
	[SerializeField] public int VerticalPadding = 4;

    [Header("Borders")]
    [SerializeField] protected bool enableBorders = true;

    public bool EnableBorders { get => enableBorders; set => enableBorders = value; }

    [ConditionalHideBoolAttribute("enableBorders", true, true)]
    [SerializeField] protected Color borderColor = Color.blue;

    public Color BorderColor { get => borderColor; set => borderColor = value; }

    [ConditionalHideBoolAttribute("enableBorders", true, true)]
    [SerializeField] protected Color selectedBorderColor = Color.red;

    public Color SelectedBorderColor { get => selectedBorderColor; set => selectedBorderColor = value; }

    [ConditionalHideBoolAttribute("enableBorders", true, true)]
    [SerializeField] protected int borderWidth = 3;

    public int BorderWidth { get => borderWidth; set => borderWidth = value; }

    [ConditionalHideBoolAttribute("enableBorders", true, true)]
    [SerializeField] protected WispBorderType borderType = WispBorderType.Centred;

    public WispBorderType BorderType { get => borderType; set => borderType = value; }

	[Header("Sub Styles")]
	[SerializeField] protected WispSubStyleBlock none;
	[SerializeField] protected WispSubStyleBlock background;
	[SerializeField] protected WispSubStyleBlock widget;
	[SerializeField] protected WispSubStyleBlock container;
	[SerializeField] protected WispSubStyleBlock picture;
	[SerializeField] protected WispSubStyleBlock icon;
	[SerializeField] protected WispSubStyleBlock titleBar;

	public void OnValidate() 
	{
		if (WispVisualComponent.LastSelectedInHierarchy != null)
		{
			if (WispVisualComponent.LastSelectedInHierarchy.StyleFollowRule == WispVisualComponent.WispVisualStyleFollowRule.Parent)
				return;
			
			if (Application.isPlaying)
			{
				WispVisualComponent.LastSelectedInHierarchy.RegisterApplyStyleRequest();
			}
			else
			{
				WispVisualComponent.LastSelectedInHierarchy.RegisterWysiwygRequest();
			}
		}
	}
}

[Serializable]
public class WispSubStyleBlock
{
	public Color activeColor = Color.black;
	public Color activeBackgroundColor = Color.white;
	public Color inactiveColor = Color.black;
	public Color inactiveBackgroundColor = Color.white;
	public Color selectedColor = Color.white;
	public Color selectedBackgroundColor = Color.blue;
	public Sprite graphics;
	public Image.Type spriteDrawMode = Image.Type.Sliced;
	public float pixelsPerUnitMultiplier = 1f;
	public Material material;
}

[Serializable]
public class WispScrollbarSpacingSettings
{
	public float horizontalScrollbarYOffset = 0;
    public float horizontalScrollbarXPaddings = 0;
    public float verticalScrollbarXOffset = 0;
    public float verticalScrollbarYPaddings = 0;
}