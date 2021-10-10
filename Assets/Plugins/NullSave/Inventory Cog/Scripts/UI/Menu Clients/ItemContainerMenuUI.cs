using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class ItemContainerMenuUI : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI itemName;
        public InventoryItemList itemList, inventoryList;

        private System.Action callback;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public InventoryItem Item { get; set; }

        #endregion

        #region Public Methods

        public void Close()
        {
            callback?.Invoke();
            gameObject.SetActive(false);
        }

        public void LoadItemContainer(InventoryCog inventory, InventoryItem item, System.Action onCloseCallback)
        {
            if(item == null || item.itemType != ItemType.Container)
            {
                Debug.LogError("Supplied item is not a container");
                return;
            }

            Inventory = inventory;
            Item = item;
            callback = onCloseCallback;

            itemList.Inventory = inventory;
            itemList.listSource = ListSource.ContainerItem;
            itemList.ContainerItem = item;
            itemList.LoadItems();

            if (itemName != null) itemName.text = item.DisplayName;

            if (inventoryList != null)
            {
                inventoryList.listSource = ListSource.InventoryCog;
                inventoryList.Inventory = inventory;
                inventoryList.LoadItems();
            }
        }

        public void StoreSelected()
        {
            int sel = inventoryList.SelectedIndex;
            Item.AddStoredItem(inventoryList.SelectedItem.Item, inventoryList.SelectedItem.Item.CurrentCount);
            Inventory.RemoveItem(inventoryList.SelectedItem.Item, inventoryList.SelectedItem.Item.CurrentCount);

            itemList.ReloadLast();
            inventoryList.ReloadLast();
            inventoryList.SelectedIndex = sel;
        }

        public void TakeSelected()
        {
            int sel = itemList.SelectedIndex;
            InventoryItem item = itemList.SelectedItem.Item;
            Inventory.AddToInventory(item, item.CurrentCount);
            Item.RemoveItem(item, item.CurrentCount);

            itemList.ReloadLast();
            inventoryList.ReloadLast();
            itemList.SelectedIndex = sel;
        }

        public void TakeAll()
        {
            foreach(InventoryItem item in Item.StoredItems)
            {
                Inventory.AddToInventory(item, item.CurrentCount);
            }
            Item.ClearStoredItems();

            itemList.ReloadLast();
            inventoryList.ReloadLast();
        }

        #endregion

    }
}