// From : https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(WispEditorCustomControlNameAttribute))]
public class WispEditorCustomControlNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        WispEditorCustomControlNameAttribute control = attribute as WispEditorCustomControlNameAttribute;

        GUI.SetNextControlName(control.ControlName);
        prop.stringValue = EditorGUI.TextField(position, label.text, prop.stringValue);
    }
}