using UnityEngine.EventSystems;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("item_icon", "#ffffff", false)]
    public class SlotItemUI : ItemUI
    {

        #region Variables

        public int slotId;

        #endregion

        #region Public Methods

        public void LoadSlot()
        {
            if (Inventory != null)
            {
                foreach (InventoryItem item in Inventory.Items)
                {
                    if (item.useSlotId && item.slotId == slotId)
                    {
                        LoadItem(Inventory, Container, item);
                        return;
                    }
                }
            }

            LoadItem(Inventory, Container, null);
        }

        #endregion

        #region Private Methods

        internal override void CompleteDrop(PointerEventData eventData)
        {
            SlotItemUI draggableItem = eventData.pointerDrag.gameObject.GetComponentInChildren<SlotItemUI>();
            if (draggableItem != null)
            {
                if (draggableItem.Item == null) return;

                if (Item != null)
                {
                    Item.slotId = draggableItem.Item.slotId;
                }

                // Swap items
                draggableItem.Item.slotId = slotId;

                InventoryItem item = draggableItem.Item;
                draggableItem.LoadItem(Inventory, Container, Item);
                LoadItem(Inventory, Container, item);
            }
        }

        #endregion

    }
}