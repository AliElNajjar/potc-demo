using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(RecipeComponentUI))]
    public class RecipeComponentUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Recipe Component UI", "Icons/tock-tag");

            SectionHeader("UI Elements");
            SimpleProperty("itemImage");
            SimpleProperty("displayName");
            SimpleProperty("description");
            SimpleProperty("subtext");
            SimpleProperty("hideIfNoSubtext");
            SimpleProperty("countNeeded");
            SimpleProperty("countAvailable");
            SimpleProperty("rarityColorIndicator");

            SectionHeader("Minimum Stat Sliders");
            SimpleProperty("conditionSlider");
            SimpleProperty("hideIfConditionZero", "Hide for Zero");
            SimpleProperty("raritySlider");
            SimpleProperty("hideIfRarityZero", "Hide for Zero");

            SectionHeader("Primary Color");
            SimpleProperty("availableColor", "Available");
            SimpleProperty("unavailableColor", "Unavailable");
            SimpleMultiSelect("colorApplication", "Apply To");

            SectionHeader("Secondary Color");
            SimpleProperty("availableColor2", "Available");
            SimpleProperty("unavailableColor2", "Unavailable");
            SimpleMultiSelect("colorApplication2", "Apply To");

            MainContainerEnd();
        }

        #endregion

    }
}