using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class ComponentUI : MonoBehaviour
    {

        #region Variables

        public Image itemImage;
        public TextMeshProUGUI displayName;
        public TextMeshProUGUI description;
        public TextMeshProUGUI subtext;
        public GameObject hideIfNoSubtext;
        public Slider conditionSlider, raritySlider;
        public bool hideIfConditionZero;
        public bool hideIfRarityZero;

        public TextMeshProUGUI countNeeded, countAvailable;
        public RarityColorIndicator rarityColorIndicator;

        public Color availableColor = Color.white;
        public Color unavailableColor = new Color(1, 1, 1, 0.4f);

        #endregion

        #region Public Methods

        public void LoadComponent(ItemReference item, InventoryCog inventory, bool checkOnHand, float minConditon, int minRarity)
        {
            if (item == null)
            {
                if (itemImage != null) itemImage.enabled = false;
                if (countNeeded != null) countNeeded.text = string.Empty;
                if (countAvailable != null) countAvailable.text = string.Empty;
                if (displayName != null) displayName.text = string.Empty;
                if (subtext != null) subtext.text = string.Empty;
                if (hideIfNoSubtext != null) hideIfNoSubtext.SetActive(false);
                if (rarityColorIndicator != null) rarityColorIndicator.LoadItem(null);
                return;
            }

            InventoryItem Item = item.item;
            Color useColor;
            if (checkOnHand)
            {
                int onHandCount = inventory.GetItemTotalCount(Item, minConditon, minRarity);
                useColor = onHandCount >= item.count ? availableColor : unavailableColor;

                if (countAvailable != null)
                {
                    countAvailable.text = onHandCount.ToString();
                    countAvailable.color = useColor;
                }

            }
            else
            {
                useColor = availableColor;
            }

            if (conditionSlider != null)
            {
                conditionSlider.minValue = 0;
                conditionSlider.maxValue = 1;
                conditionSlider.value = minConditon;
                if (hideIfConditionZero)
                {
                    conditionSlider.gameObject.SetActive(minConditon > 0);
                }
            }

            if (raritySlider != null)
            {
                raritySlider.minValue = 0;
                raritySlider.maxValue = 10;
                raritySlider.value = minConditon;
                if (hideIfRarityZero)
                {
                    raritySlider.gameObject.SetActive(minRarity > 0);
                }
            }

            if (itemImage != null)
            {
                itemImage.sprite = Item.icon;
                itemImage.color = useColor;
            }

            if (countNeeded != null)
            {
                countNeeded.text = item.count.ToString();
                countNeeded.color = useColor;
            }

            if (displayName != null)
            {
                displayName.text = Item.DisplayName;
                displayName.color = useColor;
            }

            if (description != null)
            {
                description.text = Item.DisplayName;
                description.color = useColor;
            }

            if (subtext != null)
            {
                subtext.text = Item.subtext;
                subtext.color = useColor;
            }
            if (hideIfNoSubtext != null)
            {
                hideIfNoSubtext.SetActive(Item.subtext != null && Item.subtext != string.Empty);
            }

            if (rarityColorIndicator != null) rarityColorIndicator.LoadItem(Item);
        }

        #endregion

    }
}