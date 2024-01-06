using System.Collections;
using System.Collections.Generic;
using TinyJson;
using UnityEngine;

public class MicrophoneCapture : MonoBehaviour
{    
    private bool micOperational = false;  
    private int minFreq;
    private int maxFreq;
  
    private AudioSource audioSource;

    public Transform[] visualizer;

    private bool CheckMicrophones()
    {
        if (Microphone.devices.Length <= 0)
            return false;

        return true;
    }

    void Awake()
    {
        if(CheckMicrophones())
        {
            micOperational = true;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            if (minFreq == 0 && maxFreq == 0)
                maxFreq = 44100;

            audioSource = GetComponent<AudioSource>();
        } 
        else
        {
            Debug.LogError("Нету рабочих микрофонов :(");
        }
    }

    void VisualRefresher()
    {
        Vector3 vel = new Vector3();

        for(int i = 0; i < visualizer.Length; i++) 
        {
            visualizer[i].localScale = Vector3.SmoothDamp(
                visualizer[i].localScale,
                new Vector3(visualizer[i].localScale.x, AudioSpectrum.audioSpectrum[i * 8] * 50),
                ref vel,
                .03f);
        }
    }

    void Update()
    {
        VisualRefresher();
        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            audioSource.clip = Microphone.Start(null, true, 20, maxFreq);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Microphone.End(null);
            audioSource.Play();
        }
    }
}
