using StevenUniverse.FanGame.StrategyMap;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace StevenUniverse.FanGame.Overworld
{
    // TODO : Cache read-only version of the underlying lists and return those instead of the actual lists. We don't
    // want to just return list.AsReadOnly as that causes unneeded allocations.

    /// <summary>
    /// Providing fast and efficient access to tiles and groups of tiles.
    /// </summary>
    public class TileMap<T> : IEnumerable<T> where T : ITile
    {
        IntVector3 min_;
        IntVector3 max_;
        IntVector3 size_;

        // Dictionary mapping stacks of tiles to their 2D position
        Dictionary<IntVector2, List<T>> stackDict_ = new Dictionary<IntVector2, List<T>>();
        // Dictionary mapping tiles directly to their tile index (x,y,elevation,layer)
        Dictionary<TileIndex, T> indexDict_ = new Dictionary<TileIndex, T>();
        // Dictionary mapping sets of tiles to their 3D Position (x, y, elevation)
        Dictionary<IntVector3, List<T>> elevationDict_ = new Dictionary<IntVector3, List<T>>();

        /// <summary>
        /// Comparer to sort tiles within a tilestack as they are added to the map.
        /// </summary>
        static TileComparer comparer_ = new TileComparer();

        public IntVector3 Min
        {
            get { return min_; }
        }

        public IntVector3 Max
        {
            get { return max_; }
        }

        public IntVector3 Size
        {
            get { return size_; }
        }
        
        public TileMap()
        {
        }

        public TileMap(T[] items)
        {
            AddRange(items);
        }

        public void Add(T t)
        {
            IntVector3 pos = new IntVector3(t.Position.x, t.Position.y, t.Elevation );

            //Set the mins and maxes if there are currently no elements
            if (stackDict_.Count == 0)
            {
                min_ = max_ = pos;
            }
            //Otherwise, set any new mins and maxes that surpass previous values
            else
            {
                min_ = IntVector3.Min(min_, pos);
                max_ = IntVector3.Max(max_, pos);
                size_ = max_ - min_ + IntVector3.one;
            }

            AddToStackDict(t);

            AddToIndexDict(t);

            AddToElevationDict(t);
        }

        public void AddRange(T[] ts)
        {
            for( int i = 0; i < ts.Length; ++i )
            {
                Add( ts[i] );
            }
        }

        /// <summary>
        /// Get the stack of tiles at the given position or null if no tiles are present. Note this returns
        /// the actual underlying list, which is used to avoid unneeded allocations. DON'T CACHE OR MODIFY IT.
        /// </summary>
        /// <returns>The list of tiles at the given position, or null if no tiles are present.</returns>
        public List<T> GetTileStack(int x, int y)
        {
            List<T> list;
            stackDict_.TryGetValue(new IntVector2(x, y), out list);
            return list;
        }

        /// <summary>
        /// Retrieve the tile at the given index, or null if no tile exists.
        /// </summary>
        public T GetTile( TileIndex index )
        {
            T t;
            indexDict_.TryGetValue(index, out t);
            return t;
        }

        /// <summary>
        /// Returns the list of tiles at the given 3D Position ( x, y, elevation ) or null if no tiles are present. 
        /// Note this returns the actual underlying list, which is used to avoid unneeded allocations. 
        /// DON'T CACHE OR MODIFY IT.
        /// </summary>
        /// <returns>The list of tiles at the given position, or null if no tiles are present.</returns>
        public List<T> GetTilesAtElevation( int x, int y, int elevation )
        {
            var index = new IntVector3(x, y, elevation);
            List<T> list;
            elevationDict_.TryGetValue(index, out list);
            return list;
        }

        void AddToStackDict( T t )
        {
            List<T> list;
            if (!stackDict_.TryGetValue(t.Position, out list))
            {
                list = new List<T>();
                stackDict_[t.Position] = list;
            }
            list.Add(t);

            list.Sort(comparer_);
        }

        void AddToIndexDict( T t )
        {
            var index = new TileIndex(t.Position, t.Elevation, t.SortingOrder);
            if (!indexDict_.ContainsKey(index))
            {
                indexDict_[index] = t;
            }
        }

        void AddToElevationDict( T t )
        {
            var index = new IntVector3(t.Position.x, t.Position.y, t.Elevation);

            List<T> list;
            if (!elevationDict_.TryGetValue(index, out list))
            {
                list = new List<T>();
                elevationDict_[index] = list;
            }
            list.Add(t);

            // Sort tiles by their sortingorder
            list.Sort(comparer_);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var pair in indexDict_)
                yield return pair.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        class TileComparer : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                // Order tiles by DESCENDING elevation ( high to low )
                if (x.Elevation != y.Elevation)
                    return -x.Elevation.CompareTo(y.Elevation);

                // Then by DESCENDING sorting order ( high to low )
                return -x.SortingOrder.CompareTo(y.SortingOrder);
            }
        }

    }
}