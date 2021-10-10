using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("tock-menu","#ffffff", false)]
    public class InventoryMenuUI : MonoBehaviour
    {

        #region Variables

        public NavigationType closeMode;
        public string closeButton = "Cancel";
        public KeyCode closeKey = KeyCode.Escape;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; private set; }

        #endregion

        #region Unity Methods

        public void Update()
        {
            if (Inventory.IsPromptOpen) return;

            switch (closeMode)
            {
                case NavigationType.ByButton:
#if GAME_COG
                    if (GameCog.Input.GetButtonDown(closeButton))
#else
                    if (Input.GetButtonDown(closeButton))
#endif
                    {
                        Inventory.MenuClose();
                    }
                    break;
                case NavigationType.ByKey:
#if GAME_COG
                    if (GameCog.Input.GetKeyDown(closeKey))
#else
                    if (Input.GetKeyDown(closeKey))
#endif
                    {
                        Inventory.MenuClose();
                    }
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void LoadInventory(InventoryCog inventory)
        {
            Inventory = inventory;
            if (Inventory == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Inventory = player.GetComponentInChildren<InventoryCog>();
                }
            }

            foreach (InventoryItemList itemList in GetComponentsInChildren<InventoryItemList>())
            {
                if (itemList.Inventory == null)
                {
                    itemList.Inventory = Inventory;
                    if (itemList.loadMode == ListLoadMode.OnEnable)
                    {
                        itemList.LoadItems();
                    }
                }
            }

            foreach (EquipPointUI itemList in GetComponentsInChildren<EquipPointUI>())
            {
                if (itemList.inventoryCog == null)
                {
                    itemList.inventoryCog = Inventory;
                    itemList.Refresh();
                }
            }

            foreach (RecipeList itemList in GetComponentsInChildren<RecipeList>())
            {
                if (itemList.Inventory == null)
                {
                    itemList.Inventory = Inventory;
                }
            }

            foreach (SlotItemUI itemList in GetComponentsInChildren<SlotItemUI>())
            {
                if (itemList.Inventory == null)
                {
                    itemList.Inventory = Inventory;
                }
                itemList.LoadSlot();
            }

            foreach (SkillSlotUI itemList in GetComponentsInChildren<SkillSlotUI>())
            {
                if (itemList.inventorySource == null)
                {
                    itemList.inventorySource = Inventory;
                }
                itemList.Rebind();
            }

            foreach (SortUI ui in GetComponentsInChildren<SortUI>())
            {
                if (ui.Inventory == null)
                {
                    ui.Inventory = Inventory;
                }
            }
        }

        public void RefreshChildren()
        {
            LoadInventory(Inventory);
        }

        #endregion

    }
}