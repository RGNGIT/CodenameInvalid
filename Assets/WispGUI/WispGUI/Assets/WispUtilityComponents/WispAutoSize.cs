using UnityEngine;
using UnityEngine.EventSystems;
using WispExtensions;

// [RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class WispAutoSize : UIBehaviour
{
    [SerializeField] private Vector2 minSize = new Vector2(32f,16f);
    [SerializeField] private Vector2 maxSize = new Vector2(256f,32f);

    private RectTransform rt;
    private DrivenRectTransformTracker rtTracker;

    protected override void OnEnable()
    {
        // Debug.Log("OnEnable");

        if (!CheckAndAssignRectTransform())
            return;

        rtTracker.Add(this, rt, DrivenTransformProperties.None);
        
        CheckInputValues();
    }

    protected override void OnDisable()
    {
        // Debug.Log("OnDisable");

        if (!CheckAndAssignRectTransform())
            return;

        rtTracker.Clear();
    }
    
    protected override void OnRectTransformDimensionsChange()
    {
        if (rt == null)
            return;
        
        // Debug.Log("OnDimensionChanged");
        PerformResize();
    }

    public void PerformResize()
    {
        CheckInputValues();

        // Width
        if (rt.rect.width < minSize.x)
        {
            rt.SetRectWidth(minSize.x);
        }
        else if (rt.rect.width > maxSize.x)
        {
            rt.SetRectWidth(maxSize.x);
        }

        // Height
        if (rt.rect.height < minSize.y)
        {
            rt.SetRectHeight(minSize.y);
        }
        else if (rt.rect.height > maxSize.y)
        {
            rt.SetRectHeight(maxSize.y);
        }
    }

    private void CheckInputValues()
    {
        if (minSize.x > maxSize.x)
        {
            float tmp = minSize.x;
            minSize = new Vector2(maxSize.x, minSize.y);
            maxSize = new Vector2(tmp, maxSize.y);
        }

        if (minSize.y > maxSize.y)
        {
            float tmp = minSize.y;
            minSize = new Vector2(minSize.x, maxSize.y);
            maxSize = new Vector2(maxSize.x, tmp);
        }
    }

    private bool CheckAndAssignRectTransform()
    {
        rt = gameObject.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError("WispAutoSize must be attached to a gameObject with a RectTransform component.");
            DestroyImmediate(this);
            return false;
        }

        return true;
    }
}