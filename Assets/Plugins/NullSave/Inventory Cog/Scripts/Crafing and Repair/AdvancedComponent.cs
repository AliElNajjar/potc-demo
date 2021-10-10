using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [System.Serializable]
    public class AdvancedComponent
    {

        #region Variables

        public InventoryItem item;
        public int count;
        [Range(0, 1)] public float minCondition;
        [Range(0, 10)] public int minRarity;

        #endregion

    }
}