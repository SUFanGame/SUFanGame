using UnityEngine;
using UnityEditor;
using SUGame.World;
using SUGame.Interactions;

namespace SUGame.SUGameEditor.MapEditing
{

    public class CutsceneEditor : SceneEditorWindow
    {

        //This will need to be set, either as a new scene or imported one
        Scene OpenScene_ = null;

        //Map that the cutscene is operating on, will need to be set
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


        [MenuItem("Tools/CutsceneEditor")]
        static void OpenWindow()
        {
            EditorWindow.GetWindow<CutsceneEditor>();
        }

        //Opens up the editor window
        protected override void OnGUI()
        {
            //base.OnGUI(); //calling SceneEditorWindow

            var map = SelectedMap_;
            var scene = OpenScene_;

            if (scene == null)
            {
                GUILayout.Label("Open or create a new scene");
                if (GUILayout.Button("Open Cutscne"))
                {
                    Debug.Log("opened!");
                }
                if (GUILayout.Button("Create New Cutscne"))
                {
                    Debug.Log("created!");
                }
            }

            if (map == null)
            {
                GUILayout.Label("No map selected");
                return;
            }


            // updating the current map label
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Current Map", map, typeof(Map), true);
            GUI.enabled = true;

            

            // We must process input after all gui calls to prevent unity's layout errors.
            ProcessKeyboardInput();
        }

        // Entered edit mode
        protected override void OnSceneGUI(SceneView view)
        {
            base.OnSceneGUI(view);

            Repaint();

            if (!SceneEditorUtil.EditMode_ || !mouseOverWindow == SceneView.currentDrawingSceneView || SelectedMap_ == null)
                return;



            //foreach (var panel in panels_)
            //    panel.OnSceneGUI(SelectedMap_);
            //
            //if (!MouseIsOverPanels_)
            //{
            //    var mousePos = Event.current.mousePosition + Vector2.right * 10;
            //    var mouseWorldPos = (IntVector3)HandleUtility.GUIPointToWorldRay(mousePos).origin;
            //    mouseWorldPos.z = CursorHeight_;
            //
            //    Handles.BeginGUI();
            //    EditorGUI.LabelField(new Rect(0, Screen.height - 65, 100f, 100f), mouseWorldPos.ToString("0"));
            //    Handles.EndGUI();
            //}
            //
            //
            //SceneView.currentDrawingSceneView.Repaint();
        }

    }

}