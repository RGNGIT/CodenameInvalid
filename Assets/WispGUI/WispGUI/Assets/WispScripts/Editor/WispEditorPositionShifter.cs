using System;
using UnityEngine;
using UnityEditor;
using WispExtensions;

public class WispEditorPositionShifter : EditorWindow
{
    private string InfoText = 
                                "- Enter horizontal and vertical shifting amounts." 
                                + Environment.NewLine
                                + "- Multiple selection allowed." 
                                + Environment.NewLine
                                + "- Works on RectTransform only."
                                + Environment.NewLine
                                + "- Undo is possible only once.";
    
    private string x = "0";
    private string y = "0";

    private static Vector2 undoBuffer = new Vector2(0,0);
    
    public static void Init()
    {
        WispEditorPositionShifter window = (WispEditorPositionShifter)EditorWindow.GetWindow(typeof(WispEditorPositionShifter));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Space(16f);
        GUILayout.Label(InfoText, EditorStyles.boldLabel);
        GUILayout.Space(16f);
        x = EditorGUILayout.TextField("Horizontal shifting amount : ", x);
        GUILayout.Space(8f);
        y = EditorGUILayout.TextField("Vertical shifting amount : ", y);
        GUILayout.Space(16f);

        if (GUILayout.Button("Apply Shift"))
        {
            ApplyShift(x,y);
        }

        if (GUILayout.Button("Undo"))
        {
            ApplyShift(undoBuffer.x.ToString(),undoBuffer.y.ToString());
        }

        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }

    private void ApplyShift(string ParamX, string ParamY)
    {
        float x = ParamX.ToFloat();
        float y = ParamY.ToFloat();

        undoBuffer = new Vector2(-x,-y);

        UnityEngine.Object[] objs = Selection.objects;

        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i] is GameObject)
            {
                RectTransform rt = (objs[i] as GameObject).GetComponent<RectTransform>();

                if (rt != null)
                {
                    WispRectTransformAnchorSettings settings = rt.GetAnchorSettings();
                    rt.ShiftPosition(x,y);
                    rt.SetAnchorSettings(settings);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(rt.gameObject.scene);
                }
            }
        }
    }
}