using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalHidePickerTypeAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		ConditionalHidePickerTypeAttribute condHAtt = (ConditionalHidePickerTypeAttribute)attribute;
		bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

		bool wasEnabled = GUI.enabled;
		GUI.enabled = enabled;
		if (!condHAtt.HideInInspector || enabled)
		{
			EditorGUI.PropertyField(position, property, label, true);
		}

		GUI.enabled = wasEnabled;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		ConditionalHidePickerTypeAttribute condHAtt = (ConditionalHidePickerTypeAttribute)attribute;
		bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

		if (!condHAtt.HideInInspector || enabled)
		{
			return EditorGUI.GetPropertyHeight(property, label);
		}
		else
		{
			return -EditorGUIUtility.standardVerticalSpacing;
		}
	}

	private bool GetConditionalHideAttributeResult(ConditionalHidePickerTypeAttribute condHAtt, SerializedProperty property)
	{
		bool enabled = true;
		string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
		string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
		SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

		if (sourcePropertyValue != null)
		{
			if (condHAtt.TargetPickerType.ToString() == sourcePropertyValue.enumNames[sourcePropertyValue.enumValueIndex])
				enabled = true;
			else
				enabled = false;
		}
		else
		{
			Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
		}

		return enabled;
	}
}

[CustomPropertyDrawer(typeof(ConditionalHideBoolAttribute))]
public class ConditionalHideBoolPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideBoolAttribute condHAtt = (ConditionalHideBoolAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (!condHAtt.HideInInspector || enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideBoolAttribute condHAtt = (ConditionalHideBoolAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private bool GetConditionalHideAttributeResult(ConditionalHideBoolAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null)
        {
            if (condHAtt.TargetBoolResult == sourcePropertyValue.boolValue)
                enabled = true;
            else
                enabled = false;
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        return enabled;
    }
}