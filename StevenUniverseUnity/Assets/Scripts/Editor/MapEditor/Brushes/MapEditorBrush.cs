using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util;
using SUGame.World;
using System.IO;
using System.Linq;
using UnityEditor;
using SUGame.Util.Common;

namespace SUGame.SUGameEditor.MapEditing
{
    public abstract class MapEditorBrush
    {

        public virtual string Name_ { get { return "Map Editor Brush"; } }
        /// <summary>
        /// The file name of the brush's icon texture (without the file type), in "Assets/Editor/MapEditorIcons/".
        /// </summary>
        protected abstract string IconTextureName_ { get; }
        public Texture2D BrushIconTexture_
        {
            get
            {
                if (!brushTextures_.ContainsKey(IconTextureName_))
                    return null;
                return brushTextures_[IconTextureName_];
            }
        }

        static string IconTexturePath_ = "Textures/MapEditorIcons";
        static Dictionary<string, Texture2D> brushTextures_ = null;


        public MapEditorBrush()
        {
            //cursorPoints_.Add(IntVector2.zero);

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
        public virtual void OnMouseDown( Map map, IntVector3 pos )
        {
            //for( int i = 0; i < cursorPoints_.Count; ++i )
            //{
            //    IntVector3 targetPoint = pos + (IntVector3)cursorPoints_[i];

            //    AffectMapTile(map, targetPoint);

            //}
        }

        /// <summary>
        /// Callback for when the mouse is being clicked in the scene view. By default this will be called for position
        /// within this brush's size, the brush can override <seealso cref="OnMouseDown(Map, IntVector3)"/> to change this
        /// behaviour.
        /// </summary>
        /// <param name="map">The map being affected.</param>
        /// <param name="localPos">The target point in world space.</param>
        protected virtual void AffectMapTile( Map map, IntVector3 worldPos)
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

        public virtual void OnMouseUp( Map map, IntVector3 worldPos )
        {

        }

        public virtual void OnKeyDown( KeyCode key )
        {

        }

        /// <summary>
        /// Callback for when the mouse wheel is scrolled in the scene view.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="scrollValue"></param>
        public virtual void OnScroll( Map map, float scrollValue )
        {
            //var e = Event.current;

            //if (e.shift)
            //    e.Use();

            //if (scrollValue < 0)
            //{
            //    //Size_++;
            //}
            //else if (scrollValue > 0)
            //{
            //    //Size_--;
            //}
        }

        public virtual void RenderCursor( Map map )
        {

            //var cursorPos = SceneEditorUtil.GetCursorPosition();

            //var offset = Vector2.one * .5f;
            //foreach( var pos in cursorPoints_ )
            //{
            //    Gizmos.DrawWireCube(cursorPos + pos + Vector3.back + (Vector3)offset, Vector3.one);
            //}
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