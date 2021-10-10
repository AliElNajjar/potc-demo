using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{

    public class ItemTagUI : MonoBehaviour
    {

        #region Variables

        public Image icon;
        public TextMeshProUGUI displayText;
        public bool applyTextColor = true;
        public bool applyImageColor = true;

        public bool autoSizeToText = true;
        public Padding textPadding;

        #endregion

        private void Start()
        {
            if (displayText != null && autoSizeToText)
            {
                AutoSize();
            }
        }

        #region Public Methods

        public void LoadTag(InventoryCog inventory, InventoryItemUITag tag)
        {
            if (icon != null)
            {
                icon.sprite = tag.icon;
                icon.enabled = tag.icon != null;
                if (applyImageColor)
                {
                    icon.color = tag.iconColor;
                }
            }

            if (displayText != null)
            {
                displayText.text = tag.tagText;
                if (applyTextColor)
                {
                    displayText.color = tag.textColor;
                }
            }
        }

        #endregion

        #region Private Methods

        private void AutoSize()
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1024, 768);
            displayText.ForceMeshUpdate();
            Vector2 newSize = displayText.textBounds.size;

            GetComponent<RectTransform>().sizeDelta = new Vector2(newSize.x + textPadding.left + textPadding.right, 
                newSize.y + textPadding.top + textPadding.bottom);
            displayText.ForceMeshUpdate();
        }

        #endregion

    }
}