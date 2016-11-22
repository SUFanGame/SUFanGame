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
                foreach (var chunk in tar)
                    chunk.Print();
            }
        }
    }
}
