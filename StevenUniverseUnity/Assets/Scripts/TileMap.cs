using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StevenUniverse.FanGame.OverworldEditor;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class TileMap : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<Vector3,List<TileInstanceEditor>>>
{
    [System.Serializable]
    class TileListWrapper 
    {
        public List<TileInstanceEditor> list_;
    }

    [SerializeField]
    List<TileListWrapper> serializedValues_ = null;

    [SerializeField]
    List<Vector3> serializedKeys_ = null;

    Dictionary<Vector3, List<TileInstanceEditor>> dict_ = new Dictionary<Vector3, List<TileInstanceEditor>>();

    public TileMap() { }

    public TileMap( TileInstanceEditor[] tiles )
    {
        for( int i = 0; i < tiles.Length; ++i )
        {
            AddTile(tiles[i].transform.position, tiles[i]);
        }
    }

    /// <summary>
    /// Add a tile to the given position. Any existing tiles that match the given tile's layer will be removed.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="tilePrefab"></param>
    public void AddTile( Vector3 pos, TileInstanceEditor newTile )
    {
        if( !dict_.ContainsKey(pos) )
        {
            dict_[pos] = new List<TileInstanceEditor>();
        }

        dict_[pos].Add(newTile);
    }
    
    /// <summary>
    /// Remove a SINGLE tile at the given position matching the given predicate and destroys it.
    /// </summary>
    /// <param name="pos">Position of the tile</param>
    /// <param name="match">A predicate to match the tile against. If true, the tile will be removed.</param>
    /// <returns></returns>
    public void RemoveTile( Vector3 pos, System.Predicate<TileInstanceEditor> match )
    {
        List<TileInstanceEditor> tiles;
        if (!dict_.TryGetValue(pos, out tiles))
            return;

        for( int i = tiles.Count - 1; i >= 0; --i )
        {
            var t = tiles[i];
            if (match(t))
            {
                tiles.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Remove the given tile from the given position
    /// </summary>
    public void RemoveTile(Vector3 pos, TileInstanceEditor tile )
    {
        var list = GetTiles(pos);
        if (list == null)
            return;
        list.Remove(tile);
    }
    

    /// <summary>
    /// Gets the list of all tiles at the given position.
    /// </summary>
    public List<TileInstanceEditor> GetTiles( Vector3 pos )
    {
        List<TileInstanceEditor> list;
        if (!dict_.TryGetValue(pos, out list))
            return null;

        return list;
    }

    /// <summary>
    /// Gets the count of all tiles at the given position.
    /// </summary>
    public int GetTileCount( Vector3 pos )
    {
        List<TileInstanceEditor> list;
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

    public IEnumerator<KeyValuePair<Vector3, List<TileInstanceEditor>>> GetEnumerator()
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
