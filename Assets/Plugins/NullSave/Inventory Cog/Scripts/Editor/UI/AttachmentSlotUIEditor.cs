using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(AttachmentSlotUI))]
    public class AttachmentSlotUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Attachment Slot UI", "Icons/attachment");

            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });

            MainContainerEnd();
        }

        #endregion

    }
}