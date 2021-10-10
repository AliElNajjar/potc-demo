using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class InventoryMasterList : ScriptableObject
    {

        #region Variables

        public List<Category> categories;
        public List<InventoryItem> availableItems;

        #endregion

    }
}