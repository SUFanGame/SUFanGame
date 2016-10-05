using UnityEngine;
using System.Collections.Generic;
using StevenUniverse.FanGame.OverworldEditor;
using System.Text;
using System.Linq;
using System;
using System.Collections;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    /// <summary>
    /// Maps Lists of tile instances to their 2D positions.
    /// </summary>

    [System.Serializable]
    public class TilePositionMap: IEnumerable<KeyValuePair<Vector2, List<TileInstanceEditor>>>
    {
        public TilePositionMap()
        {
            mapData_ = new Dictionary<Vector2, List<TileInstanceEditor>>();
        }

        public TilePositionMap( TileInstanceEditor[] tiles ) : this()
        {
            if (tiles == null)
                return;
            
            for( int i = 0; i < tiles.Length; ++i )
            {
                AddValue(tiles[i].transform.position, tiles[i]);
            }
        }

        public Dictionary<Vector2, List<TileInstanceEditor>> mapData_;

        /// <summary>
        /// Returns a value at the given position. Will return null if the position isn't populated.
        /// </summary>
        public List<TileInstanceEditor> Get( Vector2 pos )
        {
            if (!mapData_.ContainsKey(pos))
                return null;
            return mapData_[pos];
        }

        public void AddValue( Vector2 pos, TileInstanceEditor val )
        {
            //Debug.LogFormat("Adding {0} to list at {1}", val.name, pos );
            List<TileInstanceEditor> list;
            if( !mapData_.TryGetValue(pos, out list ) )
            {
                list = new List<TileInstanceEditor>();
                mapData_[pos] = list;
            }
            mapData_[pos].Add(val);
        }

        /// <summary>
        /// Remove the given value from the given position.
        /// </summary>
        public void RemoveValue( Vector2 pos, TileInstanceEditor tile )
        {
            if (mapData_.ContainsKey(pos))
                mapData_[pos].Remove(tile);
        }

        /// <summary>
        /// Remove all elements from the position matching the given predicate.
        /// </summary>
        public void RemoveAll( Vector2 pos, System.Predicate<TileInstanceEditor> match )
        {
            if (mapData_.ContainsKey(pos))
                mapData_[pos].RemoveAll(match);
        }

        public void Clear()
        {
            mapData_.Clear();
        }

        //public void OnBeforeSerialize()
        //{
        //    if (serializedPositions_ == null)
        //        serializedPositions_ = new List<Vector2>();

        //    if (serializedLists_ == null)
        //        serializedLists_ = new List<ListWrapper>();

        //    serializedPositions_.Clear();
        //    foreach (var list in serializedLists_)
        //        list.list_.Clear();
        //    serializedLists_.Clear();

        //    if (mapData_.Count > 0)
        //    {
        //        Debug.Log("State of map BEFORE serialization");
        //        Print();
        //    }

        //    foreach ( var pair in mapData_ )
        //    {
        //        serializedPositions_.Add(pair.Key);
        //        serializedLists_.Add( new ListWrapper() { list_ = pair.Value } );
        //    }
            
        //}

        //public void OnAfterDeserialize()
        //{
        //    //Debug.LogFormat("Positions null : {0}, Lists null: {1}", serializedPositions_ == null, serializedLists_ == null);
        //    //Debug.LogFormat("SerializedLists Count: {0}", serializedLists_.Count);

        //    foreach( var list in serializedLists_ )
        //    {
        //        foreach( var instance in list.list_ )
        //        {
        //            if (instance == null)
        //                Debug.Log("INSTANCE NULL IN POSITIONMAP LIST");
        //        }
        //    }

        //    //Debug.LogFormat("SerializedPositions Count on After Deserialize: {0}", serializedPositions_.Count);
        //    if( serializedPositions_ != null )
        //    {
        //        for( int i = 0; i < serializedPositions_.Count; ++i )
        //        {
        //            //Debug.LogFormat("List for {0} is null : {1}", serializedPositions_[i], serializedLists_[i].list_ == null);
        //            mapData_.Add(serializedPositions_[i], serializedLists_[i].list_ );
        //        }
        //        //Debug.Log(ToString());


        //        Debug.Log("State of map AFTER serialization");
        //        Print();
        //    }
            
        //}

        //[SerializeField]
        //List<Vector2> serializedPositions_ = null;
        //[SerializeField]
        //List<ListWrapper> serializedLists_ = null;
        //[System.Serializable]
        //class ListWrapper
        //{
        //    public List<TileInstanceEditor> list_;
        //}

        public void Print()
        {
            var mapString = ToString();
            var strings = mapString.Split('\n', '\r');
            foreach (var str in strings)
                if( !string.IsNullOrEmpty(str))
                    Debug.Log(str);
        }

        public override string ToString()
        {
            var enumerator = mapData_.GetEnumerator();
            StringBuilder sb = new StringBuilder();
            while( enumerator.MoveNext() )
            {
                var list = enumerator.Current.Value;

                //var elevations = list.Select(t => t.Elevation.ToString()).ToArray();
                //var eleveationsString = string.Join(", ", elevations);

                sb.AppendLine( string.Format( "{0}:Is Null : {1}", enumerator.Current.Key, list[0] == null ) );
            }

            return sb.ToString().TrimEnd('\r', '\n');
        }

        public IEnumerator<KeyValuePair<Vector2, List<TileInstanceEditor>>> GetEnumerator()
        {
            foreach (var pair in mapData_)
                yield return pair;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
