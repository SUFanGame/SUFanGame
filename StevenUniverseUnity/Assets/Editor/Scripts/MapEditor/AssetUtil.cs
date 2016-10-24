using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class AssetUtil : MonoBehaviour
    {
        /// <summary>
        /// Get all assets at the given path (assumes the given path begins at and excludes the assets folder)
        /// </summary>
        public static List<T> GetAssets<T>(string path)
        {
            var list = new List<T>();
            // AssetDatabase doesn't allow us to load all resources in a single folder.
            // So we have to iterate over each file, or use the Resources folder
            // http://answers.unity3d.com/questions/24060/can-assetdatabaseloadallassetsatpath-load-all-asse.html
            // Load all the prefabs in all subfolders of the given path
            var files = Directory.GetFiles(Application.dataPath + "/" + path, "*.prefab", SearchOption.AllDirectories);
            float progress = 0;

            foreach (var file in files)
            {
                EditorUtility.DisplayProgressBar("Loading " + typeof(T).Name, file, progress);

                var filePath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');

                // Get the actual gameobject and target object from the asset
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                var t = go.GetComponent<T>();

                if (t != null)
                {
                    // Populate our lists if our target is valid.
                    list.Add(t);
                }

                progress += 1f / (float)files.Length;
            }

            EditorUtility.ClearProgressBar();

            return list;
        }
    }

}
