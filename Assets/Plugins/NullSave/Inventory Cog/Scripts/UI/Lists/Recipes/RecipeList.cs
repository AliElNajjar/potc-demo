using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class RecipeList : MonoBehaviour
    {

        #region Variables

        // Load type
        public ListLoadMode loadMode = ListLoadMode.OnEnable;

        public InventoryCog inventoryCog;
        public bool hideSelectionWhenLocked = true;

        // Filtering
        public List<string> categories;

        public bool usePaging = false;
        public int startPage = 0;

        // Navigation
        public bool allowAutoWrap = false;
        public bool allowSelectByClick = false;
        public NavigationType navigationMode;
        public bool autoRepeat = true;
        public float repeatDelay = 0.5f;
        public bool lockInput = false;

        public NavigationType selectionMode;
        public string buttonSubmit = "Submit";
        public KeyCode keySubmit = KeyCode.Return;

        // Events
        public UnityEvent onInputLocked, onInputUnlocked, onBindingUpdated;
        public SelectedIndexChanged onSelectionChanged;
        public ItemListSubmit onItemSubmit;

        // Editor
        public int z_display_flags = 4095;

        #endregion

        #region Properties

        public InventoryCog Inventory { get { return inventoryCog; } set { inventoryCog = value; onBindingUpdated?.Invoke(); } }

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

        public virtual RecipeUI SelectedItem { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        #endregion

        #region Public Methods

        public virtual void LoadFromCategoyList(CraftingCategoryList source) { throw new System.NotImplementedException(); }

        public virtual void LoadRecipies() { throw new System.NotImplementedException(); }

        public virtual void LoadRecipies(List<string> categories) { throw new System.NotImplementedException(); }

        public virtual void ReloadLast() { throw new System.NotImplementedException(); }

        public virtual void SelectItem(RecipeUI item)
        {
            SelectedItem = item;
        }

        #endregion

        #region Private Methods

        internal virtual void LockStateChanged() { }

        #endregion

    }
}