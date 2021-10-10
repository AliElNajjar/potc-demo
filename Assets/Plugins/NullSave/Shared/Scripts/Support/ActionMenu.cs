using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK
{
    public class ActionMenu : MonoBehaviour
    {

        #region Variables

        public UnityEvent onOpen, onClose;

        #endregion

        #region Properties

#if INVENTORY_COG
        public Inventory.InventoryCog InventoryCog { get; set; }
#endif

        public ActionTrigger Owner { get; set; }

#if STATS_COG
        public Stats.StatsCog StatsCog { get; set; }
#endif

        #endregion

        #region Public Methods

        public void CloseMenu()
        {
            onClose?.Invoke();
            Owner.CloseMenu();
        }

        public void Initialize()
        {

#if INVENTORY_COG
            foreach (Inventory.RecipeList recipeList in GetComponentsInChildren<Inventory.RecipeList>())
            {
                if (recipeList.inventoryCog == null)
                {
                    recipeList.Inventory = InventoryCog;
                    if (recipeList.loadMode == Inventory.ListLoadMode.Manual)
                    {
                        recipeList.ReloadLast();
                    }
                }
            }

            foreach (Inventory.InventoryItemList itemList in GetComponentsInChildren<Inventory.InventoryItemList>())
            {
                if (itemList.Inventory == null)
                {
                    itemList.Inventory = InventoryCog;
                    if (itemList.loadMode == Inventory.ListLoadMode.Manual)
                    {
                        itemList.ReloadLast();
                    }
                }
            }
#endif

            onOpen?.Invoke();
        }

        #endregion

    }
}