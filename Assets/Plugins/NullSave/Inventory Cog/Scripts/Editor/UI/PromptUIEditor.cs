using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(PromptUI))]
    public class PromptUIditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();
            SectionHeader("Behaviour");
            DrawPropertiesExcluding(serializedObject, "m_Script");
            MainContainerEnd();
        }

        #endregion

    }
}