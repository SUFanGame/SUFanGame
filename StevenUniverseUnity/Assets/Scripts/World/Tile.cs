using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.MapEditing;

namespace SUGame.World
{
    // TODO: These may be better off as ScriptableObjects....Not sure yet.

    /// <summary>
    /// A tile. These are intended to be assets, and should not be instantiated unless per-tile-data is required.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        Mode mode_ = Mode.Normal;
        public Mode Mode_ { get { return mode_; } }

        [SerializeField]
        bool isGrounded_ = false;
        /// <summary>
        /// Grounded tiles prevent pathabilty on tiles below them (Elevation wise).
        /// </summary>
        public bool IsGrounded_ { get { return isGrounded_; } }

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
        public enum Mode
        {
            // Normal tiles don't affect pathability
            Normal,
            // Surface tiles are considered pathable if they're at the top of a tile stack.
            Surface,
            // Transitional tiles are pathable and allow pathing to move up or down one level into adjacent nodes if
            // they're at the top of a tile stack.
            Transitional,
            // Collidable tiles prevent pathing if they're at the top of a tile stack.
            Collidable
        }
        
        void Awake()
        {
            renderer_ = GetComponent<SpriteRenderer>();
        }
    }
}
