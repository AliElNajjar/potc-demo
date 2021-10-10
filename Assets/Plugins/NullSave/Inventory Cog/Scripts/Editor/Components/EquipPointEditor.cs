using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EquipPoint))]
    public class EquipPointEditor : TOCKEditorV2
    {

        #region Variables

        bool showForceStore = true;
        bool showForceUnequip = true;

        #endregion

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            bool temp;

            MainContainerBegin("Equip Point", "Icons/equip_point_icon", false);

            SectionHeader("General");
            SimpleProperty("pointId", "Equip Point Id");
            SimpleProperty("storePoint");

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

            SectionHeader("Gizmos");
            SimpleProperty("drawGizmo");
            SimpleProperty("gizmoScale");

            MainContainerEnd();
        }

        #endregion

    }
}