using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.OverworldEditor;
using System;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    [System.Serializable]
    public class PositionMap<T> : ISerializationCallbackReceiver
    {
        public PositionMap()
        {
            mapData_ = new Dictionary<Vector2, List<T>>();
        }

        Dictionary<Vector2, List<T>> mapData_;

        /// <summary>
        /// Returns a value at the given position. Will return null if the position isn't populated.
        /// </summary>
        public List<T> Get( Vector2 pos )
        {
            if (!mapData_.ContainsKey(pos))
                return null;
            return mapData_[pos];
        }

        public void AddValue( Vector2 pos, T val )
        {
            List<T> list;
            if( !mapData_.TryGetValue(pos, out list ) )
            {
                list = new List<T>();
                mapData_[pos] = list;
            }
            list.Add(val);
        }

        /// <summary>
        /// Remove the given value from the given position.
        /// </summary>
        public void RemoveValue( Vector2 pos, T val )
        {
            if (mapData_.ContainsKey(pos))
                mapData_[pos].Remove(val);
        }

        /// <summary>
        /// Remove all elements from the position matching the given predicate.
        /// </summary>
        public void RemoveAll( Vector2 pos, System.Predicate<T> match )
        {
            if (mapData_.ContainsKey(pos))
                mapData_[pos].RemoveAll(match);
        }

        public void Clear()
        {
            mapData_.Clear();
        }

        public void OnBeforeSerialize()
        {
            if (serializedPositions_ == null)
                serializedPositions_ = new List<Vector2>();

            if (serializedLists_ == null)
                serializedLists_ = new List<ListWrapper>();

            serializedPositions_.Clear();
            serializedLists_.Clear();
            
            foreach( var pair in mapData_ )
            {
                serializedPositions_.Add(pair.Key);
                //Debug.LogFormat("Adding {0} list, Size: {1}", pair.Key, pair.Value.Count);
                serializedLists_.Add( new ListWrapper() { list_ = pair.Value } );
            }

            Debug.LogFormat("SerializedLists Count before serialize: {0}", serializedLists_.Count);
        }

        public void OnAfterDeserialize()
        {
            Debug.LogFormat("Positions null : {0}, Lists null: {1}", serializedPositions_ == null, serializedLists_ == null);

            //Debug.LogFormat("SerializedPositions Count on After Deserialize: {0}", serializedPositions_.Count);
            if( serializedPositions_ != null )
            {
                for( int i = 0; i < serializedPositions_.Count; ++i )
                {
                    //Debug.LogFormat("List for {0} is null : {1}", serializedPositions_[i], serializedLists_[i].list_ == null);
                    mapData_.Add(serializedPositions_[i], serializedLists_[i].list_);
                }
            }
        }

        [SerializeField]
        List<Vector2> serializedPositions_ = null;
        // Can't serialize lists of lists by default, need to create a wrapper.
        [System.Serializable]
        class ListWrapper
        {
            public List<T> list_;
        }
        [SerializeField]
        List<ListWrapper> serializedLists_ = null;
    }
}
