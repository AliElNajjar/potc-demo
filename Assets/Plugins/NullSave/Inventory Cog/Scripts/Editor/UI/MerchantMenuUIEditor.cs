using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(MerchantMenuUI))]
    public class MerchantMenuUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Merchant Menu", "Icons/tock-menu");

            SectionHeader("Behaviour");
            SimpleProperty("loadMode");
            SimpleProperty("playerInventory", "Player Inventory");
            SimpleProperty("merchantInventory", "Merchant Inventory");

            SectionHeader("UI");
            SimpleProperty("playerList", "Player List");
            SimpleProperty("playerCurrency");
            SimpleProperty("playerFormat");
            SimpleProperty("merchantList", "Merchant List");
            SimpleProperty("merchantCurrency");
            SimpleProperty("merchantFormat");

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
