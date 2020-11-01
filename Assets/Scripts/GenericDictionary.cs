using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Generic Serializable Dictionary for Unity 2020.1 or later.
/// Simply declare your field and key/value types and you're good to go, zero boilerplate.
/// </summary>
[Serializable]
public class GenericDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    List<KeyValue> list = new List<KeyValue>();
    [SerializeField, HideInInspector]
    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
    
    [SerializeField, HideInInspector]
    #pragma warning disable 0414
    bool keyCollision;
    #pragma warning restore 0414

    /// <summary>
    /// Serializable KeyValue struct used as items in the dictionary. This is needed
    /// since the KeyValuePair in System.Collections.Generic isn't serializable.
    /// </summary>
    [Serializable]
    struct KeyValue
    {
        public TKey Key;
        public TValue Value;
        public KeyValue(TKey Key, TValue Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }

    public TValue this[TKey key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }
    public ICollection<TKey> Keys => dictionary.Keys;

    public ICollection<TValue> Values => dictionary.Values;

    public int Count => dictionary.Count;

    public bool IsReadOnly { get; set; }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();

    // Serialize dictionary into list representation.
    public void OnBeforeSerialize()
    {
        foreach (var pair in dictionary)
        {
            var kv = new KeyValue(pair.Key, pair.Value);
            if (!list.Contains(kv))
            {
                list.Add(kv);
            }
        }
    }

    // Deserialize dictionary from list while checking for key-collisions.
    public void OnAfterDeserialize()
    {
        keyCollision = false;
        dictionary = new Dictionary<TKey, TValue>(list.Count);
        foreach (var pair in list)
        {
            if (pair.Key != null && !ContainsKey(pair.Key))
            {
                Add(pair.Key, pair.Value);
            }
            else
            {
                keyCollision = true;
            }
        }
    }

    public void Add(TKey key, TValue value) => dictionary.Add(key, value);

    public void Add(KeyValuePair<TKey, TValue> item) => dictionary.Add(item.Key, item.Value);

    public void Clear()
    {
        dictionary.Clear();
        list.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        TValue value;
        if (dictionary.TryGetValue(item.Key, out value))
        {
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }
        else
        {
            return false;
        }
    }

    public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentException("The array cannot be null.");
        if (arrayIndex < 0)
           throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
        if (array.Length - arrayIndex < dictionary.Count)
            throw new ArgumentException("The destination array has fewer elements than the collection.");

        foreach (var pair in dictionary)
        {
            array[arrayIndex] = pair;
            arrayIndex++;
        }
    }

    public bool Remove(TKey key)
    {
        if (dictionary.Remove(key))
        {
            KeyValue item = new KeyValue();
            foreach (var element in list)
            {
                if (EqualityComparer<TKey>.Default.Equals(element.Key, key))
                {
                    item = element;
                    break;
                }
            }
            list.Remove(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        TValue value;
        if (dictionary.TryGetValue(item.Key, out value))
        {
            bool valueMatch = EqualityComparer<TValue>.Default.Equals(value, item.Value);
            if (valueMatch)
            {
                dictionary.Remove(item.Key);
                return true;
            }
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
}
