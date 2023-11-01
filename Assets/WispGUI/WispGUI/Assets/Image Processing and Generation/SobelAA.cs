using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class SobelAA : MonoBehaviour
{
    public WispImage Sobel;
    public WispImage Result;
    
    WispImage me;
    
    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<WispImage>();
    }

    void Filter()
    {
        Texture2D tex = me.GetTexture2D();
        Texture2D aa = GenerateSobelAntiAliasedTexture(tex);

        Result.SetValue(aa);
    }

    private Texture2D GenerateSobelFilter(Texture2D ParamTexture)
    {
        Texture2D result = WispTextureTools.GenerateTexture(ParamTexture.width, ParamTexture.height, Color.black);
        
        for (int X = 0; X < ParamTexture.width; X++)
        {
            for (int Y = 0; Y < ParamTexture.height; Y++)
            {
                // North
                Color nw = ParamTexture.GetPixel(X-1, Y+1) * -1;
                Color nc = ParamTexture.GetPixel(X, Y+1) * -1;
                Color ne = ParamTexture.GetPixel(X+1, Y+1) * -1;

                // Middle
                Color mw = ParamTexture.GetPixel(X-1, Y) * -1;
                Color mc = ParamTexture.GetPixel(X, Y) * 8;
                Color me = ParamTexture.GetPixel(X+1, Y) * -1;

                // South
                Color sw = ParamTexture.GetPixel(X-1, Y-1) * -1;
                Color sc = ParamTexture.GetPixel(X, Y-1) * -1;
                Color se = ParamTexture.GetPixel(X+1, Y-1) * -1;

                Color sum = nw + nc + ne + mw + mc + me + sw + sc + se;

                Color raw = ParamTexture.GetPixel(X,Y);
                result.SetPixel(X, Y, Color.Lerp(sum.Invert(),raw, 0.1f));
            }
        }

        result.Apply();
        return result;
    }

    private Texture2D GenerateSobelAntiAliasedTexture(Texture2D ParamTexture)
    {
        Texture2D sourceTex;
        
        try
        {
            ParamTexture.GetPixel(0,0);
            sourceTex = ParamTexture;
        }
        catch (UnityException)
        {
            sourceTex = WispTextureTools.DuplicateTexture_BlitMethod(ParamTexture);
        }

        Texture2D sobel = GenerateSobelFilter(sourceTex);
        this.Sobel.SetValue(sobel);

        Texture2D result = WispTextureTools.GenerateTexture(ParamTexture.width, ParamTexture.height, Color.clear);

        for (int X = 0; X < ParamTexture.width; X++)
        {
            for (int Y = 0; Y < ParamTexture.height; Y++)
            {
                // North
                Color nw = sourceTex.GetPixel(X-1, Y+1) * 1;
                Color nc = sourceTex.GetPixel(X, Y+1) * 1;
                Color ne = sourceTex.GetPixel(X+1, Y+1) * 1;

                // Middle
                Color mw = sourceTex.GetPixel(X-1, Y) * 1;
                Color mc = sourceTex.GetPixel(X, Y) * 1;
                Color me = sourceTex.GetPixel(X+1, Y) * 1;

                // South
                Color sw = sourceTex.GetPixel(X-1, Y-1) * 1;
                Color sc = sourceTex.GetPixel(X, Y-1) * 1;
                Color se = sourceTex.GetPixel(X+1, Y-1) * 1;

                Color average = (nw + nc + ne + mw + mc + me + sw + sc + se)/9;

                Color raw = sourceTex.GetPixel(X,Y);
                result.SetPixel(X, Y, Color.Lerp(raw,average, sobel.GetPixel(X,Y).grayscale));
            }
        }
        
        result.Apply();
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Filter();
        }
    }
}