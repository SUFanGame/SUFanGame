using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using StevenUniverse.FanGame.World;

public class LayersPane
{ 
   
    static bool foldOut_ = true;
    static bool[] toggles_ = null;
    static Map map_;

    public static void OnSceneGUI()
    {

        Handles.BeginGUI();

        int layerCount = SortingLayer.layers.Length;

        int w = 130;
        // H = dropdown + layer texts + area between texts
        int h = foldOut_ ? 18 + (layerCount * 15) + (4 * layerCount) : 22;
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
            var layers = SortingLayer.layers;

            for (int i = 0; i < layers.Length; ++i)
            {
                bool userToggle = EditorGUILayout.ToggleLeft(layers[i].name, toggles_[i]);

                if (userToggle != toggles_[i])
                {
                    Debug.LogFormat(layers[i].name + " TOGGLED");
                }
                toggles_[i] = userToggle;
            }

        }
        GUI.color = oldColor;

        EditorGUI.indentLevel--;
        GUILayout.EndArea();

        Handles.EndGUI();
    }

    static void OnLayerEnabled( SortingLayer layer )
    {
        foreach( var chunk in map_ )
        {
            chunk.ShowLayer(layer);
        }
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
