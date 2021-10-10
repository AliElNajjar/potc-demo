using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(InventoryContainer))]
    public class InventoryContainerEditor : TOCKEditorV2
    {

        #region Varaibles

        private Vector2 itemsSR;

        #endregion

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Inventory Container", "Icons/item_icon", false);

            SectionHeader("UI");
            SimpleProperty("displayName");

            SectionHeader("Behaviour");
            SimpleProperty("hasMaxStoreSlots", "Has Max Slots");
            if (serializedObject.FindProperty("hasMaxStoreSlots").boolValue)
            {
                SimpleProperty("maxStoreSlots", "Max Slots");
            }
            SimpleProperty("hasMaxStoreWeight", "Has Max Weight");
            if (serializedObject.FindProperty("hasMaxStoreWeight").boolValue)
            {
                SimpleProperty("maxStoreWeight", "Max Weight");
            }

            SectionHeader("Starting Items");
            ItemDrop();
            itemsSR = SimpleList("startingItems", itemsSR, 120, 1);

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void ItemDrop()
        {
            SerializedProperty list = serializedObject.FindProperty("startingItems");
            Type acceptedType = typeof(InventoryItem);
            GUILayout.BeginVertical();
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUI.skin.box.normal.textColor = Color.white;

            DragBox("Drag & Drop Inventory Items");

            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                list.arraySize++;
                                list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("item").objectReferenceValue = dragged;
                                list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("count").intValue = 1;
                                itemsSR.y = int.MaxValue;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            GUILayout.EndVertical();
        }

        #endregion

    }
}