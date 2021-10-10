using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(StatModifierUI))]
    public class StatModifierUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Stat Modifier UI", "Icons/inventory-stats");

            SectionHeader("UI");
            SimpleProperty("icon");
            SimpleProperty("statName");
            SimpleProperty("displayText");
            SimpleProperty("oldValue");
            SimpleProperty("newValue");
            SimpleProperty("modifierValue");
            SimpleProperty("reduceSizeOnHide");

            SectionHeader("Hide when Equipped");
            SimpleList("hideWhenEquipped", typeof(GameObject));

            MainContainerEnd();
        }

        #endregion

    }
}