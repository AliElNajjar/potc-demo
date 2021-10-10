using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(CraftingCategoryList))]
    public class CraftingCategoryListEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Crafting Category List", "Icons/tock-tag-list");

            SectionHeader("Behaviour");
            SimpleProperty("showAllItems");
            if (serializedObject.FindProperty("showAllItems").boolValue)
            {
                SimpleProperty("allItemsIcon");
                SimpleProperty("allItemsText");
            }
            SimpleProperty("categoryPrefab");
            SimpleProperty("prefabContainer");
            SimpleProperty("bindToList", "Recipe Client");

            SectionHeader("Filtering");
            SimpleProperty("categoryFilter");
            if ((ListCategoryFilter)serializedObject.FindProperty("categoryFilter").intValue != ListCategoryFilter.All)
            {
                SimpleList("categories", typeof(CraftingCategory));
            }

            SectionHeader("Navigation");
            SimpleProperty("navMode");
            switch ((NavigationType)serializedObject.FindProperty("navMode").intValue)
            {
                case NavigationType.ByButton:
                    SimpleProperty("navButton");
                    break;
                case NavigationType.ByKey:
                    SimpleProperty("backKey");
                    SimpleProperty("nextKey");
                    break;
            }
            SimpleProperty("allowAutoWrap");

            SectionHeader("Events");
            SimpleProperty("onSelectionChanged");

            MainContainerEnd();
        }

        #endregion

    }
}
