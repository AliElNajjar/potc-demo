using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(CountSelectUI))]
    public class CountSelectUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Count Select UI", "Icons/tock-ui");
            DrawPropertiesExcluding(serializedObject, "m_Script");
            MainContainerEnd();
        }

        #endregion

    }
}
