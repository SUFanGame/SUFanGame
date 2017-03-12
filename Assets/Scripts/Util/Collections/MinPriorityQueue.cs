using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace SUGame.Util.Collections
{
    /// <summary>
    /// A priority queue which sorts and returns low-priority items first. Note this does
    /// NOT allow duplicates.
    /// </summary>
    public class MinPriorityQueue<T> : ICollection<T>
    {
        MinHeap<T> heap_;
        Dictionary<T, int> priority_;
        
        public int Count { get { return heap_.Count; } }

        public bool IsReadOnly { get { return false; } }

        public MinPriorityQueue( int capacity = 4 )
        {
            priority_ = new Dictionary<T, int>( capacity );
            heap_ = new MinHeap<T>( capacity, new PriorityComparer(priority_) );
        }

        /// <summary>
        /// Remove an item from the queue.
        /// </summary>
        public T Remove()
        {
            return heap_.Remove();
        }

        /// <summary>
        /// Add an item with the given priority or updates it's priority if it's already present.
        /// Duplicate values are not allowed.
        /// </summary>
        public void Add( T val, int priority )
        {
            if( priority_.ContainsKey(val) )
            {
                UpdatePriority( val, priority );
                return;
            }

            priority_[val] = priority;
            heap_.Add(val);
        }

        /// <summary>
        /// Add an item with the lowest possible priority
        /// </summary>
        public void Add(T item)
        {
            Add(item, int.MaxValue);
        }

        public void UpdatePriority( T val, int priority )
        {
            priority_[val] = priority;
            heap_.Sort();
        }

        public int GetPriority( T val )
        {
            return priority_[val];
        }

        public void Clear()
        {
            heap_.Clear();
            priority_.Clear();
        }

        public bool Contains(T item)
        {
            return heap_.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            heap_.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return heap_.Remove(item);
        }

        public T Peek()
        {
            return heap_.Peek();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return heap_.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Comparer used for heap sorting
        /// </summary>
        class PriorityComparer : IComparer<T>
        {
            Dictionary<T, int> priority_;

            public PriorityComparer(Dictionary<T,int> p )
            {
                priority_ = p;
            }

            public int Compare(T x, T y)
            {
                if (priority_.ContainsKey(x) && priority_.ContainsKey(y))
                    return priority_[x].CompareTo(priority_[y]);
                if ( priority_.ContainsKey(x) )
                    return 1;
                if ( priority_.ContainsKey(y) )
                    return -1;
                return 0;
            }
        }
    }
}
