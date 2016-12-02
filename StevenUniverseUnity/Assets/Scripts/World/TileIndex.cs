using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util;
using SUGame.Util.MapEditing;
using SUGame.Util.Common;

namespace SUGame.World
{
    /// <summary>
    /// Defines the index of a tile within a chunk and can be used as a dictionary key.
    /// </summary>
    [System.Serializable]
    public struct TileIndex : System.IEquatable<TileIndex>
    {
        public IntVector2 position_;

        [SerializeField,HideInInspector]
        int sortingLayerValue_;
        public SortingLayer Layer_
        {
            get
            {
                return SortingLayerUtil.GetLayerFromValue(sortingLayerValue_);
            }
            set
            {
                sortingLayerValue_ = value.value;
            }
        }

        public int SortingLayerIndex_ { get { return SortingLayerUtil.GetLayerIndex(Layer_); } }

        public TileIndex(IntVector2 pos, SortingLayer layer )
        {
            position_ = pos;
            sortingLayerValue_ = layer.value;
        }

        public TileIndex(int x, int y, SortingLayer layer) : this( new IntVector2(x,y), layer)
        {}

        public override bool Equals(object obj)
        {
            if (!(obj is TileIndex))
                return false;

            TileIndex p = (TileIndex)obj;

            return this.Equals(p);
        }

        public bool Equals(TileIndex other)
        {
            return position_ == other.position_ && sortingLayerValue_ == other.sortingLayerValue_;
        }

        public static bool operator ==(TileIndex lhs, TileIndex rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(TileIndex lhs, TileIndex rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static TileIndex operator -(TileIndex lhs, TileIndex rhs)
        {
            return new TileIndex(lhs.position_ - rhs.position_, lhs.Layer_ );
        }

        //http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + position_.GetHashCode();
                hash = hash * 23 + sortingLayerValue_.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", position_.ToString("0"), Layer_.name );
        }

    }
}