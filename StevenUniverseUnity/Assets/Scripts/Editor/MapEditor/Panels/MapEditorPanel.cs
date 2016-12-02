using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using SUGame.World;

namespace SUGame.SUGameEditor.MapEditing.Panels
{
    public abstract class MapEditorPanel
    {
        private bool foldout_ = false;
        protected bool Foldout_ { get { return foldout_; } }
        /// <summary>
        /// Whether or not the mouse is in this panel.
        /// </summary>
        public bool ContainsMouse_ { get; private set; }
        public abstract Rect Area_ { get; }
        protected abstract string FoldoutTitle_ { get; }

        string EditorPrefs_FoldoutName_ { get { return GetType().Name + "Foldout"; } }

        public MapEditorPanel()
        {
            foldout_ = EditorPrefs.GetBool(EditorPrefs_FoldoutName_, true);
        }

        public virtual void OnSceneGUI(Map map)
        {
            Handles.BeginGUI();

            GUILayout.BeginArea(Area_, EditorStyles.helpBox);

            // Check if the mouse is contained in our area.
            if (Event.current.isMouse)
            {
                // The mouse position will be reported relative to our current area
                ContainsMouse_ = Area_.Contains(Event.current.mousePosition + Area_.position);
            }

            DrawFoldOut( map );

            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private void DrawFoldOut( Map map )
        {
            foldout_ = EditorGUILayout.Foldout(foldout_, FoldoutTitle_);
            if( foldout_ )
            {
                OnPanelGUI(map);
            }
        }

        protected abstract void OnPanelGUI(Map map);

        /// <summary>
        /// Panels can save relevant data to editorprefs here.
        /// </summary>
        public virtual void OnDisable()
        {
            EditorPrefs.SetBool(EditorPrefs_FoldoutName_, foldout_);
        }

        public virtual void OnKeyDown(KeyCode key)
        {

        }
    }
}