using UnityEditor;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [CustomEditor(typeof(StatMonitorTMP))]
    public class StatMonitorTMPEditor : TOCKEditorV2
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin("Stat Monitor (TMP)", "Icons/stattext");

            SimpleProperty("statsCog");
            SimpleProperty("statName");
            SimpleProperty("formattedText");
            SimpleProperty("displayAsInt", "Values to Int");

            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("{0} - Base Minimum\r\n" +
                "{1} - Base Value\r\n" +
                "{2} - Base Maximum", Skin.GetStyle("WrapText"));

            GUILayout.Label("{3} - Minimum\r\n" +
                "{4} - Value\r\n" +
                "{5} - Maximum", Skin.GetStyle("WrapText"));

            GUILayout.EndHorizontal();



            MainContainerEnd();

        }

        #endregion

    }
}