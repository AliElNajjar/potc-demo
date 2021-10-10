using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(LootItem))]
    public class LootItemEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            LootItem myTarget = (LootItem)target;

            MainContainerBegin("Loot Item", "Icons/loot_item");

            SectionHeader("Behaviour");
            SimpleProperty("currency");
            SimpleProperty("item");
            SimpleProperty("count");
            SimpleProperty("autoPickup");

            if (myTarget.item != null)
            {
                if (myTarget.item.canEquip)
                {
                    SimpleProperty("autoEquip");
                }

                if (myTarget.item.itemType == ItemType.Consumable)
                {
                    SimpleProperty("autoConsumeWhen");
                }
            }


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

            MainContainerEnd();
        }

        #endregion

    }
}