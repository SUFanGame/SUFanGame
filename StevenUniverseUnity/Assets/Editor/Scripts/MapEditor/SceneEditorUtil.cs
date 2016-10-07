using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class SceneEditorUtil
    {
        /// <summary>
        /// Draws a cursor of the given size and returns the cursor's position, snapped to the 1x1 grid
        /// </summary>
        /// <returns></returns>
        public static Vector3 DrawCursor( float cursorSize = 1f )
        {

            var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var mousePoint = mouseRay.GetPoint(0);
            for( int i = 0; i < 2; ++i )
            {
                mousePoint[i] = Mathf.Floor(mousePoint[i]);
            }

            mousePoint[2] = 0;

            var offset = Vector2.one * .5f;
            Gizmos.DrawWireCube(mousePoint + Vector3.back + (Vector3)offset, Vector3.one * cursorSize);

            SceneView.currentDrawingSceneView.Repaint();

            return mousePoint;
        }


        /// <summary>
        /// Property to control or poll whether we're editing in the scene view
        /// </summary>
        public static bool EditMode_
        {
            // This is how Unity's terrain editor does it - when you
            // enter terrain editing mode it deselects tools
            // and prevents other selections in the scene view
            // until another tool is selected or until
            // the terrain is deselected via the hierarchy
            get { return UnityEditor.Tools.current == Tool.None; }
            set
            {
                if (value)
                    UnityEditor.Tools.current = Tool.None;
                else
                    UnityEditor.Tools.current = Tool.Move;
            }
        }
        
        // TODO : Clean this up a bit, it's a mess right now. Too many magic numbers and the horizontal spacing is way off 
        // - for wide windows it leaves a huge gap on the right.
        public static int DrawSpriteGrid(
            int selectedID,
            List<Sprite> sprites,
            float scrollViewImageSize,
            float scrollViewHeight,
            Color buttonColor,
            Color selectedColor,
            ref Vector2 scrollViewPos)
        {
            if (sprites == null || sprites.Count == 0)
                return selectedID;

            int columnCount = Mathf.FloorToInt(Screen.width / (scrollViewImageSize + 13));

            int rowCount = Mathf.CeilToInt((float)sprites.Count / (float)columnCount);

            scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos, GUILayout.MinHeight(scrollViewHeight));


            int i = 0;
            EditorGUILayout.BeginVertical();
            for (int y = 0; y < rowCount && i < sprites.Count; ++y)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < columnCount && i < sprites.Count; ++x)
                {
                    var sprite = sprites[i];

                    var toggleRect = EditorGUILayout.GetControlRect(GUILayout.Width(scrollViewImageSize), GUILayout.Height(scrollViewImageSize));
                    var imageRect = toggleRect;

                    imageRect.position = toggleRect.position;
                    imageRect.size = toggleRect.size - Vector2.one * 2f;


                    var oldColor = GUI.color;
                    GUI.color = buttonColor;

                    var toggleState = GUI.Toggle(toggleRect, i == selectedID, "", GUI.skin.button);

                    if (i == selectedID)
                        GUI.color = selectedColor;

                    if (toggleState != (i == selectedID))
                        selectedID = i;

                    var texCoords = GetNormalizedSpriteRect(sprite);
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

        public static void DrawSprite(Sprite sprite, float imageSize )
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

        public static void DrawSprite( Rect rect, Sprite sprite, bool horizontalFlip = false, bool verticalFlip = false )
        {
            var texCoords = GetNormalizedSpriteRect(sprite);

            if (verticalFlip)
            {
                float y = texCoords.yMin;
                texCoords.yMin = texCoords.yMax;
                texCoords.yMax = y;
            }

            GUI.DrawTextureWithTexCoords(rect, sprite.texture, texCoords);
        }
    }
}
 