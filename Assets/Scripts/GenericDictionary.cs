using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A Generic Serializable Dictionary for Unity that requires no boilerplate.
/// Simply declare your field and key value types and you're good to go. Requires a 
/// Unity version with generic serialization support (Unity 2020.1.X and above).
/// </summary>
[Serializable]
public class GenericDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    List<KeyValue> list = new List<KeyValue>();
    [SerializeField, HideInInspector]
    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
    [SerializeField, HideInInspector]
    bool keyCollision;
    [SerializeField, HideInInspector]
    bool isReadOnly;

    /// <summary>
    /// Serializable KeyValue struct used as items in the dictionary. This struct is needed
    /// since the KeyValuePair in System.Collections.Generic isn't serializable.
    /// </summary>
    [Serializable]
    private struct KeyValue
    {
        public TKey Key;
        public TValue Value;
        public KeyValue(TKey Key, TValue Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }

    public void OnBeforeSerialize()
    {
        // Add all items in dictionary to list.
        foreach (var pair in dictionary)
        {
            var kv = new KeyValue(pair.Key, pair.Value);
            if (!list.Contains(kv))
            {
                list.Add(kv);
            }
        }
    }

    public void OnAfterDeserialize()
    {
        // Create new dictionary based on the list contents and flag key collisions.
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
                // Redundant but removes the unused reference warning.
                if (!keyCollision)  
                {
                    keyCollision = true;
                }
            }
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            return dictionary[key];
        }
        set
        {
            dictionary[key] = value;
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            return dictionary.Keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            return dictionary.Values;
        }
    }

    public int Count
    {
        get
        {
            return dictionary.Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return isReadOnly;
        }
        set
        {
            isReadOnly = value;
        }
    }

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        dictionary.Add(item.Key, item.Value);
    }

    public void Clear()
    {
        dictionary.Clear();
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

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentException("The array cannot be null.");
        if (arrayIndex < 0)
           throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
        if (array.Length < (dictionary.Count - arrayIndex))
            throw new ArgumentException("The destination array has fewer elements than the collection.");

        int i = 0;
        int j = 0;
        foreach (var pair in dictionary)
        {
            if (i >= arrayIndex)
            {
                array[j] = pair;
                j++;
            }
            i++;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        return dictionary.Remove(key);
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

    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }
}