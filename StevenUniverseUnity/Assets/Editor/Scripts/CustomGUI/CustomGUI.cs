using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace StevenUniverse.FanGameEditor.CustomGUI
{


    /// <summary>
    /// Selection grid draws black textures in SceneView GUI functions, so we'll have to do it manually.
    /// </summary>
    public static class SceneGUISelectionGrid
    {
        /// <summary>
        /// Draw a selection grid of the given textures. Although GUILayout already has a selectiongrid class
        /// it doesn't draw properly in the scene view, for whatever reason. This one does.
        /// </summary>
        /// <param name="selected">The selected texture.</param>
        /// <param name="textures">Array of textures to draw.</param>
        /// <param name="xCount">How many columns the grid should have.</param>
        /// <param name="w">Optional width for each texture in the grid. 
        /// If 0, the actual texture width will be used.</param>
        /// <param name="h">Optional height for each texture in the grid. 
        /// If 0, the actual texture height will be used.</param>
        /// <returns></returns>
        public static int Draw( int selected, Texture2D[] textures, int xCount, int w = 0, int h = 0 )
        {

            int i = 0;
            GUILayout.BeginVertical();
            for( int y = 0; i < textures.Length; ++y )
            {
                GUILayout.BeginHorizontal();
                for( int x = 0; x < xCount && i < textures.Length; ++x, ++i )
                {
                    var tex = textures[i];
                    var content = new GUIContent(tex);

                    int areaWidth = w == 0 ? tex.width : w;
                    int areaHeight = h == 0 ? tex.height : h;
                    
                    var area = EditorGUILayout.GetControlRect(
                        GUILayout.Width(areaWidth), 
                        GUILayout.Height(areaHeight));

                    var oldColor = GUI.color;
                    //GUI.DrawTexture(area, tex);
                    if( i == selected )
                    {
                        GUI.color = Color.green;
                    }
                    bool picked = GUI.Toggle(area, selected == i, "", GUI.skin.button );

                    GUI.DrawTexture(area, tex);

                    if (picked)
                        selected = i;

                    GUI.color = oldColor;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            return selected;
        }

    }
}
