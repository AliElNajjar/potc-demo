using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("tock-menu", false)]
    public class ContainerMenuUI : MonoBehaviour
    {

        #region Variables

        // Closing
        public NavigationType closeMode = NavigationType.ByButton;
        public string closeButton = "Cancel";
        public KeyCode closeKey = KeyCode.Escape;

        // Load type
        public ListLoadMode loadMode = ListLoadMode.OnEnable;

        // Player inventory
        public InventoryCog inventory;
        public InventoryItemList localInventory;

        // Container inventory
        public InventoryContainer container;
        public InventoryItemList containerInventory;

        // Events
        public UnityEvent onOpen, onClose;

        #endregion

        #region Properties

        public InventoryContainer Container
        {
            get { return container; }
            set { container = value; }
        }

        public InventoryCog Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }

        public InventoryItemList LocalInventory
        {
            get { return localInventory; }
            set { localInventory = value; }
        }

        public InventoryItemList ContainerInventory
        {
            get { return containerInventory; }
            set { containerInventory = value; }
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (loadMode == ListLoadMode.OnEnable)
            {
                RefreshLocalInventory();
                RefreshContainerInventory();
            }

            onOpen?.Invoke();
        }

        private void Start()
        {
            if (loadMode == ListLoadMode.OnEnable)
            {
                if (Inventory != null) RefreshLocalInventory();
                if (Container != null) RefreshContainerInventory();
            }

            onOpen?.Invoke();
        }

        private void Update()
        {
            switch (closeMode)
            {
                case NavigationType.ByButton:
#if GAME_COG
                    if (GameCog.Input.GetButtonDown(closeButton))
#else
                    if (Input.GetButtonDown(closeButton))
#endif
                    {
                        onClose?.Invoke();
                        Inventory.TransferMenuClose();
                    }
                    break;
                case NavigationType.ByKey:
#if GAME_COG
                    if (GameCog.Input.GetKeyDown(closeKey))
#else
                    if (Input.GetKeyDown(closeKey))
#endif
                    {
                        onClose?.Invoke();
                        Inventory.TransferMenuClose();
                    }
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void RefreshAll()
        {
            RefreshLocalInventory();
            RefreshContainerInventory();
        }

        public void RefreshLocalInventory()
        {
            if (localInventory != null)
            {
                if (Inventory != null)
                {
                    localInventory.Inventory = Inventory;
                    localInventory.LoadItems();
                }
            }
        }

        public void RefreshContainerInventory()
        {
            if (containerInventory != null)
            {
                if (Container != null)
                {
                    containerInventory.Container = Container;
                    containerInventory.LoadItems();
                }
            }
        }

        public void StoreSelected()
        {
            ItemUI itemUI = localInventory.SelectedItem;
            if (itemUI == null) return;

            container.AddStoredItem(itemUI.Item, itemUI.Item.CurrentCount);
            Inventory.RemoveItem(itemUI.Item, itemUI.Item.CurrentCount);

            localInventory.ReloadLast();
            containerInventory.ReloadLast();
        }

        public void TakeAll()
        {
            if (Container.StoredItems.Count == 0) return;

            foreach (InventoryItem item in Container.StoredItems)
            {
                Inventory.AddToInventory(item, item.CurrentCount);
            }
            container.ClearItems();

            if (localInventory != null) localInventory.ReloadLast();
            if (containerInventory != null) containerInventory.ReloadLast();
        }

        public void TakeSelected()
        {
            ItemUI itemUI = containerInventory.SelectedItem;
            if (itemUI == null || itemUI.Item == null) return;
            
            Inventory.AddToInventory(itemUI.Item, itemUI.Item.CurrentCount);
            Container.RemoveItem(itemUI.Item, itemUI.Item.CurrentCount);

            if (localInventory != null) localInventory.ReloadLast();
            if (containerInventory != null) containerInventory.ReloadLast();
        }

        #endregion

    }
}