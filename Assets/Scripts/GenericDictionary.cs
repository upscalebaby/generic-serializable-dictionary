using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Serializable Generic Dictionary for Unity that requires no boilerplate to work.
/// Simply declare your field and key value types and you're good to go. Requires a 
/// Unity version with generic serialization support (Unity 2020.1.X and above).
/// </summary>
[System.Serializable]
public class GenericDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    public Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
    [SerializeField]
    List<KeyValue<TKey, TValue>> list = new List<KeyValue<TKey, TValue>>();
    [SerializeField, HideInInspector]
    bool keyCollision;

    public void OnBeforeSerialize()
    {
        // Add all items in dictionary to list.
        foreach (KeyValuePair<TKey, TValue> pair in dict)
        {
            var kv = new KeyValue<TKey, TValue>(pair.Key, pair.Value);
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
        dict = new Dictionary<TKey, TValue>(list.Count);
        foreach (var pair in list)
        {
            if (pair.Key != null && !dict.ContainsKey(pair.Key))
            {
                dict.Add(pair.Key, pair.Value);
            }
            else
            {
                if (!keyCollision)
                {
                    keyCollision = true;
                }
            }
        }
    }
}

/// <summary>
/// Generic KeyValue struct used for emulating a Dictionary in the inspector using a List.
/// </summary>
[System.Serializable]
public struct KeyValue<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
    public KeyValue(TKey Key, TValue Value)
    {
        this.Key = Key;
        this.Value = Value;
    }
}