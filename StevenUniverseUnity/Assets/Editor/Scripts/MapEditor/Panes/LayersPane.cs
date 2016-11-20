using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using StevenUniverse.FanGame.World;
using System.Linq;
using StevenUniverse.FanGame.Util.MapEditing;

public class LayersPane
{ 
   
    static bool foldOut_ = true;
    static bool[] toggles_ = null;
    static Map map_;

    // TODO : How do we account for Tile default sorting layers? There is a "Default" Sorting LAyer,
    // but we would have to tweak all other references to account for that matching up to the Tile's Default Sorting Oorder...
    static int targetLayer_;
    /// <summary>
    /// The target layer to which all tiles will be painted.
    /// </summary>
    public static SortingLayer TargetLayer
    {
        get
        {
            return SortingLayerUtil.GetLayerFromIndex(targetLayer_);
        }
    }

    public static void OnSceneGUI()
    {

        Handles.BeginGUI();

        var layers = SortingLayer.layers;

        // One liner per layer + target layer selection.
        int lineCount = layers.Length + 1;

        int w = 130;
        // H = dropdown + layer texts + area between texts
        int h = foldOut_ ? 18 + (lineCount * 15) + (4 * lineCount) : 22;
        int x = Screen.width - w - 15;
        int y = 15;

        //var oldColor = GUI.backgroundColor;
        //GUI.color = new Color32(33, 47, 22, 150);
        GUILayout.BeginArea(new Rect(x, y, w, h), EditorStyles.helpBox);

        foldOut_ = EditorGUILayout.Foldout(foldOut_, "Layers");

        EditorGUI.indentLevel++;
        var oldColor = GUI.contentColor;

        GUI.contentColor = Color.black;
        if (foldOut_)
        {

            for (int i = 0; i < layers.Length; ++i)
            {
                bool userToggle = EditorGUILayout.ToggleLeft(layers[i].name, toggles_[i]);

                if (userToggle != toggles_[i])
                {
                    Debug.LogFormat(layers[i].name + " TOGGLED");
                }
                toggles_[i] = userToggle;
            }

            string[] targetLayerNames = new string[layers.Length + 1];
            layers.Select(l => l.name).ToArray().CopyTo(targetLayerNames, 1);
            targetLayerNames[0] = "Default";

            targetLayer_ = EditorGUILayout.Popup(targetLayer_, targetLayerNames);
        }
        GUI.color = oldColor;

        EditorGUI.indentLevel--;
        GUILayout.EndArea();

        Handles.EndGUI();
    }

    static void OnLayerEnabled( SortingLayer layer )
    {
        map_.ShowLayer(layer);
    }

    // TODO : Load relevant editorprefs
    public static void OnEnable( Map map )
    {
        foldOut_ = true;
        int layerCount = SortingLayer.layers.Length;
        toggles_ = new bool[layerCount];
        map_ = map;
    }

    // TODO : Save relevant editorprefs
    public static void OnDisable()
    {
    }
}
