using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using SUGame.World;

namespace SUGame.SUGameEditor.World.Inspector
{
    [CustomEditor(typeof(Map))]
    public class MapInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var tar = target as Map;

            if( GUILayout.Button("Hide Scene Mesh Lines") )
            {

            }

            if( GUILayout.Button("Clear"))
            {
                tar.Clear();
            }

            if( GUILayout.Button("Print Tiles") )
            {
                //tar.PrintTiles();
            }

            if( GUILayout.Button("Clear Empty Chunks") )
            {
                if (EditorUtility.DisplayDialog("Clear Empty Chunks", "Are you sure you want to clear empty chunks? This cannot be undone and will prevent outstanding Undo operations in those chunks.", "Yes", "Cancel" ) )
                {
                    tar.ClearEmptyChunks();
                }
            }

            if( GUILayout.Button("UpdateBounds") )
            {
                tar.UpdateBounds();
            }
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
        static void DrawGizmos(Map map, GizmoType gizmoType)
        {
            Gizmos.DrawWireCube(map.Bounds_.center, map.Bounds_.size);
        }
    }



}
