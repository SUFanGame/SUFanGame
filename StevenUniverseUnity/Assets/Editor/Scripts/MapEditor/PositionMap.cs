using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.OverworldEditor;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class PositionMap<T>
    {
        Dictionary<Vector2, List<T>> mapData_ = new Dictionary<Vector2, List<T>>();

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
    }
}
