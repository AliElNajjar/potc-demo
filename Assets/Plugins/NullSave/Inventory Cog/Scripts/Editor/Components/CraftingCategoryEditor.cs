using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(CraftingCategory))]
    public class CraftingCategoryEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Crafting Category", "Icons/category");

            SectionHeader("UI");
            SimpleProperty("icon");
            SimpleProperty("displayName");
            SimpleProperty("description");
            SimpleProperty("displayInList", "Display Category");

            MainContainerEnd();
        }

        #endregion

        #region Window Methods

        internal void DrawInspector()
        {
            SectionHeader("UI");
            SimpleProperty("icon");
            SimpleProperty("displayName");
            SimpleProperty("description");
            SimpleProperty("displayInList", "Display Category");
        }

        #endregion

    }
}