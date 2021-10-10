using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(LootItemWithUI))]
    public class LootItemWithUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Loot Item (with UI)", "Icons/loot_item");

            SectionHeader("Behaviour");
            SimpleProperty("item");
            SimpleProperty("count");
            SimpleProperty("autoEquip");
            SimpleProperty("pickupMode");
            switch ((NavigationType)serializedObject.FindProperty("pickupMode").intValue)
            {
                case NavigationType.ByButton:
                    SimpleProperty("pickupButton");
                    break;
                case NavigationType.ByKey:
                    SimpleProperty("pickupKey");
                    break;
            }
            SimpleProperty("pickupOnRequest");

            SectionHeader("UI");
            SimpleProperty("pickupUI");
            SimpleProperty("uiOpenType", "Display Type");
            switch ((MenuOpenType)serializedObject.FindProperty("uiOpenType").intValue)
            {
                case MenuOpenType.SpawnInTag:
                    SimpleProperty("uiTag", "Target Tag");
                    break;
                case MenuOpenType.SpawnInTransform:
                    SimpleProperty("uiParent", "Parent Transform");
                    break;
            }

            SectionHeader("Loot Settings");
            SimpleProperty("rarityGen", "Loot Rarity");
            switch ((GenerationType)serializedObject.FindProperty("rarityGen").intValue)
            {
                case GenerationType.Constant:
                    SimpleProperty("rarity1", "Rarity");
                    break;
                case GenerationType.RandomBetweenConstants:
                    SimpleProperty("rarity1", "Min");
                    SimpleProperty("rarity2", "Max");
                    break;
            }

            SimpleProperty("conditionGen", "Loot Condition");
            switch ((GenerationType)serializedObject.FindProperty("conditionGen").intValue)
            {
                case GenerationType.Constant:
                    SimpleProperty("condition1", "Condition");
                    break;
                case GenerationType.RandomBetweenConstants:
                    SimpleProperty("condition1", "Min");
                    SimpleProperty("condition2", "Max");
                    break;
            }

            SimpleProperty("valueGen", "Loot Value");
            switch ((GenerationType)serializedObject.FindProperty("valueGen").intValue)
            {
                case GenerationType.Constant:
                    SimpleProperty("value1", "Value");
                    break;
                case GenerationType.RandomBetweenConstants:
                    SimpleProperty("value1", "Min");
                    SimpleProperty("value2", "Max");
                    break;
            }

            SectionHeader("Events");
            SimpleProperty("onLoot");
            SimpleProperty("onPickupRequested");

            MainContainerEnd();
        }

        #endregion

    }
}