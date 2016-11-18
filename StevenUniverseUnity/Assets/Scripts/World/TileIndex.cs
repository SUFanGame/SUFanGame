using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.World
{
    /// <summary>
    /// Defines the index of a tile and can be used as a dictionary key.
    /// </summary>
    [System.Serializable]
    public struct TileIndex : System.IEquatable<TileIndex>
    {
        public IntVector3 Position_ { get; set; }

        public SortingLayer Layer_ { get; set; }

        public TileIndex(IntVector3 pos, SortingLayer layer )
        {
            Position_ = pos;
            Layer_ = layer;
        }

        public TileIndex(int x, int y, int z, SortingLayer layer) : this( new IntVector3(x,y, z), layer)
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
            return Position_ == other.Position_ && Layer_.id == other.Layer_.id;
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
            return new TileIndex(lhs.Position_ - rhs.Position_, lhs.Layer_ );
        }

        //http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + Position_.GetHashCode();
                hash = hash * 23 + Layer_.id.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("[Pos:{0}, Layer: {1}]", Position_, Layer_.name );
        }

    }
}