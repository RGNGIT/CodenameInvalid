using UnityEngine;

public class WispImageDemo : MonoBehaviour
{
    void Start()
    {
        // Thanks to : https://picsum.photos/
        GetComponent<WispImage>().SetValue("https://picsum.photos/200");
    }
}