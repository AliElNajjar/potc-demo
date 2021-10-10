using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(InventoryMerchant))]
    public class InventoryMerchantEditor : TOCKEditorV2
    {

        #region Variables

        private Vector2 spStock;

        #endregion

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Inventory Merchant", "Icons/inventory-coin");

            SectionHeader("Behaviour");
            SimpleProperty("displayName");
            SimpleProperty("buyModifier");
            SimpleProperty("sellModifier");
            //SimpleProperty("allowBuyBack");
            //if (serializedObject.FindProperty("allowBuyBack").boolValue)
            //{
            //    SimpleProperty("buybackModifier");
            //}
            SimpleProperty("limitVendorCurrency");
            if (serializedObject.FindProperty("limitVendorCurrency").boolValue)
            {
                SimpleProperty("currency");
            }
            SimpleProperty("stockReplenishes");
            if (serializedObject.FindProperty("stockReplenishes").boolValue)
            {
                SimpleProperty("replenishTime");
            }

            SectionHeader("Count Selection");
            SimpleProperty("allowMulticount", "Use Count Selection");
            if (serializedObject.FindProperty("allowMulticount").boolValue)
            {
                SimpleProperty("minToShowCount");
                SimpleProperty("countSelectUI");
                SimpleProperty("countContainer");
            }

            SectionHeader("Available Stock");
            DrawStock();

            MainContainerEnd();
        }

        #endregion

        #region Private Methods

        private void DrawStock()
        {
            // Draw Drop
            GUILayout.BeginVertical();
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUI.skin.box.normal.textColor = Color.white;

            DragBox("Drag Items Here");

            SerializedProperty list = serializedObject.FindProperty("availableItems");
            System.Type accpetedType = typeof(InventoryItem);
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
                            if (dragged.GetType() == accpetedType || dragged.GetType().BaseType == accpetedType
                                || dragged.GetType().GetNestedTypes().Contains(accpetedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(accpetedType) != null))
                            {
                                list.arraySize++;
                                list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("count").intValue = 1;
                                list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("item").objectReferenceValue = dragged;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            GUILayout.EndVertical();

            EditorGUILayout.Separator();
            spStock = SimpleList("availableItems", spStock, 200, 2);
        }

        #endregion

    }
}
