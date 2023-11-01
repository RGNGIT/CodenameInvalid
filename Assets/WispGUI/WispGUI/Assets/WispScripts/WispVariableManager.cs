using System;
using System.Collections.Generic;
using System.IO;
using TinyJson;
using UnityEngine;

public class WispVariableManager {

    private Dictionary<string, string> variables = new Dictionary<string, string>();
    private string filePath = "";
    private bool isEmpty = true;

    public string FilePath { get => filePath; }
    public bool IsEmpty { get => isEmpty; }

    private WispVariableManager()
    {
        // Default constructor made private.
    }

    public static WispVariableManager LoadFromFile(string ParamFilePath, bool ParamAutoCreateFile = false)
    {
        WispVariableManager result = new WispVariableManager();
        result.filePath = ParamFilePath;

        if (!File.Exists(ParamFilePath) && ParamAutoCreateFile)
        {
            File.Create(ParamFilePath);
            return result;
        }

        if (!File.Exists(ParamFilePath))
        {
            WispVisualComponent.LogError("WispConstantManager : File not found : " + ParamFilePath);
            return null;
        }

        string fileContent = File.ReadAllText(ParamFilePath);

        if (fileContent.Length == 0)
        {
            WispVisualComponent.LogWarning("WispConstantManager : Empty file : " + ParamFilePath + " -  Make sure to setup up variables by pressing the setup button.");
            return result;
        }
        else
        {
            result.isEmpty = false;
        }

        Dictionary<string, string> jsonResult = fileContent.FromJson<Dictionary<string, string>>();

        foreach(KeyValuePair<string,string> kv in jsonResult)
        {
            result.Add(kv.Key, kv.Value);
        }

        return result;
    }

    public string Get(string ParamVariableName)
    {
        if (variables.ContainsKey(ParamVariableName))
            return variables[ParamVariableName];
        else
            return null;
    }

    public void Add(string ParamVariableName, string ParamValue)
    {
        if (variables.ContainsKey(ParamVariableName))
            return;
        else
            variables.Add(ParamVariableName, ParamValue);

        isEmpty = false;
    }

    public void Set(string ParamVariableName, string ParamValue)
    {
        if (variables.ContainsKey(ParamVariableName))
            variables[ParamVariableName] = ParamValue;
        else
            variables.Add(ParamVariableName, ParamValue);
    }

    public void SaveToFile()
    {
        Debug.Log("Saving config at : " + filePath);

        try
        {
            File.WriteAllText(filePath, variables.ToJson());
        }
        catch (Exception e)
        {
            WispVisualComponent.LogError("Unable to save configuration, Exception Message : " + e.Message);
        }
    }
}