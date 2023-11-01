using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispUiFilterDemo : MonoBehaviour
{
    public TMPro.TextMeshProUGUI[] texts;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in texts)
        {
            item.fontSize = 9;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
