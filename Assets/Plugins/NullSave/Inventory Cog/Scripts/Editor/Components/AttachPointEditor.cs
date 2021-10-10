using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AttachPoint))]
    public class AttachPointEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Attach Point", "Icons/attachments", false);

            SectionHeader("General");
            SimpleProperty("pointId", "Point Id");
            SimpleProperty("slotIcon");

            SectionHeader("Gizmos");
            SimpleProperty("drawGizmo");
            SimpleProperty("gizmoScale");

            SectionHeader("Events");
            SimpleProperty("onAttachmentAdded");
            SimpleProperty("onAttachmentRemoved");

            MainContainerEnd();
        }

        #endregion

    }
}