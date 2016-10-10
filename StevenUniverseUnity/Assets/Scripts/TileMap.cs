using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StevenUniverse.FanGame.OverworldEditor;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

// TODO : Change position paramters to Vector2 and have a third "Elevation" parameter 
//        to mirror how elevation works within the framework.

/// <summary>
/// Collection mapping editor instances to a position. Each position can hold multiple tiles of different layers. No single position should have
/// two tiles of the same layer. Accepts either individual tiles (TileInstanceEditor) or Tile Groups (GroupInsanceEditor).
/// Note the "Z" value of the key refers to the "Elevation" of the tile
/// </summary>
[System.Serializable]
public class EditorInstanceMap : ISerializationCallbackReceiver//, IEnumerable<KeyValuePair<Vector3,List<InstanceEditor>>>
{
    //[System.Serializable]
    //class TileListWrapper 
    //{
    //    public List<InstanceEditor> list_;
    //}

    //[SerializeField]
    //List<TileListWrapper> serializedValues_ = null;

    //[SerializeField]
    //List<TilePosition> serializedKeys_ = null;

    [SerializeField]
    List<TileIndex> serializedKeys_ = new List<TileIndex>();

    [SerializeField]
    List<InstanceEditor> serializedValues_ = new List<InstanceEditor>();

    //Dictionary<TilePosition, List<InstanceEditor>> dict_ = new Dictionary<TilePosition, List<InstanceEditor>>();
    Dictionary<TileIndex, InstanceEditor> dict_ = new Dictionary<TileIndex, InstanceEditor>();

    public EditorInstanceMap() { }

    //public EditorInstanceMap( InstanceEditor[] instances )
    //{
    //    for( int i = 0; i < instances.Length; ++i )
    //    {
    //        AddInstance(instances[i].transform.position, instances[i]);
    //    }
    //}


    /// <summary>
    /// Add an instance to the given position. The instance can be either a group or an individual tile, but it should NOT
    /// be a prefab.
    /// </summary>
    public void AddInstance( TileIndex pos, InstanceEditor instance )
    {
        //if( !dict_.ContainsKey(pos) )
        //{
        //    dict_[pos] = new List<InstanceEditor>();
        //}

        //dict_[pos].Add(instance);

        //Debug.LogFormat("Adding {0} at {1}, HashCode: {2}", instance.name, pos, pos.GetHashCode());

        dict_[pos] = instance;
    }

    ///// <summary>
    ///// Remove the given instance from the map. Note in the case of group instances, all tiles of the group will be removed from the map.
    ///// </summary>
    //public void RemoveInstance(Vector2 pos, int elevation, InstanceEditor instance )
    //{
    //    Vector3 key = new Vector3(pos.x, pos.y, elevation);
    //    if (instance is TileInstanceEditor)
    //    {
    //        dict_.Remove(key);
    //    }
    //    else
    //    {
    //        var group = instance as GroupInstanceEditor;
    //        var tiles = group.GroupInstance.IndependantTileInstances;
    //        for( int i = 0; i < tiles.Length; ++i )
    //        {
    //            Vector2 tilePosition = tiles[i].Position;
    //            int tileElevation = tiles[i].Elevation;
    //            dict_.Remove( new Vector3( tilePosition.x, tilePosition.y, tileElevation ) );
    //        }
    //    }
    //}

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

            var tiles = group.GroupInstance.IndependantTileInstances;

            if (tiles == null)
                Debug.LogFormat("TILES IS NULL");

            // Remove the reference to our group from each of the group's cells
            foreach( var t in tiles )
            {
                if (t == null)
                    Debug.LogFormat("T IS NULL");

                if (t.TileTemplate == null)
                    Debug.LogFormat("TILE TEMPLATE IS NULL");

                var index = new TileIndex(t.Position, t.Elevation, t.TileTemplate.TileLayer);

                if( index == null )
                {

                }

                dict_.Remove(index);
            }

            return group;
        }

        //// Get a list of all instances at the given position.
        //var list = GetInstances( position, elevation );
        //if (list == null)
        //    return null;

        //// Iterate through each tile at this location to see if it matches our target layer.
        //for( int i = 0; i < list.Count; ++i )
        //{
        //    // The instance at this position. Could be a group or a tile.
        //    var instance = list[i];

        //    TileInstanceEditor instanceEditor = null;
        //    instanceEditor = instance as TileInstanceEditor;
        //    // If it's not a tile, then it's a group.
        //    if (instanceEditor == null)
        //    {
        //        var group = instance as GroupInstanceEditor;
        //        // Check the layer of the group's tile at this position
        //        // Get the position and elevation for the group tile at our cursor
        //        var targetPos = position - (Vector2)group.GroupInstance.Position;
        //        int targetElevation = elevation - group.Elevation;
       
        //        //Debug.LogFormat("Checking group {0}'s tile at {1}", group.name, groupTilePos);
        //        // Check if the group tile's layer matches our target layer. If it's not a match then this tile is not conflicting
        //        if (group.GetTile(targetPos, targetElevation).TileInstance.TileTemplate.TileLayer != layer)
        //            continue;
                
        //        // If we've determined the group contains a tile that's in our target tile then we need to iterate
        //        // through each tile of that group and remove the group reference from the world
        //        // Get the world instances for all of the group's tiles. This will give us easy access to world positions.
        //        var instances = group.GroupInstance.IndependantTileInstances;
        //        for( int j = 0; j < instances.Length; ++j )
        //        {
        //            var tile = instances[i];
        //            // Get the world data for the tile. This position and elevation is where the map has stored our group reference.
        //            Vector2 tilePosition = tile.Position;
        //            int tileElevation = tile.Elevation;

        //            //Debug.LogFormat("Retrieving list of tiles in world at {0}, Elevation: {1}", tilePosition, tileElevation );
        //            // Get the list of instances at our tile's position
        //            var tiles = GetInstances( tilePosition, tileElevation );
        //            // Remove the group reference from that position.
        //            tiles.Remove(group);
        //        }

        //        // At this point all references to our group have been removed from the map.
        //        return group;
        //    }

        //    var existingLayer = instanceEditor.TileInstance.TileTemplate.TileLayer;
        //    var existingElevation = instanceEditor.TileInstance.Elevation;

        //    Debug.LogFormat("Checking existing tile {0} at {1} : Layer {2}. against new tile layer {3}", 
        //        instanceEditor.name, instanceEditor.TileInstance.Position, 
        //        existingLayer.Name, layer.Name );

        //    // Otherwise we're dealing with a tile instance. Check our layer...
        //    if ( existingLayer == layer)
        //    {
        //        // If it's a match, remove the tile and return it.
        //        list.RemoveAt(i);
        //        return instanceEditor;
        //    }
        //}

        //// Otherwise there was no match!
        //return null;
    }
    

    ///// <summary>
    ///// Gets the list of all instances at the given position.
    ///// </summary>
    //public List<InstanceEditor> GetInstances( TilePosition pos )
    //{
    //    var key = new Vector3(pos.x, pos.y, elevation);
    //    List<InstanceEditor> list;
    //    if (!dict_.TryGetValue(key, out list))
    //        return null;

    //    return list;
    //}

    ///// <summary>
    ///// Gets the tile at the given location matching the given layer. Returns null if no match was found.
    ///// </summary>
    //public TileInstanceEditor GetTile( Vector2 pos, int elevation, StevenUniverse.FanGame.Overworld.Templates.TileTemplate.Layer layer )
    //{
    //    // Get a list of all tiles at the given location.
    //    var instances = GetInstances(pos, elevation);
    //    if (instances != null )
    //    {
    //        for (int i = 0; i < instances.Count; ++i)
    //        {
    //            var instance = instances[i];
    //            TileInstanceEditor instanceEditor = null;
    //            // Check first if the instance is a tile...
    //            instanceEditor = instance as TileInstanceEditor;
    //            // If not, we can conclude it's a group and get the tile from group
    //            if (instanceEditor == null)
    //            {
    //                var group = instance as GroupInstanceEditor;

    //                // Convert world space to local space
    //                pos -= (Vector2)group.Instance.Position;
    //                elevation -= group.Elevation;
    //                instanceEditor = group.GetTile(pos, elevation);
    //            }

    //            if (instanceEditor.TileInstance.TileTemplate.TileLayer == layer)
    //            {
    //                return instanceEditor;
    //            }
    //        }
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// Gets the count of all tiles at the given position.
    ///// </summary>
    //public int GetTileCount( Vector2 pos, int elevation )
    //{
    //    Vector3 key = new Vector3(pos.x, pos.y, elevation);
    //    List<InstanceEditor> list;
    //    if (!dict_.TryGetValue(key, out list))
    //        return 0;

    //    return list.Count;
    //}

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


    ///// <summary>
    ///// Refresh the state of the map, removing any destroyed tile references.
    ///// </summary>
    //public void Refresh()
    //{
    //    var enumerator = dict_.GetEnumerator();
    //    while( enumerator.MoveNext() )
    //    {
    //        var pos = enumerator.Current.Key;
    //        var list = enumerator.Current.Value;

    //        for( int i = list.Count - 1; i >= 0; --i )
    //        {
    //            if (list[i] == null)
    //            {
    //                Debug.LogFormat("Removing null tile at {0}", pos);
    //                list.RemoveAt(i);
    //            }
    //        }
    //    }
    //}

    //public void OnBeforeSerialize()
    //{
    //    if (serializedKeys_ == null)
    //        serializedKeys_ = new List<Vector3>();

    //    if (serializedValues_ == null)
    //        serializedValues_ = new List<TileListWrapper>();

    //    serializedKeys_.Clear();
    //    serializedValues_.Clear();

    //    foreach( var pair in dict_ )
    //    {
    //        serializedKeys_.Add(pair.Key);
    //        serializedValues_.Add(new TileListWrapper() { list_ = pair.Value });
    //    }
    //}

    //public void OnAfterDeserialize()
    //{
    //    for( int i = 0; i < serializedKeys_.Count; ++i )
    //    {
    //        dict_.Add(serializedKeys_[i], serializedValues_[i].list_);
    //    }
    //}

    //public IEnumerator<KeyValuePair<Vector3, List<InstanceEditor>>> GetEnumerator()
    //{
    //    return dict_.GetEnumerator();
    //}

    //IEnumerator IEnumerable.GetEnumerator()
    //{
    //    return this.GetEnumerator();
    //}

    //public override string ToString()
    //{
    //    return "Dict count: " + dict_.Count;
    //}
}
