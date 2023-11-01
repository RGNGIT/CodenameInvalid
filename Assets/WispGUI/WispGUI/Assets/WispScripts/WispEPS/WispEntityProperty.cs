using System;
using System.Collections.Generic;
using UnityEngine;
using TinyJson;

public class WispEntityProperty {

	protected string name;
	protected string label;
	protected string DbColumnNamePrefix;
	protected string DbColumnNameSuffix;
	protected bool displayInGrid;
	protected bool displayInEditor;
	protected bool editable;
	protected WispDefaultValue defaultValue;
	protected string groupName;
	protected float verticalHeight = 64;
	protected float extraHeight = 0;
	protected bool requireFullRow = false;
    protected bool requireFullWidth = false;
    protected bool square = false;
    protected WispEntityInstance parent;
	protected bool uniqueValue = false;
	protected string currentValue;
    //protected WispVisualComponent visualComponent;
    protected WispElement element;

    public string Value {
		get {
			return currentValue;
		}
		set {
			currentValue = value;
		}
	}

	public string Name {
		get {
			return name;
		}
	}

	public string Label {
		get {
			return label;
		}
	}

	public WispEntityInstance Parent {
		get {
			return parent;
		}
		set {
			parent = value;
		}
	}

	public virtual string GetValue()
	{
		return currentValue;
	}

	public virtual string GetHumainReadableValue()
	{
		return null;
	}

	public virtual void SetValue(string ParamValue)
	{
		// Do nothing ...
	}

	public float VerticalHeight {
		get
        {
			return verticalHeight;
		}
	}

	public float ExtraHeight {
		get {
			return extraHeight;
		}
	}

	public bool RequireFullRow {
		get {
			return requireFullRow;
		}
	}

	public bool Editable {
		get {
			return editable;
		}
		set {
			editable = value;
		}
	}

	public bool UniqueValue {
		get {
			return uniqueValue;
		}

		set { uniqueValue = value;}
	}

    //public WispVisualComponent VisualComponent { get => visualComponent; }
    public bool RequireFullWidth { get => requireFullWidth; }
    public WispElement Element { get => element; set => element = value; }
    public bool Square { get => square; }

    public virtual GameObject DrawVisualComponent(WispEntityEditor ParamParentEditor, Transform ParentTransform, Vector3 ParamPosition, WispEntityProperty ParamLinkedProperty)
	{
		return null;
	}

    public virtual string GetJson()
    {
        return "";
    }

    //public virtual void AssignScriptOnEdit(string ParamScript)
    //{
    //    //...
    //}
}

// *****************************************************************************************************************************************************************************

public class WispEntityPropertyText : WispEntityProperty {

	protected int lineCount;

	public WispEntityPropertyText(string ParamPropertyName, string ParamLabelText, int ParamLineCount = 1)
	{
		name = ParamPropertyName;
		label = ParamLabelText;
		lineCount = ParamLineCount;
	}

	//public override string GetValue()
	//{
	//	return currentValue;
	//}

	public override string GetHumainReadableValue()
	{
		return currentValue;
	}

	public override void SetValue(string ParamValue)
	{
		currentValue = ParamValue;
	}

	public override GameObject DrawVisualComponent(WispEntityEditor ParamParentEditor, Transform ParentTransform, Vector3 ParamPosition, WispEntityProperty ParamLinkedProperty)
	{
        WispElementText element = WispElementText.Draw(ParamParentEditor, ParamPosition, ParamLinkedProperty) as WispElementText;
        verticalHeight = element.MainEditBox.MyRectTransform.rect.height;
        extraHeight = ParamParentEditor.ComponentVerticalSpacing;

		return element.gameObject;
	}

    public override string GetJson()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("type", "text");
        result.Add("name", name);
        result.Add("label", label);
        result.Add("value", currentValue);

        return result.ToJson();
    }
}

// *****************************************************************************************************************************************************************************

public class WispEntityPropertyDate : WispEntityProperty {


	public WispEntityPropertyDate(string ParamPropertyName, string ParamLabelText)
	{
		name = ParamPropertyName;
		label = ParamLabelText;
	}

	//public override string GetValue()
	//{
	//	return currentValue;
	//}

	public override string GetHumainReadableValue()
	{
		char[] sep = new char[1];
		sep [0] = '-';

        if (currentValue == null)
            return "";

        string[] tmpStrArray = currentValue.Split (sep, 3);

		int y = Int32.Parse (tmpStrArray [0]);
		int m = Int32.Parse (tmpStrArray [1]);
		int d = Int32.Parse (tmpStrArray [2]);

		if (y == 0)
			y = 1;

		if (m == 0)
			m = 1;

		if (d == 0)
			d = 1;

		return new DateTime(y, m, d).ToShortDateString();
	}

	public override void SetValue(string ParamValue)
	{
		currentValue = ParamValue;
	}

	public override GameObject DrawVisualComponent(WispEntityEditor ParamParentEditor, Transform ParentTransform, Vector3 ParamPosition, WispEntityProperty ParamLinkedProperty)
	{
        WispElementDate element = WispElementDate.Draw(ParamParentEditor, ParamPosition, ParamLinkedProperty) as WispElementDate;
        verticalHeight = element.MainEditBox.MyRectTransform.rect.height;
        extraHeight = ParamParentEditor.ComponentVerticalSpacing;

        return element.gameObject;
    }

    public override string GetJson()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("type", "date");
        result.Add("name", name);
        result.Add("label", label);
        result.Add("value", GetValue());

        return result.ToJson();
    }
}

// *****************************************************************************************************************************************************************************
public class WispEntityPropertyImage : WispEntityProperty {

	public WispEntityPropertyImage(string ParamPropertyName, string ParamLabelText)
	{
		name = ParamPropertyName;
		label = ParamLabelText;
        square = true;
	}

	public override GameObject DrawVisualComponent(WispEntityEditor ParamParentEditor, Transform ParentTransform, Vector3 ParamPosition, WispEntityProperty ParamLinkedProperty)
	{
        WispElementImage element = WispElementImage.Draw(ParamParentEditor, ParamPosition, ParamLinkedProperty) as WispElementImage;
        verticalHeight = element.Image.MyRectTransform.rect.width;
        extraHeight = ParamParentEditor.ComponentVerticalSpacing;

        return element.gameObject;
    }

	//public override string GetValue()
	//{
	//	return currentValue;
	//}

	public override void SetValue(string ParamValue)
	{
		currentValue = ParamValue;
	}

    public override string GetJson()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("type", "image");
        result.Add("name", name);
        result.Add("label", label);
        result.Add("value", GetValue());

        return result.ToJson();
    }

}

// *****************************************************************************************************************************************************************************

public class WispEntityPropertySubInstance : WispEntityProperty
{
    private WispDataSet dataSet;

    public WispEntityPropertySubInstance(string ParamPropertyName, string ParamLabelText, WispDataSet ParamDataSet = null)
	{
		name = ParamPropertyName;
		label = ParamLabelText;

        if (ParamDataSet != null)
        {
            dataSet = ParamDataSet;
        }
	}

    /*
    public WispEntityPropertySubInstance(string ParamPropertyName, string ParamLabelText, WispDataSetMinimal ParamDataSet = null)
    {
        name = ParamPropertyName;
        label = ParamLabelText;

        if (ParamDataSet != null)
        {
            dataSet = ParamDataSet;
        }
    }
    */

    protected string summaryString;

	public string SummaryString {
		get {
			return summaryString;
		}
		set {
			summaryString = value;
		}
	}

    public WispDataSet DataSet { get => dataSet; }

    override public string GetHumainReadableValue()
	{
		return summaryString;
	}

	public override GameObject DrawVisualComponent(WispEntityEditor ParamParentEditor, Transform ParentTransform, Vector3 ParamPosition, WispEntityProperty ParamLinkedProperty)
	{
        WispElementSubInstance element = WispElementSubInstance.Draw(ParamParentEditor, ParamPosition, ParamLinkedProperty) as WispElementSubInstance;
        verticalHeight = element.SearchEditBox.MyRectTransform.rect.height;
        extraHeight = ParamParentEditor.ComponentVerticalSpacing;

        return element.gameObject;
    }

    //public override string GetValue()
    //{
    //    return currentValue;
    //}

    public override string GetJson()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("type", "sub_instance");
        result.Add("name", name);
        result.Add("label", label);
        //result.Add("value", GetHumainReadableValue());
        result.Add("value", GetValue());

        return result.ToJson();
    }
}

// *****************************************************************************************************************************************************************************

public class WispEntityPropertyMultiSubInstance : WispEntityProperty
{
    protected string summaryString;
    protected string subEntityName;
    protected string subEntityLabel;
    protected WispDataSet dataSet;
    protected Dictionary<string, string> subInstanceParameters = new Dictionary<string, string>();
    protected WispDataSetTable tableDataSet;

    public WispEntityPropertyMultiSubInstance(string ParamPropertyName, string ParamLabelText, Dictionary<string,string> ParamSubIntanceParameters, WispDataSet ParamDataSet = null)
    {
        name = ParamPropertyName;
        label = ParamLabelText;
        requireFullRow = true;
        requireFullWidth = true;

        if (ParamDataSet != null)
        {
            dataSet = ParamDataSet;
        }

        subInstanceParameters = ParamSubIntanceParameters;
    }

    public string SummaryString
    {
        get
        {
            return summaryString;
        }
        set
        {
            summaryString = value;
        }
    }

    public string SubEntityName
    {
        get
        {
            return subEntityName;
        }
        set
        {
            subEntityName = value;
        }
    }

    public string SubEntityLabel
    {
        get
        {
            return subEntityLabel;
        }
        set
        {
            subEntityLabel = value;
        }
    }

    public WispDataSet DataSet { get => dataSet; }
    public WispDataSetTable TableDataSet { get => tableDataSet; set => tableDataSet = value; }

    override public string GetHumainReadableValue()
    {
        return summaryString;
    }

    public override GameObject DrawVisualComponent(WispEntityEditor ParamParentEditor, Transform ParamParentTransform, Vector3 ParamPosition, WispEntityProperty ParamLinkedProperty)
    {
        WispElementMultiSubInstance element = WispElementMultiSubInstance.Draw(ParamParentEditor, ParamPosition, ParamLinkedProperty) as WispElementMultiSubInstance;
        verticalHeight = element.SearchEditBox.MyRectTransform.rect.height + 40f + element.InstanceTable.MyRectTransform.rect.height;
        extraHeight = ParamParentEditor.ComponentVerticalSpacing;

        return element.gameObject;
    }

    public List<string> GetSubIntanceParametersKeys()
    {
        List<string> result = new List<string>();

        if (subInstanceParameters == null)
            return null;

        foreach(KeyValuePair<string,string> kv in subInstanceParameters)
        {
            result.Add(kv.Key);
        }

        return result;
    }

    public string GetSubInstanceParameterLabel(string ParamIdentifier)
    {
        if (subInstanceParameters.ContainsKey(ParamIdentifier))
            return subInstanceParameters[ParamIdentifier];

        return null;
    }

    public override string GetJson()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("type", "multi_sub_instance");
        result.Add("name", name);
        result.Add("label", label);
        result.Add("value", GetValue());

        return result.ToJson();
    }
}

// *****************************************************************************************************************************************************************************

public class WispEntityPropertyBool : WispEntityProperty
{

    public WispEntityPropertyBool(string ParamPropertyName, string ParamLabelText)
    {
        name = ParamPropertyName;
        label = ParamLabelText;
    }

    public override string GetHumainReadableValue()
    {
        return currentValue;
    }

    public override void SetValue(string ParamValue)
    {
        currentValue = ParamValue;
    }

    public override GameObject DrawVisualComponent(WispEntityEditor ParamParentEditor, Transform ParentTransform, Vector3 ParamPosition, WispEntityProperty ParamLinkedProperty)
    {
        WispElementBool element = WispElementBool.Draw(ParamParentEditor, ParamPosition, ParamLinkedProperty) as WispElementBool;
        verticalHeight = element.CheckBox.MyRectTransform.rect.height;
        extraHeight = ParamParentEditor.ComponentVerticalSpacing;

        return element.gameObject;
    }

    public override string GetJson()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("type", "bool");
        result.Add("name", name);
        result.Add("label", label);
        result.Add("value", currentValue);

        return result.ToJson();
    }
}