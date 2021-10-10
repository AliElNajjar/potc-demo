using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(InventoryMenuUI))]
    public class InventoryMenuUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Inventory Menu UI", "Icons/tock-menu", false);

            SectionHeader("Behaviour");
            SimpleProperty("closeMode");
            switch ((NavigationType)serializedObject.FindProperty("closeMode").intValue)
            {
                case NavigationType.ByButton:
                    SimpleProperty("closeButton", "Button");
                    break;
                case NavigationType.ByKey:
                    SimpleProperty("closeKey", "Key");
                    break;
            }

            //SectionHeader("Components");
            //SimpleProperty("categoryUI", "Category UI Prefab");
            //SimpleProperty("categoryParent", "Categories Container");
            //SimpleProperty("categoryName");
            //SimpleProperty("categoryPage");
            //SimpleProperty("itemsList", "Category Items");

            //SectionHeader("Paging");
            //SimpleProperty("pageBack");
            //SimpleProperty("pageNext");
            //SimpleProperty("alwaysShowBack");
            //SimpleProperty("alwaysShowNext");

            //SectionHeader("Events");
            //SimpleProperty("onSelectedCategoryChanged");
            //SimpleProperty("onSelectedItemChanged");

            MainContainerEnd();
        }

        #endregion

    }
}