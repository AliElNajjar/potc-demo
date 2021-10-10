using NullSave.TOCK.Stats;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("equip_point", false)]
    public class EquipPoint : MonoBehaviour
    {

        #region Variables

        public string pointId = "EquipPoint";

        public bool drawGizmo = true;
        [Range(0, 1)] public float gizmoScale = 0.1f;
        public StorePoint storePoint;

        public List<EquipPoint> forceStore;
        public List<EquipPoint> forceUnequip;

        public ItemChanged onItemEquipped, onItemUnequipped, onItemStored;

        #endregion

        #region Properties

        public Animator Animator { get; set; }

        public InventoryItem EquipedOrStoredItem
        {
            get
            {
                if (Item != null) return Item;
                if (storePoint != null)
                {
                    return storePoint.Item;
                }
                return null;
            }
        }

        public bool IsItemEquipped
        {
            get
            {
                return Item != null;
            }
        }

        public bool IsItemEquippedOrStored
        {
            get
            {
                return IsItemEquipped || IsItemStored;
            }
        }

        public bool IsItemStored
        {
            get
            {
                if (storePoint != null)
                {
                    return storePoint.Item != null;
                }

                return false;
            }
        }

        public InventoryCog Inventory { get; set; }

        public InventoryItem Item { get; internal set; }

        public GameObject ObjectReference { get; internal set; }

        public StatsCog StatsCog { get; set; }

        #endregion

        #region Unity Methods

        private void OnDrawGizmosSelected()
        {
            if (drawGizmo)
            {
                Gizmos.color = new Color(1, 0.559f, 0.027f, 0.5f);
                Gizmos.DrawSphere(transform.position, gizmoScale);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Equip new item
        /// </summary>
        /// <param name="item"></param>
        public virtual void EquipItem(InventoryItem item)
        {
            // Check if item is equipable
            if (item != null && !item.CanEquip)
            {
                Debug.Log(name + ".EquipItem requested item .canEquip = false; ignoring request");
                return;
            }

            int insertIndex = -1;

            // Remove item from slot first (if set) to ensure max slot usage
            if (item != null && item.freeSlotWhenEquipped)
            {
                Category category = Inventory.GetCategory(item.category.name);
                insertIndex = category.AssignedItems.IndexOf(item);
                category.RemoveItem(item);
            }

            // Remove current item
            if (EquipedOrStoredItem)
            {
                UnequipItem(insertIndex);
            }

            // Nothing to do if new equip is null
            if (item == null) return;

            // Force unequip (as needed)
            foreach (EquipPoint point in forceUnequip)
            {
                if (point != null && point.IsItemEquippedOrStored)
                {
                    point.UnequipItem();
                }
            }

            // Force store (as needed)
            foreach (EquipPoint point in forceStore)
            {
                if (point != null && point.IsItemEquipped)
                {
                    point.StoreItem();
                }
            }

            // Equip new item
            Item = item;
            item.EquipState = EquipState.Equipped;
            item.CurrentEquipPoint = this;
            onItemEquipped?.Invoke(item);

            // Create object (as needed)
            if (item.equipObject != null)
            {
                ObjectReference = Instantiate(item.equipObject, transform);
                ObjectReference.transform.localPosition = Vector3.zero;
                ObjectReference.transform.localRotation = Quaternion.Euler(0, 0, 0);
                DamageDealer dd = ObjectReference.GetComponentInChildren<DamageDealer>();
                if (dd)
                {
                    dd.StatsSource = StatsCog;
                }
            }

            CreateAttachmentModels();

            // Animator updates
            if (Animator != null)
            {
                foreach (AnimatorMod mod in item.equipAnimatorMods)
                {
                    mod.ApplyMod(Animator);
                }
            }

            // Subscribe
            Item.onAttachmentAdded.AddListener(AttachmentAdded);
            Item.onAttachmentRemoved.AddListener(AttachmentRemoved);

            // Apply stat modifiers
            if (StatsCog != null)
            {
                StatsCog.AddInventoryEffects(item);
                StatsCog.UpdateDamageDealers();
            }
        }

        public virtual void StateLoad(Stream stream, InventoryCog inventory)
        {
            // Reset first
            UnequipItem();

            EquipItem(inventory.GetItemByInstanceId(stream.ReadStringPacket()));
            if (storePoint != null)
            {
                InventoryItem item = inventory.GetItemByInstanceId(stream.ReadStringPacket());
                if (item != null)
                {
                    StoreItem(item);
                }
            }
        }

        public virtual void StateSave(Stream stream)
        {
            stream.WriteStringPacket(Item == null ? string.Empty : Item.InstanceId);
            if (storePoint != null)
            {
                stream.WriteStringPacket(storePoint.Item == null ? string.Empty : storePoint.Item.InstanceId);
            }
        }

        /// <summary>
        /// Store currently equipped weapon
        /// </summary>
        public virtual void StoreItem()
        {
            // Check for item to store
            if (Item == null) return;

            // Unequip if not able to store
            if (!Item.canStore)
            {
                UnequipItem();
                return;
            }

            // Check for inability to store
            if (storePoint == null)
            {
                Debug.LogWarning(name + ".StoreItem() no associated store point; unequipping instead");
                UnequipItem();
                return;
            }

            // Check if already stored
            if (storePoint.Item != null) return;

            // Store item
            storePoint.Item = Item;
            storePoint.Item.EquipState = EquipState.Stored;
            if (ObjectReference != null)
            {
                storePoint.ObjectReference = ObjectReference;
                storePoint.ObjectReference.transform.parent = storePoint.transform;
                storePoint.ObjectReference.transform.localPosition = Vector3.zero;
                storePoint.ObjectReference.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }

            // Remove from equip point
            Item = null;
            ObjectReference = null;

            // Force unstore
            foreach (EquipPoint point in storePoint.forceUnstore)
            {
                if (point.IsItemStored)
                {
                    point.UnstoreItem();
                }
            }
        }

        /// <summary>
        /// Equip item straight to storage point
        /// </summary>
        /// <param name="item"></param>
        public virtual void StoreItem(InventoryItem item)
        {
            // Remove item from slot first (if set) to ensure max slot usage
            if (item.freeSlotWhenEquipped)
            {
                Inventory.GetCategory(item.category.name).RemoveItem(item);
            }

            // Unequip anything existing
            UnequipItem();

            // Store item (direct)
            storePoint.Item = item;
            storePoint.Item.EquipState = EquipState.Stored;
            storePoint.Item.CurrentEquipPoint = this;
            if (item.equipObject != null)
            {
                storePoint.ObjectReference = Instantiate(item.equipObject, storePoint.transform);
                storePoint.ObjectReference.transform.localPosition = Vector3.zero;
                storePoint.ObjectReference.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }

        /// <summary>
        /// Completely remove equipped or stored item
        /// </summary>
        public virtual void UnequipItem(int insertIndex = -1)
        {
            // Remove from equip point
            if (Item != null)
            {
                Item.EquipState = EquipState.NotEquipped;
                Item.CurrentEquipPoint = null;
                if (ObjectReference != null)
                {
                    Destroy(ObjectReference);
                    ObjectReference = null;
                }
                onItemUnequipped?.Invoke(Item);

                // Apply stat modifiers
                if (StatsCog != null)
                {
                    StatsCog.RemoveInventoryEffects(Item);
                    StatsCog.UpdateDamageDealers();
                }

                // Animator updates
                if (Animator != null)
                {
                    foreach (AnimatorMod mod in Item.unequipAnimatorMods)
                    {
                        mod.ApplyMod(Animator);
                    }
                }

                // Restore it inventory slot if needed
                if (Item.freeSlotWhenEquipped)
                {
                    if (insertIndex != -1)
                    {
                        Inventory.GetCategory(Item.category.name).InsertItem(Item, insertIndex);
                    }
                    else
                    {
                        Inventory.GetCategory(Item.category.name).AddItem(Item);
                    }
                }

                // Unsubscribe
                Item.onAttachmentAdded.RemoveListener(AttachmentAdded);
                Item.onAttachmentRemoved.RemoveListener(AttachmentRemoved);

                Item = null;
            }

            // Remove from store point
            if (storePoint != null && storePoint.Item != null)
            {
                storePoint.Item.EquipState = EquipState.NotEquipped;
                storePoint.Item.CurrentEquipPoint = null;
                if (storePoint.ObjectReference != null)
                {
                    Destroy(storePoint.ObjectReference);
                    storePoint.ObjectReference = null;
                }
                onItemUnequipped?.Invoke(storePoint.Item);

                // Apply stat modifiers
                if (StatsCog != null) StatsCog.RemoveInventoryEffects(storePoint.Item);

                // Restore it inventory slot if needed
                if (storePoint.Item.freeSlotWhenEquipped)
                {
                    if (insertIndex != -1)
                    {
                        Inventory.GetCategory(storePoint.Item.category.name).InsertItem(Item, insertIndex);
                    }
                    else
                    {
                        Inventory.GetCategory(storePoint.Item.category.name).AddItem(Item);
                    }
                }

                storePoint.Item = null;
            }


        }

        /// <summary>
        /// Equip currently stored item
        /// </summary>
        public virtual void UnstoreItem()
        {
            // Check for existing equip
            if (Item != null) return;

            // Check for ability to unstore
            if (storePoint == null)
            {
                Debug.LogWarning(name + ".UnstoreItem() no associated store point; exiting");
                return;
            }

            // Check if we have no stored item
            if (storePoint.Item == null) return;

            // Equip Item
            Item = storePoint.Item;
            storePoint.Item.EquipState = EquipState.Stored;
            if (storePoint.ObjectReference != null)
            {
                ObjectReference = storePoint.ObjectReference;
                ObjectReference.transform.parent = transform;
                ObjectReference.transform.localPosition = Vector3.zero;
                ObjectReference.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }

            // Remove from store point
            storePoint.Item = null;
            storePoint.ObjectReference = null;

            // Force unequip (as needed)
            foreach (EquipPoint point in forceUnequip)
            {
                if (point.IsItemEquippedOrStored)
                {
                    point.UnequipItem();
                }
            }

            // Force store (as needed)
            foreach (EquipPoint point in forceStore)
            {
                if (point.IsItemEquipped)
                {
                    point.StoreItem();
                }
            }
        }

        #endregion

        #region Private Methods

        private void AttachmentAdded(InventoryItem attachment)
        {
            DestroyAttachmentModels();
            CreateAttachmentModels();
            StatsCog.AddInventoryEffects(attachment);
        }

        private void AttachmentRemoved(InventoryItem attachment)
        {
            DestroyAttachmentModels();
            CreateAttachmentModels();
            StatsCog.RemoveInventoryEffects(attachment);
        }

        private void CreateAttachmentModels()
        {
            // Create attachments (as needed)
            if (ObjectReference != null)
            {
                AttachPoint[] attachPoints = ObjectReference.GetComponentsInChildren<AttachPoint>();
                foreach (AttachmentSlot slot in Item.Slots)
                {
                    if (slot.AttachedItem != null)
                    {
                        foreach (AttachPoint point in attachPoints)
                        {
                            if (point.pointId == slot.AttachPoint.pointId && slot.AttachedItem.attachObject != null)
                            {
                                slot.ObjectReference = Instantiate(slot.AttachedItem.attachObject, point.transform);
                                slot.ObjectReference.transform.localPosition = Vector3.zero;
                                slot.ObjectReference.transform.localRotation = Quaternion.Euler(0, 0, 0);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void DestroyAttachmentModels()
        {
            // Destroy attachments (as needed)
            if (ObjectReference != null)
            {
                AttachPoint[] attachPoints = ObjectReference.GetComponentsInChildren<AttachPoint>();
                foreach (AttachmentSlot slot in Item.Slots)
                {
                    if (slot.ObjectReference != null)
                    {
                        Destroy(slot.ObjectReference);
                        slot.ObjectReference = null;
                    }
                }
            }
        }

        #endregion

    }
}