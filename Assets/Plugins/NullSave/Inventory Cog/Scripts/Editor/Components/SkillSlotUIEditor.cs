using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SkillSlotUI))]
    public class SkillSlotUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            SimpleProperty("inventorySource");
            SimpleProperty("slotName");

            SectionHeader("UI");
            SimpleProperty("skillIcon");
            SimpleProperty("equippedIcon");

            MainContainerEnd();
        }

        #endregion

    }
}