using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WispExtensions;

public class WispDemoOpacitySlider : MonoBehaviour
{
    public WispVisualComponent[] targets;
    
    private WispSlider slider;
    
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<WispSlider>();
        slider.Base.onValueChanged.AddListener(OnValueChanged);
    }

    // Method to call whenever the slider value has changed
    private void OnValueChanged(float ParamValue)
    {
        // Slider value, between Min and Max.
        // float valueFromEvent = ParamValue;

        // Slider value, between Min and Max, returned as string and conveted to float.
        // float sliderValue = slider.GetValue().ToFloat();

        // Slider value, between 0 and 1, depending on the handle position.
        float value01 = slider.GetValue01();
        
        foreach(WispVisualComponent vc in targets)
        {
            vc.Opacity = 0.5f + (value01 / 2);
        }
    }
}