using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using SUGame.World;
using SUGame.Util;
using SUGame.SUGameEditor.MapEditing.Brushes;
using System.Linq;
using SUGame.Util.Common;
using SUGame.SUGameEditor.MapEditing.Panels;
using SUGame.SUGameEditor.Util;
using SUGame.SUGameEditor.MapEditing.Util;

namespace SUGame.SUGameEditor.MapEditing
{
    // TODO : Apparently EditorWindows have an update function!? Could make input handling so much simpler/better. Should be able to read keys
    // even if the window or scene isn't focused?
    // Note that update function won't work - the only possible way to read input in edit mode is from the GUI Event structs, which 
    // can only be read in GUI functions

    public class MapEditor : SceneEditorWindow
    {
        //Map SelectedMap_ = null;
        Map SelectedMap_
        {
            get
            {
                var go = Selection.activeGameObject;
                if (go == null)
                    return null;
                return go.GetComponent<Map>();
            }
        }

        /// <summary>
        /// Is the mouse currently hovering over one of the panels?
        /// </summary>
        bool MouseIsOverPanels_
        {
            get
            {
                
                for (int i = 0; i < panels_.Count; ++i)
                    if (panels_[i].ContainsMouse_)
                        return true;
                return false;
            }
                
        }

        static List<MapEditorPanel> panels_ = new List<MapEditorPanel>();

        BrushesPanel brushPanel_;
        HeightPanel heightPanel_;

        MapEditorBrush Brush_ { get { return brushPanel_.SelectedBrush_; } }

        static MapEditor instance_;

        static string TilePrefabPath_ = "Prefabs/Tiles";
         
        IntVector2 lastDragPos_;
        
        public static int SpecificCursorHeight_ { get; set; }

        const string PREFS_CURSORHEIGHT_NAME = "MECursorHeight";

        [MenuItem("Tools/MapEditor")]
        static void OpenWindow()
        {
            EditorWindow.GetWindow<MapEditor>();
        }
        
        protected override void OnGUI()
        {
            base.OnGUI();

            var map = SelectedMap_;

            if (map == null)
            {
                GUILayout.Label("No map selected");
                return;
            }


            GUI.enabled = false;
            EditorGUILayout.ObjectField("Current Map", map, typeof(Map), true);
            GUI.enabled = true;

            if (!SceneEditorUtil.EditMode_)
            {
                GUILayout.Label("You are not currently in edit mode. Click the button above to enter edit mode.", EditorStyles.wordWrappedLabel);
                return;
            }

            if (brushPanel_ != null)
            {
                Brush_.MapEditorGUI();
            }

            // We must process input after all gui calls to prevent unity's layout errors.
            ProcessKeyboardInput();
        }

        protected override void OnSceneGUI(SceneView view)
        {
            base.OnSceneGUI(view);

            Repaint();

            if (!SceneEditorUtil.EditMode_ || !mouseOverWindow == SceneView.currentDrawingSceneView || SelectedMap_ == null )
                return;



            foreach (var panel in panels_)
                panel.OnSceneGUI(SelectedMap_);

            if (!MouseIsOverPanels_)
            {
                var mousePos = Event.current.mousePosition + Vector2.right * 10;
                var mouseWorldPos = (IntVector3)HandleUtility.GUIPointToWorldRay(mousePos).origin;

                mouseWorldPos = Brush_.GetTargetPosition(SelectedMap_, mouseWorldPos);

                Handles.BeginGUI();
                EditorGUI.LabelField(new Rect(0,Screen.height - 65, 100f, 100f), mouseWorldPos.ToString("0") );
                Handles.EndGUI();
            }


            SceneView.currentDrawingSceneView.Repaint();
        }



        protected override void OnEnable()
        {
            base.OnEnable();

            SpecificCursorHeight_ = EditorPrefs.GetInt(PREFS_CURSORHEIGHT_NAME, 0);


            //Debug.Log("ONENABLE");

            instance_ = this;

            titleContent.text = "MapEditor";

            Undo.undoRedoPerformed += OnUndoRedo;

            //Debug.LogFormat("Selected Is Null: {0}", selectedMap_ == null);

            BuildPanels();

            Repaint();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EditorPrefs.SetInt(PREFS_CURSORHEIGHT_NAME, SpecificCursorHeight_);
            //Debug.Log("ONDISABLE");

            instance_ = null;

            foreach (var panel in panels_)
                panel.OnDisable();

            Undo.undoRedoPerformed -= OnUndoRedo;
        }


        void OnFocus()
        {
        }

        void BuildPanels()
        {
            panels_.Clear();

            //Debug.Log("Building Panels");

            List<MapEditorBrush> brushes = new List<MapEditorBrush>();
            brushes.Add(new PaintTileBrush(IOUtil.GetAssetsAtLocation<Tile>(TilePrefabPath_, "*.prefab")));
            brushes.Add(new EraseBrush());

            panels_.Add(new LayersPanel());

            // Need to keep track of height panel to reference CutoffAlpha when changing heights.
            heightPanel_ = new HeightPanel();
            panels_.Add(heightPanel_);

            brushPanel_ = new BrushesPanel(brushes);
            panels_.Add(brushPanel_);

            //panels_.Add(new BrushShapePanel(brushPanel_));
        }



        protected override void OnMouseDown(int button, Vector3 cursorWorldPos)
        {
            if (SelectedMap_ == null)
                return;

            // Ignore clicks inside the panels
            if (MouseIsOverPanels_)
                return;

            if (button == 0)
            { 
                var pos = (IntVector3)cursorWorldPos;
                pos.z = SpecificCursorHeight_;
                //Debug.LogFormat("MapEditor cursor Pos in InMouseDown: {0}", pos);

                Brush_.OnMouseDown(SelectedMap_, pos);
            }
        }

        protected override void OnMouseUp(int button, Vector3 cursorWorldPos)
        {
            base.OnMouseUp(button, cursorWorldPos);

            if (SelectedMap_ == null)
                return;

            // Ignore clicks inside the panels
            if (MouseIsOverPanels_)
                return;

            if ( button == 0 )
            {
                var pos = (IntVector3)cursorWorldPos;
                pos.z = SpecificCursorHeight_;
                Brush_.OnMouseUp(SelectedMap_, pos);
            }
        }

        protected override void OnMouseDrag(int button, Vector3 cursorWorldPos)
        {
            base.OnMouseDrag(button, cursorWorldPos);

            if (MouseIsOverPanels_)
                return;

            if (SelectedMap_ == null)
                return;

            if ( (IntVector2)cursorWorldPos == lastDragPos_ )
            {
                return;
            }
            
            lastDragPos_ = (IntVector2)cursorWorldPos;

            if( button == 0 )
            {
                var pos = (IntVector3)cursorWorldPos;
                pos.z = SpecificCursorHeight_;
                Brush_.OnDrag(SelectedMap_, pos);
            }
        }

        protected override void OnKeyDown(KeyCode key)
        {
            base.OnKeyDown(key);



           



            if (MouseIsOverPanels_)
                return;

            var map = SelectedMap_;
            if (map == null)
                return;


            if (Event.current.shift && key == KeyCode.Comma)
            {
                SpecificCursorHeight_++;
                map.heightCutoff_ = SpecificCursorHeight_;
                if ( map.cutoffType_ != CutoffType.NONE )
                {
                    map.SetCutoffAlphaOnChunks( (byte)(heightPanel_.CutoffAlpha_ * 255f) );
                    EditorUtility.SetDirty(map.gameObject);
                }
                return;
            }

            if (Event.current.shift && key == KeyCode.Period)
            {
                SpecificCursorHeight_--;
                map.heightCutoff_ = SpecificCursorHeight_;
                if (map.cutoffType_ != CutoffType.NONE)
                {
                    map.SetCutoffAlphaOnChunks( (byte)(heightPanel_.CutoffAlpha_ * 255f) );
                    EditorUtility.SetDirty(map.gameObject);
                }
                return;
            }


            Brush_.OnKeyDown(key);
            brushPanel_.OnKeyDown(key);
        }



        protected override void OnMouseScroll(Vector2 delta)
        {

            base.OnMouseScroll(delta);

            if (SelectedMap_ == null)
                return;

            Brush_.OnScroll(SelectedMap_, delta.y);
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        static void DrawGizmos(Transform t, GizmoType gizmoType)
        {
            // Bail out if we're not in "edit mode".
            if (!SceneEditorUtil.EditMode_ || instance_ == null )
                return;

            var map = instance_.SelectedMap_;

            if (map == null)
                return;

            //var cursorPos = SceneEditorUtil.GetCursorPosition();
            //var labelPos = HandleUtility.WorldToGUIPoint(cursorPos + Vector3.right + (Vector3.up));
            //var mousePos = Event.current.mousePosition;
            //// Draw our elevation label.
            //Handles.BeginGUI();
            //EditorGUI.LabelField(new Rect(mousePos.x, mousePos.y, 100f, 100f), "Test" );
            //Handles.EndGUI();
            //instance_.Brush_.RenderCursor();
            // Dont draw the brush cursor if we're hovering over a panel.
            if (instance_.MouseIsOverPanels_)
                return;

            instance_.Brush_.RenderCursor( map );

            instance_.Repaint();
        }

        
        

        void OnUndoRedo()
        {
            var map = SelectedMap_;
            if (SelectedMap_ == null)
                return;

            map.RefreshMesh();
            //map.ClearEmptyChunks();


            EditorUtility.SetDirty(map.gameObject);
        }
    }
}
