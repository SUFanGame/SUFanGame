using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.MapEditing;

namespace StevenUniverse.FanGame.World
{
    // TODO: These may be better off as ScriptableObjects....Not sure yet.

    /// <summary>
    /// A tile. These are intended to be assets, and should not be instantiated unless per-tile-data is required.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Tile : MonoBehaviour
    {
        public Sprite Sprite_
        {
            get { return Renderer_.sprite; }
        }

        [SerializeField,HideInInspector]
        SpriteRenderer renderer_;
        public SpriteRenderer Renderer_
        {
            // Note that since we may be accessing the prefabs themselves the unity message functions (Awake, Update, etc) will never be called
            // on prefabs.
            get
            {
                if( renderer_ == null )
                {
                    Awake();
                }
                return renderer_;
            }
        }

        /// <summary>
        /// The default sorting layer for this tile. Note that tiles can be inserted
        /// in the map on ANY sorting layer, this is just the default if no layer is specified.
        /// </summary>
        public SortingLayer DefaultSortingLayer_
        {
            get
            {
                return SortingLayerUtil.GetLayerFromID(renderer_.sortingLayerID);
            }
        }

        //[Tooltip( "Used when sorting tiles, for tiles that are intended to be used only in Tile Groups")]
        //[SerializeField]
        //bool groupExclusive_ = false;
        //[SerializeField]
        //bool isGrounded_ = false;
        //[SerializeField]
        //Mode mode_;

        public enum Mode
        {
            Normal,
            Surface,
            Transitional,
            Collidable
        }
        
        void Awake()
        {
            renderer_ = GetComponent<SpriteRenderer>();
        }
    }
}
