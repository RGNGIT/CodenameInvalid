using UnityEngine;
using UnityEngine.UI;

public class WispAnimationGlow : MonoBehaviour
{
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool destroyOnEnd = true;
    [SerializeField] private float duration = 0.25f; //In seconds
    [SerializeField] private bool repeat = true;
    [SerializeField] private Color color = Color.blue;

    public bool AutoStart { get => autoStart; set => autoStart = value; }
    public bool DestroyOnEnd { get => destroyOnEnd; set => destroyOnEnd = value; }
    public float Duration { get => duration; set => duration = value; }
    public bool Repeat { get => repeat; set => repeat = value; }
    public Color Color { get => color; set => color = value; }

    private bool isRunning = false;
    private float currentDuration = 0f;

    private float middleTime = 0;
    private Image image;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        middleTime = duration/2;
        image = GetComponent<Image>();
        originalColor = image.color;
        
        if (autoStart)
            StartAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            currentDuration += Time.deltaTime;
            image.color = Color.Lerp(originalColor, color, GetInterpolationAmount());

            if (currentDuration > duration)
            {
                if (repeat)
                    RestartAnimation();
                else
                    EndAnimation();
            }
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
        if (destroyOnEnd)
            Destroy(this);
        else
        {
            isRunning = false;
            currentDuration = 0;
        }
    }

    private float GetInterpolationAmount()
    {
        float result = 0;

        if (currentDuration < middleTime)
        {
            result = currentDuration/middleTime;
        }
        else if (currentDuration > middleTime)
        {
            result = 1 - ((currentDuration-middleTime) / middleTime);
        }
        else if (currentDuration == middleTime)
        {
            result = 0.5f;
        }

        return result;
    }

    public static WispAnimationGlow AttachToGameObject(GameObject ParamTarget, float ParamDuration, Color ParamColor)
    {
        WispAnimationGlow result = ParamTarget.AddComponent<WispAnimationGlow>();
        result.duration = ParamDuration;
        result.color = ParamColor; 

        return result;
    }

    void OnDestroy()
    {
        image.color = originalColor;
    }
}