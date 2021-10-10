using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class AttachmentItemList : MonoBehaviour
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
        public UnityEvent onInputLocked, onInputUnlocked;
        public SelectedIndexChanged onSelectionChanged;

        // Editor
        public int z_display_flags = 4095;

        #endregion

        #region Properties

        public AttachmentList ParentList { get; set; }

        public InventoryCog Inventory { get; set; }

        public AttachmentSlot Slot { get; set; }

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

        public virtual ItemUI SelectedItem { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        #endregion

        #region Public Methods

        public virtual void AttachToItem()
        {
            if (SelectedItem == null) return;

            Item.AddAttachment(SelectedItem.Item);
            LoadAttachments(Slot);
            ParentList.SelectedItem.Reload();
            StartCoroutine("UnlockParentList");
        }

        public virtual void LoadAttachments(AttachmentSlot attachSlot) { }

        #endregion

        #region Private Methods

        private IEnumerator UnlockParentList()
        {
            yield return new WaitForEndOfFrame();
            ParentList.CloseAttachments();
        }

        internal virtual void LockStateChanged() { }

        #endregion

    }
}