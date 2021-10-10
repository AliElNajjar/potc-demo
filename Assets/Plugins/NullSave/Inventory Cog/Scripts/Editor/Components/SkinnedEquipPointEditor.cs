using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SkinnedEquipPoint))]
    public class SkinnedEquipPointEditor : TOCKEditorV2
    {

        #region Variables

        bool showForceStore = true;
        bool showForceUnequip = true;

        #endregion

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            bool temp;

            MainContainerBegin("Equip Point (Skinned)", "Icons/skinned_equip_point");

            SectionHeader("General");
            SimpleProperty("pointId", "Equip Point Id");
            SimpleProperty("storePoint");

            SectionHeader("Skinning");
            SimpleProperty("boneSource");
            SimpleProperty("defaultSkin");

            temp = showForceStore;
            showForceStore = SectionGroup("Force Other Storage on Equip", showForceStore);
            if (temp)
            {
                DragDropList(serializedObject.FindProperty("forceStore"), typeof(EquipPoint));
            }

            temp = showForceUnequip;
            showForceUnequip = SectionGroup("Force Other Unequip on Equip", showForceUnequip);
            if (temp)
            {
                DragDropList(serializedObject.FindProperty("forceUnequip"), typeof(EquipPoint));
            }

            serializedObject.FindProperty("drawGizmo").boolValue = false;

            MainContainerEnd();
        }

        #endregion

    }
}