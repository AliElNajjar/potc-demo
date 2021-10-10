using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(AttachmentsUI))]
    public class AttachmentsUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Attachments UI", "Icons/attachment");

            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });

            MainContainerEnd();
        }

        #endregion

    }
}