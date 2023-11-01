using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WispExtensions;

public class WispEntityEditorDemo : MonoBehaviour
{
    [SerializeField] private WispTable table;

    private WispEntityInstance inventionEntity = new WispEntityInstance("invention","Invention");
    private WispEntityEditor editor;
    
    // Start is called before the first frame update
    void Start()
    {
        inventionEntity.AddProperty(new WispEntityPropertyText("name", "Invention name"));
        inventionEntity.AddProperty(new WispEntityPropertyDate("date", "Date of Invention"));
        inventionEntity.AddProperty(new WispEntityPropertyBool("is_patented", "Is Patented ?"));
        inventionEntity.AddProperty(new WispEntityPropertyMultiSubInstance("inventorsList","Inventor(s) Fullname(s)", null, null));
        
        table.AddColumn("name", "Name");
        table.AddColumn("date", "Date of Invention");
        table.AddColumn("is_patented", "Is Patented ?");
        table.AddColumn("inventorsList", "Inventor(s)");
        table.AdjustColumnWidthToView();

        editor = GetComponent<WispEntityEditor>();
        editor.RenderInstance(inventionEntity);

        editor.AddOkOnClickAction(RecordInstanceInTable);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            table.ClearRows();
        }
    }

    private void RecordInstanceInTable()
    {
        WispEntityInstance instance = editor.GetInstanceInCurrentState();

        string name = instance.GetEntityPropertyByName("name").GetValue();
        string date = instance.GetEntityPropertyByName("date").GetValue();
        string is_patented = instance.GetEntityPropertyByName("is_patented").GetValue();
        string inventorsList = instance.GetEntityPropertyByName("inventorsList").GetValue();
        string inventorCount = instance.GetEntityPropertyByName("inventorsList").Element.CastObject<WispElementMultiSubInstance>().InstanceTable.RowCount.ToString();

        WispRow row = table.AddRowWithValues(name, date, is_patented, inventorCount);
        table.GetCell("inventorsList", row.Index).HiddenValue = inventorsList;
    }
}