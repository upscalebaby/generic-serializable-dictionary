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
    int valuePropertiesCount;

    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        // Render list header and expand arrow.
        var list = property.FindPropertyRelative("list");
        var headerPos = new Rect(lineHeight, pos.y, pos.width, lineHeight);
        string fieldName = ObjectNames.NicifyVariableName(fieldInfo.Name);
        EditorGUI.PropertyField(headerPos, list, new GUIContent(fieldName));

        // Render list if expanded.
        var currentPos = new Rect(lineHeight, pos.y, pos.width, lineHeight);
        if (list.isExpanded)
        {
            // Render list size first.
            list.NextVisible(true);
            EditorGUI.indentLevel++;
            currentPos = new Rect(headerPos.x, headerPos.y + lineHeight, pos.width, lineHeight);
            EditorGUI.PropertyField(currentPos, list, new GUIContent("Size"));
            currentPos.y += vertSpace;

            // Render list content.
            do
            {
                if (list.name == "Key" || list.name == "Value")
                {
                    // Setup position and draw properties.
                    var entryPos = new Rect(currentPos.x, currentPos.y + combinedPadding, pos.width, lineHeight);
                    if (list.isExpanded)
                    {
                        currentPos.y += valuePropertiesCount * combinedPadding;
                    }
                    EditorGUI.PropertyField(entryPos, list, new GUIContent(list.name), list.isExpanded);

                    // Add padding.
                    if (list.name == "Value")
                    {
                        currentPos.y += combinedPadding + vertSpace;
                    }
                    else
                    {
                        currentPos.y += lineHeight + vertSpace;
                    }
                }
            } while (list.NextVisible(true));
        }

        // Render key collision warning box.
        bool keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            var entryPos = new Rect(lineHeight, currentPos.y + combinedPadding, pos.width, lineHeight * 2f);
            EditorGUI.HelpBox(entryPos, "Duplicate keys will not be serialized.", MessageType.Warning);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Count number of data-properties and expanded value-properties.
        var listCopy = property.Copy();
        int listCount = 0;
        int expandedCount = 0;
        do
        {
            if (listCopy.name == "data")
            {
                listCount++;
            }
            else if (listCopy.isExpanded && listCopy.name == "Value")
            {
                expandedCount++;
            }
        } while (listCopy.NextVisible(true));

        // Count number of sub-properties inside Value-property.
        var valueCopy = property.Copy();
        while (valueCopy.Next(true))
        {
            if (valueCopy.name == "Value" && valueCopy.isExpanded)
            {
                valuePropertiesCount = valueCopy.CountInProperty() - 1;
                break;
            }
        }

        // Accumulate height for all properties.
        float totHeight = 0f;

        // Height of key collision warning.
        bool keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            totHeight += lineHeight * 2f + vertSpace;
        }

        // Height of KeyValue list.
        var listProp = property.FindPropertyRelative("list");
        if (listProp.isExpanded)
        {
            var t = EditorGUI.GetPropertyHeight(listProp, false);
            totHeight += lineHeight * 2f + vertSpace;  // list header and size fields
            totHeight += listCount * 2f * combinedPadding + listCount * vertSpace;  // list contents fields
            totHeight += expandedCount * valuePropertiesCount * combinedPadding;  // expanded value fields
            return totHeight;
        }
        else
        {
            return totHeight + lineHeight;
        }
    }
}