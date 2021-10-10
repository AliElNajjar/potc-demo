using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(InventoryMessageListClient))]
    public class InventoryMessageListClientEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Message List Client", "Icons/tock-list");

            SectionHeader("Behaviour");
            SimpleProperty("inventoryCog");
            SimpleProperty("messageList");
            SimpleProperty("showItemAdd");

            SectionHeader("Add Item Formatting");
            SimpleProperty("itemAddFormatSingle", "Single");
            SimpleProperty("itemAddFormatMulti", "Multiple");

            MainContainerEnd();
        }

        #endregion

    }
}
