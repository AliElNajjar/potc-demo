using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(CategoryUI))]
    public class CategoryUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Category UI", "Icons/category");

            SectionHeader("Category Info");
            SimpleProperty("categoryImage", "Icon");
            SimpleProperty("categoryName", "Name");
            SimpleProperty("activeColor", "Active Icon");
            SimpleProperty("inactiveColor", "Inactive Icon");
            SimpleProperty("activeTextColor", "Active Text");
            SimpleProperty("inactiveTextColor", "Inactive Text");

            SectionHeader("Page Indictors");
            SimpleProperty("pageIndicator", "Prefab");
            SimpleProperty("indicatorParent", "Container");
            SimpleProperty("indicatorMode", "Indicator Mode");
            SimpleProperty("includeLocked", "Include Locked");
            SimpleProperty("slotsPerPage", "Items Per Page");
            SimpleProperty("onlyMoreThanOne", "Hide If < 2 Pages");

            MainContainerEnd();
        }

        #endregion

    }
}