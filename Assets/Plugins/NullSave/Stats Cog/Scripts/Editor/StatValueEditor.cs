using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CustomEditor(typeof(StatValue))]
    public class StatValueEditor : TOCKEditorV2
    {

        #region Variables

        private StatValue myTarget;
        private ReorderableList commandList;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target is StatValue)
            {
                myTarget = (StatValue)target;

                // Header
                commandList = new ReorderableList(serializedObject, serializedObject.FindProperty("incrementCommand"), true, true, true, true);
                commandList.elementHeight = EditorGUIUtility.singleLineHeight + 4;
                commandList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Commands"); };

                // Elements
                commandList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = commandList.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("Command", null, string.Empty));
                };
            }
        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Stat Value", "Icons/statscog");

            DrawInspector();

            MainContainerEnd();
        }

        #endregion


        #region Window Methods

        internal void DrawInspector()
        {
            SectionHeader("UI");
            SimpleProperty("icon");
            SimpleProperty("iconColor");
            SimpleProperty("displayName");
            SimpleProperty("displayInList");
            SimpleProperty("textColor");

            SectionHeader("Behaviour");
            SimpleProperty("category");
            SimpleProperty("value");
            SimpleProperty("minValue");
            SimpleProperty("maxValue");
            SimpleProperty("startWithMaxValue", "Start w/ Max Value");

            SectionHeader("Regeneration");
            SimpleProperty("enableRegen", "Enable");
            if (serializedObject.FindProperty("enableRegen").boolValue)
            {
                SimpleProperty("regenDelay", "Delay");
                SimpleProperty("regenPerSecond", "Add Per Second");
            }

            SectionHeader("Incrementing");
            SimpleProperty("enableIncrement", "Enable");
            if (serializedObject.FindProperty("enableIncrement").boolValue)
            {
                SimpleProperty("incrementWhen", "Condition");
                SimpleProperty("incrementAmount", "Add Amount");
                commandList.DoLayoutList();
            }
        }

        #endregion

    }
}