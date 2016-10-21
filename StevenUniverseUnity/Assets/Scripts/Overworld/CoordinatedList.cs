using StevenUniverse.FanGame.Battle;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace StevenUniverse.FanGame.Overworld
{
    // Tiles within a stack should be sorted by elevation, then by layer sorting order...
    // Except right now ICoordinated doesn't give us access to that information.
    // Ideally we would want to use this for both tile instances and for tile instance editors, so...
    // The best solution seems to be to push the needed data out into ICoordinated? Check with dust.

    public class CoordinatedList<T> where T : ICoordinated
    {
        IntVector2 min_;
        IntVector2 max_;

        // Dictionary mapping stacks of tiles to their 2D position
        Dictionary<IntVector2, List<T>> stackDict_ = new Dictionary<IntVector2, List<T>>();

        // Dictionary mapping tiles directly to their tile index..if we could get access to the elevation and layer data
        //Dictionary<TileIndex, T> indexDict_ = new Dictionary<TileIndex, T>();

        public CoordinatedList()
        {
        }

        public CoordinatedList(T[] items)
        {
            AddRange(items);
        }

        public void Add(T t)
        {
            IntVector2 pos = new IntVector2(t.Position);

            //Set the mins and maxes if there are currently no elements
            if (stackDict_.Count == 0)
            {
                min_ = max_ = pos;
            }
            //Otherwise, set any new mins and maxes that surpass previous values
            else
            {
                min_ = IntVector2.Min(min_, pos);
                max_ = IntVector2.Max(max_, pos);
            }

            List<T> list;
            if( !stackDict_.TryGetValue(pos, out list ) )
            {
                list = new List<T>();
                stackDict_[pos] = list;
            }
            list.Add(t);

            // TODO: Check if index dict contains index...if not, add to index dict
        }

        public void AddRange(T[] ts)
        {
            for( int i = 0; i < ts.Length; ++i )
            {
                Add( ts[i] );
            }
        }

        /// <summary>
        /// Get the stack of tiles at the given position. Returns null if no tiles are present.
        /// </summary>
        /// <returns>The list of tiles at the given position, or null if no tiles are present.</returns>
        public List<T> Get(int x, int y)
        {
            List<T> list;
            if( !stackDict_.TryGetValue(new IntVector2(x,y), out list ) )
            {
                return null;
            }
            return list;
        }

        public IntVector2 Min
        {
            get { return min_; }
            private set { min_ = value; }
        }
        
        public IntVector2 Max
        {
            get { return max_; }
            private set { max_ = value; }
        }

    }
}