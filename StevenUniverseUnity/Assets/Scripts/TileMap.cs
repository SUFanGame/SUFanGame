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
public class EditorInstanceMap : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<Vector3,List<InstanceEditor>>>
{
    [System.Serializable]
    class TileListWrapper 
    {
        public List<InstanceEditor> list_;
    }

    [SerializeField]
    List<TileListWrapper> serializedValues_ = null;

    [SerializeField]
    List<Vector3> serializedKeys_ = null;

    Dictionary<Vector3, List<InstanceEditor>> dict_ = new Dictionary<Vector3, List<InstanceEditor>>();

    public EditorInstanceMap() { }

    public EditorInstanceMap( InstanceEditor[] instances )
    {
        for( int i = 0; i < instances.Length; ++i )
        {
            AddInstance(instances[i].transform.position, instances[i]);
        }
    }


    /// <summary>
    /// Add a tile to the given position. Z is assumed to be the instance's elevation. Any existing tiles that match the given tile's layer will be removed.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="tilePrefab"></param>
    public void AddInstance( Vector3 pos, InstanceEditor instance )
    {
        if( !dict_.ContainsKey(pos) )
        {
            dict_[pos] = new List<InstanceEditor>();
        }

        dict_[pos].Add(instance);
    }

    /// <summary>
    /// Remove the given instance from the map. Note in the case of group instances, all tiles of the group will be removed from the map.
    /// </summary>
    public void RemoveInstance(Vector3 pos, InstanceEditor instance )
    {
        if (instance is TileInstanceEditor)
            dict_.Remove(pos);
        else
        {
            var group = instance as GroupInstanceEditor;
            var tiles = group.GroupInstance.IndependantTileInstances;
            for( int i = 0; i < tiles.Length; ++i )
            {
                var tilePos = tiles[i].Position;
                dict_.Remove(tilePos);
            }
        }
    }

    /// <summary>
    /// Remove the instance given position if it matches the given layer. Returns the removed instance, or null if no instance was found.
    /// Note that in the case of groups this will remove ALL TILES OF THAT GROUP from the map, then return the group instance.
    /// </summary>
    public InstanceEditor RemoveAt( Vector3 pos, StevenUniverse.FanGame.Overworld.Templates.TileTemplate.Layer layer )
    {
        // Get a list of all instances at the given position.
        var list = GetInstances(pos);
        if (list == null)
            return null;

        // Iterate through each tile at this location to see if it matches our target layer.
        for( int i = 0; i < list.Count; ++i )
        {
            // The instance at this position. Could be a group or a tile.
            var instance = list[i];

            TileInstanceEditor instanceEditor = null;
            instanceEditor = instance as TileInstanceEditor;
            // If it's not a tile, then it's a group.
            if (instanceEditor == null)
            {
                var group = instance as GroupInstanceEditor;
                // Check the layer of the group's tile at this position. If it's not a match then we can bail out.
                // If it IS a match that means this tile group  will be removed. We'll
                // remove every tile of the group from the map then return the group instance
                if (group.GetTile(pos).TileInstance.TileTemplate.TileLayer != layer)
                    continue;
                
                // Get the instances for all of the group's tiles
                var instances = group.GroupInstance.IndependantTileInstances;
                for( int j = 0; j < instances.Length; ++j )
                {
                    var tile = instances[i];
                    // Position for the tile instance for this group
                    Vector3 tilePos = new Vector3(tile.Position.x, tile.Position.y, tile.Elevation);

                    // Get the list of instances at our tile's position
                    var tiles = GetInstances(tilePos);
                    // Remove the group reference from that position.
                    tiles.Remove(group);
                }

                // At this point all references to our group have been removed from the map.
                return group;
            }

            var existingLayer = instanceEditor.TileInstance.TileTemplate.TileLayer;

            // Otherwise we're dealing with a tile instance. Check our layer...
            if ( existingLayer == layer)
            {
                // If it's a match, remove the tile and return it.
                list.RemoveAt(i);
                return instanceEditor;
            }
        }

        // Otherwise there was no match!
        return null;
    }
    

    /// <summary>
    /// Gets the list of all instances at the given position.
    /// </summary>
    public List<InstanceEditor> GetInstances( Vector3 pos )
    {
        List<InstanceEditor> list;
        if (!dict_.TryGetValue(pos, out list))
            return null;

        return list;
    }

    /// <summary>
    /// Gets the tile at the given location matching the given layer. Returns null if no match was found.
    /// </summary>
    public TileInstanceEditor GetTile( Vector3 pos, StevenUniverse.FanGame.Overworld.Templates.TileTemplate.Layer layer )
    {
        var tiles = GetInstances(pos);
        if (tiles != null )
        {
            for (int i = 0; i < tiles.Count; ++i)
            {
                var tile = tiles[i];
                TileInstanceEditor instanceEditor = null;
                instanceEditor = tile as TileInstanceEditor;
                if (instanceEditor == null)
                    instanceEditor = (tile as GroupInstanceEditor).GetTile(pos);

                if (instanceEditor.TileInstance.TileTemplate.TileLayer == layer)
                {
                    return instanceEditor;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the count of all tiles at the given position.
    /// </summary>
    public int GetTileCount( Vector3 pos )
    {
        List<InstanceEditor> list;
        if (!dict_.TryGetValue(pos, out list))
            return 0;

        return list.Count;
    }

    public void Clear()
    {
        dict_.Clear();
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

    public void OnBeforeSerialize()
    {
        if (serializedKeys_ == null)
            serializedKeys_ = new List<Vector3>();

        if (serializedValues_ == null)
            serializedValues_ = new List<TileListWrapper>();

        serializedKeys_.Clear();
        serializedValues_.Clear();

        foreach( var pair in dict_ )
        {
            serializedKeys_.Add(pair.Key);
            serializedValues_.Add(new TileListWrapper() { list_ = pair.Value });
        }
    }

    public void OnAfterDeserialize()
    {
        for( int i = 0; i < serializedKeys_.Count; ++i )
        {
            dict_.Add(serializedKeys_[i], serializedValues_[i].list_);
        }
    }

    public IEnumerator<KeyValuePair<Vector3, List<InstanceEditor>>> GetEnumerator()
    {
        return dict_.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public override string ToString()
    {
        return "Dict count: " + dict_.Count;
    }
}
