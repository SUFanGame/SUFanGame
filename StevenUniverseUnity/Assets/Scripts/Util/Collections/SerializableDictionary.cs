using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StevenUniverse.FanGame.World;



// TODO : So. Lists of objects as TVALUE simply will not serialize. The list object itself serializes but it loses all it's contents for some reason.


namespace StevenUniverse.FanGame.Util.Collections
{

    /// <summary>
    /// A dictionary that can be serialized. Note that the keys and values must also be serializable for this to work.
    /// Also note that the dictionary must be subclassed to survive serialization, unity can't serialize a generic class.
    /// IE: [System.Serializable] IntCharDict : SerializableDictionary<int,char>{}
    /// </summary>
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IDictionary<TKey, TValue>
    {
        [SerializeField, HideInInspector]
        List<TKey> keys_ = new List<TKey>();

        [SerializeField, HideInInspector]
        List<TValue> values_ = new List<TValue>();

        [HideInInspector]
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

        public void OnAfterDeserialize()
        {

            if( dict_.Count != keys_.Count )
            {
                for (int i = 0; i < keys_.Count; ++i)
                {

                    dict_[keys_[i]] = values_[i];
                }
            }
        }

        public void OnBeforeSerialize()
        {
            var enumerator = dict_.GetEnumerator();
            keys_.Clear();
            values_.Clear();
            while (enumerator.MoveNext())
            {
                var p = enumerator.Current;
                keys_.Add(p.Key);
                

                values_.Add(p.Value);
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
            ((IDictionary<TKey, TValue>)dict_).Add(item);
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

        public Dictionary<TKey,TValue>.Enumerator GetEnumerator()
        {
            return dict_.GetEnumerator();
        }

        //IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        //{
        //    return this.GetEnumerator();//((IDictionary<TKey, TValue>)dict_).GetEnumerator();
        //}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>)dict_).GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}