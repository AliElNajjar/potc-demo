using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [System.Serializable]
    public class CraftingUpgradeResult
    {

        #region Variables

        public InventoryItem upgradeResult;
        public UpgradeResult condition = UpgradeResult.NoChange;
        [Range(0, 1)] public float conditionChange;
        public UpgradeResult rarity = UpgradeResult.NoChange;
        [Range(0, 10)] public int rarityChange;

        #endregion

    }
}