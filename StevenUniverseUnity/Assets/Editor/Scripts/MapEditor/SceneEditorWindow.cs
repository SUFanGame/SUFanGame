using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    /// <summary>
    /// A base class that facilitates editing objects in the scene view in the editor.
    /// Derived classes can receive input events and use "edit mode" to make editing easier.
    /// </summary>
    public class SceneEditorWindow : EditorWindow
    {
        Scene lastScene_ = default(Scene);

        protected virtual void OnEnable()
        {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            autoRepaintOnSceneChange = true;
            
            EditorApplication.hierarchyWindowChanged -= OnSceneLoadedInternal;
            EditorApplication.hierarchyWindowChanged += OnSceneLoadedInternal;
        }

        protected virtual void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        /// <summary>
        /// Derived classes can implement to receieve mouse events in the scene view.
        /// </summary>
        /// <param name="button">The button being pressed.</param>
        /// <param name="cursorWorldPos">The current world position of the mouse cursor.</param>
        protected virtual void OnMouseDown(int button, Vector3 cursorWorldPos)
        {

        }

        /// <summary>
        /// Derived classes can override to receieve keyboard events in the scene veiew.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        protected virtual void OnKeyDown(KeyCode key)
        {

        }

        /// <summary>
        /// Derived classes can override to receieve mouse drag events in the scene view.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="cursorWorldPos"></param>
        protected virtual void OnMouseDrag(int button, Vector3 cursorWorldPos)
        {

        }

        protected virtual void OnMouseScroll( Vector2 delta )
        {
            //Debug.LogFormat("MouseScroll: {0}", delta);
        }

        /// <summary>
        /// Draws the "Toggle edit mode" button.
        /// </summary>
        protected virtual void OnGUI()
        {
            //// Draw our "Enter/Exit edit mode" button.
            string editModeString = SceneEditorUtil.EditMode_ ?
                "exit" : "enter";
            if (GUILayout.Button("Click me to " + editModeString + " edit mode"))
            {
                SceneEditorUtil.EditMode_ = !SceneEditorUtil.EditMode_;
            }

            if (!SceneEditorUtil.EditMode_)
                return;

            ProcessKeyboardInput();
        }

        protected virtual void OnSceneGUI(SceneView view)
        {

            // Prevent selection of other objects in the scene while in edit mode
            if (SceneEditorUtil.EditMode_)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                //Handles.BeginGUI();

                //GUILayout.Label("Currently in Edit mode. Click the button in the map editor window or select a Unity tool to exit.");

                //Handles.EndGUI();
            }
            else
                return;

            ProcessKeyboardInput();
            ProcessMouseInput();

            Repaint();
        }

        void ProcessKeyboardInput()
        {
            Event e = Event.current;
            if (e.isKey)
            {
                if (e.type == EventType.KeyDown)
                {
                    // Forward key presses to derived classes
                    OnKeyDown(e.keyCode);
                }
            }

        }
        

        void ProcessMouseInput()
        {
            Event e = Event.current;
            if (e.type == EventType.ScrollWheel && e.shift )
            {
                OnMouseScroll(e.delta);
                e.Use();
            }

            if (e.isMouse)
            {
                Vector2 worldPos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
                if (e.type == EventType.mouseDown)
                {
                    // Get the world position of the mouse cursor. Note this only works in 2D scenes.
                    // For 3D we would have to cast the camera ray against a plane matching our target's
                    // orientation.
                    // Forward mouse event to derived classes.
                    OnMouseDown(e.button, worldPos);
                }
                else if (e.type == EventType.MouseDrag)
                {
                    OnMouseDrag(e.button, worldPos);
                }
            }
        }

        void OnSceneLoadedInternal()
        {
            if( EditorSceneManager.GetActiveScene() != lastScene_ )
            {
                lastScene_ = EditorSceneManager.GetActiveScene();
                OnSceneLoaded();

            }
        }

        /// <summary>
        /// NOTE: Editorwindow does some weird stuff with serialization AFTER OnEnable.
        /// This means any references set up in OnEnable - even ones not intended to be serialized -
        /// will be erased if they aren't serializable. This will be called once and is guaranteed
        /// to be called when the scene is ACTUALLY loaded, unlike OnEnable apparently. 
        /// </summary>
        protected virtual void OnSceneLoaded()
        {

        }

    }

}