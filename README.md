# Generic Serializable Dictionary
Minimalist dictionary for Unity 2020.1.x with a native look and feel.

## What

* A generic and serializable Dictionary for Unity in about 70 LOC (including summaries and comments).

* Uses plain System.Collections.Generic objects in combination with Unitys built-in serializer.

* An optional property drawer that displays the Dictionary near pixel perfectly as a native List<T> but with standard spacing between each KeyValue-pair (to make it easier on the eyes).

* Zero boilerplate, just declare your field and start using it! See Example.cs for specifics.

```
// supports all types and classes (including custom ones)
public GenericDictionary<string, GameObject> myGenericDict;

// accessing the backend dictionary
myGenericDict.dict.Add("abc", this.gameObject);
```
![](example.gif)

## Why 

As of 2020.1.x Unity supports generic serialization and native support for displaying generic Lists in the inspector. But for a long time the community has wanted a generic Dictionary implementation that doesn't require you to add boilerplate for each concrete Dictionary type.

I'm not a fan of highly decorated inspectors that deviate from native Unity inspectors look and feel. This dictionary aims to look like any other Unity component.

I found this an interesting problem that could potentially help other devs so I implemented it and licensed it under MIT.

## How

The trick is quite simple: a generic struct KeyValue<TKey, TValue> is declared. From this a generic List<KeyValue<TKey, TValue>> can be defined. This List is serialized and displayed natively in the inspector by Unity. The ISerializationCallback interface is implemented to gain access to serialization callbacks that are used to sync the backend Dictionary with the frontend List upon (de)serialization.

No datastructures are modified or reimplemented: just plain old System.Collections.Generic but used together with Unitys generic serializer to display a native feeling Dictionary in the inspector.

## Features

* Behaves just like any native Unity inspector and looks almost identical to a List<T> (with some enhancements to visability/useability).

* Runtime additions to the backing Dictionary immediately shows up in the inspector (see Example.cs).

* Works regardless of context, use in MonoBehaviours and the runtime additions are cleared from the dictionary - or use in ScriptableObjects and the runtime additions remain serialized - just as expected.

* The custom property drawer displays a warning if there are duplicate keys in the inspector.

## How to use

This repo is a regular Unity project, so you have two choices:
* Clone the repo and open the project in Unity and give it a spin.
* Copy GenericDictionary.cs into your Asset folder and the GenericDictionaryPropertyDrawer.cs into an Editor folder and you're good to go.

## Requirements

A Unity version with support for generic serialization (currently 2020.1.x and above).

## License
Licensed under MIT, see license file.
