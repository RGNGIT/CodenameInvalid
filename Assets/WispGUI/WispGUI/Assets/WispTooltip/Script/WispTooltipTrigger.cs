using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class WispTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private float delay = 0.25f;

    private static Coroutine delayCoroutine;

    public float Delay { get => delay; set => delay = value; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);

        delayCoroutine = StartCoroutine(DelayCoroutine());
    }

    private IEnumerator DelayCoroutine()
    {
        yield return new WaitForSeconds(delay);
        GetComponent<WispVisualComponent>().ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);

        GetComponent<WispVisualComponent>().HideTooltip();
    }
}