using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace StevenUniverse.FanGame.Util.Collections
{

    // Used to convert lambdas into comparers that the binary heap can use
    public class FunctionalComparer<T> : IComparer<T>
    {
        System.Comparison<T> comparer;
        public FunctionalComparer(System.Comparison<T> comparer)
        {
            this.comparer = comparer;
        }
        public static IComparer<T> Create(System.Comparison<T> comparer)
        {
            return new FunctionalComparer<T>(comparer);
        }
        public int Compare(T x, T y)
        {
            return comparer(x, y);
        }
    }

}