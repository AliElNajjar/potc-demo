using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(TriggerByButton))]
    public class TriggerByButtonEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            SimpleProperty("buttonName");

            SectionHeader("Events");
            SimpleProperty("onButton");
            SimpleProperty("onButtonDown");
            SimpleProperty("onButtonUp");

            MainContainerEnd();
        }

        #endregion

    }
}