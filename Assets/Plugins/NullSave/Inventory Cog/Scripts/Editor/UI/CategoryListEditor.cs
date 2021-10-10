using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(CategoryList))]
    public class CategoryListEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Category List", "Icons/tock-tag-list");

            SectionHeader("Behaviour");
            SimpleProperty("allowClickSelect");
            SimpleProperty("showAllItems");
            if (serializedObject.FindProperty("showAllItems").boolValue)
            {
                SimpleProperty("allItemsIcon");
                SimpleProperty("allItemsText");
            }
            SimpleProperty("categoryPrefab");
            SimpleProperty("prefabContainer");

            SectionHeader("Filtering");
            SimpleProperty("categoryFilter");
            if ((ListCategoryFilter)serializedObject.FindProperty("categoryFilter").intValue != ListCategoryFilter.All)
            {
                SimpleList("categories", typeof(Category));
            }

            SectionHeader("List Control");
            SimpleProperty("bindInventory", "Inventory Client");
            SimpleProperty("bindContainer", "Container Client");

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
