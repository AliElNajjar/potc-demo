using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomPropertyDrawer(typeof(AdvancedComponent))]
    public class AdvancedComponentDrawer : PropertyDrawer
    {

        #region Public Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4 + 8;
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //EditorGUI.BeginProperty(position, label, property);

            //// Draw label
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            ////// Don't make child fields be indented
            ////var indent = EditorGUI.indentLevel;
            ////EditorGUI.indentLevel = 0;

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("item"));
            rect.y += rect.height + 2;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("count"));
            rect.y += rect.height + 2;


            Rect sliderRect = new Rect();
            sliderRect.y = rect.y;
            sliderRect.height = rect.height;
            sliderRect.x = rect.x + EditorGUIUtility.labelWidth;
            sliderRect.width = rect.width - EditorGUIUtility.labelWidth;
            if (sliderRect.width < 105)
            {
                sliderRect.x -= 105 - sliderRect.width;
                sliderRect.width = 105;
            }
            EditorGUI.LabelField(rect, "Min Condition");
            EditorGUI.Slider(sliderRect, property.FindPropertyRelative("minCondition"), 0f, 1f, label);
            rect.y += rect.height + 2;

            sliderRect.y = rect.y;
            EditorGUI.LabelField(rect, "Min Rarity");
            EditorGUI.IntSlider(sliderRect, property.FindPropertyRelative("minRarity"), 0, 10, label);
            rect.y += rect.height + 2;

            //EditorGUI.PropertyField(rect, property.FindPropertyRelative("minRarity"));
            //rect.y += rect.height + 2;

            ////EditorGUI.indentLevel = indent;

            //EditorGUI.EndProperty();

            //SerializedProperty item = property.FindPropertyRelative("item");
            //EditorGUI.BeginProperty(rect, null, item);
            //EditorGUI.PropertyField(rect, property.FindPropertyRelative("item"));
            //rect.y += rect.height + 2;
            //EditorGUI.EndProperty();


            //SerializedProperty minCondition = property.FindPropertyRelative("minCondition");
            //EditorGUI.BeginProperty(rect, null, item);
            ////EditorGUI.PropertyField(rect, property.FindPropertyRelative("minCondition"));
            //EditorGUI.Slider(rect, minCondition, 0f, 1f, label);
            //rect.y += rect.height + 2;
            //EditorGUI.EndProperty();

        }

        #endregion

    }
}
