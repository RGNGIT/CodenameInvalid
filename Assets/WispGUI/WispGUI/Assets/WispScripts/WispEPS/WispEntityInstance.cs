using System;
using System.Collections.Generic;
using TinyJson;
using UnityEngine;

public class WispEntityInstance : WispEntity {

	protected string id;
	protected string instanceID;
	protected string versionID;
	protected bool isLast;
	protected string timeStamp;
	protected string uid;
	protected bool isDeleted;
	protected string primaryProperty;
	protected string secondaryProperty;
	protected string thirdiaryProperty;
    protected string summaryString;
    protected string defaultFocusPropertyName = "";

    public string ID {
		get {
			return id;
		}
		set {
			id = value;
		}
	}

	public string InstanceID {
		get {
			return instanceID;
		}
	}

	public string VersionID {
		get {
			return versionID;
		}
	}

	public string PrimaryProperty {
		get {
			return primaryProperty;
		}
	}

	public string SecondaryProperty {
		get {
			return secondaryProperty;
		}
	}

	public string ThirdiaryProperty {
		get {
			return thirdiaryProperty;
		}
	}

    public string SummaryString { get => summaryString; set => summaryString = value; }
    public string DefaultFocusPropertyName { get => defaultFocusPropertyName; /*set => defaultFocusPropertyName = value;*/ }

    // Constructor
    public WispEntityInstance(string ParamName, string ParamDisplayName) : base(ParamName, ParamDisplayName) {}
    
    // Constructor 2
    //public WispEntityInstance FromEntity(WispEntity ParamEntity)
    //{
    //    WispEntityInstance result = new WispEntityInstance(ParamEntity.Name, ParamEntity.DisplayName);
    //}

    // ...
    public void LoadFromJson(string ParamJson)
    {
        List<string> all = ParamJson.FromJson<List<string>>();

        string[] allArray = all.ToArray();

        string typeElement = "{" + allArray[0] + "}";

        Dictionary<string, string> tmpDictionary = typeElement.FromJson<Dictionary<string, string>>();

        if (tmpDictionary["Type"] == "Object" || tmpDictionary["Type"] == "EntityInstance")
        {

            string basicElement = "{" + allArray[1] + "}";
            string propertyElement = "{" + allArray[2] + "}";

            Dictionary<string, string> basicInfoDictionary = basicElement.FromJson<Dictionary<string, string>>();
            Dictionary<string, string> propertyInfoDictionary = propertyElement.FromJson<Dictionary<string, string>>();

            name = basicInfoDictionary["EntityName"];
            displayName = basicInfoDictionary["EntityLabel"];
            id = basicInfoDictionary["ID"];
            instanceID = basicInfoDictionary["entityID"];
            versionID = basicInfoDictionary["versionID"];
            isLast = StringToBool(basicInfoDictionary["isLast"]);
            timeStamp = basicInfoDictionary["timeStamp"];
            uid = basicInfoDictionary["uid"];
            isDeleted = StringToBool(basicInfoDictionary["isDeleted"]);
            primaryProperty = basicInfoDictionary["1"];
            secondaryProperty = basicInfoDictionary["2"];
            thirdiaryProperty = basicInfoDictionary["3"];
            summaryString = basicInfoDictionary["SummaryString"];
            defaultFocusPropertyName = basicInfoDictionary["DefaultFocus"];

            foreach (var item in propertyInfoDictionary)
            {
                string singlePropertyElement = "{" + item.Value + "}";
                Dictionary<string, string> singlePropertyInfoDictionary = singlePropertyElement.FromJson<Dictionary<string, string>>();

                if (singlePropertyInfoDictionary == null)
                {
                    WispVisualComponent.LogError("NULL property information.");
                    break;
                }
    
                // Find the correct property type to add
                if (singlePropertyInfoDictionary["type"] == "text" || singlePropertyInfoDictionary["type"] == "int")
                {
                    WispEntityPropertyText tmpEP = AddProperty(new WispEntityPropertyText(singlePropertyInfoDictionary["name"], singlePropertyInfoDictionary["label"], 1)) as WispEntityPropertyText;
                    tmpEP.SetValue(singlePropertyInfoDictionary["value"]);

                    if (singlePropertyInfoDictionary["readonly"] == "true")
                    {
                        tmpEP.Editable = true;
                    }
                    else
                    {
                        tmpEP.Editable = false;
                    }

                    if (singlePropertyInfoDictionary["unique"] == "true")
                    {
                        tmpEP.UniqueValue = true;
                    }
                    else
                    {
                        tmpEP.UniqueValue = false;
                    }
                }
                else if (singlePropertyInfoDictionary["type"] == "img")
                {

                    WispEntityPropertyImage tmpEP = AddProperty(new WispEntityPropertyImage(singlePropertyInfoDictionary["name"], singlePropertyInfoDictionary["label"])) as WispEntityPropertyImage;
                    tmpEP.SetValue(singlePropertyInfoDictionary["value"]);

                }
                else if (singlePropertyInfoDictionary["type"] == "date")
                {
                    WispEntityPropertyDate tmpEP = AddProperty(new WispEntityPropertyDate(singlePropertyInfoDictionary["name"], singlePropertyInfoDictionary["label"])) as WispEntityPropertyDate;
                    tmpEP.SetValue(singlePropertyInfoDictionary["value"]);
                }
                else if (singlePropertyInfoDictionary["type"] == "sub")
                {
                    WispEntityPropertySubInstance tmpEP = AddProperty(new WispEntityPropertySubInstance(singlePropertyInfoDictionary["name"], singlePropertyInfoDictionary["label"])) as WispEntityPropertySubInstance;
                    tmpEP.Value = singlePropertyInfoDictionary["value"];
                    tmpEP.SummaryString = singlePropertyInfoDictionary["summaryString"];
                }
                else if (singlePropertyInfoDictionary["type"] == "multi_sub")
                {
                    Dictionary<string, string> opParamsDictionary = ( "{" + singlePropertyInfoDictionary["operationParameters"] + "}").FromJson<Dictionary<string, string>>();

                    WispEntityPropertyMultiSubInstance tmpEP = AddProperty(new WispEntityPropertyMultiSubInstance(singlePropertyInfoDictionary["name"], singlePropertyInfoDictionary["label"], opParamsDictionary)) as WispEntityPropertyMultiSubInstance;
                    tmpEP.Value = singlePropertyInfoDictionary["value"];
                    tmpEP.SummaryString = singlePropertyInfoDictionary["summaryString"];
                    tmpEP.SubEntityName = singlePropertyInfoDictionary["subEntityName"];
                    tmpEP.SubEntityLabel = singlePropertyInfoDictionary["subEntityDisplayName"];
                }
            }
        }
    }

	// ...
	bool StringToBool (string ParamString)
	{
		if (ParamString == "1" || ParamString == "true")
			return true;
		else
			return false;
	}

    //...
    public string GetJson()
    {
        string result = "";

        foreach(KeyValuePair<string, WispEntityProperty> kv in properties)
        {
            result += kv.Value.GetJson() + Environment.NewLine;
        }

        return result;
    }
}

