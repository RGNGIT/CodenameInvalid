using UnityEngine;
using WispExtensions;

public class WispMonoBehaviour : MonoBehaviour 
{
    protected bool isInitialized = false;

    public bool IsInitialized 
    {
        get 
        {
            return isInitialized;
        }
    }

    public T Self<T>()
    {
        return this.ConvertObject<T>();
    }

    // ...
    public virtual bool Initialize ()
    {
        return false;
    }

    public virtual bool InitializeInEditor()
    {
        return true;
    }

    // ...
    void Awake () 
    {

        Initialize ();
        
    }
}