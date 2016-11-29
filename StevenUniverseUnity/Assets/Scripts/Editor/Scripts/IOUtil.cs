using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace StevenUniverse.FanGameEditor
{
    public class IOUtil 
    {
        /// <summary>
        /// Retrieves all assets of type T at the given folder location which is relative to the assets folder, IE:
        /// "Textures/Spritesheets"
        /// </summary>
        public static T[] GetAssetsAtLocation<T>( string pathFromAssetFolder, string searchPattern ) where T : UnityEngine.Object
        {
            var separator = new string[] { Directory.GetParent(Application.dataPath).Name + "/"  };
            var absolutePath = Application.dataPath + "/" + pathFromAssetFolder + "/";
            // Get our brush textures
            var paths = Directory.GetFiles(absolutePath, searchPattern );
            // Transform the absolute paths relative to our project so they can be used in AssetDatabase
            paths = (from path in paths
                     select path.Split(separator, System.StringSplitOptions.None)[1]).ToArray();

            var assets = paths.Select(p => AssetDatabase.LoadAssetAtPath<T>(p)).ToArray();

            return assets;
        }
    }
}
