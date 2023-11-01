using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WispMouseClickHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float doubleClickDelay = 0.2f;

    private float lastClickTime = 0;
    private Coroutine checkForSingleClickCoroutine;

    private UnityEvent onClick = new UnityEvent();
    private UnityEvent onSingleClick = new UnityEvent();
    private UnityEvent onDoubleClick = new UnityEvent();

    public UnityEvent OnClick { get => onClick; }
    public UnityEvent OnSingleClick { get => onSingleClick; }
    public UnityEvent OnDoubleClick { get => onDoubleClick; }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Check if it's a double click
            if (Time.time - lastClickTime < doubleClickDelay)
            {
                StopCoroutine(checkForSingleClickCoroutine);
                lastClickTime = 0;
                onDoubleClick.Invoke();
            }
            else
            {
                onClick.Invoke();
                lastClickTime = Time.time;
                checkForSingleClickCoroutine = StartCoroutine(checkForSingleClick());
            }
        }
    }

    private IEnumerator checkForSingleClick()
    {
        yield return new WaitForSeconds(doubleClickDelay);
        onSingleClick.Invoke();
    }
}