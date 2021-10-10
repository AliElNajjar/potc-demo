using UnityEditor;

namespace NullSave.TOCK.Inventory
{
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(EquipPointUI))]
    public class EquipPointUIEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Equip Point UI", "Icons/tock-equip");

            SectionHeader("Behaviour");
            SimpleProperty("inventoryCog");
            SimpleProperty("equipPointId");
            SimpleProperty("findByTag");
            if (serializedObject.FindProperty("findByTag").boolValue)
            {
                SimpleProperty("targetTag");
            }

            SectionHeader("UI Elements");
            SimpleProperty("itemIcon");
            SimpleProperty("itemName");
            SimpleProperty("rarityColor");
            SimpleProperty("ammoContainer");
            SimpleProperty("ammoCount");

            MainContainerEnd();
        }

        #endregion

    }
}