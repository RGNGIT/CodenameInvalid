using UnityEngine;

public class WispNodeEditorDemo : MonoBehaviour
{
    [SerializeField] private TextAsset demoFile;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<WispNodeEditor>().LoadFromJson(demoFile.text);
    }
}
