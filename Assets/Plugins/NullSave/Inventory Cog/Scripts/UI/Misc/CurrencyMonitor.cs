using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class CurrencyMonitor : MonoBehaviour
    {

        #region Variables

        public InventoryCog inventoryCog;
        public TextMeshProUGUI currencyText;

        private float lastCurrency;

        #endregion

        #region Unity Methods

        private void Start()
        {
            UpdateUI();
        }

        private void Update()
        {
            if(inventoryCog.currency != lastCurrency)
            {
                UpdateUI();
            }
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            lastCurrency = inventoryCog.currency;
            currencyText.text = lastCurrency.ToString();
        }

        #endregion

    }
}