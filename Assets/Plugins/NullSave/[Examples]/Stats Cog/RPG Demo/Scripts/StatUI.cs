using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Stats
{
    public class StatUI : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI statName, currentValue, baseValue, modAmount;
        public bool hideIfModAmountZero;
        public bool displayAsInt;
        public Color positiveMod = Color.green;
        public Color negativeMod = Color.red;

        #endregion

        #region Properties

        public StatsCog StatsCog { get; set; }

        public StatValue StatValue { get; set; }

        #endregion

        #region Public Methods

        public void LoadStat(StatsCog statsCog, string statName)
        {
            StatsCog = statsCog;
            StatValue = StatsCog.FindStat(statName);
            Subscribe();
            UpdateUI(0, 0);
        }

        #endregion

        #region Private Methods

        private void Subscribe()
        {
            if (StatValue != null)
            {
                StatValue.onBaseValueChanged.AddListener(UpdateUI);
                StatValue.onValueChanged.AddListener(UpdateUI);
            }
        }

        private void Unsubscribe()
        {
            if(StatValue != null)
            {
                StatValue.onBaseValueChanged.RemoveListener(UpdateUI);
                StatValue.onValueChanged.RemoveListener(UpdateUI);
            }
        }

        private void UpdateUI(float oldValue, float newValue)
        {
            if (StatValue == null) return;

            if (statName) statName.text = StatValue.displayName;
            if (baseValue) baseValue.text = displayAsInt ? ((int)StatValue.CurrentBaseValue).ToString() : StatValue.CurrentBaseValue.ToString();
            if (currentValue) currentValue.text = displayAsInt ? ((int)StatValue.CurrentValue).ToString() : StatValue.CurrentValue.ToString();

            if(modAmount)
            {
                float diff = StatValue.CurrentValue - StatValue.CurrentBaseValue;
                modAmount.text = displayAsInt ? ((int)diff).ToString() : diff.ToString();
                if (diff > 0)
                {
                    modAmount.color = positiveMod;
                    modAmount.gameObject.SetActive(true);
                }
                else if (diff < 0)
                {
                    modAmount.color = negativeMod;
                    modAmount.gameObject.SetActive(true);
                }
                else if(hideIfModAmountZero)
                {
                    modAmount.gameObject.SetActive(false);
                }
            }
        }

        #endregion

    }
}