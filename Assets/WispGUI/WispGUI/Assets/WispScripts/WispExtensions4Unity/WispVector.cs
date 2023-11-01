using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WispExtensions
{
    public static class WispVector
    {
        public static string ToCommaSeparatedXYZ(this Vector3 ParamMe)
        {
            return ParamMe.x.ToString() + "," + ParamMe.y.ToString() + "," + ParamMe.z.ToString();
        }

        public static string ToCommaSeparatedXY(this Vector2 ParamMe)
        {
            return ParamMe.x.ToString() + "," + ParamMe.y.ToString();
        }

        public static Vector3 FromStrings(string ParamX, string ParamY, string ParamZ)
        {
            return new Vector3(ParamX.ToFloat(), ParamY.ToFloat(), ParamZ.ToFloat());
        }

        public static Vector3 SetX(this Vector3 ParamMe, float ParamX)
        {
            return new Vector3(ParamX, ParamMe.y, ParamMe.z);
        }

        public static Vector3 SetY(this Vector3 ParamMe, float ParamY)
        {
            return new Vector3(ParamMe.x, ParamY, ParamMe.z);
        }

        public static Vector3 SetZ(this Vector3 ParamMe, float ParamZ)
        {
            return new Vector3(ParamMe.x, ParamMe.y, ParamZ);
        }

        // Check if any of the vector components is NOT within range of a certain threshold.
        public static bool DeltaCheck(this Vector3 ParamMe, Vector3 ParamOther, float ParamThreshold)
        {
            if (ParamOther.x > ParamMe.x - ParamThreshold && ParamOther.x < ParamMe.x + ParamThreshold)
            {
                if (ParamOther.y > ParamMe.y - ParamThreshold && ParamOther.y < ParamMe.y + ParamThreshold)
                {
                    if (ParamOther.z > ParamMe.z - ParamThreshold && ParamOther.z < ParamMe.z + ParamThreshold)
                    {
                        return false;
                    }        
                }    
            }

            return true;
        }

        public static Vector3 GenerateRandomVariation(this Vector3 ParamMe, float ParamX, float ParamY, float ParamZ)
        {
            return new Vector3(ParamMe.x + Random.Range(-ParamX, ParamX), ParamMe.y + Random.Range(-ParamY, ParamY), ParamMe.z + Random.Range(-ParamZ, ParamZ));
        }

        public static Vector3 SnapToGrid(this Vector3 ParamMe, int ParamGridX, int ParamGridY, int ParamGridZ)
        {
            float x = ParamMe.x.SnapToGrid(ParamGridX);
            float y = ParamMe.y.SnapToGrid(ParamGridY);
            float z = ParamMe.z.SnapToGrid(ParamGridZ);

            return new Vector3(x,y,z);
        }
    }
}