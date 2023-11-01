using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WispTabNavigationHandler : MonoBehaviour
{
    [SerializeField] private Selectable previousSelectable;
    [SerializeField] private Selectable nextSelectable;

    private Selectable me;
    
    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<Selectable>();
        
        if (me == null)
        {
            Debug.LogWarning("WispTabNavigationHandler should be attached to a Selectable Gameobject.");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != me.gameObject)
            {
                return;
            }
            
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (previousSelectable)
                    previousSelectable.Select();
            }
            else
            {
                if (nextSelectable)
                    nextSelectable.Select();
            }
        }
    }
}