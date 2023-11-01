using UnityEngine;

public class WispGuiRotate : MonoBehaviour {

	public float RotationSpeed = 1;
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate (Vector3.up * (RotationSpeed * Time.deltaTime));
	}
}
