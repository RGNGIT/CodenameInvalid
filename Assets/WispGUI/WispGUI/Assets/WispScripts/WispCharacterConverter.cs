using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WispCharacterConverter : MonoBehaviour {

	protected bool isInitialized = false;
	protected InputField inputField = null;

	// Use this for initialization
	void Start () {

		Initialize ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// ...
	public void Initialize ()
	{
		if (isInitialized)
			return;

		inputField = GetComponent<InputField> ();

		if (inputField != null) {
		
			inputField.onValueChanged.AddListener (delegate {OnValueChanged();});
		
		}
	}

	// ...
	protected void OnValueChanged ()
	{
//		print ("Value changed !");
		ConvertSpecialToDigits();
	}

	// ...
	protected void ConvertSpecialToDigits ()
	{
		string inputFieldText = inputField.text;

		if (inputFieldText == "" || inputFieldText == null)
			return;

		string result = null;

		// TODO : Add to string handling library
		foreach (char c in inputFieldText) {
		
			if (c == '&')
				result += '1';
			else if (c == 'é')
				result += '2';
			else if (c == '"')
				result += '3';
			else if (c == '\'')
				result += '4';
			else if (c == '(')
				result += '5';
			else if (c == '-')
				result += '6';
			else if (c == 'è')
				result += '7';
			else if (c == '_')
				result += '8';
			else if (c == 'ç')
				result += '9';
			else if (c == 'à')
				result += '0';
			else
				result += c;
		
		}

		inputField.text = result;


	}
}
