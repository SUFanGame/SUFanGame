using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SUGame.Util.Collections
{
    /// <summary>
    /// A dictionary that can be serialized. Note that the keys and values must also be serializable for this to work.
    /// Also note that the dictionary must be subclassed to survive serialization, unity can't serialize a generic class.
    /// </summary>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IDictionary<TKey, TValue>
    {
        [SerializeField]
        List<TKey> keys_ = new List<TKey>();

        [SerializeField]
        List<TValue> values_ = new List<TValue>();

        Dictionary<TKey, TValue> dict_ = new Dictionary<TKey, TValue>();

        public ICollection<TKey> Keys
        {
            get
            {
                return dict_.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return dict_.Values;
            }
        }

        public int Count
        {
            get
            {
                return dict_.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<TKey, TValue>)dict_).IsReadOnly;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return dict_[key];
            }

            set
            {
                dict_[key] = value;
            }
        }

        public void OnBeforeSerialize()
        {
            keys_.Clear();
            values_.Clear();
            foreach (var pair in dict_)
            {
                keys_.Add(pair.Key);
                values_.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dict_.Clear();
            for (int i = 0; i < keys_.Count; ++i)
            {
                dict_[keys_[i]] = values_[i];
            }
        }



        public void Add(TKey key, TValue value)
        {
            dict_.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return dict_.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return dict_.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dict_.TryGetValue(key, out value);
        }


        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dict_.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)dict_).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>)dict_).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)dict_).Remove(item);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return dict_.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>)dict_).GetEnumerator();
        }

    }
}