using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(InventoryItemUITag))]
    public class InventoryItemUITagEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item Tag", "Icons/category");
            DrawPropertiesExcluding(serializedObject, "m_Script");
            MainContainerEnd();
        }

        #endregion

    }
}