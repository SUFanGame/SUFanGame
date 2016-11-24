using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using StevenUniverse.FanGame.World;

namespace StevenUniverse.FanGameEditor
{
    [CustomEditor(typeof(Map))]
    public class MapInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var tar = target as Map;

            if( GUILayout.Button("Clear"))
            {
                tar.Clear();
            }

            if( GUILayout.Button("Print Tiles") )
            {
                tar.PrintTiles();
            }

            if( GUILayout.Button("Clear Empty Chunks") )
            {
                if (EditorUtility.DisplayDialog("Clear Empty Chunks", "Are you sure you want to clear empty chunks? This cannot be undone and will prevent outstanding Undo operations in those chunks.", "Yes", "Cancel" ) )
                {
                    tar.ClearEmptyChunks();
                }
            }
        }
    }
}
