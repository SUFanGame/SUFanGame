using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SUGame.Util.TransformUtil
{
    public class RectTransformUtil
    {
        /// <summary>
        /// Moves a recttransform so it's center is on the given canvas position, regardless of
        /// it's pivot position.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="canvasPosition"></param>
        public static void MoveCentered( RectTransform rt, Vector3 canvasPosition )
        {
            var pivotScale = Vector2.Scale(rt.sizeDelta, (rt.pivot - (Vector2.one * .5f)));
            canvasPosition.x += pivotScale.x;
            canvasPosition.y += pivotScale.y;
            rt.position = canvasPosition;
        }
    }
}
