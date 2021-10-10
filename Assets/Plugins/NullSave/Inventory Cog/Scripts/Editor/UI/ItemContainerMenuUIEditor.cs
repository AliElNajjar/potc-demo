using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ItemContainerMenuUI))]
    public class ItemContainerMenuUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item Container Menu", "Icons/tock-menu", false);

            SectionHeader("Behaviour");
            SimpleProperty("itemName");
            SimpleProperty("itemList");
            SimpleProperty("inventoryList");

            MainContainerEnd();
        }

        #endregion

    }
}