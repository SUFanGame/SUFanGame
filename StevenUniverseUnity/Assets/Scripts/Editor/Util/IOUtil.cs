using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace SUGame.SUGameEditor.Util
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

        /// <summary>
        /// Recurses through the given directory and all subdirectories and populates the given buffer with all assets of the given type.
        /// </summary>
        public static void GetAssetsAndChildrenAtLocation<T>( string pathFromAssetFolder, string searchPattern, List<T> buffer ) where T : UnityEngine.Object
        {

            buffer.AddRange(GetAssetsAtLocation<T>(pathFromAssetFolder, searchPattern));

            var subFolders = AssetDatabase.GetSubFolders( "Assets/" + pathFromAssetFolder);

            foreach( var folder in subFolders )
            {
                var seperator = new string[] { "Assets/" };
                var subPath = folder.Split(seperator,  System.StringSplitOptions.None)[1];
                GetAssetsAndChildrenAtLocation<T>(subPath, searchPattern, buffer);
            }
            
        }
    }
}
