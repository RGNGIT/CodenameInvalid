using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispEntity {

	protected string name;
	protected string displayName;
	protected string iconName;
	protected bool displayShortcut;
	protected bool predefinedList;
	protected WispEntityProperty quickSearchProperty;
    protected Dictionary<string, WispEntityProperty> properties = new Dictionary<string, WispEntityProperty>();

	public string Name {
		get {
			return name;
		}
		set {
			name = value;
		}
	}

	public string DisplayName {
		get {
			return displayName;
		}
		set {
			displayName = value;
		}
	}

	public string IconName {
		get {
			return iconName;
		}
		set {
			iconName = value;
		}
	}

	public bool DisplayShortcut {
		get {
			return displayShortcut;
		}
		set {
			displayShortcut = value;
		}
	}

	public bool PredefinedList {
		get {
			return predefinedList;
		}
		set {
			predefinedList = value;
		}
	}

	public WispEntityProperty QuickSearchProperty {
		get {
			return quickSearchProperty;
		}
		set {
			quickSearchProperty = value;
		}
	}

	public int GetPropertyCount ()
	{
		return properties.Count;
	}

	public WispEntity (string ParamName, string ParamDisplayName)
	{
		name = ParamName;
		displayName = ParamDisplayName;
		displayShortcut = true;
	}

	// ...
	public WispEntityProperty AddProperty (WispEntityProperty ParamProperty)
	{
		properties.Add (ParamProperty.Name, ParamProperty);

		ParamProperty.Parent = this as WispEntityInstance;

		return ParamProperty;
	}

	// Get an EntityProperty by name
	public WispEntityProperty GetEntityPropertyByName (string ParamPropertyName)
	{
		if (properties.Count == 0 || !properties.ContainsKey(ParamPropertyName))
			return null;

		return properties[ParamPropertyName];
	}

	// ...
	public WispEntityProperty[] GetArrayOfProperties()
	{
		WispEntityProperty[] result = new WispEntityProperty[properties.Count];
        
        int c = 0;
        foreach (KeyValuePair<string, WispEntityProperty> kv in properties)
        {
            result[c] = kv.Value;
            c++;
        }

		return result;
	}

    public List<string> GetPropertyKeyList()
    {
        List<string> result = new List<string>();

        foreach (KeyValuePair<string, WispEntityProperty> kv in properties)
        {
            result.Add(kv.Key);
        }

        return result;
    }
}