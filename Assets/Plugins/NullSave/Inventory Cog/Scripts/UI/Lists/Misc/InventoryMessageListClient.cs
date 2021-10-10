using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class InventoryMessageListClient : MonoBehaviour
    {

        #region Variables

        public InventoryCog inventoryCog;
        public MessageList messageList;

        public bool showItemAdd = true;
        public string itemAddFormatSingle = "Added {item}";
        public string itemAddFormatMulti = "Added {item} ({count})";

        #endregion

        #region Unity Methods

        private void OnDisable()
        {
            if (showItemAdd) inventoryCog.onItemAdded.RemoveListener(ItemAdded);
        }

        private void OnEnable()
        {
            if (showItemAdd) inventoryCog.onItemAdded.AddListener(ItemAdded);
        }

        #endregion

        #region Private Methods

        private void ItemAdded(InventoryItem item, int count)
        {
            if (showItemAdd)
            {
                if (count == 1)
                {
                    messageList.AddMessage(item.icon, itemAddFormatSingle.Replace("{item}", item.DisplayName).Replace("{count}", count.ToString()));
                }
                else
                {
                    messageList.AddMessage(item.icon, itemAddFormatMulti.Replace("{item}", item.DisplayName).Replace("{count}", count.ToString()));
                }
            }
        }

        #endregion

    }
}