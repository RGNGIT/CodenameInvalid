using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
	AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHidePickerTypeAttribute : PropertyAttribute
{
	//The name of the bool field that will be in control
	public string ConditionalSourceField = "";
	//TRUE = Hide in inspector / FALSE = Disable in inspector
	public int TargetIndex = -1;
	public bool HideInInspector = false;
	// ...
	public WispEditBox.EditBoxPickerType TargetPickerType = WispEditBox.EditBoxPickerType.None;

	public ConditionalHidePickerTypeAttribute(string conditionalSourceField)
	{
		this.ConditionalSourceField = conditionalSourceField;
		this.HideInInspector = false;
	}

	public ConditionalHidePickerTypeAttribute(string conditionalSourceField, bool hideInInspector, WispEditBox.EditBoxPickerType ParamTargetPickerType)
	{
		this.ConditionalSourceField = conditionalSourceField;
		this.HideInInspector = hideInInspector;
		this.TargetPickerType = ParamTargetPickerType;
	}
}

// ---------------------------------------------------------------------------------------------------------------------------

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideBoolAttribute : PropertyAttribute
{
    //The name of the bool field that will be in control
    public string ConditionalSourceField = "";

    //TRUE = Hide in inspector / FALSE = Disable in inspector
    public bool HideInInspector = false;

    public int TargetIndex = -1;
    // ...
    //public WispEditBox.EditBoxPickerType TargetBool = WispEditBox.EditBoxPickerType.None;
    public bool TargetBoolResult = true;


    public ConditionalHideBoolAttribute(string conditionalSourceField)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = false;
    }

    public ConditionalHideBoolAttribute(string conditionalSourceField, bool hideInInspector, bool ParamTargetBoolResult)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
        this.TargetBoolResult = ParamTargetBoolResult;
    }
}
