using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using SUGame.Util;
using SUGame.SUGameEditor.CustomGUI;
using SUGame.World.DynamicMesh;

namespace SUGame.SUGameEditor.World.Inspector
{
    [CustomEditor(typeof(TiledMesh))]
    public class TiledMeshInspector : Editor
    {
        SerializedProperty layerIDProp_;

        void OnEnable()
        {
            //layerIDProp_ = serializedObject.FindProperty("sortingLayerID_");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            var tar = target as TiledMesh;

            if (!tar.isActiveAndEnabled)
                return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Sorting Layer");
            //layerIDProp_.intValue = CustomEditorGUI.DrawSortingLayersPopup(layerIDProp_.intValue);
            tar.renderer_.sortingLayerID = CustomEditorGUI.DrawSortingLayersPopup(tar.renderer_.sortingLayerID);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Sorting Order", tar.renderer_.sortingOrder.ToString());
        }

        [DrawGizmo(GizmoType.Active | GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(TiledMesh mesh, GizmoType gizmoType)
        {
            //var offset = mesh.transform.position + Vector3.one * .5f;
            //Gizmos.DrawWireCube( mesh.transform.position + (Vector3)mesh.Size_ / 2f + Vector3.forward * 5, mesh.Size_);

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

}