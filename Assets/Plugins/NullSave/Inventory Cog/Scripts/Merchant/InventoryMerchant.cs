using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("inventory-coin", false)]
    public class InventoryMerchant : MonoBehaviour
    {

        #region Variables

        public string displayName = "General Merchant";
        public List<ItemReference> availableItems;
        public float buyModifier = 1;
        public float sellModifier = 1;
        public bool allowBuyBack = true;
        public float buybackModifier = 1;
        public bool limitVendorCurrency = true;
        public float currency = 1000;
        public bool stockReplenishes = false;
        public float replenishTime = 3600;

        // Multicount
        public bool allowMulticount = false;
        public CountSelectUI countSelectUI;
        public int minToShowCount = 5;
        public Transform countContainer;

        public UnityEvent onStockReplenished, onItemBought, onItemSold;

        private float replenishWait;
        private CountSelectUI spawnedCount;

        #endregion

        #region Properties

        /// <summary>
        /// Simplified list of stock
        /// </summary>
        public List<ItemReference> BasicStock
        {
            get
            {
                List<ItemReference> stock = new List<ItemReference>();
                foreach (StockItemReference stockItem in Stock)
                {
                    stockItem.item.CurrentCount = stockItem.count;
                    stock.Add(new ItemReference(stockItem.item, stockItem.count));
                }
                return stock;
            }
        }

        /// <summary>
        /// Simplified list of buyback stock
        /// </summary>
        public List<ItemReference> BasicStockBuyback
        {
            get
            {
                List<ItemReference> stock = new List<ItemReference>();
                foreach (StockItemReference stockItem in Stock)
                {
                    if (stockItem.bought > 0)
                    {
                        stockItem.item.CurrentCount = stockItem.bought;
                        stock.Add(new ItemReference(stockItem.item, stockItem.bought));
                    }
                }
                return stock;
            }
        }

        public List<StockItemReference> BuybackStock
        {
            get
            {
                List<StockItemReference> bought = new List<StockItemReference>();
                foreach(StockItemReference stockItem in Stock)
                {
                    if (stockItem.bought > 0)
                    {
                        bought.Add(stockItem);
                    }
                }
                return bought;
            }
        }

        public bool InTransaction { get; set; }

        public List<StockItemReference> Stock { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Stock = new List<StockItemReference>();
            foreach (ItemReference item in availableItems)
            {
                Stock.Add(new StockItemReference(item));
            }
            replenishWait = replenishTime;
        }

        private void Update()
        {
            UpdateReplenishment();
        }

        #endregion

        #region Public Methods

        public int AvailableStock(InventoryItem item)
        {
            foreach (StockItemReference stockItem in Stock)
            {
                if (stockItem.item.name == item.name)
                {
                    return stockItem.count;
                }
            }

            return 0;
        }

        /// <summary>
        /// Buy item(s) from player
        /// </summary>
        /// <param name="item"></param>
        /// <param name="playerInventory"></param>
        /// <param name="callback"></param>
        public void BuyFromPlayer(InventoryItem item, InventoryCog playerInventory, System.Action<bool> callback)
        {
            bool result;

            if (allowMulticount)
            {
                float costEach = item.value * buyModifier;
                int maxCount = Mathf.Min(playerInventory.GetItemTotalCount(item), Mathf.FloorToInt(currency / costEach));
                if (maxCount >= minToShowCount)
                {
                    System.Action<bool, int> localCallback = (bool craft, int count) =>
                    {
                        if (craft)
                        {
                            result = BuyFromPlayer(item, count, playerInventory);
                            callback?.Invoke(result);
                        }
                        if (spawnedCount != null) Destroy(spawnedCount.gameObject);
                    };

                    spawnedCount = Instantiate(countSelectUI, countContainer);
                    spawnedCount.SelectCount(item.DisplayName, 1, maxCount, 1, localCallback);
                    return;
                }
            }

            result = BuyFromPlayer(item, 1, playerInventory);
            callback?.Invoke(result);
        }

        /// <summary>
        /// Buy item from player
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="playerInventory"></param>
        /// <returns></returns>
        public bool BuyFromPlayer(InventoryItem item, int count, InventoryCog playerInventory)
        {
            float cost = (item.value * count) * buyModifier;
            if (currency < cost) return false;

            // Update currencies
            currency -= cost;
            playerInventory.currency += cost;

            // Add to bought stock
            AddBoughtStock(item, count);
            playerInventory.RemoveItem(item, count);

            onItemSold?.Invoke();

            return true;
        }

        /// <summary>
        /// Check if merchant has enough currency to buy item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CanBuyFromPlayer(InventoryItem item)
        {
            return currency >= item.value * buyModifier;
        }

        /// <summary>
        /// Check if merchant has enough currency to buy item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool CanBuyFromPlayer(InventoryItem item, int count)
        {
            return currency >= (item.value * count) * buyModifier;
        }

        /// <summary>
        /// Check if player has enough currency to sell item and is in stock
        /// </summary>
        /// <param name="item"></param>
        /// <param name="playerInventory"></param>
        /// <returns></returns>
        public bool CanSellToPlayer(InventoryItem item, InventoryCog playerInventory)
        {
            foreach (StockItemReference stock in Stock)
            {
                if (stock.item.name == item.name)
                {
                    return playerInventory.currency >= stock.item.value * sellModifier;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if player has enough currency to sell item and is in stock
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="playerInventory"></param>
        /// <returns></returns>
        public bool CanSellToPlayer(InventoryItem item, int count, InventoryCog playerInventory)
        {
            foreach (StockItemReference stock in Stock)
            {
                if (stock.item == item)
                {
                    if (stock.count < count) return false;
                    return playerInventory.currency >= (stock.item.value * count) * sellModifier;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if player has enough currency to buy back sold item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="playerInventory"></param>
        /// <returns></returns>
        public bool CanPlayerBuyBack(InventoryItem item, InventoryCog playerInventory)
        {
            if (!allowBuyBack) return false;

            foreach (StockItemReference buyback in Stock)
            {
                if (buyback.item.name == item.name)
                {
                    if (buyback.bought <= 0) return false;
                    return playerInventory.currency >= buyback.item.value * buybackModifier;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if player has enough currency to buy back sold item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="playerInventory"></param>
        /// <returns></returns>
        public bool CanPlayerBuyBack(InventoryItem item, int count, InventoryCog playerInventory)
        {
            if (!allowBuyBack) return false;

            foreach (StockItemReference buyback in Stock)
            {
                if (buyback.item.name == item.name)
                {
                    if (buyback.bought < count) return false;
                    return playerInventory.currency >= (buyback.item.value * count) * buybackModifier;
                }
            }

            return false;
        }

        /// <summary>
        /// Replenish starting stock
        /// </summary>
        /// <returns></returns>
        public IEnumerator ReplenishStock()
        {
            foreach (ItemReference item in availableItems)
            {
                SetStock(item);
            }

            yield return null;
        }


        /// <summary>
        /// Buy item(s) from player
        /// </summary>
        /// <param name="item"></param>
        /// <param name="playerInventory"></param>
        /// <param name="callback"></param>
        public void SellToPlayer(InventoryItem item, InventoryCog playerInventory, System.Action<bool> callback)
        {
            bool result;

            if (allowMulticount)
            {
                float costEach = item.value * buyModifier;
                int maxCount = Mathf.Min(AvailableStock(item), Mathf.FloorToInt(playerInventory.currency / costEach));
                if (maxCount >= minToShowCount)
                {
                    System.Action<bool, int> localCallback = (bool craft, int count) =>
                    {
                        if (craft)
                        {
                            result = SellToPlayer(item, count, playerInventory);
                            callback?.Invoke(result);
                        }
                        if (spawnedCount != null) Destroy(spawnedCount.gameObject);
                    };

                    spawnedCount = Instantiate(countSelectUI, countContainer);
                    spawnedCount.SelectCount(item.DisplayName, 1, maxCount, 1, localCallback);
                    return;
                }
            }

            result = SellToPlayer(item, 1, playerInventory);
            callback?.Invoke(result);
        }

        /// <summary>
        /// Sell item to player
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="playerInventory"></param>
        /// <returns></returns>
        public bool SellToPlayer(InventoryItem item, int count, InventoryCog playerInventory)
        {
            // Check stock
            int availCount = 0;
            foreach (StockItemReference stock in Stock)
            {
                if (stock.item.name == item.name)
                {
                    availCount += stock.count;
                    break;
                }
            }
            if (availCount < count) return false;

            float cost = (item.value * count) * buyModifier;
            if (playerInventory.currency < cost) return false;

            // Update currencies
            currency += cost;
            playerInventory.currency -= cost;

            // Transfer stock
            playerInventory.AddToInventory(item, count);
            RemoveStock(item, count);

            onItemSold?.Invoke();

            return true;
        }

        /// <summary>
        /// Sell back item to player
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="playerInventory"></param>
        /// <returns></returns>
        public bool SellBackToPlayer(InventoryItem item, int count, InventoryCog playerInventory)
        {
            // Check stock
            int availCount = 0;
            foreach (StockItemReference stock in Stock)
            {
                if (stock.item.name == item.name)
                {
                    availCount += stock.bought;
                    break;
                }
            }
            if (availCount < count) return false;

            float cost = (item.value * count) * buyModifier;
            if (playerInventory.currency < cost) return false;

            // Update currencies
            currency += cost;
            playerInventory.currency -= cost;

            // Transfer stock
            playerInventory.AddToInventory(item, count);
            RemoveBoughtStock(item, count);

            onItemSold?.Invoke();

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
                    Stock.Sort((p1, p2) => p1.item.condition.CompareTo(p2.item.condition));
                    break;
                case InventorySortOrder.ConditionDesc:
                    Stock.Sort((p1, p2) => p2.item.condition.CompareTo(p1.item.condition));
                    break;
                case InventorySortOrder.DisplayNameAsc:
                    Stock.Sort((p1, p2) => p1.item.DisplayName.CompareTo(p2.item.DisplayName));
                    break;
                case InventorySortOrder.DisplayNameDesc:
                    Stock.Sort((p1, p2) => p2.item.DisplayName.CompareTo(p1.item.DisplayName));
                    break;
                case InventorySortOrder.ItemCountAsc:
                    Stock.Sort((p1, p2) => p1.item.CurrentCount.CompareTo(p2.item.CurrentCount));
                    break;
                case InventorySortOrder.ItemCountDesc:
                    Stock.Sort((p1, p2) => p2.item.CurrentCount.CompareTo(p1.item.CurrentCount));
                    break;
                case InventorySortOrder.ItemTypeAsc:
                    Stock.Sort((p1, p2) => p1.item.itemType.CompareTo(p2.item.itemType));
                    break;
                case InventorySortOrder.ItemTypeDesc:
                    Stock.Sort((p1, p2) => p2.item.itemType.CompareTo(p1.item.itemType));
                    break;
                case InventorySortOrder.RarityAsc:
                    Stock.Sort((p1, p2) => p1.item.rarity.CompareTo(p2.item.rarity));
                    break;
                case InventorySortOrder.RarityDesc:
                    Stock.Sort((p1, p2) => p2.item.rarity.CompareTo(p1.item.rarity));
                    break;
                case InventorySortOrder.ValueAsc:
                    Stock.Sort((p1, p2) => p1.item.value.CompareTo(p2.item.value));
                    break;
                case InventorySortOrder.ValueDesc:
                    Stock.Sort((p1, p2) => p2.item.value.CompareTo(p1.item.value));
                    break;
                case InventorySortOrder.WeightAsc:
                    Stock.Sort((p1, p2) => p1.item.weight.CompareTo(p2.item.weight));
                    break;
                case InventorySortOrder.WeightDesc:
                    Stock.Sort((p1, p2) => p2.item.weight.CompareTo(p1.item.weight));
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void AddBoughtStock(InventoryItem item, int count)
        {
            foreach (StockItemReference stockItem in Stock)
            {
                if (stockItem.item.name == item.name)
                {
                    stockItem.count += count;
                    if (allowBuyBack)
                    {
                        stockItem.bought += count;
                    }
                    return;
                }
            }

            Stock.Add(new StockItemReference(item, count, count));
        }

        private void RemoveBoughtStock(InventoryItem item, int count)
        {
            foreach (StockItemReference stockItem in Stock)
            {
                if (stockItem.item.name == item.name)
                {
                    stockItem.count -= 1;
                    stockItem.bought -= 1;
                    if (stockItem.count == 0)
                    {
                        Stock.Remove(stockItem);
                    }
                    return;
                }
            }
        }

        private void RemoveStock(InventoryItem item, int count)
        {
            foreach (StockItemReference stockItem in Stock)
            {
                if (stockItem.item.name == item.name)
                {
                    stockItem.count -= 1;
                    if (stockItem.bought > stockItem.count)
                    {
                        stockItem.bought = stockItem.count;
                    }
                    if (stockItem.count == 0)
                    {
                        Stock.Remove(stockItem);
                    }
                    return;
                }
            }
        }

        private void SetStock(ItemReference item)
        {
            foreach (StockItemReference stockItem in Stock)
            {
                if (stockItem.item.name == item.item.name)
                {
                    if (stockItem.count < item.count)
                    {
                        stockItem.count = item.count;
                    }
                    return;
                }
            }

            Stock.Add(new StockItemReference(item));
        }

        private void UpdateReplenishment()
        {
            if (!stockReplenishes) return;
            replenishWait -= Time.deltaTime;

            if (replenishWait <= 0 && !InTransaction)
            {
                StartCoroutine("ReplenishStock");
                replenishWait = replenishTime;
            }
        }

        #endregion

    }
}