namespace NullSave.TOCK.Inventory
{
    [System.Serializable]
    public class StockItemReference
    {

        #region Variables

        public InventoryItem item;
        public int count;
        public int bought;

        #endregion

        #region Constructors

        public StockItemReference() { }

        public StockItemReference(ItemReference itemReference)
        {
            item = itemReference.item;
            count = itemReference.count;
            bought = 0;
        }

        public StockItemReference(InventoryItem item, int count, int bought)
        {
            this.item = item;
            this.count = count;
            this.bought = bought;
        }

        #endregion

    }
}