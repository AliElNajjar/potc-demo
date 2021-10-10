using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomPropertyDrawer(typeof(PopupUI))]
    public class PopupUIDrawer : PropertyDrawer
    {

        #region Public Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lines = 3;

            if ((PickupDetection)property.FindPropertyRelative("detection").intValue == PickupDetection.MainCamRaycast)
            {
                lines += 4;
            }

            if (property.FindPropertyRelative("selection").intValue >= 2)
            {
                lines += 1;
            }

            if (property.FindPropertyRelative("openType").intValue > 1)
            {
                lines += 1;
            }

            return EditorGUIUtility.singleLineHeight * lines + (lines * 2);
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.y = SimplePropertyLine("detection", property, rect);
            if ((PickupDetection)property.FindPropertyRelative("detection").intValue == PickupDetection.MainCamRaycast)
            {
                rect.y = SimplePropertyLine("raycastCulling", property, rect);
                rect.y = SimplePropertyLine("raycastOffset", property, rect);
                rect.y += rect.height + 2;
                rect.y = SimplePropertyLine("maxDistance", property, rect);
            }

            rect.y = SimplePropertyLine("selection", property, rect);
            switch ((PickupType)property.FindPropertyRelative("selection").intValue)
            {
                case PickupType.ByButton:
                    rect.y = SimplePropertyLine("selectButton", property, rect);
                    break;
                case PickupType.ByKey:
                    rect.y = SimplePropertyLine("selectKey", property, rect);
                    break;
            }

            rect.y = SimplePropertyLine("openType", property, rect);
            switch ((MenuOpenType)property.FindPropertyRelative("openType").intValue)
            {
                case MenuOpenType.SpawnInTag:
                    rect.y = SimplePropertyLine("spawnTag", property, rect);
                    break;
                case MenuOpenType.SpawnInTransform:
                    rect.y = SimplePropertyLine("container", property, rect);
                    break;
            }

            EditorGUI.EndProperty();
        }

        #endregion

        #region Private Methods

        private float SimplePropertyLine(string propertyName, SerializedProperty property, Rect rect)
        {
            EditorGUI.PropertyField(rect, property.FindPropertyRelative(propertyName));
            return rect.y + rect.height + 2;
        }

        #endregion

    }
}