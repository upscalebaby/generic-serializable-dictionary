using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Draws the dictionary and a warning-box for duplicate keys.
/// </summary>
[CustomPropertyDrawer(typeof(GenericDictionary<,>))]
public class GenericDictionaryPropertyDrawer : PropertyDrawer
{
    private static float lineHeight = EditorGUIUtility.singleLineHeight;
    private static float vertSpace = EditorGUIUtility.standardVerticalSpacing;
    private const float warningBoxHeight = 1.5f;
    private bool keyCollision = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Draw list of key/value pairs.
        var list = property.FindPropertyRelative("list");
        EditorGUI.PropertyField(position, list, label, true);

        // Draw key collision warning.
        if (keyCollision)
        {
            position.y += EditorGUI.GetPropertyHeight(list, true);
            if (!list.isExpanded)
            {
                position.y += vertSpace;
            }
            position.height = lineHeight * warningBoxHeight;
            position = EditorGUI.IndentedRect(position);
            EditorGUI.HelpBox(position, "Duplicate keys will not be serialized.", MessageType.Warning);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Height of KeyValue list.
        float height = 0f;
        var list = property.FindPropertyRelative("list");
        height += EditorGUI.GetPropertyHeight(list, true);

        // Height of key collision warning.
        keyCollision = CheckKeyCollisions(list);
        if (keyCollision)
        {
            height += warningBoxHeight * lineHeight;
            if (!list.isExpanded)
            {
                height += vertSpace;
            }
        }
        return height;
    }

    private static bool CheckKeyCollisions(SerializedProperty list)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            var keyValuePair = list.GetArrayElementAtIndex(i);
            var keyProperty = keyValuePair.FindPropertyRelative("Key");
            var keyValue = GetValue(keyProperty);
            if (keyValue == null)
            {
                continue;
            }
            for (int j = i + 1; j < list.arraySize; j++)
            {
                var nextKeyValuePair = list.GetArrayElementAtIndex(j);
                var nextKeyProperty = nextKeyValuePair.FindPropertyRelative("Key");
                var nextKeyValue = GetValue(nextKeyProperty);
                if (nextKeyValue == null)
                {
                    continue;
                }
                if (keyValue.Equals(nextKeyValue))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static object GetValue(SerializedProperty property)
    {
        return property.propertyType switch
        {
            SerializedPropertyType.Integer =>
                property.intValue,
            SerializedPropertyType.Boolean =>
                property.boolValue,
            SerializedPropertyType.Float =>
                property.floatValue,
            SerializedPropertyType.String =>
                property.stringValue,
            SerializedPropertyType.Color =>
                property.colorValue,
            SerializedPropertyType.ObjectReference =>
                property.objectReferenceValue,
            SerializedPropertyType.LayerMask =>
                property.intValue,
            SerializedPropertyType.Enum =>
                property.enumValueIndex,
            SerializedPropertyType.Vector2 =>
                property.vector2Value,
            SerializedPropertyType.Vector3 =>
                property.vector3Value,
            SerializedPropertyType.Vector4 =>
                property.vector4Value,
            SerializedPropertyType.Rect =>
                property.rectValue,
            SerializedPropertyType.ArraySize =>
                property.arraySize,
            SerializedPropertyType.Character =>
                property.intValue,
            SerializedPropertyType.AnimationCurve =>
                property.animationCurveValue,
            SerializedPropertyType.Bounds =>
                property.boundsValue,
            SerializedPropertyType.Gradient =>
                GetGradient(property),
            SerializedPropertyType.Quaternion =>
                property.quaternionValue,
            SerializedPropertyType.ExposedReference =>
                property.exposedReferenceValue,
            SerializedPropertyType.FixedBufferSize =>
                property.fixedBufferSize,
            SerializedPropertyType.Vector2Int =>
                property.vector2IntValue,
            SerializedPropertyType.Vector3Int =>
                property.vector3IntValue,
            SerializedPropertyType.RectInt =>
                property.rectIntValue,
            SerializedPropertyType.BoundsInt =>
                property.boundsIntValue,
            SerializedPropertyType.ManagedReference =>
                property.managedReferenceValue,
            SerializedPropertyType.Hash128 =>
                property.hash128Value,
            _ => null
        };
    }

    private static Gradient GetGradient(SerializedProperty gradientProperty)
    {
        PropertyInfo propertyInfo = typeof(SerializedProperty).GetProperty("gradientValue",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (propertyInfo == null) return null;

        return propertyInfo.GetValue(gradientProperty, null) as Gradient;
    }
}
