using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using StevenUniverse.FanGame.World;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGameEditor.SceneEditing.Brushes;
using System.Linq;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class MapEditor : SceneEditorWindow
    {
        
        static Map map_;
        static List<MapEditorPanel> panels_ = new List<MapEditorPanel>();

        BrushesPanel brushPanel_;

        MapEditorBrush Brush_ { get { return brushPanel_.SelectedBrush_; } }

        static MapEditor instance_;

        static string TilePrefabPath_ = "Prefabs/Tiles";

        [MenuItem("Tools/SUFanGame/MapEditor")]
        static void OpenWindow()
        {
            EditorWindow.GetWindow<MapEditor>();
        }


        protected override void OnSceneGUI(SceneView view)
        {
            base.OnSceneGUI(view);

            foreach (var panel in panels_)
                panel.OnSceneGUI();

            var mousePos = Event.current.mousePosition + Vector2.right * 10;
            var mouseWorldPos = (IntVector2)HandleUtility.GUIPointToWorldRay(mousePos).origin;
            // Draw our elevation label.
            Handles.BeginGUI();
            EditorGUI.LabelField(new Rect(mousePos.x, mousePos.y, 100f, 100f), mouseWorldPos.ToString("0") );
            Handles.EndGUI();

            SceneView.currentDrawingSceneView.Repaint();

        }



        protected override void OnEnable()
        {
            base.OnEnable();

            instance_ = this;

            titleContent.text = "MapEditor";

            if (map_ == null)
                map_ = GameObject.FindObjectOfType<Map>();


            List<MapEditorBrush> brushes = new List<MapEditorBrush>();
            brushes.Add(new PaintTileBrush(IOUtil.GetAssetsAtLocation<Tile>(TilePrefabPath_, "*.prefab")) );
            brushes.Add(new EraseBrush());

            panels_.Add(new LayersPanel(map_));

            brushPanel_ = new BrushesPanel(panels_[0] as LayersPanel, brushes);

            panels_.Add( brushPanel_ );

        }


        protected override void OnDisable()
        {
            base.OnDisable();

            instance_ = null;

            foreach (var panel in panels_)
                panel.OnDisable();

        }

        protected override void OnMouseDown(int button, Vector3 cursorWorldPos)
        {
            // Ignore mouse activity inside our panels
            //if (LayersPane.ContainsMouse_)
            //   return;


            if (map_ == null)
                map_ = GameObject.FindObjectOfType<Map>();

            if (map_ == null)
            {
                Debug.LogWarning("No map found in the scene");
                return;
            }

            // Ignore clicks inside the panels
            foreach ( var p in panels_ )
            {
                if (p.ContainsMouse_)
                    return;
            }

            if (button == 0)
            {
                var pos = (IntVector3)cursorWorldPos;

                Brush_.OnMouseDown(map_, pos);
            }
        }

        protected override void OnMouseDrag(int button, Vector3 cursorWorldPos)
        {
            base.OnMouseDrag(button, cursorWorldPos);
            
        }

        protected override void OnMouseScroll(Vector2 delta)
        {
            base.OnMouseScroll(delta);

            Brush_.OnScroll(map_, delta.y);
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(Transform t, GizmoType gizmoType)
        {
            // Bail out if we're not in "edit mode".
            if (!SceneEditorUtil.EditMode_)
                return;

            if (instance_ == null)
                return;

            //var cursorPos = SceneEditorUtil.GetCursorPosition();
            //var labelPos = HandleUtility.WorldToGUIPoint(cursorPos + Vector3.right + (Vector3.up));
            //var mousePos = Event.current.mousePosition;
            //// Draw our elevation label.
            //Handles.BeginGUI();
            //EditorGUI.LabelField(new Rect(mousePos.x, mousePos.y, 100f, 100f), "Test" );
            //Handles.EndGUI();

            instance_.Brush_.RenderCursor();


        }

        protected override void OnGUI()
        {
            base.OnGUI();

            map_ = (Map)EditorGUILayout.ObjectField(map_, typeof(Map), true);

            Brush_.MapEditorGUI();

            //GUILayout.Label("Panels: " + panels_.Count);

            //var mousePos = Event.current.mousePosition;
            //mousePos.y = mousePos.y;
            //GUILayout.Label(mousePos.ToString());

            //GUILayout.Label("LayerPanelContainsMouse: " + LayersPane.ContainsMouse_.ToString());
            //GUILayout.Label("LayerPanelArea: " + LayersPane.Area_);
            //GUILayout.Label("LayersPanelMousePos: " + LayersPane.MousePos_);
        }

        static void DrawCursorLabel()
        {
            Handles.BeginGUI();

            var mousePos = Event.current.mousePosition + Vector2.right * 10;

            GUILayout.BeginArea(new Rect(mousePos, new Vector2(100, 22)));

            GUILayout.Label("HI");

            GUILayout.EndArea();

            Handles.EndGUI();
        }

    }
}
