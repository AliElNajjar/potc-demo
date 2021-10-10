using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(RenamePrompt))]
    public class RenamePromptEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("UI");
            SimpleProperty("currentName");
            SimpleProperty("currentNameFormat", "Format");
            SimpleProperty("newName");
            SimpleProperty("newName_TMP");
            SimpleProperty("okButton");
            SimpleProperty("cancelButton");

            SectionHeader("Events");
            SimpleProperty("onRename");
            SimpleProperty("onCancel");

            MainContainerEnd();
        }

        #endregion

    }
}