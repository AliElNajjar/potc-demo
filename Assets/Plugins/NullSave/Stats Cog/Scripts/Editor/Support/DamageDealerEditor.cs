using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CustomEditor(typeof(DamageDealer))]
    public class DamageDealerEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBeginSlim();

            SectionHeader("Behaviour");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minBetweenDamage"), new GUIContent("Sec Between Damage", null, string.Empty));

            SectionHeader("Damage");
            SimpleList("damage");

#if STATS_COG
            SectionHeader("Effects");
            SimpleList("effects");
#endif

            SectionHeader("Events");
            SimpleProperty("onDamageDealt");
            SimpleProperty("onColliderEnabled");
            SimpleProperty("onColliderDisabled");

            MainContainerEnd();
        }

        #endregion

    }
}