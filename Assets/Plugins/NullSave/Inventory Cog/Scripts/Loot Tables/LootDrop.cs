using System.Collections.Generic;

namespace NullSave.TOCK.Inventory
{
    [System.Serializable]
    public class LootDrop
    {

        #region Variables

        public float weight;
        public List<LootItem> items;

        #endregion

    }
}