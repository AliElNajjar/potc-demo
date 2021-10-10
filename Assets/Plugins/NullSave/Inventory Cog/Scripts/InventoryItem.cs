using NullSave.TOCK.Stats;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [CreateAssetMenu(menuName = "TOCK/Inventory/Inventory Item", order = 2)]
    public class InventoryItem : ScriptableObject
    {

        #region Variables

        // UI
        public Sprite icon;
        public string displayName;
        [TextArea(2, 5)] public string description;
        public string subtext;
        public Category category;
        public List<InventoryItemUITag> uiTags;
        public CraftingRecipe displayRecipe;
        public bool allowCustomName;
        public bool modifyName;
        public string nameModifier;
        public NameModifierOrder modifierOrder;
        private List<string> append, prepend;
        private string preName, postName;

        // Behaviour
        public List<StringValue> customTags;
        public ItemType itemType;
        public bool usesAmmo;
        public string ammoType;
        public int ammoPerUse = 1;
        public bool displayInInventory = true;
        public bool useSlotId = false;
        public int slotId;
        public bool freeSlotWhenEquipped;
        public bool canSell = true;

        // Preview
        public GameObject previewObject;
        public float previewScale = 1;

        // Base Stats
        [Range(0, 1)] public float condition = 1;
        [Range(0, 10)] public int rarity;
        public float weight;
        public float value;

        // Stacking
        public bool canStack = true;
        public int countPerStack = 64;
        public bool hasMaxStacks = false;
        public int maxStacks = 4;

        // Storage
        public List<InventoryItem> storedItems = new List<InventoryItem>();
        public bool mustEmptyToHold = true;
        public bool hasMaxStoreSlots = true;
        public int maxStoreSlots = 16;
        public bool hasMaxStoreWeight = false;
        public float maxStoreWeight = 100;

        // Equiping
        public GameObject equipObject;
        public CustomizationPoint customizationPoint;
        public string customizationId;
        public AutoEquipMode autoEquip = AutoEquipMode.Never;
        public AutoEquipLocation equipLocation = AutoEquipLocation.MirrorCurrent;
        public List<string> equipPoints;
        public bool canEquip = true;
        public bool canDrop = true;
        public bool canStore = true;
        public BooleanSource equipSource = BooleanSource.Static;
        public string equipExpression = "1 > 0";
        private bool curCanEquip;
        public LootItem dropObject;
        public List<AnimatorMod> equipAnimatorMods;
        public List<AnimatorMod> unequipAnimatorMods;

        // Repair
        public bool canRepair = false;
        public BooleanSource repairSource = BooleanSource.Static;
        public string repairExpression = "1 > 0";
        private bool curCanRepair;

        [Range(0, 1)] public float repairIncrement = 0.05f;
        public float incrementCost = 10;
        public List<ItemReference> incrementComponents;

        // Breakdown
        public bool canBreakdown = false;
        public BooleanSource breakdownSource = BooleanSource.Static;
        public string breakdownExpression = "1 > 0";
        private bool curCanBreakdown;
        public string breakdownCategory;
        public List<ItemReference> breakdownResult;

        // Attachments (Item)
        public AttachRequirement attachRequirement;
        public List<Category> attachCatsFilter;
        public List<InventoryItem> attachItemsFilter;
        public List<AttachmentSlot> attachSlots;

        // Attachments (Child)
        public GameObject attachObject;
        public List<string> attachPoints;

        // Stats & Subscriptions
        public List<StatEffect> statEffects;
        private List<ExpressionSubscription> expressionSubscriptions;

        // Events
        public ItemChanged onAttachmentAdded, onAttachmentRemoved;

        // Editor
        public int z_display_flags = 4095;

        #endregion

        #region Properties

        public string AssignedSkillSlot { get; internal set; }

        /// <summary>
        /// Get if an item can be broken down
        /// </summary>
        public bool CanBreakdown
        {
            get
            {
                if (equipSource == BooleanSource.Static) return canBreakdown;
                return curCanBreakdown;
            }
        }

        /// <summary>
        /// Get if an item can be equipped
        /// </summary>
        public bool CanEquip
        {
            get
            {
                if (itemType == ItemType.Attachment || itemType == ItemType.Skill) return false;
                if (equipSource == BooleanSource.Static) return canEquip;
                return curCanEquip;
            }
        }

        /// <summary>
        /// Get if an item can be repaired
        /// </summary>
        public bool CanRepair
        {
            get
            {
                if (repairSource == BooleanSource.Static) return canRepair;
                return curCanRepair;
            }
        }

        /// <summary>
        /// Calculate the cost of repair by component item value
        /// </summary>
        public float CostOfIncrementRepairComponents
        {
            get
            {
                float cost = 0;
                foreach (ItemReference component in incrementComponents)
                {
                    cost += component.item.value * component.count;
                }
                return cost;
            }
        }

        /// <summary>
        /// Get/Set current on-hand count
        /// </summary>
        public int CurrentCount { get; set; }

        /// <summary>
        /// Get/Set currently assign equip point
        /// </summary>
        public EquipPoint CurrentEquipPoint { get; set; }

        public string CustomName { get; set; }

        public string DisplayName
        {
            get
            {
                if (!allowCustomName || string.IsNullOrEmpty(CustomName)) return preName + displayName + postName;
                return CustomName;
            }
        }

        /// <summary>
        /// Get if any attachments are added to item
        /// </summary>
        public bool HasAttachments
        {
            get
            {
                foreach (AttachmentSlot slot in Slots)
                {
                    if (slot.AttachedItem != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Get reference to Inventory Cog
        /// </summary>
        public InventoryCog InventoryCog { get; private set; }

        /// <summary>
        /// Get/Set current equip stat
        /// </summary>
        public EquipState EquipState { get; set; }

        /// <summary>
        /// Get/Set instance id
        /// </summary>
        public string InstanceId { get; set; }

        public bool IsAttached { get; set; }

        public List<AttachmentSlot> Slots
        {
            get
            {
                if (attachSlots == null) attachSlots = new List<AttachmentSlot>();
                return attachSlots;
            }
        }

        /// <summary>
        /// Get a list of currently stored items (containers only)
        /// </summary>
        public List<InventoryItem> StoredItems
        {
            get
            {
                if (storedItems == null) storedItems = new List<InventoryItem>();
                return storedItems;
            }
        }

        /// <summary>
        /// Get total weight of stored items
        /// </summary>
        public float StoredWeight { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add attachment to item
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public bool AddAttachment(InventoryItem attachment)
        {
            // Find free slot
            foreach (AttachmentSlot slot in Slots)
            {
                if (slot.AttachedItem == null && attachment.attachPoints.Contains(slot.AttachPoint.pointId))
                {
                    AddAttachmentName(attachment, slot);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add attachment to item
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public bool AddAttachment(InventoryItem attachment, int slotId)
        {
            if (attachRequirement == AttachRequirement.NoneAllowed) return false;

            if (Slots.Count >= slotId && Slots[slotId].AttachedItem == null && attachment.attachPoints.Contains(Slots[slotId].AttachPoint.pointId))
            {
                AddAttachmentName(attachment, Slots[slotId]);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add attachment to item
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="attachPoint"></param>
        /// <returns></returns>
        public bool AddAttachment(InventoryItem attachment, string attachPoint)
        {
            if (attachRequirement == AttachRequirement.NoneAllowed) return false;

            // Find slot
            foreach (AttachmentSlot slot in Slots)
            {
                if (slot.AttachPoint.pointId == attachPoint)
                {
                    if (slot.AttachedItem == null)
                    {
                        AddAttachmentName(attachment, slot);
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

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
            return true;
        }

        public bool AttachesToPoint(string slotId)
        {
            return attachPoints.Contains(slotId);
        }

        /// <summary>
        /// Remove all stored items
        /// </summary>
        public void ClearStoredItems()
        {
            StoredWeight = 0;
            storedItems.Clear();
        }

        /// <summary>
        /// Get the value of a custom tag
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public string GetCustomTag(string tagName)
        {
            foreach (StringValue stringValue in customTags)
            {
                if (stringValue.Name == tagName)
                {
                    return stringValue.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the count of an item in storage (containers only)
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
        /// Initalie item and add subscriptions as needed
        /// </summary>
        /// <param name="inventoryCog"></param>
        public void Initialize(InventoryCog inventoryCog)
        {
            InventoryCog = inventoryCog;
            InstanceId = System.Guid.NewGuid().ToString();
            append = new List<string>();
            prepend = new List<string>();
            AssignedSkillSlot = string.Empty;
            attachSlots = new List<AttachmentSlot>();

            AddSubscriptions();
            UpdateSlots();
        }

        /// <summary>
        /// Remove an attachment from item
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool RemoveAttachment(AttachmentSlot slot)
        {
            foreach (AttachmentSlot aSlot in Slots)
            {
                if (aSlot == slot)
                {
                    InventoryItem oldItem = aSlot.AttachedItem;
                    InventoryCog.AddToInventory(oldItem, 1);
                    RemoveAttachmentName(aSlot.AttachedItem);
                    aSlot.AttachedItem.IsAttached = false;
                    aSlot.AttachedItem = null;
                    onAttachmentRemoved.Invoke(oldItem);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Remove an attachment from item
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public bool RemoveAttachment(int slotId)
        {
            if (Slots.Count >= slotId && Slots[slotId].AttachedItem == null)
            {
                InventoryItem oldItem = Slots[slotId].AttachedItem;
                InventoryCog.AddToInventory(oldItem, 1);
                RemoveAttachmentName(Slots[slotId].AttachedItem);
                Slots[slotId].AttachedItem.IsAttached = false;
                Slots[slotId].AttachedItem = null;
                onAttachmentRemoved.Invoke(oldItem);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove an attachment from item
        /// </summary>
        /// <param name="attachPoint"></param>
        /// <returns></returns>
        public bool RemoveAttachment(string attachPoint)
        {
            foreach (AttachmentSlot slot in Slots)
            {
                if (slot.AttachPoint.pointId == attachPoint)
                {
                    if (slot.AttachedItem != null)
                    {
                        InventoryItem oldItem = slot.AttachedItem;
                        InventoryCog.AddToInventory(oldItem, 1);
                        RemoveAttachmentName(slot.AttachedItem);
                        slot.AttachedItem.IsAttached = false;
                        slot.AttachedItem = null;
                        onAttachmentRemoved.Invoke(oldItem);

                        return true;
                    }

                    return false;
                }
            }

            return false;
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
            return true;
        }

        /// <summary>
        /// Set a custom tag value, add if not already present
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="value"></param>
        public void SetCustomTag(string tagName, string value)
        {
            StringValue newTag = new StringValue();
            newTag.Name = tagName;
            newTag.Value = value;

            for (int i = 0; i < customTags.Count; i++)
            {
                if (customTags[i].Name == tagName)
                {
                    customTags[i] = newTag;
                    return;
                }
            }

            customTags.Add(newTag);
        }

        /// <summary>
        /// Load item state
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="inventory"></param>
        public void StateLoad(Stream stream, InventoryCog inventory)
        {
            float version = stream.ReadFloat();
            if (version < 1.1f || version > 1.6f)
            {
                Debug.LogError("Invalid save version");
                return;
            }

            int count;

            // Remove any current subscriptions
            RemoveSubscriptions();

            // base stats
            condition = stream.ReadFloat();
            rarity = stream.ReadInt();
            weight = stream.ReadFloat();
            value = stream.ReadFloat();
            StoredWeight = stream.ReadFloat();
            if(version >= 1.6f)
            {
                canSell = stream.ReadBool();
            }

            // properties
            InstanceId = stream.ReadStringPacket();
            CurrentCount = stream.ReadInt();

            if (inventory != null)
            {
                CurrentEquipPoint = inventory.GetPointById(stream.ReadStringPacket());
            }
            else
            {
                stream.ReadStringPacket();
            }
            EquipState = (EquipState)stream.ReadInt();

            if (version >= 1.2f)
            {
                // Tags
                count = stream.ReadInt();
                customTags = new List<StringValue>();
                for (int i = 0; i < count; i++)
                {
                    StringValue tag = new StringValue();
                    tag.Name = stream.ReadStringPacket();
                    tag.Value = stream.ReadStringPacket();
                    customTags.Add(tag);
                }
            }

            // Stored items
            storedItems = new List<InventoryItem>();
            count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                InventoryItem item = Instantiate(InventoryDB.GetItemByName(stream.ReadStringPacket()));
                item.StateLoad(stream, inventory);
                storedItems.Add(item);
            }

            // Attachments
            if (version >= 1.5f)
            {
                UpdateSlots();
                count = stream.ReadInt();
                if(count != Slots.Count)
                {
                    Debug.LogWarning(count + " != " + Slots.Count);
                }
                for (int i = 0; i < Slots.Count; i++)
                {
                    if (stream.ReadBool())
                    {
                        string itemName = stream.ReadStringPacket();
                        InventoryItem slotItem = Instantiate(inventory.GetItemByName(itemName));
                        slotItem.name = itemName;
                        slotItem.StateLoad(stream, inventory);
                        Slots[i].AttachedItem = slotItem;
                    }
                }
            }

            // Custom name
            if (version >= 1.4f)
            {
                allowCustomName = stream.ReadBool();
                CustomName = stream.ReadBool() ? stream.ReadStringPacket() : null;
            }

            // Attachment names
            if (version >= 1.5f)
            {
                prepend = new List<string>();
                count = stream.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    prepend.Add(stream.ReadStringPacket());
                }
                append = new List<string>();
                count = stream.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    append.Add(stream.ReadStringPacket());
                }

                AssignedSkillSlot = stream.ReadStringPacket();
            }

            // Add any required subscriptions
            AddSubscriptions();
        }

        /// <summary>
        /// Save item state
        /// </summary>
        /// <param name="stream"></param>
        public void StateSave(Stream stream)
        {
            stream.WriteFloat(1.6f);

            // base stats
            stream.WriteFloat(condition);
            stream.WriteInt(rarity);
            stream.WriteFloat(weight);
            stream.WriteFloat(value);
            stream.WriteFloat(StoredWeight);
            stream.WriteBool(canSell);

            // properties
            stream.WriteStringPacket(InstanceId);
            stream.WriteInt(CurrentCount);
            if (CurrentEquipPoint == null)
            {
                stream.WriteStringPacket(string.Empty);
            }
            else
            {
                stream.WriteStringPacket(CurrentEquipPoint.pointId);
            }
            stream.WriteInt((int)EquipState);

            // Tags
            stream.WriteInt(customTags.Count);
            foreach (StringValue tag in customTags)
            {
                stream.WriteStringPacket(tag.Name);
                stream.WriteStringPacket(tag.Value);
            }

            // Stored items
            stream.WriteInt(StoredItems.Count);
            foreach (InventoryItem item in StoredItems)
            {
                stream.WriteStringPacket(item.name);
                item.StateSave(stream);
            }

            // Attached items
            stream.WriteInt(Slots.Count);
            foreach (AttachmentSlot slot in Slots)
            {
                stream.WriteBool(slot.AttachedItem != null);
                if (slot.AttachedItem != null)
                {
                    stream.WriteStringPacket(slot.AttachedItem.name);
                    slot.AttachedItem.StateSave(stream);
                }
            }

            // Custom name
            stream.WriteBool(allowCustomName);
            stream.WriteBool(CustomName != null);
            if (CustomName != null)
            {
                stream.WriteStringPacket(CustomName);
            }

            // Attachment names
            stream.WriteInt(prepend.Count);
            foreach (string text in prepend) stream.WriteStringPacket(text);
            stream.WriteInt(append.Count);
            foreach (string text in append) stream.WriteStringPacket(text);

            // Skill slot
            stream.WriteStringPacket(AssignedSkillSlot);
        }

        #endregion

        #region Private Methods

        private void AddAttachmentName(InventoryItem attachment, AttachmentSlot slot)
        {
            // Update naming
            if (attachment.modifyName)
            {
                if (attachment.modifierOrder == NameModifierOrder.BeforeName)
                {
                    prepend.Add(attachment.nameModifier);
                }
                else
                {
                    append.Add(attachment.nameModifier);
                }
                UpdateNames();
            }

            // Update Inventory
            if (attachment.CurrentCount == 1)
            {
                slot.AttachedItem = attachment;
                attachment.IsAttached = true;
                InventoryCog.RemoveItem(attachment, 1);
            }
            else
            {
                attachment.CurrentCount -= 1;

                // Add new item
                InventoryItem item = Instantiate(attachment);
                item.Initialize(InventoryCog);
                item.name = attachment.name;
                item.CurrentCount = 1;
                item.IsAttached = true;
                slot.AttachedItem = item;
            }

            onAttachmentAdded?.Invoke(attachment);
        }

        private void AddSubscriptions()
        {
            if (expressionSubscriptions == null) expressionSubscriptions = new List<ExpressionSubscription>();

            if (breakdownSource == BooleanSource.StatExpression)
            {
                System.Action<bool> breakdownSub = (bool result) => { curCanBreakdown = result; };
                ExpressionSubscription sub = new ExpressionSubscription(breakdownExpression, breakdownSub);
                sub.Subscribe(InventoryCog.StatsCog);
                expressionSubscriptions.Add(sub);
            }

            if (equipSource == BooleanSource.StatExpression)
            {
                System.Action<bool> equipSub = (bool result) => { curCanEquip = result; };
                ExpressionSubscription sub = new ExpressionSubscription(equipExpression, equipSub);
                sub.Subscribe(InventoryCog.StatsCog);
                expressionSubscriptions.Add(sub);
            }

            if (repairSource == BooleanSource.StatExpression)
            {
                System.Action<bool> repairSub = (bool result) => { curCanRepair = result; };
                ExpressionSubscription sub = new ExpressionSubscription(repairExpression, repairSub);
                sub.Subscribe(InventoryCog.StatsCog);
                expressionSubscriptions.Add(sub);
            }
        }

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

        private void RemoveSubscriptions()
        {
            if (expressionSubscriptions == null) return;
            foreach (ExpressionSubscription sub in expressionSubscriptions)
            {
                sub.Unsubscribe();
            }

            expressionSubscriptions.Clear();
        }

        private void RemoveAttachmentName(InventoryItem attachment)
        {
            if (attachment.modifyName)
            {
                if (attachment.modifierOrder == NameModifierOrder.BeforeName)
                {
                    prepend.Remove(attachment.nameModifier);
                }
                else
                {
                    append.Remove(attachment.nameModifier);
                }
                UpdateNames();
            }
        }

        private void UpdateNames()
        {
            StringBuilder sb = new StringBuilder();

            preName = string.Empty;
            foreach (string text in prepend)
            {
                sb.Append(text + " ");
            }
            preName = sb.ToString();

            sb.Clear();
            postName = string.Empty;
            foreach (string text in append)
            {
                sb.Append(" " + text);
            }
            postName = sb.ToString();
        }

        private void UpdateSlots()
        {
            attachSlots = new List<AttachmentSlot>();
            if (itemType != ItemType.Attachment && equipObject != null)
            {
                foreach (AttachPoint point in equipObject.GetComponentsInChildren<AttachPoint>())
                {
                    AttachmentSlot slot = new AttachmentSlot();
                    slot.AttachPoint = point;
                    slot.ParentItem = this;
                    attachSlots.Add(slot);
                }
            }
        }

        #endregion

    }
}