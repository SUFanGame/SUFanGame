using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


namespace StevenUniverse.FanGame.Util.Collections
{
    /// <summary>
    /// A Serializable dictionary that allows for lists of values. A little quirky to get working with Unity serialization.
    /// The "TValue" type parameter should NOT be a list type, the dictionary will handle the lists internally.
    /// Due to interface restraints you can't use Foreach to iterate over the lists. You must use <seealso cref="ListEnumerator"/>
    /// </summary>
    /// <typeparam name="TKey">Keys of the dictionary.</typeparam>
    /// <typeparam name="TValue">Values of the lists of the dictionary. Note this should NOT be a list type.</typeparam>
    [System.Serializable]
    public class SerializableDictionaryOfLists<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<TKey> keys_ = new List<TKey>();
        [SerializeField]
        List<TValue> values_ = new List<TValue>();

        Dictionary<TKey, List<TValue>> dictOfLists_ = new Dictionary<TKey, List<TValue>>();



        public List<TValue> this[TKey key]
        {
            get
            {
                return dictOfLists_[key];
            }

            set
            {
                dictOfLists_[key] = value;
            }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return default(TValue);
            }

            set
            {
            }
        }

        public int Count
        {
            get
            {
                return dictOfLists_.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<TKey, TValue>)dictOfLists_).IsReadOnly;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return dictOfLists_.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var values = dictOfLists_.Select(p => p.Value.Select(v => v));
                return values as ICollection<TValue>;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>)dictOfLists_).Add(item);
        }

        public void Add(TKey key, TValue value)
        {
            dictOfLists_[key].Add(value);
        }

        public void Clear()
        {
            dictOfLists_.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)dictOfLists_).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return dictOfLists_.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>)dictOfLists_).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Return the underlying dictionary's enumerator.
        /// </summary>
        public Dictionary<TKey, List<TValue>>.Enumerator ListEnumerator()
        {
            return dictOfLists_.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var outerPair in dictOfLists_)
            {
                foreach (var val in outerPair.Value)
                    yield return new KeyValuePair<TKey, TValue>(outerPair.Key, val);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Remove the given item from the list at the given key.
        /// </summary>
        public bool Remove( TKey key, TValue value )
        {
            return dictOfLists_[key].Remove(value);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key, item.Value);
        }

        public bool Remove(TKey key)
        {
            return dictOfLists_.Remove(key);
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        public bool TryGetValue(TKey key, out List<TValue> value)
        {
            return dictOfLists_.TryGetValue(key, out value);
        }


        public void OnBeforeSerialize()
        {
            keys_.Clear();
            values_.Clear();

            var enumerator = dictOfLists_.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var list = enumerator.Current.Value;
                for (int i = 0; i < list.Count; ++i)
                {
                    keys_.Add(enumerator.Current.Key);
                    values_.Add(list[i]);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            if (dictOfLists_.Count == 0 && keys_.Count > 0)
            {
                int i = 0;
                while (i < keys_.Count)
                {
                    var lastKey = keys_[i];
                    List<TValue> list = new List<TValue>();
                    while (i < keys_.Count && lastKey.Equals(keys_[i]))
                    {
                        list.Add(values_[i]);
                        lastKey = keys_[i++];
                    }
                    dictOfLists_.Add(lastKey, list);
                }
            }

        }

    }
}