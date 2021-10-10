using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ComponentUI))]
    public class ComponentUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Component UI", "Icons/tock-tag");

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

            SectionHeader("Color");
            SimpleProperty("availableColor");
            SimpleProperty("unavailableColor");

            MainContainerEnd();
        }

        #endregion

    }
}