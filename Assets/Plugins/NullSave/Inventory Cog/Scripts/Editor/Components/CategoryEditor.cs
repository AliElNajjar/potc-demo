using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(Category))]
    public class CategoryEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item Category", "Icons/category");

            DrawInspector();

            MainContainerEnd();
        }

        #endregion

        #region Internal Methods

        internal void DrawInspector()
        {
            SectionHeader("UI");
            SimpleProperty("icon");
            SimpleProperty("displayName");
            SimpleProperty("description");

            SectionHeader("Behaviour");
            SimpleProperty("displayInList", "Display Category");
            SimpleProperty("catUnlocked", "Category Unlocked");
            SimpleProperty("hasMaxSlots", "Has Max Slots");
            if (serializedObject.FindProperty("hasMaxSlots").boolValue)
            {
                SimpleProperty("maxSlotSource", "Max Slots Source");
                if ((ValueSource)serializedObject.FindProperty("maxSlotSource").intValue == ValueSource.Static)
                {
                    SimpleProperty("maxSlots", "Max Slots");
                }
                else
                {
                    SimpleProperty("maxSlotStat", "Max Slots");
                }
            }
            SimpleProperty("hasLockingSlots", "Has Locking Slots");
            if (serializedObject.FindProperty("hasLockingSlots").boolValue)
            {
                SimpleProperty("unlockedSource", "Unlocked Slots Source");
                if ((ValueSource)serializedObject.FindProperty("unlockedSource").intValue == ValueSource.Static)
                {
                    SimpleProperty("unlockedSlots", "Unlocked Slots");
                }
                else
                {
                    SimpleProperty("unlockedStat", "Unlocked Slots");
                }
            }
            SimpleProperty("ammoIsSlotless");
        }

        #endregion

    }
}