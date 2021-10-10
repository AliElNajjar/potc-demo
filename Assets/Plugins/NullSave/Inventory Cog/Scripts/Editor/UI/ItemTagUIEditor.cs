using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(ItemTagUI))]
    public class ItemTagUIUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item Tag UI", "Icons/category");
            DrawPropertiesExcluding(serializedObject, "m_Script");
            MainContainerEnd();
        }

        #endregion

    }
}