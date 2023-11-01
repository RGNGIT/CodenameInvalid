using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispPolarCoordinates
{
    private float radius = 0;
    private float angle = 0;

    public WispPolarCoordinates(Vector2 ParamCartesianCoordinates)
    {
        radius = Mathf.Sqrt((ParamCartesianCoordinates.x * ParamCartesianCoordinates.x) + (ParamCartesianCoordinates.y * ParamCartesianCoordinates.y));
        angle = Mathf.Atan2(ParamCartesianCoordinates.y, ParamCartesianCoordinates.x);
    }

    public WispPolarCoordinates(float ParamRadius, float ParamAngle)
    {
        radius = ParamRadius;
        angle = ParamAngle;
    }

    public float Radius { get => radius; set => radius = value; }
    public float Angle { get => angle; set => angle = value; }

    public Vector2 GetCartesian()
    {
        float radians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians) * radius, Mathf.Sin(radians) * radius);
    }

    public static Vector2 GetPointAroundPoint(Vector2 ParamCenter, float ParamRadius, float ParamAngle)
    {
        WispPolarCoordinates polar = new WispPolarCoordinates(ParamRadius, ParamAngle);
        Vector2 coord = polar.GetCartesian();
        return coord + ParamCenter;
    }
}