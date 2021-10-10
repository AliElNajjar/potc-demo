using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    [HierarchyIcon("stattext", false)]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class StatMonitorTMP : MonoBehaviour
    {

        #region Variables

        public StatsCog statsCog;
        public string statName;
        public string formattedText = "{4}";
        public bool displayAsInt = false;

        private TextMeshProUGUI text;
        private StatValue stat;

        #endregion

        #region Unity Methods

        public void OnEnable()
        {
            text = GetComponent<TextMeshProUGUI>();
            stat = statsCog.FindStat(statName);
            if (stat == null)
            {
                Debug.LogError(name + ".StatMonitorTMP could not find stat '" + statName + "'");
                enabled = false;
                return;
            }
            stat.onMaxValueChanged.AddListener(UpdateStat);
            stat.onMinValueChanged.AddListener(UpdateStat);
            stat.onValueChanged.AddListener(UpdateStat);
            UpdateStat(0, 0);
        }

        #endregion

        #region Private Methods

        private void UpdateStat(float oldVal, float newVal)
        {
            string val = formattedText;

            if (displayAsInt)
            {
                val = val.Replace("{0}", Mathf.FloorToInt(stat.CurrentBaseMinimum).ToString());
                val = val.Replace("{1}", Mathf.FloorToInt(stat.CurrentBaseValue).ToString());
                val = val.Replace("{2}", Mathf.FloorToInt(stat.CurrentBaseMaximum).ToString());
                val = val.Replace("{3}", Mathf.FloorToInt(stat.CurrentMinimum).ToString());
                val = val.Replace("{4}", Mathf.FloorToInt(stat.CurrentValue).ToString());
                val = val.Replace("{5}", Mathf.FloorToInt(stat.CurrentMaximum).ToString());
            }
            else
            {
                val = val.Replace("{0}", stat.CurrentBaseMinimum.ToString());
                val = val.Replace("{1}", stat.CurrentBaseValue.ToString());
                val = val.Replace("{2}", stat.CurrentBaseMaximum.ToString());
                val = val.Replace("{3}", stat.CurrentMinimum.ToString());
                val = val.Replace("{4}", stat.CurrentValue.ToString());
                val = val.Replace("{5}", stat.CurrentMaximum.ToString());
            }

            text.text = val;
        }

        #endregion

    }
}