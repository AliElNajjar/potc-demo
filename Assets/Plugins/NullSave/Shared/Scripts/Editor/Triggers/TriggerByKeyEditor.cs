using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(TriggerByKey))]
    public class TriggerByKeyEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            SimpleProperty("key");

            SectionHeader("Events");
            SimpleProperty("onKey");
            SimpleProperty("onKeyDown");
            SimpleProperty("onKeyUp");

            MainContainerEnd();
        }

        #endregion

    }
}