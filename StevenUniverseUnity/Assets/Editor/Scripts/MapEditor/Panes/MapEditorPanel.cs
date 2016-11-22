using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using StevenUniverse.FanGame.World;

public abstract class MapEditorPanel
{
    /// <summary>
    /// Whether or not the mouse is in this panel.
    /// </summary>
    public bool ContainsMouse_ { get; private set; }
    public abstract Rect Area_ { get; }

    public virtual void OnSceneGUI()
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(Area_, EditorStyles.helpBox);

        // Check if the mouse is contained in our area.
        if (Event.current.isMouse)
        {
            // The mouse position will be reported relative to our current area
            ContainsMouse_ = Area_.Contains(Event.current.mousePosition + Area_.position);
        }

        OnRenderArea();

        GUILayout.EndArea();
        Handles.EndGUI();
    }

    protected abstract void OnRenderArea();
    
    /// <summary>
    /// Panels can save relevant data to editorprefs here.
    /// </summary>
    public virtual void OnDisable()
    {
    }
}
