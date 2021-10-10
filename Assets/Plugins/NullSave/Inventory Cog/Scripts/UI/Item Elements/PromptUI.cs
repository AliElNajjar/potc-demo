using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class PromptUI : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI promptText;
        public string textFormat = "Pickup {0}";

        #endregion

        #region Public Methods

        public void SetPrompt(string itemName)
        {
            if (promptText != null)
            {
                promptText.text = textFormat.Replace("{0}", itemName);
            }
        }

        #endregion

    }
}