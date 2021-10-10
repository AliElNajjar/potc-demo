using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Stats
{
    [RequireComponent(typeof(Slider))]
    [HierarchyIcon("statslider", false)]
    public class SliderStat : MonoBehaviour
    {

        #region Variables

        public StatsCog statCog;
        public string valueName;
        public string minName;
        public string maxName;

        public bool includeEffects = false;
        public bool setValueWithSlider = true;

        private Slider slider;
        private bool isLocked;
        private StatValue value, min, max;

        #endregion

        #region Unity Methods

        public void Start()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(SliderChanged);

            value = statCog.FindStat(valueName);
            if (value == null)
            {
                Debug.LogError(name + ".StatSlider could not find stat '" + valueName + "'");
                enabled = false;
                return;
            }
            value.onValueChanged.AddListener(UpdateStat);

            min = statCog.FindStat(minName);
            if (min)
            {
                min.onMinValueChanged.AddListener(UpdateStat);
            }

            max = statCog.FindStat(maxName);
            if (max)
            {
                max.onMaxValueChanged.AddListener(UpdateStat);
            }

            UpdateStat(0, 0);
        }

        #endregion

        #region Private Methods

        private void SliderChanged(float value)
        {
            if (!setValueWithSlider || isLocked) return;
            statCog.SendCommand("value " + valueName + " = " + value);
        }

        private void UpdateStat(float oldVal, float newVal)
        {
            isLocked = true;
            if (includeEffects)
            {
                if (min != null)
                {
                    slider.minValue = min.CurrentMinimum;
                }
                else
                {
                    slider.minValue = value.CurrentMinimum;
                }
                if (max != null)
                {
                    slider.maxValue = max.CurrentMaximum;
                }
                else
                {
                    slider.maxValue = value.CurrentMaximum;
                }

                slider.value = value.CurrentValue;
            }
            else
            {
                if (min != null)
                {
                    slider.minValue = min.CurrentBaseMinimum;
                }
                else
                {
                    slider.minValue = value.CurrentBaseMinimum;
                }
                if (max != null)
                {
                    slider.maxValue = max.CurrentBaseMaximum;
                }
                else
                {
                    slider.maxValue = value.CurrentBaseMaximum;
                }

                slider.value = value.CurrentBaseValue;
            }
            isLocked = false;
        }

        #endregion

    }
}
