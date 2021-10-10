using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class MerchantMenuUI : MonoBehaviour
    {

        #region Variables

        // Closing
        public NavigationType closeMode = NavigationType.ByButton;
        public string closeButton = "Cancel";
        public KeyCode closeKey = KeyCode.Escape;

        // Load type
        public ListLoadMode loadMode = ListLoadMode.OnEnable;

        public InventoryCog playerInventory;
        public InventoryItemList playerList;

        public InventoryMerchant merchantInventory;
        public InventoryItemList merchantList;

        public TextMeshProUGUI playerCurrency;
        public string playerFormat = "{0}";

        public TextMeshProUGUI merchantCurrency;
        public string merchantFormat = "{0}";

        // Events
        public UnityEvent onOpen, onClose;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (loadMode == ListLoadMode.OnEnable)
            {
                RefreshPlayerInventory();
                RefreshMerchantInventory();
            }

            onOpen?.Invoke();
            if (merchantInventory != null) merchantInventory.InTransaction = true;
        }

        private void Start()
        {
            if (loadMode == ListLoadMode.OnEnable)
            {
                RefreshPlayerInventory();
                RefreshMerchantInventory();
            }

            onOpen?.Invoke();
        }

        private void Update()
        {
            switch (closeMode)
            {
                case NavigationType.ByButton:
#if GAME_COG
                    if (GameCog.Input.GetButtonDown(closeButton))
#else
                    if (Input.GetButtonDown(closeButton))
#endif
                    {
                        CloseMenu();
                    }
                    break;
                case NavigationType.ByKey:
#if GAME_COG
                    if (GameCog.Input.GetKeyDown(closeKey))
#else
                    if (Input.GetKeyDown(closeKey))
#endif
                    {
                        CloseMenu();
                    }
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void CloseMenu()
        {
            onClose?.Invoke();
            if (merchantInventory != null) merchantInventory.InTransaction = false;
            playerInventory.MerchantMenuClose();
        }

        public void BuySelectedItem()
        {
            if (merchantList == null || merchantList.SelectedItem == null) return;

            System.Action<bool> callback = (bool success) =>
            {
                if (success)
                {
                    RefreshMerchantInventory();
                    RefreshPlayerInventory();
                }
            };
            merchantInventory.SellToPlayer(merchantList.SelectedItem.Item, playerInventory, callback);
        }

        public void SellSelectedItem()
        {
            if (playerList == null || playerList.SelectedItem == null || !merchantList.SelectedItem.Item.canSell) return;

            System.Action<bool> callback = (bool success) =>
            {
                if (success)
                {
                    RefreshMerchantInventory();
                    RefreshPlayerInventory();
                }
            };
            merchantInventory.BuyFromPlayer(playerList.SelectedItem.Item, playerInventory, callback);
        }

        public void RefreshAllInventory()
        {
            RefreshMerchantInventory();
            RefreshPlayerInventory();
        }

        public void RefreshMerchantInventory()
        {
            if (merchantList != null)
            {
                if (merchantInventory != null)
                {
                    merchantList.Merchant = merchantInventory;
                    int i = merchantList.SelectedIndex;
                    merchantList.LoadItems();
                    merchantList.SelectedIndex = i;
                }
                else
                {
                    Debug.LogWarning(name + ".MerchantMenuUI no Merchant Inventory supplied");
                }

                if (merchantCurrency != null && merchantInventory != null)
                {
                    merchantCurrency.text = merchantFormat.Replace("{0}", merchantInventory.currency.ToString());
                }
            }
        }

        public void RefreshPlayerInventory()
        {
            if (playerList != null)
            {
                if (playerInventory != null)
                {
                    playerList.Inventory = playerInventory;
                    int i = playerList.SelectedIndex;
                    playerList.LoadItems();
                    playerList.SelectedIndex = i;
                }
                else
                {
                    Debug.LogWarning(name + ".MerchantMenuUI no InventoryCog supplied");
                }
            }

            if (playerCurrency != null && playerInventory != null)
            {
                playerCurrency.text = playerFormat.Replace("{0}", playerInventory.currency.ToString());
            }

        }

        #endregion

    }
}