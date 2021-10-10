using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(SlotItemUI))]
    public class SlotItemUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Slot Item UI", "Icons/item_icon", false);

            SectionHeader("Item Info");
            SimpleProperty("slotId");
            SimpleProperty("itemImage", "Icon");
            SimpleProperty("displayName", "Name");
            SimpleProperty("description");
            SimpleProperty("subtext");
            SimpleProperty("hideIfNoSubtext");

            SectionHeader("Drag and Drop");
            SimpleProperty("enableDragDrop", "Enable");

            SectionHeader("Count");
            SimpleProperty("countPrefix", "Prefix");
            SimpleProperty("count", "Text");
            SimpleProperty("countSuffix", "Suffix");
            SimpleProperty("hideIfCountSub2", "Hide if Count < 2");

            SectionHeader("Indicators");
            SimpleProperty("selectedIndicator", "Selected");
            SimpleProperty("equipableIndicator", "Can Equip");
            SimpleProperty("equippedIndicator", "Is Equiped");
            SimpleProperty("rarityColorIndicator");
            SimpleProperty("conditionSlider");
            SimpleProperty("hideIfConditionZero", "Hide if Condition 0");
            SimpleProperty("raritySlider");
            SimpleProperty("hideIfRarityZero", "Hide if Rarity 0");

            SectionHeader("Tags");
            SimpleProperty("tagPrefab", "Prefab");
            SimpleProperty("tagContainer", "Container");

            SectionHeader("Events");
            SimpleProperty("onClick");
            SimpleProperty("onLoadedItem");

            MainContainerEnd();
        }

        #endregion

    }
}