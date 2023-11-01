using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispDebugTools : MonoBehaviour
{
    public class WispLogEntry
    {
        public enum WispLogEntryType { Basic, Warning, Error, Feedback }
        
        public WispLogEntryType type;
        public string text; 
        public bool inTable = false;
    }
    
    private WispDialogWindow popup = null;
    private WispTabView tabView = null;
    private WispTable consoleTable = null;
    private WispTable componentTable = null;

    private static Dictionary<int, WispLogEntry> logEntries = new Dictionary<int, WispLogEntry>();
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (Debug.isDebugBuild && Input.GetKey(KeyCode.RightControl))
            {
                OpenDebugWindow();
            }
        }
    }

    private void OpenDebugWindow()
    {
        if (popup == null)
        {
            popup = WispDialogWindow.CreateAndOpen(null);
            popup.DestroyOnClose = false;

            popup.ButtonPanel.AddButton("close", "Close", WispWindow.CloseParentWindow());

            tabView = WispTabView.Create(popup.ScrollView.ContentRect);
            tabView.AnchorStyleExpanded(true);

            WispPage consolePage = tabView.AddPage("console", "Console");
            WispPage componentPage = tabView.AddPage("components", "Components");
            
            consoleTable = WispTable.Create(consolePage.MyRectTransform);
            consoleTable.SetParent(tabView, true);
            consoleTable.AnchorStyleExpanded(true);
            consoleTable.AddColumn("entry_id", "Entry ID");
            consoleTable.AddColumn("text", "Message");
            consoleTable.AdjustColumnWidthToView();

            componentTable = WispTable.Create(componentPage.MyRectTransform);
            componentTable.SetParent(tabView, true);
            componentTable.AnchorStyleExpanded(true);
            componentTable.AddColumn("id", "ID");
            componentTable.AddColumn("name", "Name");
            componentTable.AddColumn("parent", "Parent");
            componentTable.AddColumn("active", "Is Active ?");
            consoleTable.AdjustColumnWidthToView();
        }
        else
        {
            popup.Open();
        }

        UpdateConsoleTable();
        UpdateComponentTable();
    }

    public static void AddNewLogEntry(string ParamText, WispLogEntry.WispLogEntryType ParamType)
    {
        WispLogEntry entry = new WispLogEntry();
        entry.text = ParamText;
        entry.type = ParamType;

        logEntries.Add(logEntries.Count + 1, entry);
    }

    private void UpdateConsoleTable()
    {
        foreach(KeyValuePair<int, WispLogEntry> kvp in WispDebugTools.logEntries)
        {
            if (!kvp.Value.inTable)
            {
                consoleTable.AddRowWithValues(kvp.Key.ToString(), kvp.Value.text);
                kvp.Value.inTable = true;
            }
        }
    }

    private void UpdateComponentTable()
    {
        componentTable.ClearRows();

        List<int> vc_keys = WispVisualComponent.GetVcIDList();

        foreach(int i in vc_keys)
        {
            WispVisualComponent vc = WispVisualComponent.GetComponentById(i);

            componentTable.AddRowWithValues(vc.Id.ToString(), vc.name, vc.GetParentName(), vc.gameObject.activeInHierarchy.ToString());
        }
    }
}