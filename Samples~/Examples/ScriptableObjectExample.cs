using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableObjectExample : ScriptableObject
{
    [SerializeField]
    private GenericDictionary<string, Sprite> dictionary;
}
