using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(World))]
public class WorldInspector : Editor
{
    //List<Vector3> keyList_ = new List<Vector3>();
    string assetPath_;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var tar = target as World;

        //assetPath_ = EditorGUILayout.TextField(assetPath_);

        //if( GUILayout.Button("Create asset Test"))
        //{
        //    //EditorUtils.CreatePrefab<UnityTile>(assetPath_);
        //}

        //foreach (var pair in tar.TileMap)
        //{
        //    //for (int i = pair.Value.Count - 1; i >= 0; --i)
        //    //{
        //    //    if (pair.Value[i] == null)
        //    //        pair.Value.RemoveAt(i);
        //    //}

        //    var tileNames = pair.Value.Select(t => t.name).ToArray();
        //    var tileNamesString = string.Join(", ", tileNames);

        //    var str = string.Format("{0}:{1}", pair.Key, tileNamesString);

        //    EditorGUILayout.LabelField(str);
        //}

        //if ( GUILayout.Button("Clear") )
        //{
        //    //var tiles = FindObjectsOfType<UnityTile>();
        //    //foreach (var t in tiles)
        //    //    DestroyImmediate(t.gameObject, false);
        //    //tar.TileMap.Clear();
        //}
        
    }

}
