using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using StevenUniverse.FanGame.Util;

[CustomEditor(typeof(TiledMesh))]
public class TiledMeshInspector : Editor
{
    SerializedProperty layerIDProp_;

    void OnEnable()
    {
        layerIDProp_ = serializedObject.FindProperty("sortingLayerID_");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Sorting Layer");
        layerIDProp_.intValue = CustomEditorGUI.DrawSortingLayersPopup(layerIDProp_.intValue);
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    [DrawGizmo(GizmoType.Active | GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawGizmos(TiledMesh mesh, GizmoType gizmoType)
    {
        //var offset = mesh.transform.position + Vector3.one * .5f;
        Gizmos.DrawWireCube( mesh.transform.position + (Vector3)mesh.Size_ / 2f, mesh.Size_);

        if (mesh.showLayerOrder_)
        { 
            //var content = new GUIContent(mesh.renderer_.sortingOrder.ToString());
            //var style = new GUIStyle(EditorStyles.largeLabel);
            //var height = style.CalcSize(content).y;

            //var newColor = Color.black;//new Color32(0, 255, 255, 120);

            //style.normal.textColor = newColor;
            //style.normal.background = Texture2D.whiteTexture;

            //var pos = mesh.renderer_.bounds.min;

            //Handles.Label(pos + Vector3.up, mesh.renderer_.sortingOrder.ToString(), style);

        }
    }
}
