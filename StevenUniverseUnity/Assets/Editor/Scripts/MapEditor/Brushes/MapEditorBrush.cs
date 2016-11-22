using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;
using System.IO;
using System.Linq;
using UnityEditor;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public abstract class MapEditorBrush
    {
        int size_;
        /// <summary>
        /// The size of the cursor
        /// </summary>
        public virtual int Size_
        {
            get
            {
                return size_;
            }
            set
            {
                value = Mathf.Clamp(value, 0, int.MaxValue);

                //if (value == size_)
                //return;

                cursorPoints_.Clear();
                // 0 == one tile
                if ( value == 0 )
                {
                    size_ = 0;
                    cursorPoints_.Add(IntVector2.zero);
                    return;
                }

                // If the size is greater than 0 then we'll cache our points list so we can quickly apply
                // the effect to our target area.

                size_ = value;

                int iterSize = size_ * 2 + 1;

                IntVector2 half = (IntVector2.one * iterSize) / 2;

                for (int x = -half.x; x < half.x + 1; ++x)
                {
                    for (int y = -half.y; y < half.y + 1; ++y)
                    {
                        cursorPoints_.Add( new IntVector2(x, y) );
                    }

                }
            }
        }

        // List of points affected by our cursor.
        List<IntVector2> cursorPoints_ = new List<IntVector2>();

        public virtual string Name_ { get { return "Map Editor Brush"; } }
        /// <summary>
        /// The file name of the brush's icon texture (without the file type), in "Assets/Editor/MapEditorIcons/".
        /// </summary>
        protected abstract string IconTextureName_ { get; }
        public Texture2D Texture_
        {
            get
            {
                if (!brushTextures_.ContainsKey(IconTextureName_))
                    return null;
                return brushTextures_[IconTextureName_];
            }
        }

        static string IconTexturePath_ = "Editor/MapEditorIcons";
        static Dictionary<string, Texture2D> brushTextures_ = null;


        public MapEditorBrush()
        {
            cursorPoints_.Add(IntVector2.zero);

            if( brushTextures_ == null )
            {
                brushTextures_ = new Dictionary<string, Texture2D>();

                var textures = IOUtil.GetAssetsAtLocation<Texture2D>(IconTexturePath_, "*.png");

                foreach( var tex in textures)
                {
                    brushTextures_.Add(tex.name, tex);
                }
            }
        }
        
        /// <summary>
        /// Called by the map when the mouse is clicked
        /// </summary>
        public void OnMouseDown( Map map, IntVector3 pos )
        {
            for( int i = 0; i < cursorPoints_.Count; ++i )
            {
                IntVector3 targetPoint = pos + (IntVector3)cursorPoints_[i];

                OnClick(map, targetPoint);

            }
        }

        /// <summary>
        /// Callback for when the mouse is being clicked in the scene view.
        /// </summary>
        /// <param name="map">The map being affected.</param>
        /// <param name="localPos">The target point in world space..</param>
        protected virtual void OnClick( Map map, IntVector3 worldPos)
        {
        }

        /// <summary>
        /// Callback for when the mouse is being dragged in the scene view.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="worldPos"></param>
        public virtual void OnDrag( Map map, IntVector3 worldPos)
        {

        }

        /// <summary>
        /// Callback for when the mouse wheel is scrolled in the scene view.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="scrollValue"></param>
        public virtual void OnScroll( Map map, float scrollValue )
        {

        }

        public virtual void RenderCursor()
        {

            var cursorPos = SceneEditorUtil.GetCursorPosition();

            var offset = Vector2.one * .5f;
            foreach( var pos in cursorPoints_ )
            {
                Gizmos.DrawWireCube(cursorPos + pos + Vector3.back + (Vector3)offset, Vector3.one);
            }
        }

        /// <summary>
        /// If a brush needs to save any relevant data to editor prefs it should do so here.
        /// </summary>
        public virtual void OnDisable()
        {

        }

        /// <summary>
        /// Determines what appears in the map editor GUI. For Tile Painting this would be tiles, for tile group painting, tile groups.
        /// </summary>
        public virtual void MapEditorGUI()
        {

        }

    }
}