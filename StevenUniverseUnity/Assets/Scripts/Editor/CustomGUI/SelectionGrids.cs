using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace SUGame.SUGameEditor.CustomGUI
{


    /// <summary>
    /// Selection grid draws black textures in SceneView GUI functions, so we'll have to do it manually.
    /// </summary>
    public static class SelectionGrids
    {

        static Color selectedColor_ = new Color(.25f, .25f, .25f);
        static Color buttonColor_ = Color.white;


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
        public static int FromTextures( int selected, Texture2D[] textures, int xCount, int w = 0, int h = 0 )
        {

            int i = 0;
            GUILayout.BeginVertical();
            for( int y = 0; i < textures.Length; ++y )
            {
                GUILayout.BeginHorizontal();
                for( int x = 0; x < xCount && i < textures.Length; ++x, ++i )
                {
                    var tex = textures[i];
                    //var content = new GUIContent(tex);

                    int areaWidth = w == 0 ? tex.width : w;
                    int areaHeight = h == 0 ? tex.height : h;
                    
                    var area = EditorGUILayout.GetControlRect(
                        GUILayout.Width(areaWidth), 
                        GUILayout.Height(areaHeight));

                    var oldColor = GUI.color;
                    //GUI.DrawTexture(area, tex);
                    if( i == selected )
                    {
                        GUI.color = buttonColor_;
                    }
                    else
                    {
                        GUI.color = selectedColor_;
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


        // TODO : Clean this up a bit, it's a mess right now. Too many magic numbers and the horizontal spacing is way off 
        // - for wide windows it leaves a huge gap on the right.
        public static int FromSprites(
            int selectedID,
            List<Sprite> sprites,
            float scrollViewImageSize,
            float scrollViewHeight,
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

                    if (sprite == null)
                        continue;

                    var toggleRect = EditorGUILayout.GetControlRect(GUILayout.Width(scrollViewImageSize), GUILayout.Height(scrollViewImageSize));
                    var imageRect = toggleRect;

                    imageRect.position = toggleRect.position;
                    imageRect.size = toggleRect.size - Vector2.one * 2f;


                    var oldColor = GUI.color;
                    GUI.color = selectedColor_;

                    var toggleState = GUI.Toggle(toggleRect, i == selectedID, "", GUI.skin.button);

                    if (i == selectedID)
                        GUI.color = buttonColor_;

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

        /// <summary>
        /// Draw the asset preview texture for each of the given assets in a selectable grid.
        /// </summary>
        /// <param name="selected">The index of the selected asset.</param>
        /// <param name="assets">List of assets.</param>
        /// <param name="scrollPos">The scroll position of the scroll view containing the grid.</param>
        /// <returns></returns>
        public static int DrawAssetPreviewGrid(int selected, List<GameObject> assets, ref Vector2 scrollPos)
        {
            int i = 0;

            // Note : asset previews textures always seem to be 128x128
            int cellCountX = Mathf.FloorToInt((float)Screen.width / 128f);
            int cellCountY = Mathf.CeilToInt((float)assets.Count / (float)cellCountX);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();
            for (int y = 0; y < cellCountY && i < assets.Count; ++y)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < cellCountX && i < assets.Count; ++x)
                {
                    // GetAssetPreview runs asynchronously and seems to destroy and recreate the textures
                    // arbitrarily (meaning they can't be cached). Best way seems to be to block until we 
                    // get a valid texture. Seems to perform alright, it might explode
                    // if it's trying to draw a LOT of tile groups???
                    Texture2D tex = null;
                    while (tex == null)
                    {
                        tex = AssetPreview.GetAssetPreview(assets[i]);
                    }

                    var area = EditorGUILayout.GetControlRect(GUILayout.Width(tex.width), GUILayout.Height(tex.height));


                    var oldColor = GUI.color;
                    GUI.color = buttonColor_;

                    bool isToggled = GUI.Toggle(area, i == selected, "", GUI.skin.button);

                    if (isToggled)
                    {
                        GUI.color = selectedColor_;
                    }

                    GUI.DrawTexture(area, tex);

                    if (isToggled != (i == selected))
                        selected = i;

                    GUI.color = oldColor;
                    ++i;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            return selected;
        }

        public static void DrawSprite(Sprite sprite, float imageSize)
        {
            var texCoords = GetNormalizedSpriteRect(sprite);

            var rect = EditorGUILayout.GetControlRect(GUILayout.Width(imageSize), GUILayout.Height(imageSize));
            GUI.DrawTextureWithTexCoords(rect, sprite.texture, texCoords);

        }

        public static Rect GetNormalizedSpriteRect(Sprite sprite)
        {
            if (sprite == null)
                return default(Rect);

            var tex = sprite.texture;
            var spriteRect = sprite.textureRect;
            var normalizedRect = spriteRect;

            normalizedRect.x /= tex.width;
            normalizedRect.y /= tex.height;

            normalizedRect.width /= tex.width;
            normalizedRect.height /= tex.height;
            return normalizedRect;
        }

        public static void DrawSprite(Rect rect, Sprite sprite, bool horizontalFlip = false, bool verticalFlip = false)
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
