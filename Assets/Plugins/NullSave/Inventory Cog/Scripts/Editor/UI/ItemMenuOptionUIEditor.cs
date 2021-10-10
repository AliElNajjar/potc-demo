using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(ItemMenuOptionUI))]
    public class ItemMenuOptionUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item Menu Option", "Icons/tock-menu-item");
            DrawPropertiesExcluding(serializedObject, "m_Script");
            MainContainerEnd();
        }

        #endregion

    }
}
