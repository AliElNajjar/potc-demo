using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StorePoint))]
    public class StorePointEditor : TOCKEditorV2
    {

        #region Variables

        bool showForceUnstore = true;

        #endregion

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            bool temp;

            MainContainerBegin("Store Point", "Icons/store_point_icon", false);

            SectionHeader("General");
            SimpleProperty("pointId", "Store Point Id");

            temp = showForceUnstore;
            showForceUnstore = SectionGroup("Force Other Unstore on Store", showForceUnstore);
            if (temp)
            {
                DragDropList(serializedObject.FindProperty("forceUnstore"), typeof(EquipPoint));
            }

            SectionHeader("Gizmos");
            SimpleProperty("drawGizmo");
            SimpleProperty("gizmoScale", "Scale");

            MainContainerEnd();
        }

        #endregion

    }
}