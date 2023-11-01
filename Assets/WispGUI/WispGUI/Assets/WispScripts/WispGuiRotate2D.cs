using UnityEngine;

public class WispGuiRotate2D : MonoBehaviour
{

    public float RotationSpeed = 1;

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.forward * (RotationSpeed * Time.deltaTime));

    }
}
