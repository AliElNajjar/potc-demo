using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(PageIndicatorUI))]
    public class PageIndicatorUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Page Indicator", "Icons/category_page");
            DrawPropertiesExcluding(serializedObject, "m_Script");
            MainContainerEnd();
        }

        #endregion

    }
}