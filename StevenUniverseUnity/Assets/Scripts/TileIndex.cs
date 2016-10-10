using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StevenUniverse.FanGame.Overworld.Templates;
using StevenUniverse.FanGame.OverworldEditor;
using StevenUniverse.FanGame.Overworld.Instances;

[System.Serializable]
public struct TileIndex : System.IEquatable<TileIndex>
{
    [SerializeField]
    int x_;
    [SerializeField]
    int y_;
    [SerializeField]
    int elevation_;
    [SerializeField]
    int layer_;
    
    public int X { get { return x_; } }
    public int Y { get { return y_; } }
    public int Elevation { get { return elevation_; } }
    public int Layer { get { return layer_; } }
    public Vector2 Position { get { return new Vector2(x_, y_); } }

    public TileIndex( int x, int y, int elevation, TileTemplate.Layer layer ) : this( x, y, elevation, layer.SortingValue )
    {}

    public TileIndex( int x, int y, int elevation, int layer )
    {
        x_ = x;
        y_ = y;
        elevation_ = elevation;
        layer_ = layer;

    }

    public TileIndex(Vector2 pos, int elevation, TileTemplate.Layer layer) : 
        this( Mathf.FloorToInt(pos.x), 
              Mathf.FloorToInt(pos.y), 
              elevation, layer )
    { }

    //public TileIndex( TileInstance tile ) : 
    //    this( tile.Position, tile.Elevation, tile.TileTemplate.TileLayer )
    //{}

    public override bool Equals(object obj)
    {
        if (!(obj is TileIndex))
            return false;

        TileIndex p = (TileIndex)obj;

        return this.Equals(p);
    }

    public bool Equals(TileIndex other)
    {
        return x_ == other.x_ && y_ == other.y_ && 
               elevation_ == other.elevation_ && layer_ == other.layer_;
    }

    public static bool operator == ( TileIndex lhs, TileIndex rhs )
    {
        return lhs.Equals(rhs);
    }

    public static bool operator != ( TileIndex lhs, TileIndex rhs )
    {
        return !lhs.Equals(rhs);
    }

    public static TileIndex operator - ( TileIndex lhs, TileIndex rhs )
    {
        return new TileIndex(lhs.x_ - rhs.x_, lhs.y_ - rhs.y_, lhs.elevation_ - rhs.elevation_, lhs.layer_ );
    }

    //http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + x_.GetHashCode();
            hash = hash * 23 + y_.GetHashCode();
            hash = hash * 23 + elevation_.GetHashCode();
            hash = hash * 23 + layer_.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return string.Format("Pos:{0}, Elev:{1}, Layer:{2}", Position, elevation_, Layer );
    }

    /// <summary>
    /// Gets the tile position for the given tile group. The "Layer" value is set to whatever
    /// the group's first tile's layer is.
    /// </summary>
    public static TileIndex GetGroupPosition( GroupInstanceEditor group )
    {
        return new TileIndex((int)group.Instance.Position.x, (int)group.Instance.Position.y,
            group.Elevation, group.GroupInstance.IndependantTileInstances[0].TileTemplate.TileLayer);
    }
}
