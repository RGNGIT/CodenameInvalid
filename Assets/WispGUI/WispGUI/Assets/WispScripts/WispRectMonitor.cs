using UnityEngine.EventSystems;

class WispRectMonitor : UIBehaviour
{
    protected override void OnRectTransformDimensionsChange()
    {
        GetComponent<WispVisualComponent>().UpdatePositions();
    }
}