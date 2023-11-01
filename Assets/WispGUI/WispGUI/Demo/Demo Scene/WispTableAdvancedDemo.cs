using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WispExtensions;

public class WispTableAdvancedDemo : MonoBehaviour
{
    public Sprite[] icons;
    public string[] names;

    WispTable table;
    WispProgressBar[] bars = new WispProgressBar[5];

    float lastUpdateTime = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        table = GetComponent<WispTable>();

        WispColumn c_id = table.AddColumn("id","ID");
        c_id.Width = 32f;

        WispColumn c_icon = table.AddColumn("icon","Icon");
        c_icon.Width = 32f;

        WispColumn c_name = table.AddColumn("name","Name");
        c_name.Width = 128f;

        WispColumn c_progress = table.AddColumn("progress","Progress");
        c_progress.Width = table.Width - (c_id.Width + c_icon.Width + c_name.Width);

        for (int i = 0; i < 5; i++)
        {
            WispRow row = table.AddRow();
            table.GetCell("id", row.Index).SetValue((i+1).ToString());

            WispTableCell cell = table.GetCell("icon", row.Index);
            cell.SetValue("");

            WispImage icon = WispImage.Create(cell.transform);
            icon.SetParent(table, true);
            icon.MyRectTransform.AnchorStyleExpanded(2f);
            icon.SetValue(icons[i]);

            table.GetCell("name", row.Index).SetValue(names[i]);
            
            cell = table.GetCell("progress", row.Index);

            bars[i] = WispProgressBar.Create(cell.transform);
            bars[i].AnchorStyleExpanded(true);
            bars[i].FillSpeed = 100f;
            bars[i].SetParent(table, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > lastUpdateTime + 1)
        {
            for (int i = 0; i < 5; i++)
            {
                bars[i].SetValue(Random.Range(0,100));
            }

            lastUpdateTime = Time.time;
        }
    }
}