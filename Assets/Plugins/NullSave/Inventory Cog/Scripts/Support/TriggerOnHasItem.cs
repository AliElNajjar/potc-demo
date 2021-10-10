using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(Collider))]
    public class TriggerOnHasItem : MonoBehaviour
    {

        #region Variables

        public InventoryItem checkForItem;
        public bool removeItem;
        public UnityEvent onHasItem, onDoesNotHaveItem, onExit;

        private InventoryCog foundInventory;

        #endregion

        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            InventoryCog cog = other.gameObject.GetComponentInChildren<InventoryCog>();
            if (cog != null)
            {
                foundInventory = cog;
                if (foundInventory.GetItemFromInventory(checkForItem) == null)
                {
                    onDoesNotHaveItem?.Invoke();
                }
                else
                {
                    onHasItem?.Invoke();
                    if (removeItem)
                    {
                        cog.RemoveItem(checkForItem, 1);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (foundInventory != null)
            {
                onExit?.Invoke();
                foundInventory = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            InventoryCog cog = other.gameObject.GetComponentInChildren<InventoryCog>();
            if (cog != null)
            {
                foundInventory = cog;
                if (foundInventory.GetItemFromInventory(checkForItem) == null)
                {
                    onDoesNotHaveItem?.Invoke();
                }
                else
                {
                    onHasItem?.Invoke();
                    if (removeItem)
                    {
                        cog.RemoveItem(checkForItem, 1);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (foundInventory != null)
            {
                onExit?.Invoke();
                foundInventory = null;
            }
        }

        #endregion

    }
}