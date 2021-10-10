using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class SkillSlotUI : MonoBehaviour, IDropHandler
    {

        #region Variables

        public InventoryCog inventorySource;
        public string slotName;
        public Image skillIcon;
        public Image equippedIcon;

        private bool hasListener;

        #endregion

        #region Propeties

        public InventoryItem AttachedSkillItem { get; private set; }

        #endregion

        #region Unity Methods

        public void OnDrop(PointerEventData eventData)
        {
            ItemUI draggableItem = eventData.pointerDrag.gameObject.GetComponentInChildren<ItemUI>();
            if (draggableItem != null)
            {
                if (draggableItem != null && draggableItem.Item.itemType == ItemType.Skill)
                {
                    inventorySource.SkillAssign(draggableItem.Item, slotName);
                }
            }
        }

        private void Start()
        {
            Rebind();
        }

        #endregion

        #region Public Methods

        public void Rebind()
        {
            if (inventorySource != null && !hasListener)
            {
                inventorySource.onSkillSlotChanged.AddListener(UpdateUI);
                UpdateUI(slotName, inventorySource.GetAssignedSkill(slotName));
                hasListener = true;
            }
        }

        #endregion

        #region Private Methods

        private void UpdateUI(string skillSlot, InventoryItem skill)
        {
            if (slotName == skillSlot)
            {
                AttachedSkillItem = skill;

                if (skill == null)
                {
                    skillIcon.gameObject.SetActive(false);
                    if (equippedIcon != null) equippedIcon.gameObject.SetActive(false);
                }
                else
                {
                    skillIcon.sprite = skill.icon;
                    skillIcon.gameObject.SetActive(true);
                    if (equippedIcon != null) equippedIcon.gameObject.SetActive(true);
                }


            }
        }

        #endregion

    }
}