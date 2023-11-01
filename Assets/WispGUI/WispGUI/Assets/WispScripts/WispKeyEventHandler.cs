/*
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WispKeyEventHandler : WispMonoBehaviour
{
    private WispVisualComponent visualComponent;
    private Dictionary<KeyCode, UnityAction> keysAndActions;
    private long inputDrainFrame = 0; // Input drain is when a component handles the input from a key and prevents all other components from handling the input, the drain last only one frame.
    
    void Awake()
    {
        Initialize();
    }

    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        visualComponent = GetComponent<WispVisualComponent>();

        if (visualComponent == null)
        {
            WispVisualComponent.LogError("Null visual component for input handler, input handler will be destroyed.");
            Destroy(this);
        }

        keysAndActions = new Dictionary<KeyCode, UnityAction>();

        isInitialized = true;

        return true;
    }

    void Update()
    {
        if (!visualComponent.IsAvailableForInput || keysAndActions == null)
            return;
        
        foreach (KeyValuePair<KeyCode, UnityAction> kv in keysAndActions)
        {
            if (Input.GetKeyDown(kv.Key))
                kv.Value.Invoke();
        }
    }

    public void AddEventOnKey(KeyCode ParamKeyCode, UnityAction ParamAction)
    {
        keysAndActions.Add(ParamKeyCode, ParamAction);
    }
}
*/