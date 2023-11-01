using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using WispExtensions;

public class WispControlPanelDemoTempSlider : MonoBehaviour
{
    public TextMeshProUGUI temperatureText;
    public float minTemperature = 12f;
    public float maxTemperature = 36f;
    
    private WispCircularSlider slider;
    
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<WispCircularSlider>();
        slider.OnValueChanged.AddListener(UpdateText);
    }

    void UpdateText()
    {
        float t = slider.GetValue01();
        temperatureText.text = Mathf.Lerp(minTemperature, maxTemperature, t).ToString("N0") + "° C";
    }
}
