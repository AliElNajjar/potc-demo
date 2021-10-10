using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomPropertyDrawer(typeof(CraftingUpgradeResult))]
    public class CraftingUpgradeResultDrawer : PropertyDrawer
    {

        #region Public Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float size = EditorGUIUtility.singleLineHeight * 3 + 6;

            if ((UpgradeResult)property.FindPropertyRelative("condition").intValue == UpgradeResult.AddAmount)
            {
                size += EditorGUIUtility.singleLineHeight + 2;
            }

            if ((UpgradeResult)property.FindPropertyRelative("rarity").intValue == UpgradeResult.AddAmount)
            {
                size += EditorGUIUtility.singleLineHeight + 2;
            }

            return size;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var rect = position;
            rect.width += rect.x - 18;
            rect.x = 18;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("upgradeResult"), new GUIContent("Result Item", null, string.Empty));
            rect.y += rect.height + 2;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("condition"));
            rect.y += rect.height + 2;
            if ((UpgradeResult)property.FindPropertyRelative("condition").intValue == UpgradeResult.AddAmount)
            {
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("conditionChange"), new GUIContent("Amount", null, string.Empty));
                rect.y += rect.height + 2;
            }
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("rarity"));
            rect.y += rect.height + 2;
            if ((UpgradeResult)property.FindPropertyRelative("rarity").intValue == UpgradeResult.AddAmount)
            {
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("rarityChange"), new GUIContent("Amount", null, string.Empty));
                rect.y += rect.height + 2;
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        #endregion

    }
}
