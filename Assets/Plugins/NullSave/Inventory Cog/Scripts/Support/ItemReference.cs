using System;


namespace NullSave.TOCK.Inventory
{
    [Serializable]
    public class ItemReference
    {

        #region Variables

        public InventoryItem item;
        public int count;

        #endregion

        #region Constructors

        public ItemReference()
        {
            count = 1;
        }

        public ItemReference(InventoryItem item, int count)
        {
            this.item = item;
            this.count = count;
        }

        #endregion

    }
}
