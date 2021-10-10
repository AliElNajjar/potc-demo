using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("item_icon", "#ffffff", false)]
    public class ItemUI : MonoBehaviour, IDragHandler, IEndDragHandler, IDropHandler
    {

        #region Variables

        public Image itemImage;
        public TextMeshProUGUI displayName;
        public TextMeshProUGUI description;
        public TextMeshProUGUI subtext;
        public GameObject hideIfNoSubtext;
        public GameObject equippedIndicator;
        public GameObject equipableIndicator;
        public string countPrefix = "x";
        public TextMeshProUGUI count;
        public string countSuffix;
        public GameObject hideIfCountSub2;
        public GameObject selectedIndicator;
        public RarityColorIndicator rarityColorIndicator;
        public Slider conditionSlider;
        public Image lockedIndicator;

        public TextMeshProUGUI healthMod;
        public GameObject hideIfHealthModZero;

        public TextMeshProUGUI damageMod;
        public GameObject hideIfDamageModZero;

        public Slider raritySlider;
        public bool hideIfConditionZero;
        public bool hideIfRarityZero;

        public ItemTagUI tagPrefab;
        public Transform tagContainer;

        public RecipeUI recipeUI;

        public ItemChanged onLoadedItem;
        public ItemUIClick onClick, onZeroCount;

        // Drag and Drop
        public bool enableDragDrop;
        private bool dragStarted;
        private GameObject moveGO;

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public InventoryContainer Container { get; set; }

        public InventoryItem Item { get; set; }

        public InventoryItemList ItemListParent { get; set; }

        #endregion

        #region Unity Methods

        public void OnDrag(PointerEventData eventData)
        {
            if (Item == null || !enableDragDrop || (ItemListParent != null && !ItemListParent.enableDragDrop)) return;

            if (!dragStarted)
            {
                // Find canvas
                Canvas c = GetComponentInParent<Canvas>();

                moveGO = new GameObject("InventoryCog_DragItem");
                Image img = moveGO.AddComponent<Image>();
                img.sprite = Item.icon;
                img.raycastTarget = false;
                SlotItemUI slotItemUI = moveGO.AddComponent<SlotItemUI>();
                slotItemUI.Item = Item;
                slotItemUI.Inventory = Inventory;
                slotItemUI.Container = Container;
                moveGO.transform.SetParent(transform.parent);
                moveGO.transform.position = transform.localPosition;
                moveGO.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
                moveGO.transform.SetParent(c.transform);

                dragStarted = true;
            }

            moveGO.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragStarted = false;
            Destroy(moveGO);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (enableDragDrop)
            {
                CompleteDrop(eventData);
            }
        }

        private void Update()
        {
            if (Item == null) return;
            UpdateUI();
        }

        #endregion

        #region Public Methods

        public void Click()
        {
            onClick?.Invoke(this);
        }

        public void Equip()
        {
            if (Inventory != null)
            {
                Inventory.EquipItem(Item);
            }
        }

        public void Unequip()
        {
            if (Inventory != null)
            {
                Inventory.UnequipItem(Item);
            }
        }

        public virtual void LoadItem(InventoryCog inventory, InventoryItem inventoryItem)
        {
            LoadItem(inventory, null, inventoryItem);
        }

        public virtual void LoadItem(InventoryCog inventory, InventoryContainer container, InventoryItem inventoryItem)
        {
            Inventory = inventory;
            Container = container;
            Item = inventoryItem;

            if (lockedIndicator)
            {
                lockedIndicator.gameObject.SetActive(false);
            }

            if (Item == null)
            {
                if (itemImage != null) itemImage.enabled = false;
                if (count != null) count.text = string.Empty;
                if (equippedIndicator != null) equippedIndicator.SetActive(false);
                if (displayName != null) displayName.text = string.Empty;
                if (subtext != null) subtext.text = string.Empty;
                if (hideIfNoSubtext != null) hideIfNoSubtext.SetActive(false);
                if (hideIfDamageModZero != null) hideIfDamageModZero.SetActive(false);
                if (hideIfHealthModZero != null) hideIfHealthModZero.SetActive(false);
                if (rarityColorIndicator != null) rarityColorIndicator.LoadItem(null);
                if (conditionSlider != null) conditionSlider.value = conditionSlider.minValue;
                if (raritySlider != null) raritySlider.value = raritySlider.minValue;
                if (hideIfCountSub2 != null) hideIfCountSub2.SetActive(false);
                if (hideIfConditionZero && conditionSlider != null) conditionSlider.gameObject.SetActive(false);
                if (hideIfRarityZero && raritySlider != null) raritySlider.gameObject.SetActive(false);
                if (recipeUI != null) recipeUI.gameObject.SetActive(false);
                return;
            }

            UpdateUI();

            onLoadedItem?.Invoke(Item);
        }

        public virtual void LoadItemByReference(InventoryCog inventory, ItemReference reference)
        {
            if (reference == null || reference.item == null)
            {
                LoadItem(inventory, Container, null);
                return;
            }

            InventoryItem item = InventoryDB.GetItemByName(reference.item.name);
            item.CurrentCount = reference.count;
            LoadItem(inventory, Container, item);
        }

        public void LoadLockedSlot(InventoryCog inventory)
        {
            Inventory = inventory;
            Item = null;

            if (lockedIndicator)
            {
                lockedIndicator.gameObject.SetActive(true);
            }

            if (itemImage != null) itemImage.enabled = false;
            if (count != null) count.text = string.Empty;
            if (equippedIndicator != null) equippedIndicator.SetActive(false);
            if (displayName != null) displayName.text = string.Empty;
            if (subtext != null) subtext.text = string.Empty;
            if (hideIfNoSubtext != null) hideIfNoSubtext.SetActive(false);
            if (hideIfDamageModZero != null) hideIfDamageModZero.SetActive(false);
            if (hideIfHealthModZero != null) hideIfHealthModZero.SetActive(false);
            if (rarityColorIndicator != null) rarityColorIndicator.LoadItem(null);
            if (conditionSlider != null) conditionSlider.value = conditionSlider.minValue;
            if (raritySlider != null) raritySlider.value = raritySlider.minValue;
            if (hideIfCountSub2 != null) hideIfCountSub2.SetActive(false);
            if (hideIfConditionZero && conditionSlider != null) conditionSlider.gameObject.SetActive(false);
            if (hideIfRarityZero && raritySlider != null) raritySlider.gameObject.SetActive(false);
            if (recipeUI != null) recipeUI.gameObject.SetActive(false);

        }

        public void SetSelected(bool selected)
        {
            if (selectedIndicator != null) selectedIndicator.SetActive(selected);
        }

        #endregion

        #region Private Methods

        internal virtual void CompleteDrop(PointerEventData eventData)
        {
            SlotItemUI draggableItem = eventData.pointerDrag.gameObject.GetComponentInChildren<SlotItemUI>();
            if (draggableItem != null)
            {
                if (draggableItem.Item == null) return;

                InventoryItem item = draggableItem.Item;
                draggableItem.LoadItem(Inventory, Container, Item);
                LoadItem(Inventory, Container, item);
            }
            else if (ItemListParent != null)
            {
                ItemListParent.OnDrop(eventData);
            }
        }

        private void UpdateUI()
        {
            if (itemImage != null)
            {
                itemImage.sprite = Item.icon;
                itemImage.enabled = itemImage.sprite != null;
            }

            if (count != null)
            {
                if (Item.canStack)
                {
                    count.text = countPrefix + Item.CurrentCount + countSuffix;
                }
                else
                {
                    count.text = string.Empty;
                }
            }
            if (hideIfCountSub2 != null) hideIfCountSub2.SetActive(Item.CurrentCount > 1);

            if (conditionSlider != null)
            {
                conditionSlider.minValue = 0;
                conditionSlider.maxValue = 1;
                conditionSlider.value = Item.condition;
                if (hideIfConditionZero)
                {
                    conditionSlider.gameObject.SetActive(Item.condition > 0);
                }
            }

            if (raritySlider != null)
            {
                raritySlider.minValue = 0;
                raritySlider.maxValue = 10;
                raritySlider.value = Item.rarity;
                if (hideIfRarityZero)
                {
                    raritySlider.gameObject.SetActive(Item.rarity > 0);
                }
            }

            if (equippedIndicator != null)
            {
                if (Inventory != null)
                {
                    if (Item.itemType == ItemType.Ammo)
                    {
                        equippedIndicator.SetActive(Inventory.GetSelectedAmmo(Item.ammoType) == Item);
                    }
                    else if (Item.CanEquip)
                    {
                        equippedIndicator.SetActive(Item.EquipState != EquipState.NotEquipped);
                    }
                    else if (Item.itemType == ItemType.Skill)
                    {
                        equippedIndicator.SetActive(!string.IsNullOrEmpty(Item.AssignedSkillSlot));
                    }
                    else
                    {
                        equippedIndicator.SetActive(false);
                    }
                }
                else
                {
                    equippedIndicator.SetActive(false);
                }
            }

            if (displayName != null)
            {
                displayName.text = Item.DisplayName;
            }

            if (description != null)
            {
                description.text = Item.DisplayName;
            }

            if (subtext != null)
            {
                subtext.text = Item.subtext;
            }
            if (hideIfNoSubtext != null)
            {
                hideIfNoSubtext.SetActive(Item.subtext != null && Item.subtext != string.Empty);
            }

            if (tagPrefab != null)
            {
                foreach (InventoryItemUITag tag in Item.uiTags)
                {
                    Instantiate(tagPrefab, tagContainer).LoadTag(Inventory, tag);
                }
            }

            if (rarityColorIndicator != null) rarityColorIndicator.LoadItem(Item);
        }

        #endregion

    }
}