using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NullSave.TOCK.Inventory
{
    public class InventoryItemList : MonoBehaviour, IDropHandler
    {

        #region Variables

        // Load type
        public ListLoadMode loadMode = ListLoadMode.OnEnable;

        public ListSource listSource;
        public InventoryCog inventoryCog;
        public InventoryContainer container;
        public InventoryMerchant merchant;
        public bool hideSelectionWhenLocked = true;
        public bool showLockedSlots;
        public bool enableDragDrop;

        // Extra UI
        public AttachmentsUI attachmentsClient;
        public ItemDetailUI detailClient;
        public ItemContainerMenuUI itemContainerUI;
        public bool hideEmptyDetails = true;
        public SingleCheckoutUI checkoutUI;

        // Filtering
        public bool requireBreakdown;
        public bool requireRepair;
        public bool excludeContainers;

        public ListCategoryFilter categoryFilter = ListCategoryFilter.All;
        public List<Category> categories;

        public bool useTagFiltering;
        public List<CustomTagFilter> tags;

        public bool usePaging;
        public int startPage = 0;

        // Navigation
        public bool allowAutoWrap;
        public bool allowSelectByClick;
        public NavigationType navigationMode;
        public bool autoRepeat = true;
        public float repeatDelay = 0.5f;
        public bool lockInput;

        public NavigationType selectionMode;
        public string buttonSubmit = "Submit";
        public KeyCode keySubmit = KeyCode.Return;

        // Events
        public UnityEvent onInputLocked, onInputUnlocked, onBindingUpdated;
        public SelectedIndexChanged onSelectionChanged;
        public UnityEvent onNeedPreviousCategory, onNeedNextCategory;
        public PageChanged onPageChanged;
        public ItemListSubmit onItemSubmit;

        // Editor
        public int z_display_flags = 4095;

        #endregion

        #region Properties

        public InventoryContainer Container { get { return container; } set { if (container == value) return; container = value; onBindingUpdated?.Invoke(); } }

        public InventoryItem ContainerItem { get; set; }

        public InventoryCog Inventory { get { return inventoryCog; } set { if (inventoryCog == value) return; inventoryCog = value; onBindingUpdated?.Invoke(); } }

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

        public InventoryMerchant Merchant { get { return merchant; } set { if (merchant == value) return; merchant = value; onBindingUpdated?.Invoke(); } }

        public virtual int SelectedIndex { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        public virtual ItemUI SelectedItem { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }

        #endregion

        #region Unity Methods

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (!enableDragDrop) return;

            ItemUI draggedItem = eventData.pointerDrag.gameObject.GetComponentInChildren<ItemUI>();
            if (draggedItem != null)
            {
                if (draggedItem.Inventory != null)
                {
                    if (draggedItem.Inventory == Inventory) return;
                    AddDraggedItem(draggedItem.Item);
                    draggedItem.Inventory.RemoveItem(draggedItem.Item, draggedItem.Item.CurrentCount);
                }
                else if (draggedItem.Container != null)
                {
                    if (draggedItem.Container == Container) return;
                    AddDraggedItem(draggedItem.Item);
                    draggedItem.Container.RemoveItem(draggedItem.Item, draggedItem.Item.CurrentCount);
                }

                draggedItem.OnEndDrag(eventData);
            }
        }

        #endregion

        #region Public Methods

        public virtual void BreakdownSelected()
        {
            if (LockInput) return;
            int selIndex = SelectedIndex;
            Inventory.BreakdownItem(SelectedItem.Item);
            ReloadLast();
            SelectedIndex = selIndex;
        }

        public virtual void DropSelected()
        {
            if (LockInput) return;
            int selIndex = SelectedIndex;
            Inventory.DropItem(SelectedItem.Item, SelectedItem.Item.CurrentCount);
            ReloadLast();
            SelectedIndex = selIndex;
        }

        public virtual void EquipSelected()
        {
            if (LockInput) return;
            int selIndex = SelectedIndex;
            SelectedItem.Equip();
            ReloadLast();
            SelectedIndex = selIndex;
        }

        public virtual void LoadFromCategoyList(CategoryList source) { throw new System.NotImplementedException(); }

        public virtual void LoadFromCategoyList(CategoryList source, bool startFromLastPage) { throw new System.NotImplementedException(); }

        public virtual void LoadItems() { throw new System.NotImplementedException(); }

        public virtual void LoadItems(List<InventoryItem> items) { throw new System.NotImplementedException(); }

        public virtual void OpenItemContainer()
        {
            if (LockInput || itemContainerUI == null || SelectedItem.Item.itemType != ItemType.Container) return;

            System.Action onClose = new System.Action(() => { LockInput = false; ReloadLast(); });

            LockInput = true;
            itemContainerUI.gameObject.SetActive(true);
            itemContainerUI.LoadItemContainer(Inventory, SelectedItem.Item, onClose);
        }

        public virtual void ReloadLast() { throw new System.NotImplementedException(); }

        public virtual void RenameSelected()
        {
            if (LockInput) return;
            Inventory.OpenRenamePrompt(SelectedItem.Item);
            LockInput = true;
            Inventory.onMenuClose.AddListener(UnlockInput);
            Inventory.onMenuClose.AddListener(PromptReload);
        }

        public virtual void RepairSelected()
        {
            if (LockInput) return;
            Inventory.RepairItem(SelectedItem.Item);
            ReloadLast();
        }

        public virtual void SelectItem(ItemUI item)
        {
            SelectedItem = item;
        }

        public virtual void SetSelectedSkill(string skillSlot)
        {
            if (LockInput) return;
            Inventory.SkillAssign(SelectedItem.Item, skillSlot);
        }

        public virtual void ShowAttachmentsUI()
        {
            if (!LockInput && attachmentsClient != null && SelectedItem.Item != null && SelectedItem.Item.attachRequirement != AttachRequirement.NoneAllowed)
            {
                int restoreSel = SelectedIndex;
                System.Action onClose = new System.Action(() => { LockInput = false; ReloadLast(); SelectedIndex = restoreSel; });

                LockInput = true;
                attachmentsClient.gameObject.SetActive(true);
                attachmentsClient.LoadItem(Inventory, SelectedItem.Item, onClose);
            }
        }

        public virtual void Sort(InventorySortOrder sortOrder)
        {
            switch (listSource)
            {
                case ListSource.InventoryCog:
                    Inventory.Sort(sortOrder);
                    break;
                case ListSource.InventoryContainer:
                    Container.Sort(sortOrder);
                    break;
                case ListSource.InventoryMerchant:
                    Merchant.Sort(sortOrder);
                    break;
            }
            ReloadLast();
        }

        public virtual void SortByOrderId(int inventorySortOrderId)
        {
            Sort((InventorySortOrder)inventorySortOrderId);
        }

        public virtual void ToggleSelectedSkill(string skillSlot)
        {
            if (LockInput) return;
            Inventory.SkillToggle(SelectedItem.Item, skillSlot);
        }

        public virtual void UnequipSelected()
        {
            if (LockInput) return;
            int selIndex = SelectedIndex;
            SelectedItem.Unequip();
            ReloadLast();
            SelectedIndex = selIndex;
        }

        public virtual void UpdateCheckoutUI()
        {
            if (checkoutUI != null) checkoutUI.LoadUI(this);
        }

        #endregion

        #region Private Methods

        private void AddDraggedItem(InventoryItem item)
        {
            if (Inventory != null)
            {
                Inventory.AddToInventory(item, item.CurrentCount);
            }
            else if (Container != null)
            {
                Container.AddStoredItem(item, item.CurrentCount);
            }
        }

        internal List<InventoryItem> FilterItems(List<InventoryItem> items)
        {
            List<InventoryItem> removeList = new List<InventoryItem>();

            // Generate item list
            if (useTagFiltering)
            {
                foreach (InventoryItem checkItem in items)
                {
                    foreach (CustomTagFilter tagFilter in tags)
                    {
                        if (!tagFilter.PassMatch(checkItem))
                        {
                            removeList.Add(checkItem);
                            break;
                        }
                    }
                }
            }

            foreach (InventoryItem item in items)
            {
                if ((requireBreakdown && !item.CanBreakdown) ||
                    (requireRepair && (!item.CanRepair || item.condition == 1)) ||
                    (excludeContainers && item.itemType == ItemType.Container))
                {
                    if (!removeList.Contains(item))
                    {
                        removeList.Add(item);
                    }
                }
            }

            // Remove filtered
            foreach (InventoryItem item in removeList)
            {
                items.Remove(item);
            }

            return items;
        }

        internal virtual void LockStateChanged() { }

        private void UnlockInput()
        {
            LockInput = false;
            Inventory.onMenuClose.RemoveListener(UnlockInput);
        }

        private void PromptReload()
        {
            int sel = SelectedIndex;
            ReloadLast();
            SelectedIndex = sel;
            Inventory.onMenuClose.RemoveListener(PromptReload);
        }

        internal void Subscribe()
        {
            if (Inventory != null)
            {
                Inventory.onItemAdded.AddListener(UpdateInventory);
                Inventory.onItemRemoved.AddListener(UpdateInventory);
            }

            if (Container != null)
            {
                container.onItemStored.AddListener(UpdateInventory);
                container.onItemRemoved.AddListener(UpdateInventory);
            }
        }

        internal void Unsubscribe()
        {
            if (Inventory != null)
            {
                Inventory.onItemAdded.RemoveListener(UpdateInventory);
                Inventory.onItemRemoved.RemoveListener(UpdateInventory);
            }

            if (Container != null)
            {
                container.onItemStored.RemoveListener(UpdateInventory);
                container.onItemRemoved.RemoveListener(UpdateInventory);
            }
        }

        internal void UpdateInventory(InventoryItem item, int count)
        {
            ReloadLast();
        }

        #endregion

    }
}