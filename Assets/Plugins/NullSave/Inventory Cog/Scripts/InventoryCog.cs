using NullSave.TOCK.Stats;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [DefaultExecutionOrder(-100)]
    [HierarchyIcon("inventory_icon", "#ffffff")]
    public class InventoryCog : MonoBehaviour
    {

        #region Variables

        public List<ItemReference> startingItems;
        public float currency;

        // Inventory Menu UI
        public NavigationType menuMode;
        public string menuButton = "Cancel";
        public KeyCode menuKey = KeyCode.I;
        public InventoryMenuUI menu;
        public MenuOpenType openType;
        public Transform menuContainer;
        public string menuSpawnTag;

        // Pickup & Drop
        public PromptUI pickupUI;
        public PopupUI pickupPopup;
        public bool spawnDropManually;
        public Vector3 dropOffset = new Vector3(0, 1.5f, 2);

        // Container UI
        public PromptUI containerUI;
        public ContainerMenuUI containerMenu;
        public PopupUI containerPopup;

        // Merchant
        public PromptUI merchantUI;
        public MerchantMenuUI merchantMenu;
        public PopupUI merchantPopup;

        // Renaming
        public RenamePrompt renamePrompt;
        public MenuOpenType renameOpenType;
        public Transform renameContainer;
        public string renameSpawnTag;

        // Pickup & Drop (obsolete)
        public PickupDetection pickupDetection = PickupDetection.Trigger;
        public Vector3 raycastOffset = Vector3.zero;
        public LayerMask raycastCulling = 1;
        public float maxDistance = 1.5f;
        public PickupType pickupMode;
        public string pickupButton = "Submit";
        public KeyCode pickupKey = KeyCode.E;

        // Container UI (obsolete)
        public NavigationType containerMode;
        public PickupDetection containerDetection = PickupDetection.Trigger;
        public string containerButton = "Submit";
        public KeyCode containerKey = KeyCode.E;
        public MenuOpenType containerOpenType;
        public Transform containerMenuContainer;
        public string containerSpawnTag;

        // Merchant UI (obsolete)
        public PickupDetection merchantDetection = PickupDetection.Trigger;
        public NavigationType merchantMode;
        public string merchantButton = "Submit";
        public KeyCode merchantKey = KeyCode.E;
        public MenuOpenType merchantOpenType;
        public Transform merchantMenuMerchant;
        public string merchantSpawnTag;

        // Skills
        public List<string> skillSlots;
        private Dictionary<string, string> assignedSkills;

        // Crafting
        public List<CraftingResult> failedResult;

        // Events
        public ItemCountChanged onItemDropped, onItemAdded, onItemRemoved, onSpawnDropRequested;
        public ItemChanged onItemEquipped, onItemStored, onItemUnequipped;
        public CraftingFailed onCraftingFailed, onCraftQueued, onQueuedCraftComplete;
        public UnityEvent onMenuOpen, onMenuClose;
        public SkillSlotChanged onSkillSlotChanged;

        // Loadouts
        public InventoryLoadout[] loadouts = new InventoryLoadout[5];

        private Dictionary<string, InventoryItem> activeAmmo;
        private InventoryMenuUI spawnedMenu;
        private RenamePrompt spawnedRename;

        private bool initialized;

        // Editor
        public int z_display_flags = 4095;

        #endregion

        #region Properties

        /// <summary>
        /// Get list of instantiated categories
        /// </summary>
        public List<Category> Categories { get; private set; }

        public List<CraftingQueueItem> CraftingQueue { get; private set; }

        public EquipPoint[] EquipPoints { get; private set; }

        /// <summary>
        /// Get/Set menu open flag
        /// </summary>
        public bool IsMenuOpen
        {
            get
            {
                return spawnedMenu != null;
            }
        }

        public bool IsPromptOpen
        {
            get
            {
                if (spawnedRename != null) return true;
                if (pickupPopup.ActivePopup != null) return true;
                if (containerPopup.ActivePopup != null) return true;
                if (merchantPopup.ActivePopup != null) return true;
                return false;
            }
        }

        /// <summary>
        /// Get list of instantiated items
        /// </summary>
        public List<InventoryItem> Items { get; private set; }

        /// <summary>
        /// Get list of instantiated recipies
        /// </summary>
        public List<CraftingRecipe> Recipes { get; private set; }

        /// <summary>
        /// Resort the inventory
        /// </summary>
        /// <param name="sortOrder"></param>
        public void Sort(InventorySortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case InventorySortOrder.ConditionAsc:
                    Items.Sort((p1, p2) => p1.condition.CompareTo(p2.condition));
                    break;
                case InventorySortOrder.ConditionDesc:
                    Items.Sort((p1, p2) => p2.condition.CompareTo(p1.condition));
                    break;
                case InventorySortOrder.DisplayNameAsc:
                    Items.Sort((p1, p2) => p1.DisplayName.CompareTo(p2.DisplayName));
                    break;
                case InventorySortOrder.DisplayNameDesc:
                    Items.Sort((p1, p2) => p2.DisplayName.CompareTo(p1.DisplayName));
                    break;
                case InventorySortOrder.ItemCountAsc:
                    Items.Sort((p1, p2) => p1.CurrentCount.CompareTo(p2.CurrentCount));
                    break;
                case InventorySortOrder.ItemCountDesc:
                    Items.Sort((p1, p2) => p2.CurrentCount.CompareTo(p1.CurrentCount));
                    break;
                case InventorySortOrder.ItemTypeAsc:
                    Items.Sort((p1, p2) => p1.itemType.CompareTo(p2.itemType));
                    break;
                case InventorySortOrder.ItemTypeDesc:
                    Items.Sort((p1, p2) => p2.itemType.CompareTo(p1.itemType));
                    break;
                case InventorySortOrder.RarityAsc:
                    Items.Sort((p1, p2) => p1.rarity.CompareTo(p2.rarity));
                    break;
                case InventorySortOrder.RarityDesc:
                    Items.Sort((p1, p2) => p2.rarity.CompareTo(p1.rarity));
                    break;
                case InventorySortOrder.ValueAsc:
                    Items.Sort((p1, p2) => p1.value.CompareTo(p2.value));
                    break;
                case InventorySortOrder.ValueDesc:
                    Items.Sort((p1, p2) => p2.value.CompareTo(p1.value));
                    break;
                case InventorySortOrder.WeightAsc:
                    Items.Sort((p1, p2) => p1.weight.CompareTo(p2.weight));
                    break;
                case InventorySortOrder.WeightDesc:
                    Items.Sort((p1, p2) => p2.weight.CompareTo(p1.weight));
                    break;
            }

            foreach (Category category in Categories)
            {
                category.Sort(sortOrder);
            }
        }

        public StatsCog StatsCog { get; private set; }

        /// <summary>
        /// Get total weight of all held items
        /// </summary>
        public float TotalWeight { get; private set; }

        #endregion

        #region Unity Methods

        private void OnDisable()
        {
            // Unsubscribe from points
            foreach (EquipPoint point in EquipPoints)
            {
                point.onItemEquipped.RemoveListener(PointEquipped);
                point.onItemStored.RemoveListener(PointStored);
                point.onItemUnequipped.RemoveListener(PointUnequipped);
            }
        }

        private void OnEnable()
        {
            if (StatsCog == null)
            {
                StatsCog = GetComponentInChildren<Stats.StatsCog>();
            }
            Animator animator = GetComponentInChildren<Animator>();

            // Subscribe to points
            EquipPoints = GetComponentsInChildren<EquipPoint>();
            foreach (EquipPoint point in EquipPoints)
            {
                point.Inventory = this;
                point.Animator = animator;
                point.onItemEquipped.AddListener(PointEquipped);
                point.onItemStored.AddListener(PointStored);
                point.onItemUnequipped.AddListener(PointUnequipped);
                point.StatsCog = StatsCog;
            }

            if (!initialized)
            {
                // Setup ammo
                activeAmmo = new Dictionary<string, InventoryItem>();

                // Setup categories
                Categories = new List<Category>();
                if (InventoryDB.Categories != null)
                {
                    foreach (Category category in InventoryDB.Categories)
                    {
                        if (category != null)
                        {
                            Category instance = Instantiate(category);
                            instance.name = category.name;
                            instance.StatsCog = StatsCog;
                            instance.Initialize();
                            Categories.Add(instance);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("No categories loaded into Inventory DB");
                }

                // Setup items
                Items = new List<InventoryItem>();
                foreach (ItemReference item in startingItems)
                {
                    if (item != null)
                    {
                        AddToInventory(item.item, item.count);
                    }
                }

                // Setup Recipies
                Recipes = new List<CraftingRecipe>();
                if (InventoryDB.Recipes != null)
                {
                    foreach (CraftingRecipe recipe in InventoryDB.Recipes)
                    {
                        if (recipe != null)
                        {
                            CraftingRecipe instance = Instantiate(recipe);
                            instance.name = recipe.name;
                            instance.Initialize(this);
                            Recipes.Add(recipe);
                        }
                    }
                }

                // Setup crafting queue
                CraftingQueue = new List<CraftingQueueItem>();

                // Loadouts
                for (int i = 0; i < 5; i++)
                {
                    loadouts[i] = new InventoryLoadout();
                }

                assignedSkills = new Dictionary<string, string>();

                initialized = true;
            }

            StartCoroutine("LoadPromptUIs");
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.enabled) return;
            TriggerEnter(other.gameObject);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.enabled) return;
            TriggerEnter(other.gameObject);
        }

        public void OnTriggerExit(Collider other)
        {
            if (!other.enabled) return;
            TriggerExit(other.gameObject);
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (!other.enabled) return;
            TriggerExit(other.gameObject);
        }

        private void Update()
        {
            if (!IsMenuOpen)
            {
#if GAME_COG
                if ((menuMode == NavigationType.ByButton && GameCog.Input.GetButtonDown(menuButton)) ||
                    (menuMode == NavigationType.ByKey && GameCog.Input.GetKeyDown(menuKey)))
#else
                if ((menuMode == NavigationType.ByButton && Input.GetButtonDown(menuButton)) ||
                    (menuMode == NavigationType.ByKey && Input.GetKeyDown(menuKey)))
#endif
                {
                    MenuOpen();
                    return;
                }

                pickupPopup.UpdateRaycast(typeof(LootItem), transform);
                if (pickupPopup.CheckInput())
                {
                    PerformPickup();
                }

                containerPopup.UpdateRaycast(typeof(InventoryContainer), transform);
                if (containerPopup.CheckInput())
                {
                    TransferMenuOpen();
                }

                merchantPopup.UpdateRaycast(typeof(InventoryMerchant), transform);
                if (merchantPopup.CheckInput())
                {
                    MerchantMenuOpen();
                }
            }

            UpdateCraftingQueue();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add item to inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>Count to drop from full inventory</returns>
        public int AddToInventory(LootItem item, bool allowAutoEquip = true)
        {
            currency += item.currency;
            item.currency = 0;
            if (item.item == null)
            {
                item.onLoot?.Invoke();
                return 0;
            }

            int looted = AddToInventory(item.GenerateValues(), item.count, allowAutoEquip);
            if (looted != item.count) item.onLoot?.Invoke();
            return looted;
        }

        /// <summary>
        /// Add item to inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>Count to drop from full inventory</returns>
        public int AddToInventory(InventoryItem item, int count, bool allowAutoEquip = true, bool raiseEvents = true)
        {
            if (item == null) return count;

            int orgCount = count;
            int dropCount = 0;

            // Check container
            if (item.itemType == ItemType.Container && item.mustEmptyToHold && item.storedItems.Count > 0)
            {
                return count;
            }

            // Add to current stack if able
            if (item.canStack)
            {
                InventoryItem localItem;
                if (IsItemInInventory(item, out localItem))
                {
                    dropCount = AddToItemStack(localItem, count);
                    if (raiseEvents) onItemAdded?.Invoke(item, orgCount - dropCount);
                    return dropCount;
                }
            }

            int holdOver = 0;

            // Add new item
            InventoryItem invItem = Instantiate(item);
            invItem.Initialize(this);
            invItem.name = item.name;
            invItem.CurrentCount = count;
            invItem.value = item.value;
            invItem.rarity = item.rarity;
            invItem.slotId = item.slotId;

            if (invItem.useSlotId)
            {
                if (IsSlotIdUsed(invItem.slotId))
                {
                    invItem.slotId = GetFirstFreeSlotId();
                }
            }

            // Check stack
            if (invItem.canStack && invItem.CurrentCount > invItem.countPerStack)
            {
                holdOver = invItem.CurrentCount - invItem.countPerStack;
                invItem.CurrentCount = invItem.countPerStack;
            }
            else if (invItem.CurrentCount > 1 && !invItem.canStack)
            {
                holdOver = invItem.CurrentCount - 1;
                invItem.CurrentCount = 1;
            }

            // Do we have an empty slot?
            Category category = GetItemCategory(invItem);
            if (CategoryHasFreeSlot(category) && ItemHasFreeStack(invItem, category))
            {
                // Add to inventory
                Items.Add(invItem);
                category.AddItem(invItem);
                TotalWeight += invItem.weight * invItem.CurrentCount;

                // Auto equip
                if (allowAutoEquip)
                {
                    switch (invItem.autoEquip)
                    {
                        case AutoEquipMode.Always:
                            EquipFirstOrEmpty(invItem);
                            break;
                        case AutoEquipMode.IfSlotFree:
                            EquipFirstEmpty(invItem);
                            break;
                    }
                }

                // Set active
                if (invItem.itemType == ItemType.Ammo)
                {
                    if (GetSelectedAmmo(invItem.ammoType) == null)
                    {
                        SetSelectedAmmo(invItem);
                    }
                }
            }
            else
            {
                if (raiseEvents) onItemAdded?.Invoke(item, orgCount - holdOver);
                return holdOver;
            }

            if (holdOver > 0)
            {
                dropCount = AddToInventory(invItem, holdOver, allowAutoEquip, false);
                if (raiseEvents) onItemAdded?.Invoke(item, orgCount - dropCount);
                return dropCount;
            }

            if (raiseEvents) onItemAdded?.Invoke(item, orgCount);
            return 0;
        }

        /// <summary>
        /// Check if any attachments that fit specified item are available in inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AttachmentsAvailableForItem(InventoryItem item)
        {
            switch (item.attachRequirement)
            {
                case AttachRequirement.InCategory:
                    List<string> catNames = new List<string>();
                    foreach (Category cat in item.attachCatsFilter)
                    {
                        catNames.Add(cat.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (catNames.Contains(invItem.category.name) && CanAttachToItem(invItem, item))
                        {
                            return true;
                        }
                    }
                    break;
                case AttachRequirement.InItemList:
                    List<string> itemNames = new List<string>();
                    foreach (InventoryItem itm in item.attachItemsFilter)
                    {
                        itemNames.Add(itm.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (itemNames.Contains(invItem.name) && CanAttachToItem(invItem, item))
                        {
                            return true;
                        }
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Breakdown an item and add parts to inventory
        /// </summary>
        /// <param name="item"></param>
        public void BreakdownItem(InventoryItem item)
        {
            if (!item.CanBreakdown || item.CurrentCount < 1) return;

            foreach (ItemReference component in item.breakdownResult)
            {
                AddToInventory(component.item, component.count);
            }

            item.CurrentCount -= 1;
            if (item.CurrentCount == 0) FinalizeRemove(item, GetItemCategory(item));
        }

        /// <summary>
        /// Check if an attachment can be added to a specified item
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="hostItem"></param>
        /// <returns></returns>
        public bool CanAttachToItem(InventoryItem attachment, InventoryItem hostItem)
        {
            if (attachment.itemType != ItemType.Attachment || attachment.IsAttached) return false;
            List<string> availSlotNames = new List<string>();
            foreach (AttachmentSlot slot in hostItem.Slots)
            {
                availSlotNames.Add(slot.AttachPoint.pointId);
            }
            foreach (string slotName in attachment.attachPoints)
            {
                if (availSlotNames.Contains(slotName)) return true;
            }
            return false;
        }

        /// <summary>
        /// Remove all items from inventory
        /// </summary>
        public void ClearInventory()
        {
            foreach (Category category in Categories)
            {
                category.Clear();
            }

            TotalWeight = 0;
            Items.Clear();
            CraftingQueue.Clear();
        }

        /// <summary>
        /// Close inventory menu
        /// </summary>
        public void CloseRenamePrompt()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif
            switch (openType)
            {
                case MenuOpenType.ActiveGameObject:
                    renamePrompt.gameObject.SetActive(false);
                    break;
                default:
                    Destroy(spawnedRename.gameObject);
                    break;
            }

            spawnedRename = null;
            onMenuClose?.Invoke();
        }

        /// <summary>
        /// Craft a recipe X times
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<CraftingResult> Craft(CraftingRecipe recipe, int count, bool addToInventory = true, bool removeFromCounts = true)
        {
            if (!recipe.Unlocked)
            {
                return new List<CraftingResult>();
            }

            // Create list of items (return if not in stock)
            List<ItemReference> master = new List<ItemReference>();
            if (recipe.componentType == ComponentType.Standard)
            {
                foreach (ItemReference component in recipe.components)
                {
                    if (GetItemTotalCount(component.item) < component.count * count) return null;
                    ItemReference newComponent = new ItemReference();
                    newComponent.count = component.count * count;
                    newComponent.item = component.item;
                    master.Add(newComponent);
                }
            }
            else
            {
                foreach (AdvancedComponent component in recipe.advancedComponents)
                {
                    if (GetItemTotalCount(component.item, component.minCondition, component.minRarity) < component.count * count) return null;
                    ItemReference newComponent = new ItemReference();
                    newComponent.count = component.count * count;
                    newComponent.item = component.item;
                    newComponent.item.rarity = component.minRarity;
                    newComponent.item.condition = component.minCondition;
                    master.Add(newComponent);
                }
            }

            // Queue non-instant recipes
            if (recipe.craftTime != CraftingTime.Instant)
            {
                // Create entry
                CraftingQueueItem queueItem = new CraftingQueueItem();
                queueItem.recipe = recipe;
                queueItem.timeStarted = System.DateTime.Now;
                queueItem.count = count;
                queueItem.usedComponents = master;
                if (recipe.craftTime == CraftingTime.RealTime)
                {
                    queueItem.realWorldEnd = System.DateTime.Now.AddSeconds(recipe.craftSeconds);
                }
                else
                {
                    queueItem.secondsRemaining = recipe.craftSeconds;
                }
                CraftingQueue.Add(queueItem);

                // Remove counts
                if (removeFromCounts)
                {
                    foreach (ItemReference item in master)
                    {
                        if (recipe.componentType == ComponentType.Standard)
                        {
                            RemoveItem(item.item, item.count);
                        }
                        else
                        {
                            RemoveItem(item.item, item.count, item.item.condition, item.item.rarity);
                        }
                    }
                }

                // Raise queue event
                onCraftQueued?.Invoke(recipe, count);

                return new List<CraftingResult>();
            }

            // Get success/fail count
            int successCount = 0;
            int failCount = 0;
            float successChange = recipe.GetSuccessChance();
            if (successChange == 1)
            {
                successCount = count;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    float craftRoll = Random.Range(0f, 1f);

                    if (craftRoll >= successChange)
                    {
                        failCount += 1;
                    }
                }
            }

            List<CraftingResult> results = new List<CraftingResult>();

            // Add success result to inventory
            if (successCount > 0)
            {
                recipe.SuccessCount += successCount;
                if (recipe.FirstCrafted == null) recipe.FirstCrafted = System.DateTime.Now;
                recipe.LastCrafted = System.DateTime.Now;

                foreach (CraftingResult item in recipe.result)
                {
                    InventoryItem result = Instantiate(item.item);
                    item.SetResults(master, result);
                    result.name = item.item.name;
                    result.InstanceId = System.Guid.NewGuid().ToString();
                    if (addToInventory)
                    {
                        AddToInventory(result, item.count * successCount);
                    }

                    CraftingResult craft = new CraftingResult();
                    craft.item = result;
                    craft.count = result.CurrentCount;
                    results.Add(craft);
                }
            }

            // Add fail result to inventory
            if (failCount > 0)
            {
                recipe.FailCount += failCount;

                foreach (CraftingResult item in recipe.failResult)
                {
                    InventoryItem result = Instantiate(item.item);
                    item.SetResults(master, result);
                    result.name = item.item.name;
                    result.InstanceId = System.Guid.NewGuid().ToString();
                    if (addToInventory)
                    {
                        AddToInventory(result, item.count * successCount);
                    }

                    CraftingResult craft = new CraftingResult();
                    craft.item = result;
                    craft.count = result.CurrentCount;
                    results.Add(craft);
                }

                onCraftingFailed?.Invoke(recipe, failCount);
            }

            // Remove components from inventory
            if (removeFromCounts)
            {
                foreach (ItemReference item in master)
                {
                    if (recipe.componentType == ComponentType.Standard)
                    {
                        RemoveItem(item.item, item.count);
                    }
                    else
                    {
                        RemoveItem(item.item, item.count, item.item.condition, item.item.rarity);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Attempt to craft using a list of items
        /// </summary>
        /// <param name="items">References to items and counts</param>
        /// <param name="addToInventory">Add result(s) to inventory if true</param>
        /// <param name="removeFromCounts">Remove used items from inventory when true</param>
        /// <returns></returns>
        public List<CraftingResult> Craft(List<ItemReference> items, bool addToInventory = true, bool removeFromCounts = true)
        {
            // Create master list
            int i;
            bool found;
            List<ItemReference> master = new List<ItemReference>();
            foreach (ItemReference item in items)
            {
                found = false;
                for (i = 0; i < master.Count; i++)
                {
                    if (master[i].item == item.item)
                    {
                        master[i].count += item.count;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    master.Add(item);
                }
            }

            CraftingRecipe recipe = GetRecipe(master);
            if (recipe == null)
            {
                if (!recipe.Unlocked)
                {
                    return new List<CraftingResult>();
                }

                if (failedResult != null)
                {
                    if (addToInventory)
                    {
                        foreach (CraftingResult item in failedResult)
                        {
                            InventoryItem result = Instantiate(item.item);
                            result.name = item.item.name;
                            result.InstanceId = System.Guid.NewGuid().ToString();
                            result.CurrentCount = item.count;
                            item.SetResults(master, result);
                            AddToInventory(result, item.count);
                        }
                    }

                    if (removeFromCounts)
                    {
                        foreach (ItemReference item in items)
                        {
                            UseItem(item.item, item.count);
                        }
                    }

                    return failedResult;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return Craft(recipe, 1, addToInventory, removeFromCounts);
            }
        }

        /// <summary>
        /// Complete a queue item immediately
        /// </summary>
        /// <param name="recipe"></param>
        public void CompleteQueueRecipe(CraftingQueueItem recipe)
        {
            foreach (CraftingQueueItem item in CraftingQueue)
            {
                if (item == recipe)
                {
                    CraftingQueue.Remove(item);
                    CompleteCraft(item.recipe, item.count, item.usedComponents);
                    onQueuedCraftComplete?.Invoke(item.recipe, item.count);
                }
            }
        }

        /// <summary>
        /// Use up a consumable or ingredient item and apply mods
        /// Mods requires Stats Cog
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void ConsumeItem(InventoryItem item, int count)
        {
            switch (item.itemType)
            {
                case ItemType.Consumable:
                    while (count > 0)
                    {
                        if (item.CurrentCount > 0)
                        {
                            StatsCog.AddInventoryEffects(item, 1);
                            count -= 1;
                            item.CurrentCount -= 1;
                            TotalWeight -= item.weight;

                            if (item.CurrentCount == 0) FinalizeRemove(item, GetItemCategory(item));
                        }
                        else
                        {
                            FinalizeRemove(item, GetItemCategory(item));
                            InventoryItem newItem = GetItemFromInventory(item);
                            if (newItem != null)
                            {
                                ConsumeItem(newItem, count);
                            }
                            return;
                        }
                    }
                    break;
                default:
                    if (item.CurrentCount > count)
                    {
                        item.CurrentCount -= count;
                        TotalWeight -= item.weight * count;
                        StatsCog.AddInventoryEffects(item, count);

                    }
                    else if (item.CurrentCount == count)
                    {
                        FinalizeRemove(item, GetItemCategory(item));
                        TotalWeight -= item.weight * count;
                        StatsCog.AddInventoryEffects(item, count);
                    }
                    else
                    {
                        count -= item.CurrentCount;
                        FinalizeRemove(item, GetItemCategory(item));
                        TotalWeight -= item.weight * item.CurrentCount;
                        StatsCog.AddInventoryEffects(item, count);

                        InventoryItem newItem = GetItemFromInventory(item);
                        if (newItem != null)
                        {
                            ConsumeItem(newItem, count);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Drop an item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void DropItem(InventoryItem item, int count, bool spawnDrop = true)
        {
            if (count < 1) return;

            if (Items.Contains(item))
            {
                if (item.canDrop)
                {
                    if (item.canStack && item.CurrentCount > count)
                    {
                        item.CurrentCount -= count;
                        TotalWeight -= item.weight * count;
                        if (spawnDrop)
                        {
                            SpawnDrop(item, count);
                        }
                    }
                    else if (item.canStack)
                    {
                        if (item.EquipState != EquipState.NotEquipped)
                        {
                            item.CurrentEquipPoint.UnequipItem();
                        }
                        if (spawnDrop)
                        {
                            SpawnDrop(item, item.CurrentCount);
                        }
                        FinalizeRemove(item, GetItemCategory(item));
                        count -= item.CurrentCount;
                        TotalWeight -= item.weight * item.CurrentCount;
                        if (count > 0)
                        {
                            DropItem(GetItemFromInventory(item), count, spawnDrop);
                        }
                    }
                    else
                    {
                        if (item.EquipState != EquipState.NotEquipped)
                        {
                            item.CurrentEquipPoint.UnequipItem();
                        }
                        if (spawnDrop)
                        {
                            SpawnDrop(item, 1);
                        }
                        TotalWeight -= item.weight;
                        FinalizeRemove(item, GetItemCategory(item));
                        count -= 1;
                        if (count > 0)
                        {
                            DropItem(item, count);
                        }
                    }
                }

                onItemDropped?.Invoke(item, count);
                return;
            }
        }

        /// <summary>
        /// Equip an item
        /// </summary>
        /// <param name="item"></param>
        public void EquipItem(InventoryItem item)
        {
            InventoryItem targetItem = GetItemInstanceInInventory(item);
            if (targetItem == null) return;

            // Check if item is already equipped
            if (targetItem.CurrentEquipPoint != null)
            {
                return;
            }

            // Equip item
            EquipFirstOrEmpty(targetItem);
        }

        /// <summary>
        /// Equip item to a specific slot
        /// </summary>
        /// <param name="item"></param>
        /// <param name="equipPointId"></param>
        public void EquipItem(InventoryItem item, string equipPointId)
        {
            // Get item reference
            InventoryItem targetItem = GetItemInstanceInInventory(item);
            if (targetItem == null) return;

            // Check if equip point is allowed
            bool validTarget = false;
            foreach (string epi in item.equipPoints)
            {
                if (epi == equipPointId)
                {
                    validTarget = true;
                    break;
                }
            }
            if (!validTarget) return;

            // Get target equip point
            EquipPoint ep = GetEquipPoint(equipPointId);
            if (ep == null) return;

            // Check if item is already equipped
            if (targetItem.CurrentEquipPoint != null)
            {
                // Check if it's already on the requested point
                if (targetItem.CurrentEquipPoint.pointId == equipPointId)
                {
                    return;
                }

                // Unequip from current location
                targetItem.CurrentEquipPoint.UnequipItem();
            }

            // Assign item
            ep.EquipItem(targetItem);
        }

        /// <summary>
        /// Get the skill (InventoryItem) assigned to a slot
        /// </summary>
        /// <param name="skillSlot"></param>
        /// <returns></returns>
        public InventoryItem GetAssignedSkill(string skillSlot)
        {
            if (!assignedSkills.ContainsKey(skillSlot)) return null;
            return GetItemFromInventory(assignedSkills[skillSlot]);
        }

        /// <summary>
        /// Check if there are any attachments compatible with supplied item in the inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool GetAnyAttachmentsInInventory(InventoryItem item)
        {
            if (item == null)
            {
                return false;
            }
            switch (item.attachRequirement)
            {
                case AttachRequirement.InCategory:
                    List<string> catNames = new List<string>();
                    foreach (Category cat in item.attachCatsFilter)
                    {
                        catNames.Add(cat.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (catNames.Contains(invItem.category.name))
                        {
                            return true;
                        }
                    }
                    break;
                case AttachRequirement.InItemList:
                    List<string> itemNames = new List<string>();
                    foreach (InventoryItem itm in item.attachItemsFilter)
                    {
                        itemNames.Add(itm.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (itemNames.Contains(invItem.name))
                        {
                            return true;
                        }
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Get a list of available attachments for item in inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<InventoryItem> GetAvailableAttachments(InventoryItem item)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            switch (item.attachRequirement)
            {
                case AttachRequirement.InCategory:
                    List<string> catNames = new List<string>();
                    foreach (Category cat in item.attachCatsFilter)
                    {
                        catNames.Add(cat.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (catNames.Contains(invItem.category.name) && CanAttachToItem(invItem, item))
                        {
                            result.Add(invItem);
                        }
                    }
                    break;
                case AttachRequirement.InItemList:
                    List<string> itemNames = new List<string>();
                    foreach (InventoryItem itm in item.attachItemsFilter)
                    {
                        itemNames.Add(itm.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (itemNames.Contains(invItem.name) && CanAttachToItem(invItem, item))
                        {
                            result.Add(invItem);
                        }
                    }

                    break;
            }

            return result;
        }

        public List<InventoryItem> GetAvailableAttachments(InventoryItem item, string slotId)
        {
            List<InventoryItem> result = new List<InventoryItem>();
            switch (item.attachRequirement)
            {
                case AttachRequirement.InCategory:
                    List<string> catNames = new List<string>();
                    foreach (Category cat in item.attachCatsFilter)
                    {
                        catNames.Add(cat.name);
                    }

                    foreach (InventoryItem invItem in Items)
                    {
                        if (catNames.Contains(invItem.category.name) && invItem.AttachesToPoint(slotId) && CanAttachToItem(invItem, item))
                        {
                            result.Add(invItem);
                        }
                    }
                    break;
                case AttachRequirement.InItemList:
                    List<string> itemNames = new List<string>();
                    foreach (InventoryItem itm in item.attachItemsFilter)
                    {
                        itemNames.Add(itm.name);
                    }
                    foreach (InventoryItem invItem in Items)
                    {
                        if (itemNames.Contains(invItem.name) && invItem.AttachesToPoint(slotId) && CanAttachToItem(invItem, item))
                        {
                            result.Add(invItem);
                        }
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// Get list of breakable items
        /// </summary>
        /// <returns></returns>
        public List<InventoryItem> GetBreakableItems()
        {
            List<InventoryItem> breakable = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.CanBreakdown)
                {
                    breakable.Add(item);
                }
            }

            return breakable;
        }

        /// <summary>
        /// Get list of breakable items in category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBreakableItems(string categoryName)
        {
            List<InventoryItem> breakable = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.CanBreakdown && item.category.name == categoryName)
                {
                    breakable.Add(item);
                }
            }

            return breakable;
        }

        /// <summary>
        /// Get list of breakable items in category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBreakableItems(Category category)
        {
            return GetBreakableItems(category.name);
        }

        /// <summary>
        /// Get list of breakable items in categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBreakableItems(List<string> categories)
        {
            List<InventoryItem> breakable = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.CanBreakdown && categories.Contains(item.category.name))
                {
                    breakable.Add(item);
                }
            }

            return breakable;
        }

        /// <summary>
        /// Get list of breakable items in categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBreakableItems(List<Category> categories)
        {
            List<string> categoryList = new List<string>();
            foreach (Category category in categories)
            {
                categoryList.Add(category.name);
            }
            return GetBreakableItems(categoryList);
        }

        /// <summary>
        /// Get total number of times a recipe can be crafted with current inventory
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public int GetCraftableCount(CraftingRecipe recipe)
        {
            int craftable = int.MaxValue;
            float itemCount;
            int itemCraftable;
            int baseItemCount = 0;

            if (recipe.craftType == CraftingType.Upgrade)
            {
                // Check for original item
                foreach (InventoryItem item in Items)
                {
                    if (item.name == recipe.baseItem.upgradeItem.name &&
                        item.condition >= recipe.baseItem.minCondition && item.condition <= recipe.baseItem.maxCondition &&
                        item.rarity >= recipe.baseItem.minRarity && item.rarity <= recipe.baseItem.maxRarity)
                    {
                        baseItemCount += 1;
                    }
                }
                if (baseItemCount == 0) return 0;
            }

            if (recipe.componentType == ComponentType.Standard)
            {
                foreach (ItemReference component in recipe.components)
                {
                    itemCount = GetItemTotalCount(component.item);
                    if (itemCount == 0) return 0;
                    itemCraftable = Mathf.FloorToInt(itemCount / component.count);
                    if (itemCraftable == 0) return 0;
                    if (itemCraftable < craftable) craftable = itemCraftable;
                }
            }
            else
            {
                foreach (AdvancedComponent component in recipe.advancedComponents)
                {
                    itemCount = GetItemTotalCount(component.item, component.minCondition, component.minRarity);
                    if (itemCount == 0) return 0;
                    itemCraftable = Mathf.FloorToInt(itemCount / component.count);
                    if (itemCraftable == 0) return 0;
                    if (itemCraftable < craftable) craftable = itemCraftable;
                }
            }

            if (recipe.craftType == CraftingType.Upgrade && craftable > baseItemCount)
            {
                craftable = baseItemCount;
            }

            return craftable;
        }

        /// <summary>
        /// Get all recipes that can be crafted with current inventory items
        /// </summary>
        /// <returns></returns>
        public List<CraftingRecipe> GetCraftableRecipes()
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.Unlocked && GetRecipeCraftable(recipe))
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get all recipes in a category that can be crafted with current inventory items
        /// </summary>
        /// <returns></returns>
        public List<CraftingRecipe> GetCraftableRecipesByCategory(string categoryName)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.craftingCategory.name == categoryName && recipe.Unlocked && GetRecipeCraftable(recipe))
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get all recipes in a category set that can be crafted with current inventory items
        /// </summary>
        /// <returns></returns>
        public List<CraftingRecipe> GetCraftableRecipesByCategory(List<string> categories)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (categories.Contains(recipe.craftingCategory.name) && recipe.Unlocked && GetRecipeCraftable(recipe))
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get instanced copy of a category by name
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public Category GetCategory(string categoryName)
        {
            foreach (Category category in Categories)
            {
                if (category.name == categoryName)
                {
                    return category;
                }
            }

            return null;
        }

        /// <summary>
        /// Get a list of displayed categories
        /// </summary>
        /// <param name="excludeLockedCategories"></param>
        /// <returns></returns>
        public List<Category> GetDisplayedCategories(bool excludeLockedCategories)
        {
            List<Category> categories = new List<Category>();
            foreach (Category category in Categories)
            {
                if (category.displayInList && (category.catUnlocked || !excludeLockedCategories))
                {
                    categories.Add(category);
                }
            }
            return categories;
        }

        /// <summary>
        /// Get equip point by name
        /// </summary>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public EquipPoint GetEquipPoint(string pointId)
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.pointId == pointId)
                {
                    return point;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the available count for active ammo by type
        /// </summary>
        /// <param name="ammoType"></param>
        /// <returns></returns>
        public int GetEquippedAmmoCount(string ammoType)
        {
            if (activeAmmo.ContainsKey(ammoType))
            {
                return GetItemTotalCount(activeAmmo[ammoType]);
            }
            return 0;
        }

        /// <summary>
        /// Get the first unused slot id (does not check if there is room for item)
        /// </summary>
        /// <returns></returns>
        public int GetFirstFreeSlotId()
        {
            List<int> usedId = new List<int>();
            foreach (InventoryItem item in Items)
            {
                if (item.useSlotId)
                {
                    usedId.Add(item.slotId);
                }
            }

            int i = 0;
            while (true)
            {
                if (usedId.Contains(i))
                {
                    i++;
                }
                else
                {
                    return i;
                }
            }
        }

        /// <summary>
        /// Get total count of all items grouped by InventoryItem regardless of stacking
        /// </summary>
        /// <returns></returns>
        public List<ItemReference> GetGroupedCounts()
        {
            Dictionary<string, int> tempResult = new Dictionary<string, int>();

            foreach (InventoryItem item in Items)
            {
                if (tempResult.ContainsKey(item.name))
                {
                    tempResult[item.name] += item.CurrentCount;
                }
                else
                {
                    tempResult.Add(item.name, item.CurrentCount);
                }
            }

            List<ItemReference> result = new List<ItemReference>();
            foreach (KeyValuePair<string, int> entry in tempResult)
            {
                ItemReference item = new ItemReference();
                item.count = entry.Value;
                item.item = GetItemByName(entry.Key);
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Get total count of all items grouped by InventoryItem regardless of stacking
        /// </summary>
        /// <returns></returns>
        public List<ItemReference> GetGroupedCounts(string categoryName)
        {
            Dictionary<string, int> tempResult = new Dictionary<string, int>();

            foreach (InventoryItem item in Items)
            {
                if (item.category.name == categoryName)
                {
                    if (tempResult.ContainsKey(item.name))
                    {
                        tempResult[item.name] += item.CurrentCount;
                    }
                    else
                    {
                        tempResult.Add(item.name, item.CurrentCount);
                    }
                }
            }

            List<ItemReference> result = new List<ItemReference>();
            foreach (KeyValuePair<string, int> entry in tempResult)
            {
                ItemReference item = new ItemReference();
                item.count = entry.Value;
                item.item = GetItemFromInventory(entry.Key);
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Get total count of all items grouped by InventoryItem regardless of stacking
        /// </summary>
        /// <returns></returns>
        public List<ItemReference> GetGroupedCounts(List<string> categoryNames)
        {
            Dictionary<string, int> tempResult = new Dictionary<string, int>();

            foreach (InventoryItem item in Items)
            {
                if (categoryNames.Contains(item.category.name))
                {
                    if (tempResult.ContainsKey(item.name))
                    {
                        tempResult[item.name] += item.CurrentCount;
                    }
                    else
                    {
                        tempResult.Add(item.name, item.CurrentCount);
                    }
                }
            }

            List<ItemReference> result = new List<ItemReference>();
            foreach (KeyValuePair<string, int> entry in tempResult)
            {
                ItemReference item = new ItemReference();
                item.count = entry.Value;
                item.item = GetItemByName(entry.Key);
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Get item by assigned instance id
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public InventoryItem GetItemByInstanceId(string instanceId)
        {
            foreach (InventoryItem item in Items)
            {
                if (item.InstanceId == instanceId)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve an item from the list of all inventory items by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public InventoryItem GetItemByName(string name)
        {
            foreach (InventoryItem item in InventoryDB.AvailableItems)
            {
                if (item != null && item.name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public InventoryItem GetItemInstanceInInventory(InventoryItem item)
        {
            if (!Items.Contains(item))
            {
                foreach (InventoryItem invItem in Items)
                {
                    if (invItem.name == item.name)
                    {
                        return invItem;
                    }
                }
            }
            else
            {
                return item;
            }

            return null;
        }

        /// <summary>
        /// Get a count of item from all stacks in inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetItemTotalCount(InventoryItem item)
        {
            int count = 0;

            foreach (InventoryItem invItem in Items)
            {
                if (invItem.name == item.name)
                {
                    count += invItem.CurrentCount;
                }
            }

            return count;
        }

        /// <summary>
        /// Get a count of item from all stacks in inventory with minimum condition & rarity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="minCondition"></param>
        /// <param name="minRarity"></param>
        /// <returns></returns>
        public int GetItemTotalCount(InventoryItem item, float minCondition, int minRarity)
        {
            int count = 0;

            foreach (InventoryItem invItem in Items)
            {
                if (invItem.name == item.name && invItem.condition >= minCondition && invItem.rarity >= minRarity)
                {
                    count += invItem.CurrentCount;
                }
            }

            return count;
        }

        /// <summary>
        /// Get first active instance of an item from the inventory
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public InventoryItem GetItemFromInventory(string itemName)
        {
            foreach (InventoryItem iitem in Items)
            {
                if (iitem.name == itemName && iitem.CurrentCount > 0)
                {
                    return iitem;
                }
            }
            return null;
        }

        /// <summary>
        /// Get first active instance of an item from the inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public InventoryItem GetItemFromInventory(InventoryItem item)
        {
            foreach (InventoryItem iitem in Items)
            {
                if (iitem.name == item.name && iitem.CurrentCount > 0)
                {
                    return iitem;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a list of all items in a category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItems(string categoryName)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
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
        public List<InventoryItem> GetItems(Category category)
        {
            return GetItems(category.name);
        }

        /// <summary>
        /// Get a list of all items in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItems(List<string> categories)
        {
            List<InventoryItem> result = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
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
        public List<InventoryItem> GetItems(List<Category> categories)
        {
            List<string> categoryNames = new List<string>();
            foreach (Category category in categories)
            {
                categoryNames.Add(category.name);
            }

            return GetItems(categoryNames);
        }

        /// <summary>
        ///  Get a list of all inventory items with specified tag value
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItemsWithTag(string tagName, string value)
        {
            List<InventoryItem> results = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.GetCustomTag(tagName) == value)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        /// <summary>
        ///  Get a list of all inventory items with specified tag value in category
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItemsWithTag(string tagName, string value, string category)
        {
            List<InventoryItem> results = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.category.name == category && item.GetCustomTag(tagName) == value)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        /// <summary>
        ///  Get a list of all inventory items with specified tag value in categories
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<InventoryItem> GetItemsWithTag(string tagName, string value, List<string> categories)
        {
            List<InventoryItem> results = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (categories.Contains(item.category.name) && item.GetCustomTag(tagName) == value)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        /// <summary>
        /// Get the equip pointby id
        /// </summary>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public EquipPoint GetPointById(string pointId)
        {
            // Attempt to find empty slot
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.pointId == pointId)
                {
                    return point;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the equip point that will be used if item is equipped
        /// </summary>
        /// <param name="item">First Empty or First Matching</param>
        /// <returns></returns>
        public EquipPoint GetPointToUse(InventoryItem item)
        {
            // Attempt to find empty slot
            foreach (EquipPoint point in EquipPoints)
            {
                if (item.equipPoints.Contains(point.pointId))
                {
                    if (!point.IsItemEquippedOrStored)
                    {
                        return point;
                    }
                }
            }

            // Find first matching slot
            foreach (EquipPoint point in EquipPoints)
            {
                if (item.equipPoints.Contains(point.pointId))
                {
                    return point;
                }
            }

            return null;
        }

        /// <summary>
        /// Get total count of queued recipe
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public int GetQueuedCount(CraftingRecipe recipe)
        {
            int count = 0;
            foreach (CraftingQueueItem item in CraftingQueue)
            {
                if (item.recipe.name == recipe.name)
                {
                    count += item.count;
                }
            }

            return count;
        }

        /// <summary>
        /// Get the progress of first instance of queued recipe
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public float GetQueuedFirstProgress(CraftingRecipe recipe)
        {
            foreach (CraftingQueueItem item in CraftingQueue)
            {
                if (item.recipe.name == recipe.name)
                {
                    switch (item.recipe.craftTime)
                    {
                        case CraftingTime.Instant:
                            return 0;
                        case CraftingTime.GameTime:
                            return 1 - (item.secondsRemaining / item.recipe.craftSeconds);
                        case CraftingTime.RealTime:
                            return (System.DateTime.Now.Ticks - item.timeStarted.Ticks) / (float)(item.realWorldEnd.Ticks - item.timeStarted.Ticks);
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Find a reciepe based on ingredients
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public CraftingRecipe GetRecipe(List<ItemReference> items)
        {
            bool matches;

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.components.Count == items.Count)
                {
                    matches = true;
                    foreach (ItemReference component in recipe.components)
                    {
                        if (!ListContainsComponent(component, items))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                    {
                        return recipe;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a list of all recipies by category name
        /// </summary>
        /// <param name="craftingCategory"></param>
        /// <returns></returns>
        public List<CraftingRecipe> GetRecipesByCategory(string craftingCategory)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.craftingCategory.name == craftingCategory)
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of all recipies by category name
        /// </summary>
        /// <param name="craftingCategory"></param>
        /// <returns></returns>
        public List<CraftingRecipe> GetRecipesByCategory(List<string> craftingCategories)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (craftingCategories.Contains(recipe.craftingCategory.name))
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Check if inventory contains items needed to craft recipe
        /// </summary>
        /// <returns></returns>
        public bool GetRecipeCraftable(CraftingRecipe recipe)
        {
            if (recipe == null) return false;

            if (recipe.componentType == ComponentType.Standard)
            {
                foreach (ItemReference item in recipe.components)
                {
                    if (GetItemTotalCount(item.item) < item.count) return false;
                }
            }
            else
            {
                foreach (AdvancedComponent item in recipe.advancedComponents)
                {
                    if (GetItemTotalCount(item.item, item.minCondition, item.minRarity) < item.count) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if we have the components required for repair
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool GetRepairable(InventoryItem item)
        {
            if (item == null || !item.CanRepair || item.condition == 1) return false;

            foreach (ItemReference component in item.incrementComponents)
            {
                if (GetItemTotalCount(component.item) < component.count) return false;
            }

            return true;
        }

        /// <summary>
        /// Get a list of all items we have components to repair
        /// </summary>
        /// <returns></returns>
        public List<InventoryItem> GetRepairableItems()
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (GetRepairable(item))
                {
                    repairableItems.Add(item);
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items we have components to repair in a category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairableItems(string categoryName)
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.category.name == categoryName && GetRepairable(item))
                {
                    repairableItems.Add(item);
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items we have components to repair in a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairableItems(Category category)
        {
            return GetRepairableItems(category.name);
        }

        /// <summary>
        /// Get a list of all items we have components to repair in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairableItems(List<string> categories)
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (categories.Contains(item.category.name) && GetRepairable(item))
                {
                    repairableItems.Add(item);
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items we have components to repair in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairableItems(List<Category> categories)
        {
            List<string> categoryNames = new List<string>();
            foreach (Category category in categories)
            {
                categoryNames.Add(category.name);
            }

            return GetRepairableItems(categoryNames);
        }

        /// <summary>
        /// Get the maximum number of increments we can repair w/ current inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetRepairMaxIncrements(InventoryItem item)
        {
            if (item == null || !item.CanRepair || item.condition == 1) return 0;

            int maxRepairCount = Mathf.CeilToInt((1 - item.condition) / item.repairIncrement);
            int availCount = maxRepairCount;
            int itemCount;
            int itemAvail;

            // Calculate how many increments we can repair
            foreach (ItemReference component in item.incrementComponents)
            {
                itemCount = GetItemTotalCount(component.item);
                if (itemCount == 0) return 0;

                itemAvail = Mathf.FloorToInt(itemCount / (float)component.count);
                if (itemAvail == 0) return 0;
                if (itemAvail < availCount)
                {
                    availCount = itemAvail;
                }
            }

            return availCount;
        }

        /// <summary>
        /// Get a list of all items needing repair
        /// </summary>
        /// <returns></returns>
        public List<InventoryItem> GetRepairNeededItems()
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.CanRepair && item.condition < 1)
                {
                    repairableItems.Add(item);
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items needing repair in a category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairNeededItems(string categoryName)
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (item.category.name == categoryName && item.CanRepair && item.condition < 1)
                {
                    repairableItems.Add(item);
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items needing repair in a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairNeededItems(Category category)
        {
            return GetRepairNeededItems(category.name);
        }

        /// <summary>
        /// Get a list of all items needing repair in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairNeededItems(List<string> categories)
        {
            List<InventoryItem> repairableItems = new List<InventoryItem>();

            foreach (InventoryItem item in Items)
            {
                if (categories.Contains(item.category.name))
                {
                    if (item.CanRepair && item.condition < 1)
                    {
                        repairableItems.Add(item);
                    }
                }
            }

            return repairableItems;
        }

        /// <summary>
        /// Get a list of all items needing repair in a list of categories
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public List<InventoryItem> GetRepairNeededItems(List<Category> categories)
        {
            List<string> categoryNames = new List<string>();
            foreach (Category category in categories)
            {
                categoryNames.Add(category.name);
            }

            return GetRepairNeededItems(categoryNames);
        }

        /// <summary>
        /// Get a list of all unlocked recipies by category name
        /// </summary>
        /// <param name="craftingCategory"></param>
        /// <returns></returns>
        public List<CraftingRecipe> GetUnlockedRecipesByCategory(string craftingCategory)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.Unlocked && recipe.craftingCategory.name == craftingCategory)
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a list of all unlocked recipies by category name
        /// </summary>
        /// <param name="craftingCategory"></param>
        /// <returns></returns>
        public List<CraftingRecipe> GetUnlockedRecipesByCategory(List<string> craftingCategories)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();

            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.Unlocked && craftingCategories.Contains(recipe.craftingCategory.name))
                {
                    result.Add(recipe);
                }
            }

            return result;
        }

        /// <summary>
        /// Get the currently active ammo by type
        /// </summary>
        /// <param name="ammoType"></param>
        /// <returns></returns>
        public InventoryItem GetSelectedAmmo(string ammoType)
        {
            if (activeAmmo.ContainsKey(ammoType))
            {
                return activeAmmo[ammoType];
            }
            return null;
        }

        /// <summary>
        /// Load inventory state from stream
        /// </summary>
        /// <param name="stream"></param>
        public void InventoryStateLoad(Stream stream)
        {
            if (stream.Length == 0) return;

            float version = stream.ReadFloat();
            if (version < 1 || version > 1.5f)
            {
                Debug.LogError("Unknown file version");
                return;
            }

            TotalWeight = stream.ReadFloat();
            if (version >= 1.2f)
            {
                currency = stream.ReadFloat();
            }

            Items.Clear();
            string itemName, value;
            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                itemName = stream.ReadStringPacket();
                InventoryItem baseItem = GetItemByName(itemName);
                if (baseItem == null)
                {
                    Debug.Log("Cannot find:" + baseItem);
                }
                InventoryItem item = Instantiate(GetItemByName(itemName));
                item.name = itemName;
                item.Initialize(this);
                item.StateLoad(stream, this);
                Items.Add(item);
            }

            if (version >= 1.4f)
            {
                count = stream.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    LoadCategoryState(stream.ReadStringPacket(), stream);
                }
            }
            else
            {
                foreach (Category category in Categories)
                {
                    category.StateLoad(stream, this);
                }
            }

            // Read EquipPoint data
            foreach (EquipPoint point in EquipPoints)
            {
                point.StateLoad(stream, this);
            }

            if (version >= 1.1f)
            {
                CraftingQueue.Clear();
                count = stream.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    CraftingQueueItem item = new CraftingQueueItem();
                    item.LoadState(stream);
                    CraftingQueue.Add(item);
                }
            }

            if (version >= 1.3f)
            {
                foreach (CraftingRecipe recipe in Recipes)
                {
                    recipe.StateLoad(stream);
                }
            }

            if (version >= 1.5f)
            {
                // Loadouts
                for (int i = 0; i < 5; i++)
                {
                    loadouts[i].Load(stream);
                }

                // Assigned Skills
                assignedSkills.Clear();
                count = stream.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    itemName = stream.ReadStringPacket();
                    value = stream.ReadStringPacket();
                    assignedSkills.Add(itemName, value);
                    onSkillSlotChanged?.Invoke(itemName, GetItemFromInventory(value));
                }
            }
        }

        /// <summary>
        /// Load inventory state from file
        /// </summary>
        /// <param name="filename"></param>
        public void InventoryStateLoad(string filename)
        {
            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    InventoryStateLoad(fs);
                }
            }
            else
            {
                Debug.LogWarning(name + ".InventoryCog.InventoryStateLoad file does not exist: " + filename);
            }
        }

        /// <summary>
        /// Save current inventory state to a stream
        /// </summary>
        /// <param name="stream"></param>
        public void InventoryStateSave(Stream stream)
        {
            // Write version number
            stream.WriteFloat(1.5f);

            stream.WriteFloat(TotalWeight);
            stream.WriteFloat(currency);

            // Write item data
            stream.WriteInt(Items.Count);
            foreach (InventoryItem item in Items)
            {
                stream.WriteStringPacket(item.name);
                item.StateSave(stream);
            }

            // Write category data
            stream.WriteInt(Categories.Count);
            foreach (Category category in Categories)
            {
                stream.WriteStringPacket(category.name);
                category.StateSave(stream);
            }

            // Write EquipPoint data
            foreach (EquipPoint point in EquipPoints)
            {
                point.StateSave(stream);
            }

            // Write CraftingQueue data
            stream.WriteInt(CraftingQueue.Count);
            foreach (CraftingQueueItem item in CraftingQueue)
            {
                item.SaveState(stream);
            }

            // Write recipe data
            foreach (CraftingRecipe recipe in Recipes)
            {
                recipe.StateSave(stream);
            }

            // Write loadouts
            for (int i = 0; i < 5; i++)
            {
                loadouts[i].Save(stream);
            }

            // Write skill slots
            stream.WriteInt(assignedSkills.Count);
            foreach (var item in assignedSkills)
            {
                stream.WriteStringPacket(item.Key);
                stream.WriteStringPacket(item.Value);
            }
        }

        /// <summary>
        /// Save current inventory state to a file
        /// </summary>
        /// <param name="filename"></param>
        public void InventoryStateSave(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                InventoryStateSave(fs);
            }
        }

        /// <summary>
        /// Save inventory to specified loadout
        /// </summary>
        /// <param name="loadoutIndex"></param>
        /// <param name="equippedOnly"></param>
        public void InventoryToLoadout(int loadoutIndex, bool equippedOnly)
        {
            InventoryLoadout loadout = new InventoryLoadout();
            loadout.PopulateFromInventory(this, equippedOnly);
        }

        /// <summary>
        /// Check if an item is using the requested slot id
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public bool IsSlotIdUsed(int slotId)
        {
            foreach (InventoryItem item in Items)
            {
                if (item.useSlotId && item.slotId == slotId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if we have a recipe and components to upgrade an item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ItemCanUpgrade(InventoryItem item)
        {
            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe.craftType == CraftingType.Upgrade && recipe.baseItem.upgradeItem.name == item.name)
                {
                    if (GetRecipeCraftable(recipe)) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Update inventory to reflect loadout
        /// </summary>
        /// <param name="loadoutIndex"></param>
        /// <param name="requireInInventory"></param>
        /// <param name="clearExistingInventory"></param>
        public void LoadoutToInventory(int loadoutIndex, bool requireInInventory, bool clearExistingInventory)
        {
            if (clearExistingInventory)
            {
                ClearInventory();
            }

            InventoryItem item = null;
            foreach (InventoryLoadoutItem loadoutItem in loadouts[loadoutIndex].loadoutItems)
            {
                if (requireInInventory)
                {
                    item = GetItemByInstanceId(loadoutItem.instanceId);
                }
                else
                {
                    item = GetItemByName(item.name);
                }

                if (item != null)
                {
                    if (!requireInInventory)
                    {
                        AddToInventory(item, loadoutItem.count);
                    }
                }
            }
        }

        /// <summary>
        /// Close inventory merchant menu
        /// </summary>
        public void MerchantMenuClose()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif
            merchantPopup.HideInteract();
            merchantPopup.HidePopup();

            onMenuClose?.Invoke();
        }

        /// <summary>
        /// Open inventory merchant menu
        /// </summary>
        public void MerchantMenuOpen()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif
            merchantPopup.ShowInteract();

            MerchantMenuUI menu = merchantPopup.ActiveInteract.GetComponent<MerchantMenuUI>();
            menu.playerInventory = this;
            menu.merchantInventory = (InventoryMerchant)merchantPopup.ActiveTargets[merchantPopup.ActiveTargets.Count - 1];
            menu.RefreshPlayerInventory();
            menu.RefreshMerchantInventory();
            merchantPopup.HidePopup();

            onMenuOpen?.Invoke();
        }

        /// <summary>
        /// Close inventory menu
        /// </summary>
        public void MenuClose()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif
            if (!IsMenuOpen) return;

            switch (openType)
            {
                case MenuOpenType.ActiveGameObject:
                    menu.gameObject.SetActive(false);
                    break;
                default:
                    Destroy(spawnedMenu.gameObject);
                    break;
            }
            spawnedMenu = null;

            onMenuClose?.Invoke();
        }

        /// <summary>
        /// Open inventory menu
        /// </summary>
        public void MenuOpen()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif
            if (IsMenuOpen) return;

            switch (openType)
            {
                case MenuOpenType.ActiveGameObject:
                    spawnedMenu = menu;
                    menu.gameObject.SetActive(true);
                    break;
                case MenuOpenType.SpawnInTransform:
                    spawnedMenu = Instantiate(menu, menuContainer);
                    break;
                case MenuOpenType.SpawnOnFirstCanvas:
                    Canvas canvas = FindObjectOfType<Canvas>();
                    spawnedMenu = Instantiate(menu, canvas.transform);
                    break;
                case MenuOpenType.SpawnInTag:
                    spawnedMenu = Instantiate(menu, GameObject.FindGameObjectWithTag(menuSpawnTag).transform);
                    break;
            }

            spawnedMenu.LoadInventory(this);
            onMenuOpen?.Invoke();
        }

        /// <summary>
        /// Open prompt to rename item
        /// </summary>
        /// <param name="item"></param>
        public void OpenRenamePrompt(InventoryItem item)
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif
            if (renamePrompt == null)
            {
                Debug.LogError("No rename prompt supplied.");
                return;
            }

            if (spawnedRename != null)
            {
                Debug.LogWarning("Rename prompt already open.");
            }

            if (!Items.Contains(item))
            {
                item = GetItemFromInventory(item.name);
                if (item == null)
                {
                    Debug.LogWarning("Requested item not in inventory");
                    return;
                }
            }

            switch (renameOpenType)
            {
                case MenuOpenType.ActiveGameObject:
                    spawnedRename = renamePrompt;
                    spawnedRename.gameObject.SetActive(true);
                    break;
                case MenuOpenType.SpawnInTransform:
                    spawnedRename = Instantiate(renamePrompt, renameContainer);
                    break;
                case MenuOpenType.SpawnOnFirstCanvas:
                    Canvas canvas = FindObjectOfType<Canvas>();
                    spawnedRename = Instantiate(renamePrompt, canvas.transform);
                    break;
                case MenuOpenType.SpawnInTag:
                    spawnedRename = Instantiate(renamePrompt, GameObject.FindGameObjectWithTag(renameSpawnTag).transform);
                    break;
            }

            spawnedRename.Inventory = this;
            spawnedRename.Item = item;
            spawnedRename.SetUI();

            onMenuOpen?.Invoke();
        }

        /// <summary>
        /// Remove an item from inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public void RemoveItem(InventoryItem item, int count)
        {
            if (count < 1) return;

            foreach (InventoryItem invItem in Items)
            {
                if (invItem.name == item.name)
                {
                    if (invItem.canStack && invItem.CurrentCount > count)
                    {
                        invItem.CurrentCount -= count;
                        TotalWeight -= invItem.weight * count;
                    }
                    else if (invItem.canStack)
                    {
                        if (invItem.EquipState != EquipState.NotEquipped)
                        {
                            invItem.CurrentEquipPoint.UnequipItem();
                        }
                        FinalizeRemove(invItem, GetItemCategory(invItem));
                        count -= invItem.CurrentCount;
                        TotalWeight -= invItem.weight * invItem.CurrentCount;
                        if (count > 0)
                        {
                            DropItem(GetItemFromInventory(invItem), count);
                        }
                    }
                    else
                    {
                        if (invItem.EquipState != EquipState.NotEquipped)
                        {
                            invItem.CurrentEquipPoint.UnequipItem();
                        }
                        TotalWeight -= invItem.weight;
                        FinalizeRemove(invItem, GetItemCategory(invItem));
                        count -= 1;
                        if (count > 0)
                        {
                            DropItem(invItem, count);
                        }
                    }

                    onItemRemoved?.Invoke(item, count);
                    return;
                }
            }
        }

        /// <summary>
        /// Remove an item from inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="minCondition"></param>
        /// <param name="minRarity"></param>
        public void RemoveItem(InventoryItem item, int count, float minCondition, int minRarity)
        {
            if (count < 1) return;

            foreach (InventoryItem invItem in Items)
            {
                if (invItem.name == item.name && invItem.rarity >= minRarity && invItem.condition >= minCondition)
                {
                    if (invItem.canStack && invItem.CurrentCount > count)
                    {
                        invItem.CurrentCount -= count;
                        TotalWeight -= invItem.weight * count;
                    }
                    else if (invItem.canStack)
                    {
                        if (invItem.EquipState != EquipState.NotEquipped)
                        {
                            invItem.CurrentEquipPoint.UnequipItem();
                        }
                        FinalizeRemove(invItem, GetItemCategory(invItem));
                        count -= invItem.CurrentCount;
                        TotalWeight -= invItem.weight * invItem.CurrentCount;
                        if (count > 0)
                        {
                            DropItem(GetItemFromInventory(invItem), count);
                        }
                    }
                    else
                    {
                        if (invItem.EquipState != EquipState.NotEquipped)
                        {
                            invItem.CurrentEquipPoint.UnequipItem();
                        }
                        TotalWeight -= invItem.weight;
                        FinalizeRemove(invItem, GetItemCategory(invItem));
                        count -= 1;
                        if (count > 0)
                        {
                            DropItem(invItem, count);
                        }
                    }

                    return;

                }
            }
        }

        /// <summary>
        /// Repair item with components
        /// </summary>
        /// <param name="item"></param>
        public void RepairItem(InventoryItem item)
        {
            if (item == null || !item.CanRepair || item.condition == 1) return;

            // Consume repair components
            int availCount = GetRepairMaxIncrements(item);
            foreach (ItemReference component in item.incrementComponents)
            {
                UseItem(component.item, component.count * availCount);
            }

            // Apply repair
            item.condition = Mathf.Clamp(item.condition + item.repairIncrement * availCount, 0, 1);
        }

        /// <summary>
        /// Set the active ammo for a type
        /// </summary>
        /// <param name="item"></param>
        public void SetSelectedAmmo(InventoryItem item)
        {
            if (activeAmmo.ContainsKey(item.ammoType))
            {
                activeAmmo[item.ammoType] = item;
            }
            else
            {
                activeAmmo.Add(item.ammoType, item);
            }
        }

        public void SkillAssign(InventoryItem item, string skillSlot)
        {
            if (item == null || item.itemType != ItemType.Skill)
            {
                Debug.LogError(name + ".InvnetoryCog.SkillAssign supplied item is not a skill");
                return;
            }

            if (!skillSlots.Contains(skillSlot))
            {
                Debug.LogError(name + ".InvnetoryCog.SkillAssign '" + skillSlot + "' is not a Skill Slot");
                return;
            }

            // Check for item already assigned
            foreach (var skill in assignedSkills)
            {
                if (skill.Value == item.name)
                {
                    if (skill.Key == skillSlot) return;
                    onSkillSlotChanged?.Invoke(skill.Key, null);
                    assignedSkills.Remove(skill.Key);
                    break;
                }
            }

            // Check for existing key w/ different value
            if (assignedSkills.ContainsKey(skillSlot))
            {
                GetItemFromInventory(assignedSkills[skillSlot]).AssignedSkillSlot = null;
                assignedSkills.Remove(skillSlot);
                assignedSkills.Add(skillSlot, item.name);
                item.AssignedSkillSlot = skillSlot;
                onSkillSlotChanged?.Invoke(skillSlot, item);
                return;
            }

            // New entry
            assignedSkills.Add(skillSlot, item.name);
            item.AssignedSkillSlot = skillSlot;
            onSkillSlotChanged?.Invoke(skillSlot, item);
        }

        public void SkillToggle(InventoryItem item, string skillSlot)
        {
            // Check for existing
            if (assignedSkills.ContainsKey(skillSlot))
            {
                if (assignedSkills[skillSlot] == item.name)
                {
                    assignedSkills.Remove(skillSlot);
                    item.AssignedSkillSlot = null;
                    onSkillSlotChanged?.Invoke(skillSlot, null);
                    return;
                }
            }

            SkillAssign(item, skillSlot);
        }

        /// <summary>
        /// Move equipment to storage point
        /// </summary>
        /// <param name="pointId">Equip point id</param>
        public void StorePointById(string pointId)
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.pointId == pointId)
                {
                    point.StoreItem();
                    return;
                }
            }
        }

        /// <summary>
        /// Close inventory container menu
        /// </summary>
        public void TransferMenuClose()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif
            containerPopup.HideInteract();
            containerPopup.HidePopup();

            onMenuClose?.Invoke();
        }

        /// <summary>
        /// Open inventory container menu
        /// </summary>
        public void TransferMenuOpen()
        {
#if GAME_COG
            if (GameCog.IsModalVisible) return;
#endif
            containerPopup.ShowInteract();
            ContainerMenuUI menu = containerPopup.ActiveInteract.GetComponent<ContainerMenuUI>();

            menu.Inventory = this;
            menu.container = (InventoryContainer)containerPopup.ActiveTargets[containerPopup.ActiveTargets.Count - 1];
            menu.RefreshLocalInventory();
            menu.RefreshContainerInventory();
            containerPopup.HidePopup();

            onMenuOpen?.Invoke();
        }

        /// <summary>
        /// Show local pick-up UI and set text
        /// </summary>
        /// <param name="text"></param>
        public void UITextShow(string text)
        {
            SetPromptUI(pickupUI, true, text);
        }

        /// <summary>
        /// Hide local pick-up UI
        /// </summary>
        public void UITextHide()
        {
            SetPromptUI(pickupUI, false, null);
        }

        /// <summary>
        /// Unequip an item (if currently equipped)
        /// </summary>
        /// <param name="item"></param>
        public void UnequipItem(InventoryItem item)
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.IsItemEquipped && point.Item == item)
                {
                    point.UnequipItem();
                    return;
                }

                if (point.IsItemStored && point.storePoint.Item == item)
                {
                    point.UnequipItem();
                    return;
                }
            }
        }

        /// <summary>
        /// Move an item from storage to equip point
        /// </summary>
        /// <param name="pointId">Equip point id</param>
        public void UnstorePointById(string pointId)
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (point.pointId == pointId)
                {
                    point.UnstoreItem();
                    return;
                }
            }
        }

        /// <summary>
        /// Remove count from inventory. Unequip and remove as needed
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="removeOnZero"></param>
        public void UseItem(InventoryItem item, int count, bool removeOnZero = true)
        {
            if (item.CurrentCount > count)
            {
                item.CurrentCount -= count;
                TotalWeight -= item.weight * count;
            }
            else if (item.CurrentCount == count)
            {
                TotalWeight -= item.weight * item.CurrentCount;
                item.CurrentCount = 0;
                if (removeOnZero) FinalizeRemove(item, GetItemCategory(item));
            }
            else
            {
                count -= item.CurrentCount;
                TotalWeight -= item.weight * item.CurrentCount;
                if (removeOnZero) FinalizeRemove(item, GetItemCategory(item));

                InventoryItem newItem = GetItemFromInventory(item);
                if (newItem != null)
                {
                    UseItem(newItem, count, removeOnZero);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add count to item stack
        /// </summary>
        /// <param name="item"></param>
        /// <param name="countToAdd"></param>
        /// <returns></returns>
        private int AddToItemStack(InventoryItem item, int countToAdd)
        {
            int usedStacks = 0;
            int added;
            int i;

            // Append whatever possible to existing stacks
            for (i = 0; i < Items.Count; i++)
            {
                if (Items[i].name == item.name && Items[i].condition == item.condition && Items[i].rarity == item.rarity && !item.IsAttached)
                {
                    usedStacks += 1;
                    if (Items[i].CurrentCount < item.countPerStack)
                    {
                        added = Mathf.Min(countToAdd, item.countPerStack - Items[i].CurrentCount);
                        Items[i].CurrentCount += added;

                        countToAdd -= added;
                        if (countToAdd == 0)
                        {
                            return 0;
                        }
                    }
                }
            }

            // Create new stacks (where possible)
            while (countToAdd > 0)
            {
                // Return if out of space
                if (item.hasMaxStacks && usedStacks >= item.maxStacks)
                {
                    return countToAdd;
                }

                usedStacks += 1;
                InventoryItem newItem = Instantiate(item);
                newItem.name = item.name;
                newItem.Initialize(this);
                added = Mathf.Min(countToAdd, item.countPerStack);
                newItem.CurrentCount = added;
                countToAdd -= added;

                if (newItem.useSlotId)
                {
                    if (IsSlotIdUsed(newItem.slotId))
                    {
                        newItem.slotId = GetFirstFreeSlotId();
                    }
                }

                Items.Add(newItem);
                GetItemCategory(item).AddItem(newItem);
                TotalWeight += newItem.weight + newItem.CurrentCount;
            }

            // Set remainder
            return 0;
        }

        /// <summary>
        /// Check if we have a free slot in our target category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        private bool CategoryHasFreeSlot(Category category)
        {
            if (category == null) return false;
            if (!category.hasMaxSlots) return true;
            if (category.hasLockingSlots) return category.UsedSlots < category.UnlockedSlots;
            return category.UsedSlots < category.MaximumSlots;
        }

        private void CompleteCraft(CraftingRecipe recipe, int count, List<ItemReference> master)
        {
            // Get success/fail count
            int successCount = 0;
            int failCount = 0;
            float successChance = recipe.GetSuccessChance();
            if (successChance == 1)
            {
                successCount = count;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    float craftRoll = Random.Range(0f, 1f);

                    if (craftRoll >= successChance)
                    {
                        failCount += 1;
                    }
                }
            }

            // Add success result to inventory
            List<CraftingResult> results = new List<CraftingResult>();
            if (recipe.craftType == CraftingType.Create)
            {
                if (successCount > 0)
                {
                    recipe.SuccessCount += successCount;
                    if (recipe.FirstCrafted == null) recipe.FirstCrafted = System.DateTime.Now;
                    recipe.LastCrafted = System.DateTime.Now;

                    foreach (CraftingResult item in recipe.result)
                    {
                        InventoryItem result = Instantiate(item.item);
                        item.SetResults(master, result);
                        result.name = item.item.name;
                        result.InstanceId = System.Guid.NewGuid().ToString();
                        AddToInventory(result, item.count * successCount);

                        CraftingResult craft = new CraftingResult();
                        craft.item = result;
                        craft.count = result.CurrentCount;
                        results.Add(craft);
                    }
                }

                // Add fail result to inventory
                if (failCount > 0)
                {
                    recipe.FailCount += failCount;

                    foreach (CraftingResult item in recipe.failResult)
                    {
                        InventoryItem result = Instantiate(item.item);
                        item.SetResults(master, result);
                        result.name = item.item.name;
                        result.InstanceId = System.Guid.NewGuid().ToString();
                        AddToInventory(result, item.count * failCount);

                        CraftingResult craft = new CraftingResult();
                        craft.item = result;
                        craft.count = result.CurrentCount;
                        results.Add(craft);
                    }

                    onCraftingFailed?.Invoke(recipe, failCount);
                }
            }
            else
            {
                if (successCount > 0)
                {
                    InventoryItem item = Instantiate(recipe.upgradeSuccess.upgradeResult);
                    item.name = recipe.upgradeSuccess.upgradeResult.name;
                    item.CurrentCount = successCount;
                    if (recipe.upgradeSuccess.rarity == UpgradeResult.AddAmount) item.rarity = master[0].item.rarity + recipe.upgradeSuccess.rarityChange;
                    if (recipe.upgradeSuccess.condition == UpgradeResult.AddAmount) item.condition = master[0].item.condition + recipe.upgradeSuccess.conditionChange;
                    AddToInventory(item, successCount);
                }
            }
        }

        /// <summary>
        /// Equip only to the first EMPTY slot
        /// </summary>
        /// <param name="item"></param>
        private bool EquipFirstEmpty(InventoryItem item)
        {
            foreach (EquipPoint point in EquipPoints)
            {
                if (item.equipPoints.Contains(point.pointId))
                {
                    if (!point.IsItemEquippedOrStored)
                    {
                        if (item.equipLocation == AutoEquipLocation.AlwaysStore && point.storePoint != null)
                        {
                            point.StoreItem(item);
                        }
                        else
                        {
                            point.EquipItem(item);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Equip to first empty slot, if none equip to first slot
        /// </summary>
        /// <param name="item"></param>
        private void EquipFirstOrEmpty(InventoryItem item)
        {
            // Equip empty if possible
            if (EquipFirstEmpty(item)) return;

            // Swap w/ first item
            foreach (EquipPoint point in EquipPoints)
            {
                if (item.equipPoints.Contains(point.pointId))
                {
                    bool wasStored = point.IsItemStored;
                    //point.UnequipItem();

                    // Assign
                    if (wasStored && item.equipLocation == AutoEquipLocation.MirrorCurrent || (point.storePoint != null && item.equipLocation == AutoEquipLocation.AlwaysStore))
                    {
                        // Store
                        point.StoreItem(item);
                    }
                    else
                    {
                        // Equip
                        point.EquipItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// Finalize removal of item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="category"></param>
        private void FinalizeRemove(InventoryItem item, Category category)
        {
            if (item.CanEquip && item.EquipState != EquipState.NotEquipped) UnequipItem(item);
            category.RemoveItem(item);
            Items.Remove(item);
        }

        private Canvas GetOrCreateCanvas()
        {
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas != null) return canvas;

            GameObject go = new GameObject("InventoryCogCreatedCanvas");
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            go.AddComponent<UnityEngine.EventSystems.EventSystem>();
            go.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            return canvas;
        }

        /// <summary>
        /// Get active category reference for item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Category GetItemCategory(InventoryItem item)
        {
            if (item.category == null)
            {
                Debug.LogError(item.name + ": No category assigned to item.");
                return null;
            }

            foreach (Category category in Categories)
            {
                if (category.name == item.category.name)
                {
                    return category;
                }
            }

            return null;
        }

        /// <summary>
        /// Check if item is in active inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="inventoryItem"></param>
        /// <returns></returns>
        private bool IsItemInInventory(InventoryItem item, out InventoryItem inventoryItem)
        {
            foreach (InventoryItem localItem in Items)
            {
                if (localItem.name == item.name && localItem.rarity == item.rarity &&
                    localItem.condition == item.condition && localItem.value == item.value)
                {
                    inventoryItem = localItem;
                    return true;
                }
            }

            inventoryItem = null;
            return false;
        }

        /// <summary>
        /// Check if we can create a new stack for item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private bool ItemHasFreeStack(InventoryItem item, Category category)
        {
            if (!item.hasMaxStacks) return true;
            return category.GetItemUsedStacks(item) < item.maxStacks;
        }

        /// <summary>
        /// Check if list has all required instances of an item
        /// </summary>
        /// <param name="component"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private bool ListContainsComponent(ItemReference component, List<ItemReference> items)
        {
            foreach (ItemReference item in items)
            {
                if (item.item.name == component.item.name)
                {
                    return item.count == component.count;
                }
            }

            return false;
        }

        /// <summary>
        /// Loads a streamed category
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="stream"></param>
        private void LoadCategoryState(string categoryName, Stream stream)
        {
            foreach (Category cat in Categories)
            {
                if (cat.name == categoryName)
                {
                    cat.StateLoad(stream, this);
                    return;
                }
            }

            Category newCat = ScriptableObject.Instantiate<Category>(Categories[0]);
            newCat.name = categoryName;
            Categories.Add(newCat);
            newCat.StateLoad(stream, this);
        }

        private IEnumerator LoadPromptUIs()
        {
            yield return new WaitForEndOfFrame();

            // Obsolete repair
            if (!pickupPopup.obsoleteFixed)
            {
                pickupPopup.detection = pickupDetection;
                pickupPopup.raycastCulling = raycastCulling;
                pickupPopup.raycastOffset = raycastOffset;
                pickupPopup.maxDistance = maxDistance;
                pickupPopup.selection = pickupMode;
                pickupPopup.selectButton = pickupButton;
                pickupPopup.selectKey = pickupKey;
                pickupPopup.obsoleteFixed = true;
            }

            if (!containerPopup.obsoleteFixed)
            {
                containerPopup.detection = containerDetection;
                containerPopup.maxDistance = maxDistance;
                switch (containerMode)
                {
                    case NavigationType.ByButton:
                        containerPopup.selection = PickupType.ByButton;
                        break;
                    case NavigationType.ByKey:
                        containerPopup.selection = PickupType.ByKey;
                        break;
                    case NavigationType.Manual:
                        containerPopup.selection = PickupType.Manual;
                        break;
                }
                containerPopup.selectButton = containerButton;
                containerPopup.selectKey = containerKey;
                containerPopup.openType = containerOpenType;
                containerPopup.container = containerMenuContainer;
                containerPopup.spawnTag = containerSpawnTag;
                containerPopup.obsoleteFixed = true;
            }

            if (!merchantPopup.obsoleteFixed)
            {
                merchantPopup.detection = merchantDetection;
                merchantPopup.maxDistance = maxDistance;
                switch (merchantMode)
                {
                    case NavigationType.ByButton:
                        merchantPopup.selection = PickupType.ByButton;
                        break;
                    case NavigationType.ByKey:
                        merchantPopup.selection = PickupType.ByKey;
                        break;
                    case NavigationType.Manual:
                        merchantPopup.selection = PickupType.Manual;
                        break;
                }
                merchantPopup.selectButton = merchantButton;
                merchantPopup.selectKey = merchantKey;
                merchantPopup.openType = merchantOpenType;
                merchantPopup.container = merchantMenuMerchant;
                merchantPopup.spawnTag = merchantSpawnTag;
                merchantPopup.obsoleteFixed = true;
            }

            // Initialization
            if (pickupUI != null) pickupPopup.TargetPrompt = pickupUI.gameObject;
            pickupPopup.onPromptRequest.AddListener((PopupUI caller, object target) =>
            {
                LootItem lootItem = (LootItem)target;
                if (!lootItem.enabled)
                {
                    caller.ActiveTargets.Remove(target);
                }
                else
                {
                    if (caller.selection == PickupType.Automatic || lootItem.autoPickup)
                    {
                        currency += lootItem.currency;
                        lootItem.currency = 0;

                        if (lootItem.item != null)
                        {
                            if (lootItem.item.itemType == ItemType.Consumable && StatsCog != null && StatsCog.EvaluateCondition(lootItem.autoConsumeWhen))
                            {
                                StatsCog.AddInventoryEffects(lootItem.item, lootItem.count);
                            }
                            else
                            {
                                AddToInventory(lootItem);
                            }
                        }
                        else
                        {
                            lootItem.onLoot?.Invoke();
                        }

                        caller.ActiveTargets.Remove(target);
                        Destroy(lootItem.gameObject);
                    }
                    else
                    {
                        pickupUI.SetPrompt(lootItem.item == null ? string.Empty : lootItem.item.DisplayName);
                        caller.ShowPopup();
                    }
                }
            });

            if (containerUI != null) containerPopup.TargetPrompt = containerUI.gameObject;
            if (containerMenu != null) containerPopup.InteractInterface = containerMenu.gameObject;
            containerPopup.onPromptRequest.AddListener((PopupUI caller, object target) =>
            {
                InventoryContainer container = (InventoryContainer)target;
                if (!container.enabled)
                {
                    caller.ActiveTargets.Remove(target);
                }
                else
                {
                    if (caller.selection == PickupType.Automatic)
                    {
                        TransferMenuOpen();
                    }
                    else
                    {
                        containerUI.SetPrompt(container.displayName);
                        caller.ShowPopup();
                    }
                }
            });

            if (merchantUI != null) merchantPopup.TargetPrompt = merchantUI.gameObject;
            if (merchantMenu != null) merchantPopup.InteractInterface = merchantMenu.gameObject;
            merchantPopup.onPromptRequest.AddListener((PopupUI caller, object target) =>
            {
                InventoryMerchant merchant = (InventoryMerchant)target;
                if (!merchant.enabled)
                {
                    caller.ActiveTargets.Remove(target);
                }
                else
                {
                    if (caller.selection == PickupType.Automatic)
                    {
                        MerchantMenuOpen();
                    }
                    else
                    {
                        merchantUI.SetPrompt(merchant.displayName);
                        caller.ShowPopup();
                    }
                }
            });

        }

        private void PerformPickup()
        {
            LootItem target = (LootItem)pickupPopup.ActiveTargets[pickupPopup.ActiveTargets.Count - 1];

            currency += target.currency;
            target.currency = 0;
            if (target.item == null)
            {
                target.onLoot?.Invoke();
            }
            else
            {
                if (target.item.itemType == ItemType.Consumable && StatsCog != null && StatsCog.EvaluateCondition(target.autoConsumeWhen))
                {
                    StatsCog.AddInventoryEffects(target.item, target.count);
                }
                else
                {
                    int leftOver = AddToInventory(target.item, target.count, target.autoEquip);
                    if (leftOver > 0)
                    {
                        target.count = leftOver;
                        target.gameObject.transform.position = transform.position + (transform.forward * 2);
                    }
                    else
                    {
                        target.onLoot?.Invoke();
                        if (target != null && target.gameObject != null)
                        {
                            Destroy(target.gameObject);
                        }
                    }
                }
            }

            pickupPopup.ActiveTargets.Remove(target);
            pickupPopup.Revalidate();
        }

        /// <summary>
        /// Raise equip event
        /// </summary>
        /// <param name="item"></param>
        private void PointEquipped(InventoryItem item)
        {
            onItemEquipped?.Invoke(item);
        }

        /// <summary>
        /// Raise store event
        /// </summary>
        /// <param name="item"></param>
        private void PointStored(InventoryItem item)
        {
            onItemStored?.Invoke(item);
        }

        /// <summary>
        /// Raise un-equip event
        /// </summary>
        /// <param name="item"></param>
        private void PointUnequipped(InventoryItem item)
        {
            onItemUnequipped?.Invoke(item);
        }

        private void SetPromptUI(PromptUI target, bool enabled, string text)
        {
            if (target == null) return;
            pickupUI.SetPrompt(text);
            pickupPopup.ShowPopup();
        }

        /// <summary>
        /// Spawn drop object as needed
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        private void SpawnDrop(InventoryItem item, int count)
        {
            if (spawnDropManually)
            {
                onSpawnDropRequested?.Invoke(item, count);
            }
            else
            {
                if (item.dropObject == null) return;

                LootItem drop = Instantiate(item.dropObject);
                drop.transform.position = transform.position + dropOffset; // (transform.forward * 2) + (transform.up * 1.5f);
            }
        }

        private void TriggerEnter(GameObject other)
        {
            if (other.GetComponent<LootItemWithUI>() != null) return;
            pickupPopup.TriggerEnter(other, typeof(LootItem));
            containerPopup.TriggerEnter(other, typeof(InventoryContainer));
            merchantPopup.TriggerEnter(other, typeof(InventoryMerchant));
        }

        private void TriggerExit(GameObject other)
        {
            if (other.GetComponent<LootItemWithUI>() != null) return;
            pickupPopup.TriggerExit(other, typeof(LootItem));
            containerPopup.TriggerExit(other, typeof(InventoryContainer));
            merchantPopup.TriggerExit(other, typeof(InventoryMerchant));
        }

        private void UpdateCraftingQueue()
        {
            if (CraftingQueue == null)
            {
                CraftingQueue = new List<CraftingQueueItem>();
                return;
            }

            List<CraftingQueueItem> removeList = new List<CraftingQueueItem>();

            foreach (CraftingQueueItem item in CraftingQueue)
            {
                if (item.recipe.craftTime == CraftingTime.GameTime)
                {
                    item.secondsRemaining = Mathf.Clamp(item.secondsRemaining - Time.deltaTime, 0, item.secondsRemaining);
                    if (item.secondsRemaining == 0)
                    {
                        removeList.Add(item);
                        CompleteCraft(item.recipe, item.count, item.usedComponents);
                    }
                }
                else if (System.DateTime.Now >= item.realWorldEnd)
                {
                    removeList.Add(item);
                    CompleteCraft(item.recipe, item.count, item.usedComponents);
                }
            }

            foreach (CraftingQueueItem item in removeList)
            {
                CraftingQueue.Remove(item);
                onQueuedCraftComplete?.Invoke(item.recipe, item.count);
            }
        }

        #endregion

    }
}