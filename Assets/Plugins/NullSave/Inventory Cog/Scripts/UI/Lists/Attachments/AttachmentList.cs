using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class AttachmentList : MonoBehaviour
    {

        #region Variables

        // Navigation
        public bool allowAutoWrap = false;
        public bool allowSelectByClick = false;
        public NavigationType navigationMode;
        public bool autoRepeat = true;
        public float repeatDelay = 0.5f;
        public bool hideSelectionWhenLocked = true;
        public bool lockInput = false;

        public NavigationType selectionMode;
        public string buttonSubmit = "Submit";
        public KeyCode keySubmit = KeyCode.Return;

        // Events
        public UnityEvent onInputLocked, onInputUnlocked, onAttachmentsAvail, onNoAttachmentsAvail, onHasAttachments, onHasNoAttachments;
        public SelectedIndexChanged onSelectionChanged;

        // Editor
        public int z_display_flags = 4095;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public InventoryItem Item { get; set; }

        public bool LockInput
        {
            get { return lockInput; }
            set
            {
                if (lockInput == value) return;
                lockInput = value;
                if (lockInput)
                {
                    onInputLocked?.Invoke();
                }
                else
                {
                    onInputUnlocked?.Invoke();
                }
                LockStateChanged();
            }
        }

        public virtual int SelectedIndex { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        public virtual AttachmentSlotUI SelectedItem { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        #endregion

        #region Public Methods

        public virtual void CloseAttachments() { }

        public virtual void LoadSlots(InventoryItem item) { }

        public virtual void OpenAttachments() { }

        public virtual void RemoveAttachment()
        {
            AttachmentSlotUI selItem = SelectedItem;
            if (selItem == null || selItem.Slot.AttachedItem == null) return;
            selItem.Slot.ParentItem.RemoveAttachment(selItem.Slot);
            selItem.Reload();
            int sel = SelectedIndex;
            LoadSlots(Item);
            SelectedIndex = sel;
        }

        #endregion

        #region Private Methods

        internal virtual void LockStateChanged() { }

        #endregion

    }
}