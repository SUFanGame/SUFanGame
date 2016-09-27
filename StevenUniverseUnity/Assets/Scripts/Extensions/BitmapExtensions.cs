using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace StevenUniverse.FanGame.Extensions
{
    public static class BitmapExtensions
    {
        public static Rectangle GetTrueBounds(this Bitmap source)
        {
            GraphicsUnit units = GraphicsUnit.Pixel;
            RectangleF boundsF = source.GetBounds(ref units);
            return Rectangle.Round(boundsF);
        }

        public static Texture2D ConvertToTexture2D(this Bitmap source)
        {
            Rectangle bounds = source.GetTrueBounds();
            Texture2D output = new Texture2D(bounds.Width, bounds.Height);

            //Convert the system colors of the bitmap into unity colors
            List<UnityEngine.Color> outputColors = new List<UnityEngine.Color>();
            foreach (System.Drawing.Color sourceColor in source.GetPixels(0, 0, bounds.Width, bounds.Height))
            {
                outputColors.Add(sourceColor.GetUnityColor());
            }

            //Set the pixels of the output Texture2D equal to the colors of the source Bitmap
            output.SetPixels(0, 0, output.width, output.height, outputColors.ToArray());
            output.filterMode = FilterMode.Point;
            output.Apply();
            return output;
        }

        public static System.Drawing.Color[] GetPixels(this Bitmap source, int pivotX, int pivotY, int width, int height)
        {
            List<System.Drawing.Color> pixels = new List<System.Drawing.Color>();

            //Iterate through each pixel row by row
            for (int y = pivotY; y < pivotY + height; y++)
            {
                for (int x = pivotX; x < pivotX + width; x++)
                {
                    pixels.Add(source.GetPixel(x, y));
                }
            }

            return pixels.ToArray();
        }

        public static void SetPixels(this Bitmap source, int pivotX, int pivotY, int width, int height,
            System.Drawing.Color[] pixels)
        {
            int counter = 0;
            for (int y = pivotY; y < pivotY + height; y++)
            {
                for (int x = pivotX; x < pivotX + width; x++)
                {
                    source.SetPixel(x, y, pixels[counter]);
                    counter++;
                }
            }
        }
    }
}