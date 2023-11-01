using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class WispDataSet
{
    
}

public class WispInstanceDataSet : WispDataSet
{
    private Dictionary<string, WispEntityInstance> instances = new Dictionary<string, WispEntityInstance>();

    private bool AddInstance(string ParamIdentifier, WispEntityInstance ParamInstance)
    {
        if (instances.ContainsKey(ParamIdentifier))
        {
            WispVisualComponent.LogError("Invalid instance identifier.");
            return false;
        }

        instances.Add(ParamIdentifier, ParamInstance);

        return true;
    }

    public WispEntityInstance GetInstance(string ParamIdentifier)
    {
        return instances[ParamIdentifier];
    }
}

public class WispMinimalDataSet : WispDataSet
{
    // ID and Summary string dictionary, useful for Dropdown lists.
    private Dictionary<string, string> instances = new Dictionary<string, string>();

    public bool AddInstance(string ParamIdentifier, WispEntityInstance ParamInstance)
    {
        if (instances.ContainsKey(ParamIdentifier))
        {
            WispVisualComponent.LogError("Invalid instance identifier.");
            return false;
        }

        instances.Add(ParamIdentifier, ParamInstance.SummaryString);

        return true;
    }

    public bool AddSummaryString(string ParamIdentifier, string ParamSummaryString)
    {
        if (instances.ContainsKey(ParamIdentifier))
        {
            WispVisualComponent.LogError("Invalid instance identifier.");
            return false;
        }

        instances.Add(ParamIdentifier, ParamSummaryString);

        return true;
    }

    public string GetSummaryString(string ParamIdentifier)
    {
        return instances[ParamIdentifier];
    }

    public void Clear()
    {
        instances.Clear();
    }

    public List<string> GetKeys()
    {
        List<string> result = new List<string>();

        foreach(KeyValuePair<string, string> kv in instances)
        {
            result.Add(kv.Key);
        }

        return result;
    }
}

public class WispDataSetTable : WispDataSet
{
    Dictionary<string, Dictionary<int, string>> columns = new Dictionary<string, Dictionary<int, string>>();

    public void AddColumn(string ParamIdentifier)
    {
        if (columns.ContainsKey(ParamIdentifier))
        {
            WispVisualComponent.LogError("Dataset already has a column named : " + ParamIdentifier);
            return;
        }

        columns.Add(ParamIdentifier, new Dictionary<int, string>());
    }

    public string GetColumnIdentifierByIndex(int ParamIndex)
    {
        int c = 0;
        foreach (KeyValuePair<string, Dictionary<int, string>> kvp in columns)
        {
            if (c == ParamIndex)
                return kvp.Key;

            c++;
        }

        return null;
    }

    public List<string> GetColumns()
    {
        return columns.Keys.ToList<string>();
    }

    public void AddRow(int ParamIndex)
    {
        foreach(KeyValuePair<string, Dictionary<int,string>> kv in columns)
        {
            if (kv.Value.ContainsKey(ParamIndex))
            {
                WispVisualComponent.LogError("Dataset already contains row : " + ParamIndex);
                return;
            }

            kv.Value.Add(ParamIndex, "");
        }
    }

    public void AddRow(int ParamIndex, params string[] ParamValues)
    {
        int c = 0;

        foreach (KeyValuePair<string, Dictionary<int, string>> kv in columns)
        {
            if (kv.Value.ContainsKey(ParamIndex))
            {
                WispVisualComponent.LogError("Dataset already contains row : " + ParamIndex);
                return;
            }

            kv.Value.Add(ParamIndex, ParamValues[c]);

            c++;
        }
    }

    public List<int> GetRows()
    {
        List<int> result = new List<int>();

        string idOfFirstColumn = GetColumnIdentifierByIndex(0);

        if (idOfFirstColumn != null)
        {
            foreach (KeyValuePair<int, string> kv in columns[idOfFirstColumn])
            {
                result.Add(kv.Key);
            }
        }

        return result;
    }

    //public int GetRowCount()
    //{
    //    return 
    //}

    public void SetCell(string ParamIdentifier, int ParamIndex, string ParamValue)
    {
        if (!columns.ContainsKey(ParamIdentifier))
        {
            WispVisualComponent.LogError("Dataset has no column named : " + ParamIdentifier);
            return;
        }

        if (!columns[ParamIdentifier].ContainsKey(ParamIndex))
        {
            WispVisualComponent.LogError("Dataset has no row : " + ParamIdentifier);
            return;
        }

        columns[ParamIdentifier][ParamIndex] = ParamValue;
    }

    public string GetCell(string ParamIdentifier, int ParamIndex)
    {
        if (!columns.ContainsKey(ParamIdentifier))
        {
            WispVisualComponent.LogError("Dataset has no column named : " + ParamIdentifier);
            return null;
        }

        if (!columns[ParamIdentifier].ContainsKey(ParamIndex))
        {
            WispVisualComponent.LogError("Dataset has no row : " + ParamIdentifier);
            return null;
        }

        return columns[ParamIdentifier][ParamIndex];
    }

    public bool DoesRowExist(int ParamRowIndex)
    {
        if (columns.Count == 0)
            return false;

        string idOfFirstColumn = GetColumnIdentifierByIndex(0);

        return columns[idOfFirstColumn].ContainsKey(ParamRowIndex);
    }
}