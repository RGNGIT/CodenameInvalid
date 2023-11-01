using UnityEngine;
using UnityEngine.Events;

public class WispAnimationFade : MonoBehaviour
{
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool destroyOnEnd = true;
    [SerializeField] private float duration = 0.25f; //In seconds
    [SerializeField] [Range(0f, 1f)] private float alphaStartingValue = 0f;
    [SerializeField] [Range(0f, 1f)] private float alphaFinalValue = 1f;

    public bool AutoStart { get => autoStart; set => autoStart = value; }
    public bool DestroyOnEnd { get => destroyOnEnd; set => destroyOnEnd = value; }
    public float Duration { get => duration; set => duration = value; }
    public float AlphaStartingValue { get => alphaStartingValue; set => alphaStartingValue = value; }
    public float AlphaFinalValue { get => alphaFinalValue; set => alphaFinalValue = value; }
    public UnityEvent OnEnd { get => onEnd; }

    private CanvasGroup canvasGroup;
    private bool isRunning = false;
    private float currentDuration = 0f;
    private UnityEvent onEnd = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // canvasGroup.alpha = 0;
        canvasGroup.alpha = alphaStartingValue;

        if (autoStart)
            StartAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            currentDuration += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(alphaStartingValue, alphaFinalValue, currentDuration/duration);

            if (currentDuration > duration)
                EndAnimation();
        }
    }

    public void StartAnimation()
    {
        isRunning = true;
    }

    public void RestartAnimation()
    {
        isRunning = true;
        currentDuration = 0f;
    }

    private void EndAnimation()
    {
        if (onEnd != null)
            onEnd.Invoke();
        
        if (destroyOnEnd)
            Destroy(this);
        else
        {
            isRunning = false;
            currentDuration = 0;
        }
    }
}