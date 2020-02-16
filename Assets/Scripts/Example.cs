﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// An example of using the generic dictionary with a MonoBehaviour.
/// </summary>
public class Example : MonoBehaviour
{
    // Simply declare the key/value types, zero boilerplate.
    public GenericDictionary<string, GameObject> myGenericDict;

    void Start()
    {
        // Runtime test
        string keyToCheck = "abc";
        bool contains = myGenericDict.ContainsKey(keyToCheck);
        Debug.LogFormat("myGenericDict contains '{0}': {1}", keyToCheck, contains);
    }

    void Update()
    {
        // Runtime test showing that inspector reflects runtime additions to dict.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string newKey = "runtime example";
            myGenericDict.Add(newKey, this.gameObject);
            Debug.LogFormat("Added '{0}' to myGenericDict.", newKey);
        }
    }
}