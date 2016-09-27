using System;
using UnityEngine;

namespace StevenUniverse.FanGame.Extensions
{
    public static class ColorExtensions
    {
        //THIS WORKS
        public static System.Drawing.Color GetSystemColor(this UnityEngine.Color source)
        {
            int a = Mathf.RoundToInt(source.a*255);
            int r = Mathf.RoundToInt(source.r*255);
            int g = Mathf.RoundToInt(source.g*255);
            int b = Mathf.RoundToInt(source.b*255);

            System.Drawing.Color converted = System.Drawing.Color.FromArgb(a, r, g, b);
            //Debug.Log(string.Format("Unity({0},{1},{2},{3}) => System({4},{5},{6},{7})", source.r, source.g, source.b, source.a, converted.R, converted.G, converted.B, converted.A));
            return converted;
        }

        //THIS WORKS
        public static UnityEngine.Color GetUnityColor(this System.Drawing.Color source)
        {
            float r = Convert.ToInt32(source.R)/255f;
            float g = Convert.ToInt32(source.G)/255f;
            float b = Convert.ToInt32(source.B)/255f;
            float a = Convert.ToInt32(source.A)/255f;

            UnityEngine.Color converted = new UnityEngine.Color(r, g, b, a);
            //Debug.Log(string.Format("System({0},{1},{2},{3}) => Unity({4},{5},{6},{7})",source.R,source.G,source.B,source.A,converted.r,converted.g,converted.b,converted.a));
            return converted;
        }
    }
}