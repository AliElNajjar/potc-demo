using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class AttachmentsUI : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI itemTitle;
        public Image itemIcon;

        public AttachmentList slotList;

        private System.Action callback;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public InventoryItem Item { get; set; }

        #endregion

        #region Public Methods

        public void Close()
        {
            gameObject.SetActive(false);
            callback?.Invoke();
        }

        public void LoadItem(InventoryCog inventory, InventoryItem item, System.Action onClose)
        {
            Inventory = inventory;
            Item = item;

            callback = onClose;

            // Update UI
            if (itemTitle != null) itemTitle.text = Item.DisplayName;
            if (itemIcon != null) itemIcon.sprite = Item.icon;

            if (slotList != null)
            {
                slotList.Inventory = inventory;
                slotList.LoadSlots(Item);
            }
        }

        #endregion

    }
}