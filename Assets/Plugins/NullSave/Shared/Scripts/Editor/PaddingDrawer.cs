using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK
{
    [CustomPropertyDrawer(typeof(Padding))]
    public class PaddingDrawer : PropertyDrawer
    {

        #region Public Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight * 5 + 10;
            }

            return EditorGUIUtility.singleLineHeight + 2;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);

            EditorGUI.indentLevel += 1;

            if (property.isExpanded)
            {
                rect = position;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += rect.height + 2;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("left"));
                rect.y += rect.height + 2;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("top"));
                rect.y += rect.height + 2;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("right"));
                rect.y += rect.height + 2;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("bottom"));
                rect.y += rect.height + 2;
            }

            EditorGUI.indentLevel -= 1;

            EditorGUI.EndProperty();
        }

        #endregion

    }
}
