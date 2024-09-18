using System.Collections.Generic; 
using System.Collections;
using UnityEngine;
using System;

namespace AtomEngine.SystemFunc.Collections
{
    /// <summary>
    /// TValue not serializeble type of Unity
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] protected List<TKey> keys = new List<TKey>(); 
        [SerializeField] protected List<TValue> values = new List<TValue>();


        protected Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key] = value;
                    int index = keys.IndexOf(key);
                    values[index] = value;
                }
                else
                {
                    dictionary[key] = value;
                    keys.Add(key);
                    values.Add(value);
                }
            }
        }

        public ICollection<TKey> Keys => dictionary.Keys;
        public ICollection<TValue> Values => dictionary.Values;
        public int Count => dictionary.Count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        { 
            dictionary.Add(key, value);
            keys.Add(key);
            values.Add(value);
        }

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        public bool Remove(TKey key)
        {
            if (dictionary.Remove(key))
            {
                int index = keys.IndexOf(key);

                values.RemoveAt(index);
                keys.RemoveAt(index);
                values.RemoveAt(index);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            dictionary.Clear();
            keys.Clear();
            values.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => dictionary.ContainsKey(item.Key) && dictionary[item.Key].Equals(item.Value);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var pair in dictionary)
            {
                array[arrayIndex++] = pair;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Эти методы нужны для сериализации и десериализации данных в Unity
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var kvp in dictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();

            if (keys.Count != values.Count)
            {
                Debug.LogError("Keys and values count mismatch");
                return;
            }

            for (int i = 0; i < keys.Count; i++)
            {  
                dictionary[keys[i]] = values[i];
            }
        }
    }
}
