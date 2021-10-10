using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(TriggerAfterTime))]
    public class TriggerAfterTimeEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            SimpleProperty("timeDelay");
            SimpleProperty("loop");

            SectionHeader("Events");
            SimpleProperty("onTimeElapsed");

            MainContainerEnd();
        }

        #endregion

    }
}