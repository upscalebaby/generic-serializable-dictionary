using UnityEditor;
using UnityEngine;

/// <summary>
/// Draws the generic dictionary a bit nicer than Unity would natively (not as many expand-arrows
/// and better spacing between KeyValue pairs). Also renders a warning-box if there are duplicate
/// keys in the dictionary.
/// </summary>
[CustomPropertyDrawer(typeof(GenericDictionary<,>))]
public class GenericDictionaryPropertyDrawer : PropertyDrawer
{
    static float lineHeight = EditorGUIUtility.singleLineHeight;
    static float vertSpace = EditorGUIUtility.standardVerticalSpacing;
    static float combinedPadding = lineHeight + vertSpace;
    int valuePropCount;

    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        // Setup variables used for drawing.
        var currentPos = new Rect(lineHeight, pos.y, pos.width, lineHeight);
        bool isExpanded = false;
        var propCopy = property.Copy();
        var enumerator = property.GetEnumerator();

        // Iterate properties and draw them.
        while (enumerator.MoveNext())
        {
            var currentProp = ((SerializedProperty)enumerator.Current);
            if (currentProp.name == "list")
            {
                // Draw list header and expand arrow.
                string fieldName = ObjectNames.NicifyVariableName(fieldInfo.Name);
                EditorGUI.PropertyField(currentPos, currentProp, new GUIContent(fieldName));
                isExpanded = currentProp.isExpanded;
            }
            else if (currentProp.name == "size" && isExpanded)
            {
                // Draw size property.
                EditorGUI.indentLevel++;
                currentPos = new Rect(currentPos.x, currentPos.y + lineHeight, pos.width, lineHeight);
                EditorGUI.PropertyField(currentPos, currentProp, new GUIContent("Size"));
                currentPos.y += vertSpace;
            }
            else if (isExpanded && (currentProp.name == "Key" || currentProp.name == "Value"))
            {
                // Setup position and draw KeyValue-properties.
                var entryPos = new Rect(currentPos.x, currentPos.y + combinedPadding, pos.width, lineHeight);
                if (currentProp.isExpanded)
                {
                    currentPos.y += valuePropCount * combinedPadding;
                }
                EditorGUI.PropertyField(entryPos, currentProp, new GUIContent(currentProp.name), currentProp.isExpanded);

                // Add padding.
                if (currentProp.name == "Value")
                {
                    currentPos.y += combinedPadding + vertSpace;
                }
                else
                {
                    currentPos.y += lineHeight + vertSpace;
                }
            }
        }

        // Draw key collision warning box.
        bool keyCollision = propCopy.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            var entryPos = new Rect(lineHeight, currentPos.y + combinedPadding, pos.width, lineHeight * 2f);
            EditorGUI.HelpBox(entryPos, "Duplicate keys will not be serialized.", MessageType.Warning);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Make copies of property so we can iterate it more than once
        var listCopy = property.Copy();
        var valueCopy = property.Copy();

        // Count number of data-properties and expanded value-properties.
        int listCount = 0;
        int expandedCount = 0;
        var enumerator = property.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var currentProp = ((SerializedProperty)enumerator.Current);
            if (currentProp.name == "data")
            {
                listCount++;
            }
            else if (currentProp.isExpanded && currentProp.name == "Value")
            {
                expandedCount++;
            }
        }

        // Count number of sub-properties inside Value-property.
        enumerator = valueCopy.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var currentProp = ((SerializedProperty)enumerator.Current);
            if (currentProp.name == "Value" && currentProp.isExpanded)
            {
                valuePropCount = currentProp.CountInProperty() - 1;
                break;
            }
        }

        // Accumulate height for all properties.
        float totHeight = 0f;

        // Height of key collision warning.
        bool keyCollision = listCopy.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            totHeight += lineHeight * 2f + vertSpace;
        }

        // Height of KeyValue list.
        var listProp = listCopy.FindPropertyRelative("list");
        if (listProp.isExpanded)
        {
            var t = EditorGUI.GetPropertyHeight(listProp, false);
            totHeight += lineHeight * 2f + vertSpace;  // list header and size fields
            totHeight += listCount * 2f * combinedPadding + listCount * vertSpace;  // list contents fields
            totHeight += expandedCount * valuePropCount * combinedPadding;  // expanded value fields
            return totHeight;
        }
        else
        {
            return totHeight + lineHeight;
        }
    }
}