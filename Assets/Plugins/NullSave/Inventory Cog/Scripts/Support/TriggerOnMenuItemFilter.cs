using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    public class TriggerOnMenuItemFilter : MonoBehaviour
    {

        #region Enumerations

        public enum ItemFilterType
        {
            ItemType,
            CanEquip,
            IsEquipped,
            CanDrop,
            SupportsAttachments
        }

        #endregion

        #region Variables

        public ItemFilterType filterType;
        public ItemType itemType;

        public UnityEvent onMatch, onDoesNotMatch;

        #endregion

        #region Public Methods

        public void PerformMatch(InventoryItem item)
        {
            switch (filterType)
            {
                case ItemFilterType.CanDrop:
                    MatchInvokes(item.canDrop);
                    break;
                case ItemFilterType.CanEquip:
                    MatchInvokes(item.CanEquip);
                    break;
                case ItemFilterType.IsEquipped:
                    MatchInvokes(item.EquipState != EquipState.NotEquipped);
                    break;
                case ItemFilterType.ItemType:
                    MatchInvokes(itemType == item.itemType);
                    break;
                case ItemFilterType.SupportsAttachments:
                    MatchInvokes(item.attachRequirement != AttachRequirement.NoneAllowed);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void MatchInvokes(bool match)
        {
            if (match)
            {
                onMatch?.Invoke();
            }
            else
            {
                onDoesNotMatch?.Invoke();
            }
        }

        #endregion

    }
}