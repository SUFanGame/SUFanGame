using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SUGame.SUGameEditor.Util
{
    public class ForcePointFilter
    {
        [MenuItem("Assets/Font/Set To Point")]
        public static void SetPointFilter( MenuCommand command )
        {
            //Debug.Log(command.context.GetType().Name);
            //var font = command.context as Font;

            //Debug.LogFormat("Font is null {0}:", font == null);
            var font = Selection.activeObject as Font;
            font.material.mainTexture.filterMode = FilterMode.Point;
        }
        
        [MenuItem("Assets/Font/Set To Point", true)]
        public static bool ValidateSetPointFilter()
        {
            var font = Selection.activeObject as Font;
            return font != null;
        }

        //[MenuItem("Assets/Font/Set To Smooth Raster")]
        //public static void SetToSmoothRaster()
        //{
        //    var objects = Selection.objects;
        //    if (objects == null)
        //        return;
        //    foreach( var o in objects )
        //    {
        //        var font = o as Font;
        //        if (font == null)
        //            continue;
                
        //    }
        //}
    }
}
