using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(ContainerMenuUI))]
    public class ContainerMenuUIEditor :TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Container Menu", "Icons/tock-menu");

            SectionHeader("Behaviour");
            SimpleProperty("loadMode");
            SimpleProperty("inventory", "Player Inventory");
            SimpleProperty("container", "Container Inventory");

            SectionHeader("UI");
            SimpleProperty("localInventory", "Player List");
            SimpleProperty("containerInventory", "Container List");

            SectionHeader("Navigation");
            SimpleProperty("closeMode");
            switch ((NavigationType)serializedObject.FindProperty("closeMode").intValue)
            {
                case NavigationType.ByButton:
                    SimpleProperty("closeButton");
                    break;
                case NavigationType.ByKey:
                    SimpleProperty("closeKey");
                    break;
            }

            SectionHeader("Events");
            SimpleProperty("onOpen");
            SimpleProperty("onClose");

            MainContainerEnd();
        }

        #endregion

    }
}
