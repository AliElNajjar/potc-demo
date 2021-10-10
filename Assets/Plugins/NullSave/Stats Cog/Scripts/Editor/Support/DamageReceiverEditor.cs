using UnityEditor;

namespace NullSave.TOCK.Stats
{
    [CustomEditor(typeof(DamageReceiver))]
    public class DamageReceiverEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Events");
            SimpleProperty("onTakeDamage");

            MainContainerEnd();
        }

        #endregion

    }
}