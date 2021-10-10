using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class InventoryContainer : MonoBehaviour
    {

        #region Variables

        public string displayName = "Container";
        public InventoryMasterList masterList;

        // Storage
        public List<ItemReference> startingItems = new List<ItemReference>();
        public bool hasMaxStoreSlots = true;
        public int maxStoreSlots = 16;
        public bool hasMaxStoreWeight = false;
        public float maxStoreWeight = 100;

        // Events
        public ItemCountChanged onItemStored, onItemRemoved;

        #endregion

        #region Properties

        public List<InventoryItem> StoredItems { get; private set; }

        public float StoredWeight { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            StoredItems = new List<InventoryItem>();
            foreach (ItemReference item in startingItems)
            {
                AddStoredItem(item.item, item.count);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add item to storage
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool AddStoredItem(InventoryItem item, int count)
        {
            float addedWeight = item.weight * count;

            // Prevent nested containers
            if (item.itemType == ItemType.Container) return false;

            // Check for max weight
            if (hasMaxStoreWeight)
            {
                if (StoredWeight + addedWeight > maxStoreWeight)
                {
                    return false;
                }
            }

            // Add to current stack if able
            if (item.canStack)
            {
                int availStackSlots = AvailableStackSlots(item);
                if (availStackSlots < count)
                {
                    int requiredStacks = Mathf.CeilToInt((float)(count - availStackSlots) / item.countPerStack);
                    if (hasMaxStoreSlots && requiredStacks > maxStoreSlots)
                    {
                        return false;
                    }
                }

                // Add slot items if able
                if (availStackSlots > 0)
                {
                    int change;
                    foreach (InventoryItem storedItem in StoredItems)
                    {
                        if (storedItem.CurrentCount < storedItem.countPerStack)
                        {
                            change = storedItem.countPerStack - storedItem.CurrentCount;
                            if (change > count)
                            {
                                change = count;
                            }
                            storedItem.CurrentCount += change;
                            count -= change;
                            if (count == 0)
                            {
                                StoredWeight += addedWeight;
                                onItemStored?.Invoke(item, count);
                                return true;
                            }
                        }
                    }
                }

                // Add remaining stacks
                while (count > 0)
                {
                    InventoryItem storedItem = Instantiate(item);
                    storedItem.InstanceId = System.Guid.NewGuid().ToString();
                    storedItem.name = item.name;
                    storedItem.value = item.value;
                    storedItem.rarity = item.rarity;

                    if (count <= storedItem.countPerStack)
                    {
                        storedItem.CurrentCount = count;
                        StoredItems.Add(storedItem);
                        StoredWeight += addedWeight;
                        onItemStored?.Invoke(item, count);
                        return true;
                    }
                    else
                    {
                        storedItem.CurrentCount = storedItem.countPerStack;
                        count -= storedItem.countPerStack;
                        StoredItems.Add(storedItem);

                    }
                }

                StoredWeight += addedWeight;
                onItemStored?.Invoke(item, count);
                return true;
            }

            // Check for slots
            if (hasMaxStoreSlots && StoredItems.Count + count > maxStoreSlots)
            {
                return false;
            }

            // Add unstacked
            for (int i = 0; i < count; i++)
            {
                InventoryItem storedItem = Instantiate(item);
                storedItem.InstanceId = System.Guid.NewGuid().ToString();
                storedItem.name = item.name;
                storedItem.value = item.value;
                storedItem.rarity = item.rarity;
                storedItem.CurrentCount = 1;
                StoredItems.Add(storedItem);
            }

            StoredWeight += addedWeight;
            onItemStored?.Invoke(item, count);
            return true;
        }

        /// <summary>
        /// Reset container
        /// </summary>
        public void ClearItems()
        {
            StoredItems.Clear();
            onItemStored?.Invoke(null, 0);
            StoredWeight = 0;
        }

        /// <summary>
        /// Get cout of item currently stored
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemStoredCount(InventoryItem item)
        {
            int count = 0;
            foreach (InventoryItem storedItem in StoredItems)
            {
                if (storedItem.name == item.name)
                {
                    count += storedItem.CurrentCount;
                }
            }
            return count;
        }

        /// <summary>
        /// Get a list of all items in a category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<InventoryItem> GetStoredItems(string categoryName)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            foreach (InventoryItem item in StoredItems)
            {
                if (item.category.name == categoryName)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of all items in a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<InventoryItem> GetStoredItems(Category category)
        {
            return GetStoredItems(category.name);
        }

        /// <summary>
        /// Get a list of all items in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetStoredItems(List<string> categories)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            foreach (InventoryItem item in StoredItems)
            {
                if (categories.Contains(item.category.name))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of all items in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetStoredItems(List<Category> categories)
        {
            List<string> categoryNames = new List<string>();
            foreach (Category category in categories)
            {
                categoryNames.Add(category.name);
            }

            return GetStoredItems(categoryNames);
        }

        /// <summary>
        /// Remove item from storage
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool RemoveItem(InventoryItem item, int count)
        {
            int stored = GetItemStoredCount(item);
            if (stored < count) return false;

            List<InventoryItem> toRemove = new List<InventoryItem>();
            foreach (InventoryItem storedItem in StoredItems)
            {
                if (storedItem.name == item.name)
                {
                    if (storedItem.CurrentCount >= count)
                    {
                        storedItem.CurrentCount -= count;
                        if (storedItem.CurrentCount == 0)
                        {
                            toRemove.Add(storedItem);
                        }
                        break;
                    }
                    else
                    {
                        count -= storedItem.CurrentCount;
                        storedItem.CurrentCount = 0;
                        toRemove.Add(storedItem);
                    }
                }
            }

            foreach (InventoryItem itm in toRemove)
            {
                StoredItems.Remove(item);
            }

            StoredWeight -= item.weight * count;
            onItemRemoved?.Invoke(item, count);
            return true;
        }

        /// <summary>
        /// Resort the inventory
        /// </summary>
        /// <param name="sortOrder"></param>
        public void Sort(InventorySortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case InventorySortOrder.ConditionAsc:
                    StoredItems.Sort((p1, p2) => p1.condition.CompareTo(p2.condition));
                    break;
                case InventorySortOrder.ConditionDesc:
                    StoredItems.Sort((p1, p2) => p2.condition.CompareTo(p1.condition));
                    break;
                case InventorySortOrder.DisplayNameAsc:
                    StoredItems.Sort((p1, p2) => p1.DisplayName.CompareTo(p2.DisplayName));
                    break;
                case InventorySortOrder.DisplayNameDesc:
                    StoredItems.Sort((p1, p2) => p2.DisplayName.CompareTo(p1.DisplayName));
                    break;
                case InventorySortOrder.ItemCountAsc:
                    StoredItems.Sort((p1, p2) => p1.CurrentCount.CompareTo(p2.CurrentCount));
                    break;
                case InventorySortOrder.ItemCountDesc:
                    StoredItems.Sort((p1, p2) => p2.CurrentCount.CompareTo(p1.CurrentCount));
                    break;
                case InventorySortOrder.ItemTypeAsc:
                    StoredItems.Sort((p1, p2) => p1.itemType.CompareTo(p2.itemType));
                    break;
                case InventorySortOrder.ItemTypeDesc:
                    StoredItems.Sort((p1, p2) => p2.itemType.CompareTo(p1.itemType));
                    break;
                case InventorySortOrder.RarityAsc:
                    StoredItems.Sort((p1, p2) => p1.rarity.CompareTo(p2.rarity));
                    break;
                case InventorySortOrder.RarityDesc:
                    StoredItems.Sort((p1, p2) => p2.rarity.CompareTo(p1.rarity));
                    break;
                case InventorySortOrder.ValueAsc:
                    StoredItems.Sort((p1, p2) => p1.value.CompareTo(p2.value));
                    break;
                case InventorySortOrder.ValueDesc:
                    StoredItems.Sort((p1, p2) => p2.value.CompareTo(p1.value));
                    break;
                case InventorySortOrder.WeightAsc:
                    StoredItems.Sort((p1, p2) => p1.weight.CompareTo(p2.weight));
                    break;
                case InventorySortOrder.WeightDesc:
                    StoredItems.Sort((p1, p2) => p2.weight.CompareTo(p1.weight));
                    break;
            }
        }

        /// <summary>
        /// Load state of storage
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="inventory"></param>
        public void StateLoad(Stream stream)
        {
            if (stream.Position >= stream.Length) return;

            float version = stream.ReadFloat();
            if (version != 1.1f)
            {
                Debug.LogError("Invalid save version");
                return;
            }

            // Stored items
            StoredItems = new List<InventoryItem>();
            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string itemName = stream.ReadStringPacket();
                InventoryItem item = Instantiate(InventoryDB.GetItemByName(itemName));
                item.name = itemName;
                item.StateLoad(stream, null);
                StoredItems.Add(item);
            }
        }

        /// <summary>
        /// Save state of storage
        /// </summary>
        /// <param name="stream"></param>
        public void StateSave(Stream stream)
        {
            stream.WriteFloat(1.1f);

            // Stored items
            stream.WriteInt(StoredItems.Count);
            foreach (InventoryItem item in StoredItems)
            {
                stream.WriteStringPacket(item.name);
                item.StateSave(stream);
            }
        }

        /// <summary>
        /// Take all items in container (inventory cog found by "Player" tag)
        /// </summary>
        public void TakeAll()
        {
            TakeAll(GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<InventoryCog>());
        }

        public void TakeAll(InventoryCog inventory)
        {
            foreach (InventoryItem item in StoredItems)
            {
                inventory.AddToInventory(item, item.CurrentCount);
            }

            StoredItems.Clear();
        }

        #endregion

        #region Private Methods

        private int AvailableStackSlots(InventoryItem item)
        {
            int count = 0;
            foreach (InventoryItem storedItem in StoredItems)
            {
                if (storedItem.name == item.name)
                {
                    count += storedItem.countPerStack - storedItem.CurrentCount;
                }
            }

            return count;
        }

        #endregion

    }
}