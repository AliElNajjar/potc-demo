using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(InventoryListMonitor))]
    public class InventoryListMonitorEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Inventory List Monitor", "Icons/tock-eent", false);

            SectionHeader("Behaviour");
            SimpleProperty("targetList");

            SectionHeader("Events");
            SimpleProperty("onCanEquip");
            SimpleProperty("onCannotEquip");
            SimpleProperty("onCanRemove");
            SimpleProperty("onCannotRemove");
            SimpleProperty("onCanModify");
            SimpleProperty("onCannotModify");
            SimpleProperty("onCanRepair");
            SimpleProperty("onCannotRepair");
            SimpleProperty("onCanDrop");
            SimpleProperty("onCannotDrop");
            SimpleProperty("onCanBreakdown");
            SimpleProperty("onCannotBreakDown");
            SimpleProperty("onCanAttach");
            SimpleProperty("onCannotAttach");
            SimpleProperty("onCanUnattach");
            SimpleProperty("onCannotUnAttach");
            SimpleProperty("onCanRename");
            SimpleProperty("onCannotRename");
            SimpleProperty("onIsContainer");
            SimpleProperty("onIsNotContainer");
            SimpleProperty("onIsSkill");
            SimpleProperty("onIsNotSkill");

            MainContainerEnd();
        }

        #endregion

    }
}