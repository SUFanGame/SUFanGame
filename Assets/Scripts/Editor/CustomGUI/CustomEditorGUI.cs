﻿using UnityEngine;
using UnityEditor;
using System.Linq;
using SUGame.Util.MapEditing;

namespace SUGame.SUGameEditor.CustomGUI
{
    public class CustomEditorGUI
    {
        public static int DrawSpriteGrid(
            int selectedID,
            Sprite[] sprites,
            float scrollViewImageSize,
            float scrollViewHeight,
            Color buttonColor,
            Color selectedColor,
            ref Vector2 scrollViewPos)
        {
            if (sprites == null || sprites.Length == 0)
                return selectedID;

            int columnCount = Mathf.FloorToInt(Screen.width / (scrollViewImageSize + 13));

            int rowCount = Mathf.CeilToInt((float)sprites.Length / (float)columnCount);

            scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos, GUILayout.MinHeight(scrollViewHeight));


            int i = 0;
            EditorGUILayout.BeginVertical();
            for (int y = 0; y < rowCount && i < sprites.Length; ++y)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < columnCount && i < sprites.Length; ++x)
                {
                    var sprite = sprites[i];

                    var toggleRect = EditorGUILayout.GetControlRect(GUILayout.Width(scrollViewImageSize), GUILayout.Height(scrollViewImageSize));
                    var imageRect = toggleRect;

                    imageRect.position = toggleRect.position;
                    imageRect.size = toggleRect.size - Vector2.one * 2f;

                    var texCoords = GetNormalizedSpriteRect(sprite);

                    var oldColor = GUI.color;
                    GUI.color = buttonColor;

                    var toggleState = GUI.Toggle(toggleRect, i == selectedID, "", GUI.skin.button);

                    if (i == selectedID)
                        GUI.color = selectedColor;

                    if (toggleState != (i == selectedID))
                        selectedID = i;



                    GUI.DrawTextureWithTexCoords(imageRect, sprite.texture, texCoords);
                    GUI.Label(imageRect, i.ToString());

                    ++i;

                    GUI.color = oldColor;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();

            return selectedID;
        }

        public static void DrawSprite(Sprite sprite, float imageSize)
        {
            var texCoords = GetNormalizedSpriteRect(sprite);
            var rect = EditorGUILayout.GetControlRect(GUILayout.Width(imageSize), GUILayout.Height(imageSize));
            GUI.DrawTextureWithTexCoords(rect, sprite.texture, texCoords);

        }

        public static Rect GetNormalizedSpriteRect(Sprite sprite)
        {
            var tex = sprite.texture;
            var spriteRect = sprite.textureRect;
            var normalizedRect = spriteRect;

            normalizedRect.x /= tex.width;
            normalizedRect.y /= tex.height;

            normalizedRect.width /= tex.width;
            normalizedRect.height /= tex.height;
            return normalizedRect;
        }

        /// <summary>
        /// Draws the script field shown on default inspectors.
        /// </summary>
        public static void DrawScriptField( SerializedObject serializedObject )
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
        }
    }

}
