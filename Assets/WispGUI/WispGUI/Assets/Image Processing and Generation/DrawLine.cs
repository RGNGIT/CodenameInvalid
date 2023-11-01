using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WispExtensions;

public class DrawLine : MonoBehaviour
{
    // public bool LineDrawerEnabled = false;
    // public Vector2 pointA;
    // public Vector2 pointB;
    public bool ShapeDrawerV2Enabled = false;
    [Space(8)]
    public bool ShapeDrawerEnabled = false;
    [Range(2, 64)] public int PointCount = 8;
    [Range(0, 360)] public float Rotation = 0;
    // public Vector2 Center = new Vector2(64f,64f);
    [Range(1, 256)] public float ShapeSize = 32f;
    public Color OutlineColor = Color.red;
    [Space(8)]
    public bool ShapeFillEnabled = false;
    public Color FillColor = Color.white;
    // public int maxFloodFillCounter = 1000;
    [Space(8)]
    public bool DrawHorizonEnabled = false;
    public Color HorizonColor = Color.blue;
    public Color HorizonFillColor = Color.cyan;

    [Space(8)]
    public bool DrawHalfLineEnabled = false;
    public Color HalfLineFillColor = Color.green;
    [Range(0, 360)] public float HalfLineRayAngle = 0;
    
    private WispImage image;
    private Texture2D texture;
    private Vector2[] points;
    private Vector2[] horizonPoints;
    private List<Vector2> outlinePoints = new List<Vector2>();
    private byte[,] shapeMask;
    // private int floodFillCounter = 0;

    private static List<Vector2> inside = new List<Vector2>();
    
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<WispImage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ShapeDrawerV2Enabled)
            RenderShape();
    }

    private void RenderShape()
    {
        image.Base.color = Color.white;
        texture = WispTextureTools.GenerateTexture((int)image.Width, (int)image.Height, Color.clear);
        outlinePoints.Clear();

        points = new Vector2[PointCount];
        Vector2 center = new Vector2(image.Width/2, image.Height/2);
        float sliceSize = 360 / PointCount;

        for (int i = 0; i < PointCount; i++)
        {
            float angle = ((i * sliceSize) + Rotation) * 0.0174533f;

            float x = center.x + (ShapeSize * Mathf.Cos(angle));
            float y = center.y + (ShapeSize * Mathf.Sin(angle));

            points[i] = new Vector2(x,y);
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            outlinePoints.AddRange(WispTextureTools.DrawLineOnTexture(texture, OutlineColor, points[i], points[i+1]));
        }

        if (PointCount > 2)
        {
            outlinePoints.AddRange(WispTextureTools.DrawLineOnTexture(texture, OutlineColor, points[PointCount-1], points[0]));
        }

        shapeMask = new byte[texture.width, texture.height];
        // 0 : Nothing
        // 1 : Outline
        // 2 : Fill

        foreach(Vector2 p in outlinePoints)
        {
            shapeMask[(int)p.x, (int)p.y] = 1;
        }

        // BFS fill the mask with 2 from the center
        Stack<Vector2> queue = new Stack<Vector2>();
        int px;
        int py;

        queue.Push(center);

        while(queue.Count != 0)
        {
            Vector2 poped = queue.Pop();
            px = (int)poped.x;
            py = (int)poped.y;

            if (shapeMask[px, py] == 0)
            {
                shapeMask[px, py] = 2;

                if (shapeMask[px+1, py] == 0)
                    queue.Push(new Vector2(px+1, py));
                
                if (shapeMask[px-1, py] == 0)
                    queue.Push(new Vector2(px-1, py));

                if (shapeMask[px, py+1] == 0)
                    queue.Push(new Vector2(px, py+1));

                if (shapeMask[px, py-1] == 0)
                    queue.Push(new Vector2(px, py-1));
            }
        }

        for (int i = 0; i < shapeMask.GetLength(0); i++)
        {
            for (int j = 0; j < shapeMask.GetLength(1); j++)
            {
                if (shapeMask[i,j] == 1)
                {
                    texture.SetPixel(i,j,OutlineColor);
                }
                else if (shapeMask[i,j] == 2)
                {
                    texture.SetPixel(i,j,FillColor);
                }
                else
                {
                    texture.SetPixel(i,j,Color.clear);
                }
            }
        }

        texture.Apply();
        image.SetValue(texture);
    }

    private void RenderShape_old()
    {
        // if (!Input.GetKeyDown(KeyCode.Mouse4))
        //     return;
        
        image.Base.color = Color.white;

        // if (LineDrawerEnabled)
        // {
        //     texture = WispTextureTools.GenerateTexture((int)image.Width, (int)image.Height, Color.white);
        //     WispTextureTools.DrawLineOnTexture(texture, Color.red, pointA, pointB);
        //     image.SetValue(texture);
        // }

        if (ShapeDrawerEnabled)
        {
            texture = WispTextureTools.GenerateTexture((int)image.Width, (int)image.Height, Color.clear);

            points = new Vector2[PointCount];
            Vector2 center = new Vector2(image.Width/2, image.Height/2);
            float sliceSize = 360 / PointCount;

            for (int i = 0; i < PointCount; i++)
            {
                float angle = ((i * sliceSize) + Rotation) * 0.0174533f;

                float x = center.x + (ShapeSize * Mathf.Cos(angle));
                float y = center.y + (ShapeSize * Mathf.Sin(angle));

                points[i] = new Vector2(x,y);
            }

            for (int i = 0; i < points.Length - 1; i++)
            {
                WispTextureTools.DrawLineOnTexture(texture, OutlineColor, points[i], points[i+1]);
            }

            if (PointCount > 2)
            {
                WispTextureTools.DrawLineOnTexture(texture, OutlineColor, points[PointCount-1], points[0]);
            }

            // floodFillCounter = 0;
            if (ShapeFillEnabled)
            {
                // FloodFillTexture(texture, (int)center.x, (int)center.y, FillColor);
            }

            if (DrawHorizonEnabled)
            {
                int cx = (int)center.x;
                int cy = (int)center.y;
                // List<Vector2> horizonPoints = new List<Vector2>();
                horizonPoints = new Vector2[texture.width];
                horizonPoints[0] = new Vector2(cx,cy);
                int c = 1;
                
                texture.SetPixel(cx, cy, HorizonColor);

                for (int i = 1; i < texture.width; i++)
                {
                    Color col = texture.GetPixel(cx + i, cy);
                    
                    if (col == Color.clear)
                    {
                        texture.SetPixel(cx + i, cy, HorizonColor);
                        horizonPoints[c] = new Vector2(cx + i, cy);
                        c++;
                    }
                    else
                        break;
                }

                for (int i = 1; i < texture.width; i++)
                {
                    Color col = texture.GetPixel(cx - i, cy);
                    
                    if (col == Color.clear)
                    {
                        texture.SetPixel(cx - i, cy, HorizonColor);
                        horizonPoints[c] = new Vector2(cx - i, cy);
                        c++;
                    }
                    else
                        break;
                }

                foreach(Vector2 p in horizonPoints)
                {
                    cx = (int)p.x;
                    cy = (int)p.y;
                    
                    for (int i = 1; i < texture.height; i++)
                    {
                        Color col = texture.GetPixel(cx, cy + i);
                        
                        if (col == Color.clear)
                        {
                            texture.SetPixel(cx, cy + i, HorizonFillColor);
                        }
                        else
                            break;
                    }

                    for (int i = 1; i < texture.height; i++)
                    {
                        Color col = texture.GetPixel(cx, cy - i);
                        
                        if (col == Color.clear)
                        {
                            texture.SetPixel(cx, cy - i, HorizonFillColor);
                        }
                        else
                            break;
                    }
                }

                texture.Apply();
            }

            if (DrawHalfLineEnabled)
            {
                int minor = Mathf.FloorToInt(PointCount/2);
                
                int major;
                
                if (PointCount.IsEven())
                    major = minor * 2;
                else
                    major = (minor * 2) + 1;

                List<Vector2> lineResult = WispTextureTools.DrawLineOnTexture(texture, HalfLineFillColor, points[minor-1], points[major-1]);
                // float minorMajorAngle = Vector2.Angle(points[minor-1], points[major-1]);
                float minorMajorAngle = Mathf.Atan2(points[major-1].y - points[minor-1].y, points[major-1].x - points[minor-1].x) * 180 / Mathf.PI;
                // print("Half Line Angle : " + minorMajorAngle);

                foreach(Vector2 p in lineResult)
                {
                    print("A = " + texture.GetPixel((int)p.x, (int)p.y));
                    texture.SetPixel((int)p.x, (int)p.y, HalfLineFillColor);
                    print("B = " + texture.GetPixel((int)p.x, (int)p.y));

                    // for (int i = 1; i < ShapeSize * 2; i++)
                    // {
                    //     float x = p.x + (i * Mathf.Cos((minorMajorAngle+90) * 0.0174533f));
                    //     float y = p.y + (i * Mathf.Sin((minorMajorAngle+90) * 0.0174533f));
                    //     int xf = Mathf.FloorToInt(x);
                    //     int xc = Mathf.FloorToInt(x);
                    //     int yf = Mathf.CeilToInt(y);
                    //     int yc = Mathf.CeilToInt(y);

                    //     if (texture.GetPixel(xf, yf) == Color.clear)
                    //         texture.SetPixel((int)x, (int)y, Color.magenta);
                    //     // if (texture.GetPixel(xc, yc) == Color.clear)
                    //     //     texture.SetPixel((int)x, (int)y, Color.magenta);
                    //     else if (texture.GetPixel(xf,yf) == Color.magenta)
                    //     {
                    //         continue;
                    //     }
                    //     else
                    //         break;
                    // }

                    for (int i = 1; i < ShapeSize * 2; i++)
                    {
                        float x = p.x + (i * Mathf.Cos((minorMajorAngle-90) * 0.0174533f));
                        float y = p.y + (i * Mathf.Sin((minorMajorAngle-90) * 0.0174533f));
                        int xf = Mathf.FloorToInt(x);
                        int xc = Mathf.FloorToInt(x);
                        int yf = Mathf.CeilToInt(y);
                        int yc = Mathf.CeilToInt(y);

                        if (texture.GetPixel(xf, yf).a == 0 || texture.GetPixel(xc, yc).a == 0)
                        {
                            // texture.SetPixel(xf, yf, HalfLineColor);
                            if (texture.GetPixel(xf, yf).a == 0)
                            {
                                WispTextureTools.SetTexturePixel_9(texture, xf, yf, HalfLineFillColor, false);
                            }

                            if (texture.GetPixel(xc, yc).a == 0)
                            {
                                WispTextureTools.SetTexturePixel_9(texture, xc, yc, HalfLineFillColor, false);
                            }
                        }
                        else if (texture.GetPixel(xf, yf) == HalfLineFillColor || texture.GetPixel(xc, yc) == HalfLineFillColor)
                        {
                            continue;
                        }
                        // if (texture.GetPixel(xc, yc) == Color.clear)
                        // {
                        //     texture.SetPixel(xc, yc, HalfLineColor);
                        //     // WispTextureTools.SetTexturePixel_9(texture, xf, yf, Color.magenta, false);
                        // }
                        // if (texture.GetPixel(xc, yc) == Color.clear)
                        //     texture.SetPixel((int)x, (int)y, Color.magenta);
                        // else if (texture.GetPixel(xf,yf) == Color.magenta)
                        // {
                        //     continue;
                        // }
                        else
                            break;
                            // continue;
                    }
                }

                texture.Apply();
            }

            image.SetValue(texture);
        }
    }
}