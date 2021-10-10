using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class AttachmentSlotUI : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI displayName;
        public Image icon;
        public Image slotIcon;
        public GameObject selectionIndicator;

        #endregion

        #region Properties

        public AttachmentSlot Slot { get; private set; }

        #endregion

        #region Public Methods

        public void LoadSlot(AttachmentSlot slot)
        {
            Slot = slot;
            if (slot.AttachedItem == null)
            {
                if (displayName != null) displayName.gameObject.SetActive(false);
                if (icon != null) icon.gameObject.SetActive(false);
            }
            else
            {
                if (displayName != null) displayName.text = slot.AttachedItem.DisplayName;
                if (icon != null)
                {
                    icon.sprite = slot.AttachedItem.icon;
                    icon.gameObject.SetActive(slot.AttachedItem.icon != null);
                }
            }

            if (slotIcon)
            {
                slotIcon.gameObject.SetActive(slot.AttachPoint.slotIcon != null);
                slotIcon.sprite = slot.AttachPoint.slotIcon;
            }

        }

        public void Reload()
        {
            LoadSlot(Slot);
        }

        public void SetSelected(bool selected)
        {
            if (selectionIndicator != null) selectionIndicator.SetActive(selected);
        }

        #endregion

    }
}