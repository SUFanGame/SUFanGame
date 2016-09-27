using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace StevenUniverse.FanGame.Extensions
{
    public static class Texture2DExtensions
    {
        //convert to bitmap
        public static Bitmap ConvertToBitmap(this Texture2D source)
        {
            Bitmap output = new Bitmap(source.width, source.height);

            //Convert the unity colors of the texture into system colors
            List<System.Drawing.Color> outputColors = new List<System.Drawing.Color>();
            foreach (UnityEngine.Color sourceColor in source.GetPixels(0, 0, source.width, source.height))
            {
                outputColors.Add(sourceColor.GetSystemColor());
            }

            //Set the pixels of the bitmap equal to the colors of the source texture
            output.SetPixels(0, 0, output.Width, output.Height, outputColors.ToArray());
            return output;
        }
    }
}