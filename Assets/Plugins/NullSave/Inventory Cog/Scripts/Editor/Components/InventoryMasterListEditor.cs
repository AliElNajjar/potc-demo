using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(InventoryMasterList))]
    public class InventoryMasterListEditor : TOCKEditorV2
    {

        #region Variables

        private InventoryMasterList myTarget;
        private ReorderableList categories;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            myTarget = (InventoryMasterList)target;

            categories = new ReorderableList(serializedObject, serializedObject.FindProperty("categories"), true, true, true, true);
            categories.elementHeight = EditorGUIUtility.singleLineHeight + 2;
            categories.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Categories"); };
            categories.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = categories.serializedProperty.GetArrayElementAtIndex(index);
                if (element.objectReferenceValue == null)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent("{null}", null, string.Empty));
                }
                else
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent(((Category)element.objectReferenceValue).displayName, null, string.Empty));
                }
            };

        }

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Master List", "Icons/category");

            SectionHeader("Categories");
            DragBox(serializedObject.FindProperty("categories"), typeof(Category));
            categories.DoLayoutList();

            SectionHeader("Available Items");
            SimpleList(serializedObject.FindProperty("availableItems") , typeof(InventoryItem));

            MainContainerEnd();
        }

        #endregion

    }
}
