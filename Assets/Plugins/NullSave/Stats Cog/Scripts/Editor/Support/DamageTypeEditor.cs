using UnityEditor;

namespace NullSave.TOCK
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DamageType))]
    public class DamageTypeEditor : TOCKEditorV2
    {

        #region Unity Events

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Damage Type", "icons/damage");

            SimpleProperty("displayName");
            SimpleProperty("icon");

            MainContainerEnd();
        }

        #endregion


    }
}