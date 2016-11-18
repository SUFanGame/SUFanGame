using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Collections.ObjectModel;
using System;

namespace StevenUniverse.FanGame.Util.Collections
{
    /// <summary>
    /// Simulates a 2D array but mapped to a single dimension array 
    /// In order to be serialized derived classes must implement this 
    /// with whatever generic type they want
    /// </summary>
    [System.Serializable]
	public class Array2D<T> : IEnumerable<T>
	{
        [SerializeField]
        protected IntVector2 size_;
        /// <summary>
        /// The size for each dimension of the array. Resizing will maintain as much state as possible
        /// given size restrains.
        /// </summary>
        public virtual IntVector2 Size_ 
        { 
            get { return size_; }
            set
            {
                value = IntVector2.Clamp(value, 0, int.MaxValue);
                if( value != size_ )
                {
                    Resize(value);
                    size_ = value;
                }
            }
        }

        [SerializeField]
        //[HideInInspector]
        protected T[] array_;

        protected ReadOnlyCollection<T> readOnlyArray_;
        /// <summary>
        /// Read only access to the array for fast iteration
        /// </summary>
        public ReadOnlyCollection<T> Array_
        {
            get
            {
                if (readOnlyArray_ == null)
                    readOnlyArray_ = System.Array.AsReadOnly(array_);
                return readOnlyArray_;
            }
        }

        public int Length_ { get { return array_.Length; } }

        #region Constructors
        /// <summary>
        /// Note that arrays are always initialized with default values
        /// </summary>
        public Array2D() : this( 3, 3 ) { }

        /// <summary>
        /// Note that arrays are always initialized with default values
        /// </summary>
        public Array2D(int xDim, int yDim)
        {
            size_ = new IntVector2(xDim, yDim);
            Initialize();
        }

        /// <summary>
        /// Note that arrays are always initialized with default values
        /// </summary>
        public Array2D(IntVector2 size) : this( size.x, size.y ) {}

        public Array2D( Array2D<T> other ) : this( other.size_.x, other.size_.y )
        {
            array_ = (T[])other.array_.Clone();
        }
        #endregion

        /// <summary>
        /// Recreates the array using the current Size_
        /// </summary>
        public virtual void Initialize()
        {
            array_ = new T[size_.x * size_.y];
            readOnlyArray_ = System.Array.AsReadOnly(array_);
        }

        /// <summary>
        /// Directly assign the underlying array using a pre-built array.
        /// Useful for deserialization of data directly into an array2d
        /// </summary>
        /// <param name="arr"></param>
        public void AssignArray( T[] arr )
        {
            array_ = arr;
        }

        /// <summary>
        /// Resizes the array, maintainting as much of the current state as possible given size restraints
        /// </summary>
        /// <param name="newSize"></param>
        public virtual void Resize( IntVector2 newSize )
        { 
            if (newSize.x <= 0 || newSize.y <= 0 )
                return;

            var newArray = new T[newSize.x * newSize.y];

            var min = IntVector2.Min(size_, newSize);

            for (int x = 0; x < min.x; ++x)
            {
                for (int y = 0; y < min.y; ++y)
                {
                    int newIndex = y * newSize.x + x;
                    int currIndex = y * size_.x + x;
                    newArray[newIndex] = array_[currIndex]; 
                }
            }
            
            array_ = newArray;
            readOnlyArray_ = System.Array.AsReadOnly(array_);
            size_ = newSize;
        }

        /// <summary>
        /// Shallow Copy
        /// </summary>
        public Array2D<T> Copy()
        {
            return new Array2D<T>(this);
        }

        /// <summary>
        /// Copies as much of the given array as possible into this array given size restrains, overriding any existing values. Doesn't cause allocations.
        /// </summary>
        public void Merge(Array2D<T> from)
        {
            Merge( from, size_, from.size_);
        }
        /// <summary>
        /// Copies values from the from array into the to array as much as possible given size restraints. Doesn't cause allocations.
        /// </summary>
        /// <param name="other"></param>
        void Merge( Array2D<T> from, IntVector2 toSize, IntVector2 fromSize )
        {
            var min = IntVector2.Min(toSize, fromSize);

            for (int x = 0; x < min.x; ++x)
            {
                for (int y = 0; y < min.y; ++y)
                {
                    int fromIndex = from.GetIndex(x, y);
                    int toIndex = GetIndex(x, y);


                    array_[toIndex] = from[fromIndex];
                }
            }
        }

        /// <summary>
        /// Resets all elements of the array to default values.
        /// </summary>
        public void Clear()
        {
            System.Array.Clear(array_, 0, array_.Length);
        }


        #region indexers
        public T this[int x, int y]
        {
            get
            {
                return array_[y * size_.x + x];
 
            }
            set { array_[y * size_.x + x] = value; }
        }

        public T this[IntVector2 index]
        {
            get { return this[index.y * size_.x + index.x]; }
            set { array_[index.y * size_.x + index.x] = value; }
        }

        public T this[int i]
        { 
            get 
            { 
                return array_[i];  
            } 
        }
        #endregion


        public int GetIndex( int x, int y )
        {
            return y * size_.x + x;
        }

        public bool InBounds( Vector3 localPos )
        {
            return localPos.x >= 0 && localPos.x < Size_.x && localPos.y >= 0 && localPos.y < Size_.y;
        }

        public bool InBounds( int x, int y )
        {
            return x >= 0 && x < Size_.x && y >= 0 && y < Size_.y;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < array_.Length; ++i)
                yield return array_[i++];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
}
