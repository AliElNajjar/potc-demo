using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("loot_item", false)]
    public class LootItemWithUI : LootItem
    {

        #region Variables

        public NavigationType pickupMode = NavigationType.Manual;
        public string pickupButton = "Submit";
        public KeyCode pickupKey = KeyCode.F;
        public bool pickupOnRequest = true;

        public PickupMenuUI pickupUI;
        public MenuOpenType uiOpenType = MenuOpenType.ActiveGameObject;
        public Transform uiParent;
        public string uiTag;

        public UnityEvent onPickupRequested;

        private bool hadInventory;
        private PickupMenuUI spawnedUI;

        #endregion

        #region Unity Methods

        private void LateUpdate()
        {
            if (PlayerInventory != null)
            {
                if (!hadInventory)
                {
                    // Entered this frame
                    ShowUI();
                }
                hadInventory = true;

                // Check for input
                switch (pickupMode)
                {
                    case NavigationType.ByButton:
#if GAME_COG
                        if (GameCog.Input.GetButtonDown(pickupButton))
#else
                        if (Input.GetButtonDown(pickupButton))
#endif
                        {
                            PickupRequest();
                        }
                        break;
                    case NavigationType.ByKey:
#if GAME_COG
                        if (GameCog.Input.GetKeyDown(pickupKey))
#else
                        if (Input.GetKeyDown(pickupKey))
#endif
                        {
                            PickupRequest();
                        }
                        break;
                }
            }
            else
            {
                if (hadInventory)
                {
                    // Exited this frame
                    HideUI();
                }
                hadInventory = false;
            }
        }

        #endregion

        #region Public Methods

        public override void AddToInventory()
        {
            if (PlayerInventory == null)
            {
                Debug.LogError(name + ".LootItem.AddToInventory no Player present in trigger");
            }
            else
            {
                count = PlayerInventory.AddToInventory(this, autoEquip);
                if (count == 0)
                {
                    HideUI();
                    Destroy(gameObject);
                }
            }
        }

        public void HideUI()
        {
            if (spawnedUI == null) return;

            switch (uiOpenType)
            {
                case MenuOpenType.ActiveGameObject:
                    spawnedUI.gameObject.SetActive(false);
                    break;
                default:
                    Destroy(spawnedUI.gameObject);
                    break;
            }

            spawnedUI = null;
        }

        public void PickupRequest()
        {
            onPickupRequested?.Invoke();
            if (pickupOnRequest) AddToInventory();
        }

        #endregion

        #region Private Methods

        private void ShowUI()
        {
            switch (uiOpenType)
            {
                case MenuOpenType.ActiveGameObject:
                    spawnedUI = pickupUI;
                    break;
                case MenuOpenType.SpawnInTransform:
                    spawnedUI = Instantiate(pickupUI, uiParent);
                    break;
                case MenuOpenType.SpawnOnFirstCanvas:
                    Canvas canvas = FindObjectOfType<Canvas>();
                    spawnedUI = Instantiate(pickupUI, canvas.transform);
                    break;
                case MenuOpenType.SpawnInTag:
                    spawnedUI = Instantiate(pickupUI, GameObject.FindGameObjectWithTag(uiTag).transform);
                    break;
            }

            spawnedUI.LootUIOwner = this;
            spawnedUI.InventoryCog = PlayerInventory;
        }

        #endregion

    }
}