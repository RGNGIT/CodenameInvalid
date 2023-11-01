using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public enum WispBorderType { Outer, Centred, Inner }

public class WispTextureTools
{
    private static Sprite dotSprite;

    public static Texture2D CreateGradTexture(Gradient grad, int width = 32, int height = 1)
    {
        var gradTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        gradTex.filterMode = FilterMode.Bilinear;
        float inv = 1f / (width - 1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var t = x * inv;
                Color col = grad.Evaluate(t);
                gradTex.SetPixel(x, y, col);
            }
        }
        gradTex.Apply();
        return gradTex;
    }

    public static Gradient GenerateGradientToCenter(Color ParamColor)
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[1];
        colorKeys[0].color = ParamColor;
        colorKeys[0].time = 0.5f;

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
        alphaKeys[0].alpha = 0f;
        alphaKeys[0].time = 0.0f;
        alphaKeys[1].alpha = 0.4f;
        alphaKeys[1].time = 0.5f;
        alphaKeys[2].alpha = 0f;
        alphaKeys[2].time = 1.0f;

        gradient.SetKeys(colorKeys, alphaKeys);

        return gradient;
    }

    public static RawImage GenerateRectTransformBorders(RectTransform ParamRT, int ParamBorderWidth, Color ParamColor, WispBorderType ParamBorderType, bool ParamAttach = true)
    {
        int w = 0;
        int h = 0;

        if (ParamBorderType == WispBorderType.Outer)
        {
            w = Mathf.FloorToInt(ParamRT.rect.width) + ParamBorderWidth * 2;
            h = Mathf.FloorToInt(ParamRT.rect.height) + ParamBorderWidth * 2;
        }
        else if (ParamBorderType == WispBorderType.Centred)
        {
            w = Mathf.FloorToInt(ParamRT.rect.width) + ParamBorderWidth;
            h = Mathf.FloorToInt(ParamRT.rect.height) + ParamBorderWidth;
        }
        else if (ParamBorderType == WispBorderType.Inner)
        {
            w = Mathf.FloorToInt(ParamRT.rect.width);
            h = Mathf.FloorToInt(ParamRT.rect.height);
        }
        else
        {
            // Center by default
            w = Mathf.FloorToInt(ParamRT.rect.width) + ParamBorderWidth;
            h = Mathf.FloorToInt(ParamRT.rect.height) + ParamBorderWidth;
        }

        Texture2D tex = GenerateBorder(w, h, ParamBorderWidth, ParamColor);

        RawImage result = new GameObject("WispBorders").AddComponent<RawImage>();
        result.texture = tex;

        RectTransform rt = result.GetComponent<RectTransform>();

        if (ParamAttach)
        {
            rt.SetParent(ParamRT);
            rt.anchoredPosition = new Vector2(0, 0);
        }

        rt.sizeDelta = new Vector2(w, h);
        rt.AnchorStyleExpanded(true);

        rt.SetTop(-ParamBorderWidth / 2);
        rt.SetBottom(-ParamBorderWidth / 2);
        rt.SetRight(-ParamBorderWidth / 2);
        rt.SetLeft(-ParamBorderWidth / 2);

        result.raycastTarget = false;

        return result;
    }

    // Based on : https://github.com/M-Fatah/texture_maker
    public static Texture2D GenerateBorder(int width, int height, int borderWidth, Color color)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        Color[] colors = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if ((x < borderWidth || x >= width - borderWidth) || (y < borderWidth || y >= height - borderWidth))
                    colors[x + y * width] = color;
                else
                    colors[x + y * width] = Color.clear;
            }
        }

        tex.SetPixels(colors);
        tex.Apply();

        return tex;
    }

    public static Texture2D FastBlur(Texture2D ParamImage, int ParamRadius, int ParamIterations)
    {
        Blur blur = new Blur();
        return blur.FastBlur(ParamImage, ParamRadius, ParamIterations);
    }

    // From : https://stackoverflow.com/questions/44733841/how-to-make-texture2d-readable-via-script
    public static Texture2D DuplicateTexture_BlitMethod(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableTex = new Texture2D(source.width, source.height);
        readableTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableTex.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableTex;
    }

    // From : https://stackoverflow.com/questions/44733841/how-to-make-texture2d-readable-via-script
    public static Texture2D DuplicateTexture_RawMethod(Texture2D ParamSource)
    {
        byte[] pix = ParamSource.GetRawTextureData();
        Texture2D readableText = new Texture2D(ParamSource.width, ParamSource.height, ParamSource.format, false);
        readableText.LoadRawTextureData(pix);
        readableText.Apply();
        return readableText;
    }

    //---------------------------------------------------------------------------------
    // From : https://forum.unity.com/threads/contribution-texture2d-blur-in-c.185694/
    public class Blur
    {
        private float avgR = 0;
        private float avgG = 0;
        private float avgB = 0;
        // private float avgA = 0;
        private float blurPixelCount = 0;

        private Texture2D tex;

        public int radius = 2;
        public int iterations = 2;

        public Texture2D FastBlur(Texture2D image, int radius, int iterations)
        {
            Texture2D tex = image;

            for (var i = 0; i < iterations; i++)
            {

                tex = BlurImage(tex, radius, true);
                tex = BlurImage(tex, radius, false);

            }

            return tex;
        }

        public Texture2D BlurImage(Texture2D image, int blurSize, bool horizontal)
        {

            Texture2D blurred = new Texture2D(image.width, image.height);
            int _W = image.width;
            int _H = image.height;
            int xx, yy, x, y;

            if (horizontal)
            {
                for (yy = 0; yy < _H; yy++)
                {
                    for (xx = 0; xx < _W; xx++)
                    {
                        ResetPixel();

                        //Right side of pixel

                        for (x = xx; (x < xx + blurSize && x < _W); x++)
                        {
                            AddPixel(image.GetPixel(x, yy));
                        }

                        //Left side of pixel

                        for (x = xx; (x > xx - blurSize && x > 0); x--)
                        {
                            AddPixel(image.GetPixel(x, yy));

                        }


                        CalcPixel();

                        for (x = xx; x < xx + blurSize && x < _W; x++)
                        {
                            blurred.SetPixel(x, yy, new Color(avgR, avgG, avgB, 1.0f));

                        }
                    }
                }
            }

            else
            {
                for (xx = 0; xx < _W; xx++)
                {
                    for (yy = 0; yy < _H; yy++)
                    {
                        ResetPixel();

                        //Over pixel

                        for (y = yy; (y < yy + blurSize && y < _H); y++)
                        {
                            AddPixel(image.GetPixel(xx, y));
                        }
                        //Under pixel

                        for (y = yy; (y > yy - blurSize && y > 0); y--)
                        {
                            AddPixel(image.GetPixel(xx, y));
                        }
                        CalcPixel();
                        for (y = yy; y < yy + blurSize && y < _H; y++)
                        {
                            blurred.SetPixel(xx, y, new Color(avgR, avgG, avgB, 1.0f));

                        }
                    }
                }
            }

            blurred.Apply();
            return blurred;
        }
        void AddPixel(Color pixel)
        {
            avgR += pixel.r;
            avgG += pixel.g;
            avgB += pixel.b;
            blurPixelCount++;
        }

        void ResetPixel()
        {
            avgR = 0.0f;
            avgG = 0.0f;
            avgB = 0.0f;
            blurPixelCount = 0;
        }

        void CalcPixel()
        {
            avgR = avgR / blurPixelCount;
            avgG = avgG / blurPixelCount;
            avgB = avgB / blurPixelCount;
        }
    }

    public static Texture2D GenerateDot(Color ParamDotColor)
    {
        Texture2D result = new Texture2D(1, 1);
        result.SetPixel(0, 0, ParamDotColor);
        return result;
    }

    public static Sprite GetClearDotSprite()
    {
        if (dotSprite != null)
            return dotSprite;

        dotSprite = Sprite.Create(GenerateDot(Color.clear), new Rect(0, 0, 1, 1), Vector2.zero);

        return dotSprite;
    }

    public static Texture2D GenerateTexture(int ParamWidth, int ParamHeight, Color ParamColor)
    {
        Texture2D tex = new Texture2D(ParamWidth, ParamHeight, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        Color[] colors = new Color[ParamWidth * ParamHeight];

        for (int y = 0; y < ParamHeight; y++)
        {
            for (int x = 0; x < ParamWidth; x++)
            {
                colors[x + y * ParamWidth] = ParamColor;
            }
        }

        tex.SetPixels(colors);
        tex.Apply();

        return tex;
    }

    public static void DrawLineOnTexture_Bresenham(Texture2D ParamTexture, Color ParamColor, Vector2 ParamPointA, Vector2 ParamPointB)
    {
        // From : https://www.cc.gatech.edu/grads/m/Aaron.E.McClennen/Bresenham/code.html
        int dy, dx, incrE, incrNE, d,x,y;

        dx = (int)ParamPointB.x - (int)ParamPointA.x;
        dy = (int)ParamPointB.y - (int)ParamPointA.y;
        d = 2 * dy - dx;
        incrE = 2*dy;
        incrNE = 2*(dy - dx);
        x = (int)ParamPointA.x;
        y = (int)ParamPointA.y;

        ParamTexture.SetPixel(x,y,ParamColor);

        while(x < ParamPointB.x)
        {
            if (d <= 0)
            {
                d += incrE;
                x++;
            }
            else
            {
                d += incrNE;
                x++;
                y++;
            }
            ParamTexture.SetPixel(x,y,ParamColor);
        } 

        ParamTexture.Apply();
    }

    // From : https://rosettacode.org/wiki/Xiaolin_Wu%27s_line_algorithm
    public static List<Vector2> DrawLineOnTexture(Texture2D ParamTexture, Color ParamColor, Vector2 ParamPointA, Vector2 ParamPointB)
    {
        List<Vector2> result = new List<Vector2>();
        
        float x0, y0, x1, y1;
        Color lineColor;

        x0 = ParamPointA.x;
        y0 = ParamPointA.y;
        x1 = ParamPointB.x;
        y1 = ParamPointB.y;

        lineColor = ParamColor;

        bool steep = Mathf.Abs(y1-y0)>Mathf.Abs(x1-x0);
        float temp;

        if(steep)
        {
            temp=x0; 
            x0=y0; 
            y0=temp;
            temp=x1;
            x1=y1;
            y1=temp;
        }
        if(x0>x1)
        {
            temp = x0;
            x0=x1;
            x1=temp;
            temp = y0;
            y0=y1;
            y1=temp;
        }

        float dx = x1-x0;
        float dy = y1-y0;
        float gradient = dy/dx;

        int xEnd = round(x0);
        float yEnd = y0+gradient*(xEnd-x0);
        float xGap = rfpart(x0 + 0.5f);
        int xPixel1 = xEnd;
        int yPixel1 = ipart(yEnd);

        if(steep)
        {
            ParamTexture.SetPixel(yPixel1,   xPixel1, lineColor.ColorOpacity(rfpart(yEnd)*xGap));
            result.Add(new Vector2(yPixel1, xPixel1));
            ParamTexture.SetPixel(yPixel1+1, xPixel1, lineColor.ColorOpacity(fpart(yEnd)*xGap));
            result.Add(new Vector2(yPixel1+1, xPixel1));
        }
        else
        {
            ParamTexture.SetPixel(xPixel1, yPixel1, lineColor.ColorOpacity(rfpart(yEnd)*xGap));
            result.Add(new Vector2(xPixel1, yPixel1));
            ParamTexture.SetPixel(xPixel1, yPixel1+1, lineColor.ColorOpacity(fpart(yEnd)*xGap));
            result.Add(new Vector2(xPixel1, yPixel1+1));
        }

        float intery = yEnd+gradient;

        xEnd = round(x1);
        yEnd = y1+gradient*(xEnd-x1);
        xGap = fpart(x1+0.5f);
        int xPixel2 = xEnd;
        int yPixel2 = ipart(yEnd);

        if(steep)
        {
            ParamTexture.SetPixel(yPixel2,   xPixel2, lineColor.ColorOpacity(rfpart(yEnd)*xGap));
            result.Add(new Vector2(yPixel2, xPixel2));
            ParamTexture.SetPixel(yPixel2+1, xPixel2, lineColor.ColorOpacity(fpart(yEnd)*xGap));
            result.Add(new Vector2(yPixel2+1, xPixel2));
        }
        else
        {
            ParamTexture.SetPixel(xPixel2, yPixel2, lineColor.ColorOpacity(rfpart(yEnd)*xGap));
            result.Add(new Vector2(xPixel2, yPixel2));
            ParamTexture.SetPixel(xPixel2, yPixel2+1, lineColor.ColorOpacity(fpart(yEnd)*xGap));
            result.Add(new Vector2(xPixel2, yPixel2+1));
        }

        if(steep)
        {
            for(int x=(int)(xPixel1+1);x<=xPixel2-1;x++)
            {
                ParamTexture.SetPixel(ipart(intery), x, lineColor.ColorOpacity(rfpart(intery)));
                result.Add(new Vector2(ipart(intery), x));
                ParamTexture.SetPixel(ipart(intery)+1, x, lineColor.ColorOpacity(fpart(intery)));
                result.Add(new Vector2(ipart(intery)+1, x));
                intery+=gradient;
            }
        }
        else
        {
            for(int x=(int)(xPixel1+1);x<=xPixel2-1;x++)
            {
                ParamTexture.SetPixel(x, ipart(intery), lineColor.ColorOpacity(rfpart(intery)));
                result.Add(new Vector2(x, ipart(intery)));
                ParamTexture.SetPixel(x, ipart(intery)+1, lineColor.ColorOpacity(fpart(intery)));
                result.Add(new Vector2(x, ipart(intery)+1));
                intery+=gradient;
            }
        }

        ParamTexture.Apply();
        
        return result;
    }

    private static int ipart(double ParamValue)
    { 
        return (int)ParamValue;
    }

    private static int round(double ParamValue) 
    {
        return ipart(ParamValue+0.5);
    }

    private static float fpart(float ParamValue) 
    {
        if(ParamValue<0) return (1-(ParamValue-Mathf.Floor(ParamValue)));
        return (ParamValue-Mathf.Floor(ParamValue));
    }

    private static float rfpart(float ParamValue) 
    {
        return 1-fpart(ParamValue);
    }

    public static void SetTexturePixel_9(Texture2D ParamTexture, int ParamX, int ParamY, Color ParamColor, bool ParamApply = true)
    {
        ParamTexture.SetPixel(ParamX, ParamY - 1, ParamColor);
        ParamTexture.SetPixel(ParamX-1, ParamY - 1, ParamColor);
        ParamTexture.SetPixel(ParamX+1, ParamY - 1, ParamColor);

        ParamTexture.SetPixel(ParamX, ParamY, ParamColor);
        ParamTexture.SetPixel(ParamX-1, ParamY, ParamColor);
        ParamTexture.SetPixel(ParamX+1, ParamY, ParamColor);

        ParamTexture.SetPixel(ParamX, ParamY + 1, ParamColor);
        ParamTexture.SetPixel(ParamX-1, ParamY + 1, ParamColor);
        ParamTexture.SetPixel(ParamX+1, ParamY + 1, ParamColor);

        if (ParamApply)
            ParamTexture.Apply();
    }

    // From : https://stackoverflow.com/questions/56949217/how-to-resize-a-texture2d-using-height-and-width
    public static Texture2D ResizeTexture(Texture2D ParamTexture, int ParamWidth, int ParamHeight)
    {
        RenderTexture rt = new RenderTexture(ParamWidth, ParamHeight, 24);
        RenderTexture.active = rt;
        Graphics.Blit(ParamTexture, rt);
        Texture2D result = new Texture2D(ParamWidth, ParamHeight);
        result.ReadPixels(new Rect(0, 0, ParamWidth, ParamHeight), 0, 0);
        result.Apply();
        return result;
    }

    /// <summary>
    /// <para>!! This will overwrite the target png file if it exists already !!</para>
    /// <para>Path should be something like C:\Images\MyTexture.png</para>
    /// </summary>
    public static void Texture2DToPngFile(Texture2D ParamTexture, string ParamPath, bool ParamCreateDirectory = true)
    {
        // Check file name
        if (!ParamPath.EndsWith(".png"))
        {
            UnityEngine.Debug.LogError("There was an error while trying to save Texture2D to file : File name must end with .png");
            return;
        }
        
        // Check file path
        FileInfo info;
        try
        {
            info = new FileInfo(ParamPath);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("There was an error while trying to save Texture2D to file.");
            throw e;
        }

        // Create directory if non-existant
        if (!info.Directory.Exists && ParamCreateDirectory)
        {
            Directory.CreateDirectory(info.Directory.FullName);
        }

        // UnityEngine.Debug.Log(info.FullName);
        byte[] bytes = ParamTexture.EncodeToPNG();
        File.WriteAllBytes(info.FullName, bytes);
    }
}