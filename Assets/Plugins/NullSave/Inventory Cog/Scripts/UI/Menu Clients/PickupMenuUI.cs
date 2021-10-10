using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class PickupMenuUI : MonoBehaviour
    {

        #region Properties

        public InventoryCog InventoryCog { get; set; }

        public LootItemWithUI LootUIOwner { get; set; }

        #endregion

        #region Public Methods

        public void Pickup()
        {
            LootUIOwner.PickupRequest();
        }

        #endregion

    }
}