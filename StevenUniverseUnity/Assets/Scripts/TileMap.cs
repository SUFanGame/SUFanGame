using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StevenUniverse.FanGame.OverworldEditor;
using System.Linq;
using StevenUniverse.FanGame.Battle;

// TODO : Now that we've simplified this it can be made generic pretty easily, serializable parts will survive if then derived to
// a serializable class (IE: How UnityActions work)

/// <summary>
/// Collection mapping editor instances to a position. Each position can hold multiple tiles of different layers. No single position should have
/// two tiles of the same layer. Accepts either individual tiles (TileInstanceEditor) or Tile Groups (GroupInsanceEditor).
/// Note the "Z" value of the key refers to the "Elevation" of the tile
/// </summary>
[System.Serializable]
public class TileMap<T> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TileIndex, T>>
{

    [SerializeField]
    List<TileIndex> serializedKeys_ = new List<TileIndex>();

    [SerializeField]
    List<T> serializedValues_ = new List<T>();
    
    // Dictionary mapping tiles to their index based on position, layer and elevation
    Dictionary<TileIndex, T> dict_ = new Dictionary<TileIndex, T>();

    // Dictionary mapping lists of tiles to their 3d position (X,Y,ELEVATION)
    Dictionary<IntVector3, List<T>> positionDict_ = new Dictionary<IntVector3, List<T>>();

    public TileMap() { }

    /// <summary>
    /// Add an instance to the given position. The instance can be either a group or an individual tile, but it should NOT
    /// be a prefab.
    /// </summary>
    public void AddInstance( TileIndex index, T tile )
    {
        dict_[index] = tile;
        AddToList(new IntVector3(index.X, index.Y, index.Elevation), tile);
    }

    /// <summary>
    /// Remove the instance given position if it matches the given layer. Returns the removed instance, or null if no instance was found.
    /// Note that in the case of groups this will remove ALL REFERENCES TO THAT GROUP from the map, then return the group instance.
    /// </summary>
    public InstanceEditor RemoveAt( TileIndex pos )
    {
        if (!dict_.ContainsKey(pos))
            return null;

        var instance = dict_[pos];

        if (instance is TileInstanceEditor)
        {
            var tile = instance as TileInstanceEditor;
            dict_.Remove(pos);
            return tile;
        }
        else
        {
            var group = instance as GroupInstanceEditor;

            // Can't use these as tiletemplate references are null, which is the only way to
            // access tile layer information from a tile instance
            //var tiles = group.GroupInstance.IndependantTileInstances;

            var indices = GroupTileIndices(group);

            // Remove the reference to our group from each of the group's cells
            foreach( var index in indices )
            {
                dict_.Remove(index);
            }

            return group;
        }
    }

    public T Get( TileIndex index )
    {
        T t;

        dict_.TryGetValue(index, out t);

        return t;
    }

    public void Clear()
    {
        dict_.Clear();
    }

    public void OnBeforeSerialize()
    {
        serializedKeys_.Clear();
        serializedValues_.Clear();

        foreach( var pair in dict_ )
        {
            //Debug.LogFormat("Serializing key {0}", pair.Key);
            serializedKeys_.Add(pair.Key);
            serializedValues_.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dict_.Clear();
        for( int i = 0; i < serializedKeys_.Count; ++i )
        {
            //Debug.LogFormat("Deserializing key {0}", serializedKeys_[i].ToString(), serializedKeys_[i].GetHashCode());
            dict_.Add(serializedKeys_[i], serializedValues_[i]);
        }
    }

    /// <summary>
    /// Build a list of WORLD indices for each tile of the given tile group.
    /// </summary>
    public static List<TileIndex> GroupTileIndices(GroupInstanceEditor group)
    {
        List<TileIndex> tileIndices = new List<TileIndex>();

        var instances = group.GetComponentsInChildren<TileInstanceEditor>();

        foreach (var tile in instances)
        {
            tileIndices.Add(new TileIndex(
                tile.transform.position,
                //Tile instance elevations in a group are relative to the group's elevation.
                tile.Elevation + group.Elevation,
                tile.TileInstance.TileTemplate.TileLayer));
        }

        return tileIndices;
    }

    void AddToList( IntVector3 pos, T value )
    {
        if( !positionDict_.ContainsKey(pos) )
        {
            positionDict_[pos] = new List<T>();
        }

        positionDict_[pos].Add(value);
    }

    public IEnumerator<KeyValuePair<TileIndex, T>> GetEnumerator()
    {
        return dict_.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
