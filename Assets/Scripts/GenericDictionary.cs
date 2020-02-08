using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Serializable Generic Dictionary for Unity that requires no boilerplate to work. Simply 
/// declare your key value types when creating a field and you're good to go. Requires a 
/// Unity version with generic serialization support (Unity 2020.1.X and above).
/// Feel free to report issues on XXXXXXXXXXXXX
/// </summary>
[System.Serializable]
public class GenericDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    public Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
    [SerializeField]
    List<KeyValue<TKey, TValue>> list = new List<KeyValue<TKey, TValue>>();

    public void OnBeforeSerialize()
    {
        // Sync the dictionary and list, this could be optimized if need be
        // but I opted for better readability and ease of implementation here.
        var tempList = new List<KeyValue<TKey, TValue>>(dict.Count);
        foreach (var pair in dict)
        {
            //tempList.Add(new KeyValue<TKey, TValue>(pair.Key, pair.Value));
        }

        foreach (KeyValuePair<TKey, TValue> pair in dict)
        {
            var kv = new KeyValue<TKey, TValue>(pair.Key, pair.Value);
            if (!list.Contains(kv))
            {
                list.Add(kv);
            }
        }

        foreach (var pair in tempList)
        {
            if (!list.Contains(pair))
            {
                //list.Add(pair);
            }
        }
    }

    public void OnAfterDeserialize()
    {
        dict = new Dictionary<TKey, TValue>(list.Count);
        foreach (var pair in list)
        {
            // Unity autofills new entries in inspector to the last entrys value so
            // assume if key already exists user is currently adding keys in inspector.
            // They will be serialized later in the OnBeforeSerialize callback.
            if (pair.Key != null && !dict.ContainsKey(pair.Key))
            {
                dict.Add(pair.Key, pair.Value);
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