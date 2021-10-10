using UnityEditor;

namespace NullSave.TOCK
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ActionRaycastTrigger))]
    public class ActionRaycastTriggerEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Action Raycast Trigger", "Icons/tock-event");

            SectionHeader("Behaviour");
            SimpleProperty("actionType");
            switch ((NavigationType)serializedObject.FindProperty("actionType").intValue)
            {
                case NavigationType.ByButton:
                    SimpleProperty("actionButton");
                    break;
                case NavigationType.ByKey:
                    SimpleProperty("actionKey");
                    break;
            }
            SimpleProperty("raycastOffset");
            SimpleProperty("raycastCulling");
            SimpleProperty("maxDistance");

            SectionHeader("UI Elements");
            SimpleProperty("actionUI");
            SimpleProperty("setUIText");
            if (serializedObject.FindProperty("setUIText").boolValue)
            {
                SimpleProperty("uiText");
                SimpleProperty("textFormat");
            }

            MainContainerEnd();
        }

        #endregion

    }
}