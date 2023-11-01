// Thanks to : https://softwarebydefault.com/2013/06/08/calculating-gaussian-kernels/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GaussianBlur : MonoBehaviour
{
    public WispImage Source;
    public WispImage Result;

    public float StandardDeviation = 20f;
    public int Range = 7;

    float[] kernel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformGaussianBlur();
        }
    }

    public void PerformGaussianBlur()
    {
        float t = Time.realtimeSinceStartup;

        kernel = Calculate_OneDim(Range, StandardDeviation);

        // ===================================================
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < kernel.Length; i++)
        {
            sb.Append(kernel[i] + " // ");
        }
        print("Gaussian weights : " + sb.ToString());
        // ===================================================
        
        Texture2D source = Source.GetTexture2D();
        
        // Color[] pixels = source.GetPixels();
        // print(pixels.Length);

        Texture2D sourceReadable = WispTextureTools.DuplicateTexture_RawMethod(source);

        Texture2D resultVertical = WispTextureTools.GenerateTexture(sourceReadable.width, sourceReadable.height, Color.gray);

        // Vertical        
        for (int i = 0; i < sourceReadable.width; i++)
        {
            for (int j = 0; j < sourceReadable.height; j++)
            {
                Color vGauss = GaussianSampleVertical(sourceReadable, i, j, StandardDeviation, Range);
                resultVertical.SetPixel(i, j, vGauss);
            }
        }

        resultVertical.Apply();

        Texture2D resultHorizontal = WispTextureTools.GenerateTexture(sourceReadable.width, sourceReadable.height, Color.gray);

        // Horizontal
        for (int i = 0; i < sourceReadable.width; i++)
        {
            for (int j = 0; j < sourceReadable.height; j++)
            {
                Color hGauss = GaussianSampleHorizontal(resultVertical, i, j, StandardDeviation, Range);
                resultHorizontal.SetPixel(i, j, hGauss);
            }
        }

        resultHorizontal.Apply();

        Result.SetValue(resultHorizontal);

        print("Time : " + (Time.realtimeSinceStartup - t));
    }

    public Color GaussianSampleVertical(Texture2D ParamTexture, int ParamX, int ParamY, float ParamStandardDeviation, int ParamRange)
    {
        if (ParamRange <= 0)
            return ParamTexture.GetPixel(ParamX, ParamY);

        Color result = ParamTexture.GetPixel(ParamX, ParamY) * kernel[0];

        for (int i = 1; i < ParamRange; i++)
        {
            result += ParamTexture.GetPixel(ParamX, ParamY + i) * kernel[i];
            result += ParamTexture.GetPixel(ParamX, ParamY - i) * kernel[i];
        }

        Color finalResult = result / ((ParamRange*2)+1);

        return result;
    }

    public Color GaussianSampleHorizontal(Texture2D ParamTexture, int ParamX, int ParamY, float ParamStandardDeviation, int ParamRange)
    {
        if (ParamRange <= 0)
            return ParamTexture.GetPixel(ParamX, ParamY);

        Color result = ParamTexture.GetPixel(ParamX, ParamY) * kernel[0];

        for (int i = 1; i < ParamRange; i++)
        {
            result += ParamTexture.GetPixel(ParamX + i, ParamY) * kernel[i];
            result += ParamTexture.GetPixel(ParamX - i, ParamY) * kernel[i];
        }

        Color finalResult = result / ((ParamRange*2)+1);

        return result;
    }

    public static float[] Calculate_OneDim(int ParamRange, float ParamStandardDeviation)
    {
        float[] Kernel = new float [ParamRange];
        float sumTotal = 0;
    
        float distance = 0;
    
        float calculatedEuler = 1.0f / (2.0f * Mathf.PI * Mathf.Pow(ParamStandardDeviation, 2));
    
        for (int i = 0; i < ParamRange; i++)
        {
            distance = ((i * i) + (0 * 0)) / (2 * (ParamStandardDeviation * ParamStandardDeviation));

            Kernel[i] = calculatedEuler * Mathf.Exp(-distance);

            sumTotal += Kernel[i];
        }

        for (int i = 1; i < ParamRange; i++)
        {
            sumTotal += Kernel[i];
        }
    
        for (int i = 0; i < ParamRange; i++)
        { 
            Kernel[i] = Kernel[i] * (1.0f / sumTotal);
        }

        return Kernel;
    }
}