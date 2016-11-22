using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.Util.MapEditing;

namespace StevenUniverse.FanGame.World
{
    /// <summary>
    /// Defines the index of a tile within a chunk and can be used as a dictionary key.
    /// </summary>
    [System.Serializable]
    public struct TileIndex : System.IEquatable<TileIndex>
    {
        public IntVector2 position_;

        public SortingLayer layer_;

        public int SortingLayerIndex_ { get { return SortingLayerUtil.GetLayerIndex(layer_); } }

        public TileIndex(IntVector2 pos, SortingLayer layer )
        {
            position_ = pos;
            layer_ = layer;
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
            return position_ == other.position_ && layer_.id == other.layer_.id;
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
            return new TileIndex(lhs.position_ - rhs.position_, lhs.layer_ );
        }

        //http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + position_.GetHashCode();
                hash = hash * 23 + layer_.id.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("[Pos:{0}, Layer: {1}]", position_, layer_.name );
        }

    }
}