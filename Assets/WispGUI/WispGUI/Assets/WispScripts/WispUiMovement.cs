using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispUiMovement : MonoBehaviour {

	public float Speed = 1.0f;
	public float Distance = 1.0f;

	protected RectTransform rect;

	// Use this for initialization
	void Start () {

		rect = GetComponent<RectTransform> ();
		
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 pos = rect.localPosition;

		float x = pos.x;
		float y = pos.y + (Mathf.Sin (Time.time * Speed) * Distance * Time.deltaTime);
		float z = pos.z;

		Vector3 newPos = new Vector3 (x, y, z);

		rect.localPosition = newPos;
		
	}
}
