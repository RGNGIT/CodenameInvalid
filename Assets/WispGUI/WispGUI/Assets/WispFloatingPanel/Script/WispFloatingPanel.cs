using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispFloatingPanel : WispPanel
{
    /// <summary>
    /// ...
    /// </summary>
    public static new WispFloatingPanel Create(Transform ParamTransform)
    {
        GameObject go;
        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.FloatingPanel, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.FloatingPanel);
        }

        return go.GetComponent<WispFloatingPanel>();
    }
}