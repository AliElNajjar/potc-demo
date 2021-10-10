using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomPropertyDrawer(typeof(CustomTagFilter))]
    public class CustomTagFilterDrawer : PropertyDrawer
    {

        #region Public Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            switch ((TagFilterType)property.FindPropertyRelative("filterType").intValue)
            {
                case TagFilterType.Matches:
                    return EditorGUIUtility.singleLineHeight * 3 + 6;
                default:
                    return EditorGUIUtility.singleLineHeight * 2 + 3;
            }
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //// Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("filterType"));
            rect.y += rect.height + 2;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("tagName"), new GUIContent("Name", null, string.Empty));
            rect.y += rect.height + 2;
            if ((TagFilterType)property.FindPropertyRelative("filterType").intValue == TagFilterType.Matches)
            {
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("tagValue"), new GUIContent("Value", null, string.Empty));
                rect.y += rect.height + 2;
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        #endregion

    }
}
