using UnityEditor;
using UnityEngine;

/// <summary>
/// Draws the dictionary and a warning-box if there are duplicate keys.
/// </summary>
[CustomPropertyDrawer(typeof(GenericDictionary<,>))]
public class GenericDictionaryPropertyDrawer : PropertyDrawer
{
    static float lineHeight = EditorGUIUtility.singleLineHeight;
    static float vertSpace = EditorGUIUtility.standardVerticalSpacing;

    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        // Draw list.
        var list = property.FindPropertyRelative("list");
        string fieldName = ObjectNames.NicifyVariableName(fieldInfo.Name);
        EditorGUI.PropertyField(pos, list, new GUIContent(fieldName), true);

        // Draw key collision warning.
        var keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            pos.y += EditorGUI.GetPropertyHeight(list, true) + vertSpace;
            pos.height = lineHeight * 2f ;
            pos = EditorGUI.IndentedRect(pos);
            EditorGUI.HelpBox(pos, "Duplicate keys will not be serialized.", MessageType.Warning);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totHeight = 0f;

        // Height of KeyValue list.
        var listProp = property.FindPropertyRelative("list");
        totHeight += EditorGUI.GetPropertyHeight(listProp, true);

        // Height of key collision warning.
        bool keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            totHeight += lineHeight * 2f + vertSpace;
        }

        return totHeight;
    }
}
