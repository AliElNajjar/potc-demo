using System.Collections.Generic;
using System.IO;

namespace NullSave.TOCK.Inventory
{
    public class InventoryLoadout
    {

        #region Variables

        public List<InventoryLoadoutItem> loadoutItems;

        #endregion

        #region Public Methods

        public void Load(Stream stream)
        {
            loadoutItems = new List<InventoryLoadoutItem>();
            int count = stream.ReadInt();
            for (int i = 0; i < count; i++)
            {
                InventoryLoadoutItem loadoutItem = new InventoryLoadoutItem();
                loadoutItem.Load(stream);
                loadoutItems.Add(loadoutItem);
            }
        }

        public void PopulateFromInventory(InventoryCog inventory, bool equippedOnly)
        {
            foreach (InventoryItem item in inventory.Items)
            {
                if (!equippedOnly || item.EquipState != EquipState.NotEquipped)
                {
                    InventoryLoadoutItem loadoutItem = new InventoryLoadoutItem(item);
                    loadoutItems.Add(loadoutItem);
                }
            }
        }

        public void Save(Stream stream)
        {
            if (loadoutItems == null)
            {
                stream.WriteInt(0);
                return;
            }

            stream.WriteInt(loadoutItems.Count);
            foreach (InventoryLoadoutItem item in loadoutItems)
            {
                item.Save(stream);
            }
        }

        #endregion

    }
}