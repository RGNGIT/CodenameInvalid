using UnityEngine;
using UnityEngine.Events;

public class WispAnimationFloat : MonoBehaviour
{
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool destroyOnEnd = true;
    [SerializeField] private float duration = 0.25f; //In seconds
    [SerializeField] [Range(0f, 1f)] private float floatStartingValue = 0f;
    [SerializeField] [Range(0f, 1f)] private float floatFinalValue = 1f;

    public bool AutoStart { get => autoStart; set => autoStart = value; }
    public bool DestroyOnEnd { get => destroyOnEnd; set => destroyOnEnd = value; }
    public float Duration { get => duration; set => duration = value; }
    public float FloatStartingValue { get => floatStartingValue; set => floatStartingValue = value; }
    public float FloatFinalValue { get => floatFinalValue; set => floatFinalValue = value; }
    public UnityEvent OnEnd { get => onEnd; }
    public Material TargetMaterial { get => targetMaterial; set => targetMaterial = value; }
    public string FloatPropertyName { get => floatPropertyName; set => floatPropertyName = value; }

    private Material targetMaterial = null;
    private string floatPropertyName = "";
    private bool isRunning = false;
    private float currentDuration = 0f;
    private UnityEvent onEnd = new UnityEvent();

    void Start()
    {
        targetMaterial.SetFloat(floatPropertyName, floatStartingValue);

        if (autoStart)
            StartAnimation();
    }

    void Update()
    {
        if (isRunning)
        {
            currentDuration += Time.deltaTime;
            targetMaterial.SetFloat(floatPropertyName, Mathf.Lerp(floatStartingValue, floatFinalValue, currentDuration/duration));

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