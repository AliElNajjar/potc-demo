using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CustomEditor(typeof(StatEffectList))]
    public class StatEffectListEditor : TOCKEditorV2
    {

        #region Variables

        private ReorderableList effects;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            effects = new ReorderableList(serializedObject, serializedObject.FindProperty("availableEffects"), true, true, true, true);
            effects.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            effects.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Available Effects"); };
            effects.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = effects.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
            };

        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Stat Effect List", "Icons/effect_list", false);

            SimpleList(serializedObject.FindProperty("availableEffects"), typeof(StatEffect));

            MainContainerEnd();
        }

        #endregion

        #region Window Methods

        internal void AddEffect(StatEffect effect)
        {
            ((StatEffectList)target).availableEffects.Add(effect);
        }

        internal int DrawEffectList()
        {
            int result = -1;
            effects.DoLayoutList();
            if (effects.index > -1)
            {
                if (GUILayout.Button("Edit Selected"))
                {
                    result = effects.index;
                }
            }
            return result;
        }

        #endregion

    }
}