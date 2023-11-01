using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TinyJson;
using WispExtensions;

public class WispElement : WispVisualComponent
{
    protected WispEntityProperty linkedProperty;
    protected WispEntityEditor parentEditor;

    public WispEntityProperty LinkedProperty { get => linkedProperty; set => linkedProperty = value; }
    public WispEntityEditor ParentEditor { get => parentEditor; set => parentEditor = value; }

    // public virtual void Focus()
    // {
    //     //...
    // }
}

// --------------------------------------------------------------------------------------------------------------------------

public class WispElementText : WispElement
{
    private WispEditBox mainEditBox;

    public WispEditBox MainEditBox { get => mainEditBox; set => mainEditBox = value; }

    public override string GetValue()
    {
        return mainEditBox.GetValue();
    }

    public static WispElementText Draw (WispEntityEditor ParamParentEditor, Vector2 ParamPosition, WispEntityProperty ParamLinkedProperty)
    {
        GameObject go = WispEditBox.Create(ParamParentEditor.MyRectTransform).gameObject as GameObject;
        go.name = "TextElement";

        WispElementText element = go.AddComponent<WispElementText>();
        element.SetParent(ParamParentEditor.ScrollView, true, false);
        element.mainEditBox = go.GetComponent<WispEditBox>();
        element.mainEditBox.Initialize();
        element.mainEditBox.SetParent(element, true);
        element.ParentEditor = ParamParentEditor;
        int index = ParamParentEditor.PushOrderedVisualComponent(element.mainEditBox);
        element.mainEditBox.Base.onSelect.AddListener(delegate { ParamParentEditor.SetCurrentFocusIndex(index); });

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition3D = ParamPosition;
        rt.SetParent(ParamParentEditor.ContentRect);

        element.mainEditBox.Label = ParamLinkedProperty.Label;
        element.mainEditBox.SetValue(ParamLinkedProperty.GetValue());
        element.mainEditBox.ReadOnly = ParamLinkedProperty.Editable;

        ParamLinkedProperty.Element = element;

        return element;
    }

    public override void Focus()
    {
        mainEditBox.Focus();
    }
}

// --------------------------------------------------------------------------------------------------------------------------

public class WispElementDate : WispElement
{
    private WispEditBox mainEditBox;

    public WispEditBox MainEditBox { get => mainEditBox; set => mainEditBox = value; }

    public override string GetValue()
    {
        return mainEditBox.GetValue();
    }

    public static WispElementDate Draw(WispEntityEditor ParamParentEditor, Vector2 ParamPosition, WispEntityProperty ParamLinkedProperty)
    {
        GameObject go = UnityEngine.Object.Instantiate(WispPrefabLibrary.Default.EditBox) as GameObject;
        go.name = "DateElement";

        WispElementDate element = go.AddComponent<WispElementDate>();
        element.SetParent(ParamParentEditor.ScrollView, true, false);
        element.mainEditBox = go.GetComponent<WispEditBox>();
        element.mainEditBox.Initialize();
        element.mainEditBox.SetParent(element, true);
        element.ParentEditor = ParamParentEditor;
        int index = ParamParentEditor.PushOrderedVisualComponent(element.mainEditBox);
        element.mainEditBox.Base.onSelect.AddListener(delegate { ParamParentEditor.SetCurrentFocusIndex(index); });

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition3D = ParamPosition;
        rt.SetParent(ParamParentEditor.ContentRect);

        element.mainEditBox.PickerType = WispEditBox.EditBoxPickerType.DatePicker;
        element.mainEditBox.Label = ParamLinkedProperty.Label;
        element.mainEditBox.SetValue(ParamLinkedProperty.GetValue());
        element.mainEditBox.ReadOnly = ParamLinkedProperty.Editable;

        ParamLinkedProperty.Element = element;
        return element;
    }

    public override void Focus()
    {
        mainEditBox.Focus();
    }
}

// --------------------------------------------------------------------------------------------------------------------------
public class WispElementImage : WispElement
{
    private WispImage image;

    public WispImage Image { get => image; set => image = value; }

    public static WispElementImage Draw(WispEntityEditor ParamParentEditor, Vector2 ParamPosition, WispEntityProperty ParamLinkedProperty)
    {
        GameObject go = WispImage.Create(ParamParentEditor.MyRectTransform).gameObject;
        go.name = "ImageElement";

        WispElementImage element = go.AddComponent<WispElementImage>();
        element.SetParent(ParamParentEditor.ScrollView, true);
        element.image = go.GetComponent<WispImage>();
        element.image.Initialize();
        element.image.SetParent(element, true);
        element.ParentEditor = ParamParentEditor;
        element.image.BorderRule = WispBorderRule.Always;
        int index = ParamParentEditor.PushOrderedVisualComponent(element.image);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition3D = ParamPosition;
        rt.SetParent(ParamParentEditor.ContentRect);

        element.image.SetValue(ParamLinkedProperty.GetValue());

        ParamLinkedProperty.Element = element;
        return element;
    }

    public override void Focus()
    {
        image.Focus();
    }
}

// --------------------------------------------------------------------------------------------------------------------------

public class WispElementSubInstance : WispElement
{
    private WispEditBox searchEditBox;

    public WispEditBox SearchEditBox { get => searchEditBox; }

    public override string GetValue()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        result.Add("field_value", searchEditBox.Base.text);

        if (searchEditBox.HiddenValue == null)
            result.Add("hidden_value", "");
        else
            result.Add("hidden_value", searchEditBox.HiddenValue);

        if (searchEditBox.SelectedItem != null)
            result.Add("item_text", searchEditBox.SelectedItem.TextValue);
        else
            result.Add("item_text", "");

        return result.ToJson();
    }

    public static WispElementSubInstance Draw(WispEntityEditor ParamParentEditor, Vector2 ParamPosition, WispEntityProperty ParamLinkedProperty)
    {
        GameObject go = UnityEngine.Object.Instantiate(WispPrefabLibrary.Default.EditBox) as GameObject;
        go.name = "SubIntanceElement";

        WispElementSubInstance element = go.AddComponent<WispElementSubInstance>();
        element.SetParent(ParamParentEditor.ScrollView, true, false);
        element.searchEditBox = go.GetComponent<WispEditBox>();
        element.searchEditBox.Initialize();
        element.searchEditBox.SetParent(element, true);
        element.ParentEditor = ParamParentEditor;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition3D = ParamPosition;
        rt.SetParent(ParamParentEditor.ContentRect);

        WispEntityPropertySubInstance property = ParamLinkedProperty as WispEntityPropertySubInstance;

        element.searchEditBox.PickerType = WispEditBox.EditBoxPickerType.List;
        element.searchEditBox.Label = ParamLinkedProperty.Label;
        element.searchEditBox.SetValue(property.SummaryString);
        element.searchEditBox.ReadOnly = ParamLinkedProperty.Editable;
        int index = ParamParentEditor.PushOrderedVisualComponent(element.searchEditBox);
        element.searchEditBox.Base.onSelect.AddListener(delegate { ParamParentEditor.SetCurrentFocusIndex(index); });

        if (property.DataSet != null)
            element.searchEditBox.DropDownList.LoadFromDataSet(property.DataSet);

        ParamLinkedProperty.Element = element;
        return element;
    }

    public override void Focus()
    {
        searchEditBox.Focus();
    }
}

// --------------------------------------------------------------------------------------------------------------------------

public class WispElementMultiSubInstance : WispElement
{
    private WispEditBox searchEditBox;
    private Dictionary<string, WispEditBox> operationParameters = new Dictionary<string, WispEditBox>();
    private WispTable instanceTable;
    private UnityAction onSearchEditBoxConfirm = null;
    private new WispEntityPropertyMultiSubInstance linkedProperty;
    private List<int> onSearchEditBoxConfirmEventIds = new List<int>();
    private Dictionary<WispRow, WispButton> deleteButtons = new Dictionary<WispRow, WispButton>();
    private WispButton submitButton;

    public WispEditBox SearchEditBox { get => searchEditBox; }
    public WispTable InstanceTable { get => instanceTable; }
    public UnityAction OnSearchEditBoxConfirm
    {
        set
        {
            WispKeyBoardEventSystem.ReplaceAction(onSearchEditBoxConfirm, value);
            onSearchEditBoxConfirm = value;

            submitButton.Base.onClick.RemoveAllListeners();
            submitButton.AddOnClickAction(value);
        }
    }

    public override string GetValue()
    {
        return instanceTable.GetJson();
    }

    public static WispElementMultiSubInstance Draw(WispEntityEditor ParamParentEditor, Vector2 ParamPosition, WispEntityProperty ParamLinkedProperty/*, Dictionary<string,string> ParamSubInstanceList, List<WispEntityInstance> ParamInstances*/)
    {
        GameObject go = new GameObject("MyGO", typeof(RectTransform));
        go.name = "MultiSubIntanceElement";

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition3D = ParamPosition;
        rt.SetParent(ParamParentEditor.ContentRect);

        WispElementMultiSubInstance element = go.AddComponent<WispElementMultiSubInstance>();
        element.ParentEditor = ParamParentEditor;
        element.SetParent(ParamParentEditor.ScrollView, true, false);

        element.searchEditBox = WispEditBox.Create(rt);
        element.searchEditBox.Initialize();
        element.searchEditBox.SetParent(element, true);
        element.searchEditBox.Label = ParamLinkedProperty.Label;
        element.searchEditBox.Clear();  
        element.searchEditBox.ReadOnly = ParamLinkedProperty.Editable;
        element.searchEditBox.MyRectTransform.sizeDelta = new Vector2(ParamParentEditor.ComponentWidth, ParamParentEditor.ComponentHeight);
        element.searchEditBox.AnchorTo("left-top");
        element.searchEditBox.PivotAround("left-top");
        element.searchEditBox.Set_X_Position(ParamPosition.x);
        element.searchEditBox.Set_Y_Position(ParamPosition.y);
        int indx = ParamParentEditor.PushOrderedVisualComponent(element.searchEditBox);
        element.searchEditBox.Base.onSelect.AddListener(delegate { ParamParentEditor.SetCurrentFocusIndex(indx); });

        WispEntityPropertyMultiSubInstance property = ParamLinkedProperty as WispEntityPropertyMultiSubInstance;
        
        // Preparing some variables for operation parameters
        List<string> prms = property.GetSubIntanceParametersKeys();
        int prmCount = 0;

        if (prms != null)
            prmCount = prms.Count;

        // List from dataset or Simple input
        if (property.DataSet != null)
        {
            element.searchEditBox.PickerType = WispEditBox.EditBoxPickerType.List;
            element.searchEditBox.DropDownList.LoadFromDataSet(property.DataSet);
        }
        else
        {
            element.searchEditBox.PickerType = WispEditBox.EditBoxPickerType.None;
        }

        element.onSearchEditBoxConfirm = delegate { element.defaultOnSearchEditBoxConfirm(); };
        element.onSearchEditBoxConfirmEventIds = WispKeyBoardEventSystem.AddEventOnKey(element.onSearchEditBoxConfirm, element.searchEditBox, true, KeyCode.Return, KeyCode.KeypadEnter);
        
        //Caculate the table width
        float tableWidth = ParamParentEditor.Width - (ParamParentEditor.StartingX*2) - ParamParentEditor.Width.GetPercentage(10);
        const float spacing = 8;
        float prmWidth = (tableWidth/(prmCount+2)); // +1 For the searchEditBox and +1 for the submit button // +2 in total.
        
        element.searchEditBox.Width = prmWidth - spacing;
        float submitButtonXPos = prmWidth;

        if (prms != null)
        {
            int c = 1;

            foreach (string s in prms)
            {
                WispEditBox edt = WispEditBox.Create(rt);
                edt.SetParent(element, true);
                edt.AnchorTo("left-top");
                edt.PivotAround("left-top");
                edt.Width = prmWidth - spacing;
                edt.Height = 32;

                edt.Set_X_Position(c*prmWidth);
                submitButtonXPos += prmWidth; 

                edt.Set_Y_Position(element.searchEditBox.Y);
                edt.Label = property.GetSubInstanceParameterLabel(s);

                c++;

                element.operationParameters.Add(s, edt);
                int i = ParamParentEditor.PushOrderedVisualComponent(edt);
                edt.Base.onSelect.AddListener(delegate { ParamParentEditor.SetCurrentFocusIndex(i); });

                WispKeyBoardEventSystem.AddEventOnKey(element.onSearchEditBoxConfirm, edt, true, KeyCode.Return, KeyCode.KeypadEnter);
            }
        }

        //Create the submit button.
        element.submitButton = WispButton.Create(rt);
        element.submitButton.SetParent(element, true);
        element.submitButton.AnchorTo("left-top");
        element.submitButton.PivotAround("left-top");
        element.submitButton.Height = 32;
        element.submitButton.Set_X_Position(submitButtonXPos);
        element.submitButton.Set_Y_Position(element.searchEditBox.Y);
        element.submitButton.Width = prmWidth;
        element.submitButton.SetValue("Submit");
        element.submitButton.AddOnClickAction(element.onSearchEditBoxConfirm);

        //Create the instance table.
        element.instanceTable = WispTable.Create(rt);
        element.instanceTable.Initialize();
        element.instanceTable.SetParent(element, true);
        element.instanceTable.AnchorTo("left-top");
        element.instanceTable.Width = tableWidth;
        element.instanceTable.Height = ParamParentEditor.ComponentHeight + 256;
        element.instanceTable.PivotAround("left-top");
        element.instanceTable.Set_X_Position(ParamPosition.x);
        element.instanceTable.Set_Y_Position(ParamPosition.y - 40);
        element.instanceTable.AllowEditing = false;
        element.instanceTable.AllowHeaderEditing = false;
        ParamParentEditor.PushOrderedVisualComponent(element.instanceTable);

        WispColumn toolColumn = element.instanceTable.AddColumn("tool-delete", "");
        element.instanceTable.ExcludeColumnFromJsonOutput("tool-delete");
        toolColumn.Width = 192f;

        // Fill Instance Table
        if (ParamLinkedProperty.Value != null && ParamLinkedProperty.Value != "")
        {
            List<string> instances = ( "[" + ParamLinkedProperty.Value + "]" ).FromJson<List<string>>();

            foreach (string s in instances)
            {
                Dictionary<string, string> instance = ("{" + s + "}").FromJson<Dictionary<string, string>>();

                List<string> values = new List<string>();
                values.Add(""); // tool-delete column has no text

                Dictionary<string, string> properties = ("{" + instance["properties"] + "}").FromJson<Dictionary<string, string>>();

                foreach(KeyValuePair<string,string> kvp in properties)
                {
                    Dictionary<string, string> propertiesDetail = ("{" + kvp.Value + "}").FromJson<Dictionary<string, string>>();

                    WispColumn col = element.instanceTable.GetColumnByID(propertiesDetail["name"]);

                    if (col == null)
                    {
                        element.instanceTable.AddColumn(propertiesDetail["name"], propertiesDetail["label"]);
                    }

                    values.Add(propertiesDetail["value"]);
                }

                WispRow row = element.instanceTable.AddRowWithValues(values.ToArray());
                row.HiddenValue = instance["ID"];

                element.deleteButtons.Add(row, element.GenerateDeleteButton(row));
            }
        }

        ParamLinkedProperty.Element = element;
        element.linkedProperty = ParamLinkedProperty as WispEntityPropertyMultiSubInstance;

        return element;
    }

    private void defaultOnSearchEditBoxConfirm()
    {
        if (searchEditBox.GetValue() == "")
            return;

        bool initial = instanceTable.BlockObserverNotification;
        instanceTable.BlockObserverNotification = true;

        // Add column if non-existant
        if (instanceTable.GetColumnByID(linkedProperty.SubEntityName) == null)
            instanceTable.AddColumn(linkedProperty.Name, linkedProperty.Label);

        // Add to the table
        WispRow row = instanceTable.AddRow();
        instanceTable.GetCell(linkedProperty.Name, row.Index).SetValue(searchEditBox.GetValue());

        foreach (KeyValuePair<string, WispEditBox> kv in operationParameters)
        {
            // Add column if non-existant
            if (instanceTable.GetColumnByID(kv.Key) == null)
                instanceTable.AddColumn(kv.Key, kv.Value.Label);

            instanceTable.GetCell(kv.Key, row.Index).SetValue(kv.Value.GetValue());

            kv.Value.Clear();
        }

        // Delete button
        deleteButtons.Add(row, GenerateDeleteButton(row));

        // Clear field
        searchEditBox.Clear();
        searchEditBox.Focus();

        instanceTable.BlockObserverNotification = initial;
        instanceTable.NotifyObserversOnEdit();
    }

    public List<string> GetOperationParametersKeys()
    {
        List<string> result = new List<string>();

        foreach (KeyValuePair<string, WispEditBox> kvp in operationParameters)
        {
            result.Add(kvp.Key);
        }

        return result;
    }

    public string GetOperationParameterValue(string ParamID)
    {
        if (operationParameters.ContainsKey(ParamID))
            return operationParameters[ParamID].GetValue();
        else
            return null;
    }

    public override void Focus()
    {
        searchEditBox.Focus();
    }

    // For when you want to setup custom actions for the delete row button.
    public void ClearDeleteButtonEvents()
    {
        foreach(KeyValuePair<WispRow, WispButton> kvp in deleteButtons)
        {
            kvp.Value.Base.onClick.RemoveAllListeners();
        }
    }

    public WispButton GetDeleteButton(WispRow ParamRow)
    {
        if (deleteButtons.ContainsKey(ParamRow))
        {
            return deleteButtons[ParamRow];
        }

        return null;
    }

    public void ClearAndResetOperationParameters()
    {
        foreach(KeyValuePair<string, WispEditBox> kvp in operationParameters)
        {
            kvp.Value.Clear();
        }

        searchEditBox.Clear();
        searchEditBox.Focus();
    }

    public override void SetBusyMode(bool ParamState)
    {
        foreach (KeyValuePair<string, WispEditBox> kvp in operationParameters)
        {
            kvp.Value.SetBusyMode(ParamState);
        }

        searchEditBox.SetBusyMode(ParamState);
        instanceTable.SetBusyMode(ParamState);
    }

    private WispButton GenerateDeleteButton(WispRow ParamRow)
    {
        WispButton btn = WispButton.Create(instanceTable.GetCell("tool-delete", ParamRow.Index).transform);
        btn.SetParent(instanceTable, true);
        btn.AnchorStyleExpanded(true);
        btn.EnableIcon = false;
        btn.BorderRule = WispBorderRule.Never;
        btn.SetValue("x");
        btn.AddOnClickAction(delegate { instanceTable.RemoveRow(ParamRow); });

        return btn;
    }
}

// --------------------------------------------------------------------------------------------------------------------------

public class WispElementBool : WispElement
{
    private WispCheckBox checkBox;

    public WispCheckBox CheckBox { get => checkBox; }

    public override string GetValue()
    {
        return checkBox.GetValue();
    }

    public static WispElementBool Draw(WispEntityEditor ParamParentEditor, Vector2 ParamPosition, WispEntityProperty ParamLinkedProperty)
    {
        GameObject go = WispCheckBox.Create(ParamParentEditor.MyRectTransform).gameObject as GameObject;
        go.name = "BoolElement";

        WispElementBool element = go.AddComponent<WispElementBool>();
        element.SetParent(ParamParentEditor.ScrollView, true, false);
        element.checkBox = go.GetComponent<WispCheckBox>();
        element.checkBox.Initialize();
        element.checkBox.SetParent(element, true);
        element.ParentEditor = ParamParentEditor;
        int index = ParamParentEditor.PushOrderedVisualComponent(element.checkBox);
        element.checkBox.OnSelect.AddListener(delegate { ParamParentEditor.SetCurrentFocusIndex(index); });

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition3D = ParamPosition;
        rt.SetParent(ParamParentEditor.ContentRect);

        ParamLinkedProperty.Element = element;

        element.checkBox.Label = ParamLinkedProperty.Label;
        element.checkBox.SetValue(ParamLinkedProperty.GetValue());

        return element;
    }

    public override void Focus()
    {
        checkBox.Focus();
    }
}