using UnityEngine;
using StevenUniverse.FanGame.Overworld.Templates;
using StevenUniverse.FanGame.StrategyMap;


/// <summary>
/// Defines the index of a tile and can be used as a dictionary key.
/// </summary>
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
    
    /// <summary>
    /// Tile's x position.
    /// </summary>
    public int X { get { return x_; } }
    /// <summary>
    /// Tile's y position.
    /// </summary>
    public int Y { get { return y_; } }
    /// <summary>
    /// Tile's elevation.
    /// </summary>
    public int Elevation { get { return elevation_; } }

    public IntVector2 Position { get { return new IntVector2(x_, y_); } }

    /// <summary>
    /// Tile's layer. Made an int since Dust's "Enhanced Enums" (custom layers) are not serializable.
    /// </summary>
    public int Layer { get { return layer_; } }

    public TileIndex( IntVector2 pos, int elevation, TileTemplate.Layer layer ) : this( pos, elevation, layer.SortingValue )
    {}

    public TileIndex(IntVector2 pos, int elevation, int layer) : this( pos.x, pos.y, elevation, layer )
    {}


    public TileIndex( int x, int y, int elevation, TileTemplate.Layer layer ) : this( x, y, elevation, layer.SortingValue )
    {}

    public TileIndex( int x, int y, int elevation, int layer )
    {
        x_ = x;
        y_ = y;
        elevation_ = elevation;
        layer_ = layer;

    }

    /// <summary>
    /// Construct a tile index from a vector2. Position will be floored.
    /// </summary>
    public TileIndex(Vector2 pos, int elevation, TileTemplate.Layer layer) : 
        this( Mathf.FloorToInt(pos.x), 
              Mathf.FloorToInt(pos.y), 
              elevation, layer )
    { }

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
        return new TileIndex(lhs.Position - rhs.Position, lhs.elevation_ - rhs.elevation_, lhs.layer_ );
    }

    //http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            hash = hash * 23 + Position.GetHashCode();
            hash = hash * 23 + elevation_.GetHashCode();
            hash = hash * 23 + layer_.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return string.Format("Pos:{0}, Elev:{1}, Layer:{2}", Position, elevation_, Layer );
    }

}
