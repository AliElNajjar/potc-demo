using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [System.Serializable]
    public class CraftingUpgradeReference
    {

        #region Variables

        public InventoryItem upgradeItem;
        [Range(0, 10)] public int minRarity;
        [Range(0, 10)] public int maxRarity = 10;
        [Range(0, 1)] public float minCondition;
        [Range(0, 1)] public float maxCondition = 1;

        #endregion

    }
}