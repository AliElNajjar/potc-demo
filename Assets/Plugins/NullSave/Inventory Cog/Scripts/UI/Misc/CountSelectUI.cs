using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class CountSelectUI : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI title, minCount, maxCount, curCount;
        public Slider countSlider;

        private Action<bool, int> callback;

        #endregion

        #region Public Methods

        public void CancelCount()
        {
            callback?.Invoke(false, 0);
        }

        public void ConfirmCount()
        {
            callback?.Invoke(true, (int)countSlider.value);
        }

        public void SelectCount(string title, int minCount, int maxCount, int defaultCount, Action<bool, int> callback)
        {
            if (this.title != null) this.title.text = title;
            if (this.minCount != null) this.minCount.text = minCount.ToString();
            if (this.maxCount != null) this.maxCount.text = maxCount.ToString();
            if (this.curCount != null) this.curCount.text = defaultCount.ToString();
            if (countSlider != null)
            {
                countSlider.minValue = minCount;
                countSlider.maxValue = maxCount;
                countSlider.value = defaultCount;
                countSlider.onValueChanged.RemoveListener(UpdateCount);
                countSlider.onValueChanged.AddListener(UpdateCount);
            }
            this.callback = callback;
        }

        #endregion

        #region Private Methods

        private void UpdateCount(float count)
        {
            if (curCount != null)
            {
                curCount.text = ((int)count).ToString();
            }
        }

        #endregion

    }
}