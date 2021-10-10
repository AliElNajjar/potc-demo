using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(InventoryComponentList))]
    public class InventoryComponentListEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Component List", "Icons/tock-list");
            DrawPropertiesExcluding(serializedObject, "m_Script");
            MainContainerEnd();
        }

        #endregion

    }
}
