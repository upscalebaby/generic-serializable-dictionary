# Generic Serializable Dictionary
Minimalist dictionary for Unity 2020.1.x with a native look and feel.

## What

* Generic and serializable Dictionary for Unity in about 70 LOC (including summaries and comments).

* Uses plain System.Collections.Generic objects in combination with Unitys built-in serializer.

* Optional property drawer that displays the Dictionary near pixel perfectly as a List but with standard spacing between each KeyValue-pair (to make it easier on the eyes).

![](example.gif)

* Zero boilerplate, declare your field and start using it! See Example.cs for specifics.

Todo: insert pic of new code

## Why 

As of 2020.1.x Unity supports generic serialization and native support for displaying generic Lists in the inspector. But for a long time the community has wanted a generic Dictionary implementation that doesn't require you to add boilerplate for each concrete Dictionary type.

Personally I'm not a fan of heavily decorated or bloated inspectors that deviate from Unity's standard inspector look and feel. Unlike most dictionary implementations for unity, this dictionary aims to look and work like the standard components you already know and use.

## How

Todo: upate with new implentation details

The trick is quite simple: a generic struct KeyValue<TKey, TValue> is declared. From this a generic List<KeyValue<TKey, TValue>> can be defined. This List is serialized and displayed natively in the inspector by Unity. The ISerializationCallback interface is implemented to gain access to serialization callbacks that are used to sync the backend Dictionary with the frontend List upon (de)serialization.

No datastructures are modified or reimplemented: just plain old System.Collections.Generic but used together with Unitys generic serializer to display a native feeling Dictionary in the inspector.

## Features

* Looks and feels like a native inspector, renders much like a List<T> (with some enhancements to visability/useability).

* Runtime additions to the Dictionary immediately renders in the inspector (see Example.cs).

* Works regardless of context, use in MonoBehaviours and the runtime additions are cleared from the dictionary - or use in ScriptableObjects and the runtime additions remain serialized - just as expected.

* The custom property drawer displays a standard warning box for key collisions - similar to the floating point precision warning in the Transform component.

Todo: picture of transform and key collision warnings

## How to use

This repo is a regular Unity project, so you have two choices:
* Clone the repo and open the project in Unity and give it a spin.
* Copy GenericDictionary.cs into your Asset folder and the GenericDictionaryPropertyDrawer.cs into an Editor folder and you're good to go.

## Requirements

A Unity version with support for generic serialization (currently 2020.1.x and above).

## License
Licensed under MIT, see license file.
