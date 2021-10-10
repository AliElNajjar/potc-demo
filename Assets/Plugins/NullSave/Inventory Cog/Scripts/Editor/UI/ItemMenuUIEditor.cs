using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(ItemMenuUI))]
    public class ItemMenuUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Item Menu UI", "Icons/tock-menu");

            SectionHeader("Behaviour");
            SimpleProperty("allowAutoWrap");
            SimpleProperty("allowSelectByClick", "Enable Click Select");

            SectionHeader("Navigation");
            SimpleProperty("navigationMode");
            switch((NavigationType)serializedObject.FindProperty("navigationMode").intValue)
            {
                case NavigationType.ByButton:
                    SimpleProperty("navButton");
                    break;
                case NavigationType.ByKey:
                    SimpleProperty("backKey");
                    SimpleProperty("nextKey");
                    break;
            }
            if ((NavigationType)serializedObject.FindProperty("navigationMode").intValue != NavigationType.Manual)
            {
                SimpleProperty("invertInput");
                SimpleProperty("autoRepeat");
                if (serializedObject.FindProperty("autoRepeat").boolValue)
                {
                    SimpleProperty("repeatDelay");
                }
            }
            SimpleProperty("selectionMode");
            switch ((NavigationType)serializedObject.FindProperty("selectionMode").intValue)
            {
                case NavigationType.ByButton:
                    SimpleProperty("buttonSubmit");
                    break;
                case NavigationType.ByKey:
                    SimpleProperty("keySubmit");
                    break;
            }

            MainContainerEnd();
        }

        #endregion

    }
}
