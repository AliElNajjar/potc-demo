using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ItemPreviewUI))]
    public class ItemPreviewUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item Preview UI", "Icons/view");

            SimpleProperty("previewContainer");

            MainContainerEnd();
        }

        #endregion

    }
}