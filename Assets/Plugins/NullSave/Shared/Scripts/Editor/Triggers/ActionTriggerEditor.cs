using UnityEditor;

namespace NullSave.TOCK
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ActionTrigger))]
    public class ActionTriggerEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Action Trigger", "Icons/tock-event");

            SectionHeader("Behaviour");
            SimpleProperty("actionKey");
            SimpleProperty("fillTime");
            SimpleProperty("container");
            SimpleProperty("hideAfterUse");

            SectionHeader("UI Elements");
            SimpleProperty("fillLeft");
            SimpleProperty("fillCenter");
            SimpleProperty("fillRight");

            SectionHeader("Menu");
            SimpleProperty("actionMenu");
            SimpleProperty("actionOpen");
            switch ((MenuOpenType)serializedObject.FindProperty("actionOpen").intValue)
            {
                case MenuOpenType.SpawnInTransform:
                    SimpleProperty("menuContainer", "Parent Transform");
                    break;
                case MenuOpenType.SpawnInTag:
                    SimpleProperty("spawnTag", "Target Tag");
                    break;
            }

            SectionHeader("Events");
            SimpleProperty("onActionTriggered");


            MainContainerEnd();
        }

        #endregion

    }
}