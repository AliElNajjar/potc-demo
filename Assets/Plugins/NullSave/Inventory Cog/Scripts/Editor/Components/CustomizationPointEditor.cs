using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CustomEditor(typeof(CustomizationPoint))]
    public class CustomizationPointEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Customization Point", "Icons/customization_point");
            DrawPropertiesExcluding(serializedObject, "m_Script");
            MainContainerEnd();
        }

        #endregion

    }
}