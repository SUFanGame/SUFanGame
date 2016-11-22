using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using StevenUniverse.FanGame.World;
using System.Linq;
using StevenUniverse.FanGame.Util.MapEditing;
using StevenUniverse.FanGame.Util;

// TODO : Use editorprefs to store settings
namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class LayersPanel : MapEditorPanel
    {
        public override Rect Area_
        {
            get
            {
                int lineCount = SortingLayer.layers.Length;
                int w = 130;
                int h = foldOut_ ? 18 + (lineCount * 15) + (4 * lineCount) : 22;
                int x = Screen.width - w - 15;
                int y = 15;

                return new Rect(x, y, w, h);
            }
        }

        static bool foldOut_ = true;
        static bool[] toggles_ = null;
        static Map map_;

        const string PREFS_LAYERSFOLDOUT_NAME = "MELayersFoldout";

        public LayersPanel( Map map )
        {
            map_ = map;
            foldOut_ = EditorPrefs.GetBool(PREFS_LAYERSFOLDOUT_NAME, true);
            int layerCount = SortingLayer.layers.Length;
            toggles_ = new bool[layerCount];
            map_ = map;
        }

        
        protected override void OnRenderArea()
        {
            var layers = SortingLayer.layers;

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
            }
            GUI.color = oldColor;

            EditorGUI.indentLevel--;
        }

        static void OnLayerEnabled(SortingLayer layer)
        {
            map_.ShowLayer(layer);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            EditorPrefs.SetBool(PREFS_LAYERSFOLDOUT_NAME, foldOut_ );
        }

    }
}