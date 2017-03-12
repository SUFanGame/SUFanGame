using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using SUGame.StrategyMap;
using SUGame.Util;
using SUGame.World;
using SUGame.Util.Common;

[CustomEditor(typeof(Grid))]
public class GridInspector : Editor
{
    Vector3 addPos_;
    static bool showNodes_ = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        addPos_ = EditorGUILayout.Vector3Field("Add Pos", addPos_);

        var grid = target as Grid;
        if( GUILayout.Button("AddNode") )
        {
            grid.AddNodeTest(new Node((IntVector3)addPos_, Node.PathType.Surface));
            EditorUtility.SetDirty(grid.gameObject);
        }

        EditorGUILayout.LabelField("Node count: " + grid.NodeCount_);

        var newShowNodesval = EditorGUILayout.Toggle("Show Nodes", showNodes_);

        if( newShowNodesval != showNodes_ )
        {
            showNodes_ = newShowNodesval;
            EditorUtility.SetDirty(grid.gameObject);
        }

        if( GUILayout.Button("Build From Map") )
        {
            var map = grid.GetComponent<Map>();
            if (map == null)
            {
                Debug.Log("No map found on grid gameobject");
            }
            else
            {
                grid.BuildFromMap(map);
                EditorUtility.SetDirty(map.gameObject);
            }
        
        }
    }

    [DrawGizmo(GizmoType.Active | GizmoType.InSelectionHierarchy)]
    static void OnGizmos( Grid grid, GizmoType gizmoType )
    {
        if (!showNodes_)
            return;

        foreach( var node in grid )
        {
            Gizmos.DrawWireSphere((Vector3)node.Pos_ + Vector3.back * .25f + Vector3.one * .5f, .45f);
        }
        
    }
   
}
