using UnityEngine;

public class WispEditorCustomControlNameAttribute : PropertyAttribute
{
    public string ControlName = "";
    
    public WispEditorCustomControlNameAttribute(string ParamControlName)
    {
        ControlName = ParamControlName;
    }
}