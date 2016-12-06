using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace SUGame.SUGameEditor.Util.Tools
{
    public class ForceFontPointFilter
    {
        [MenuItem("Tools/SetFontsToPointFilter")]
        static void SetFontsToPointFilter()
        {
            var selected = Selection.activeObject;

            //var projectPath = Directory.GetParent(Application.dataPath) + "/";

            //Debug.Log(projectPath);

            if (selected == null )
            {
                Debug.LogError("Nothing is selected");

                return;
            }

            List<Font> fonts = new List<Font>();
            IOUtil.GetAssetsAndChildrenAtLocation<Font>("Font", "*.ttf", fonts);

            foreach( var f in fonts )
            {
                f.material.mainTexture.filterMode = FilterMode.Point;
                AssetDatabase.SaveAssets();
            }
            
            

            //AssetDatabase.Load
            //Debug.Log(AssetDatabase.GetAssetPath(selected));
            //var objects = AssetDatabase.LoadAllAssetsAtPath(Directory.GetParent(AssetDatabase.GetAssetPath(selected)).Name);

            //foreach( var o in objects )
            //{
            //    Debug.Log(o.name);
            //}

///var path = projectPath + AssetDatabase.GetAssetPath(selected);
            //Debug.Log);
        }
    }
}