using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class RecipeComponentUI : MonoBehaviour
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
        public ComponentUIColor colorApplication = (ComponentUIColor)255;

        public Color availableColor2 = Color.white;
        public Color unavailableColor2 = new Color(1, 1, 1, 0.4f);
        public ComponentUIColor colorApplication2;

        #endregion

        #region Public Methods

        public void LoadComponent(ItemReference item, InventoryCog inventory, float minConditon, int minRarity)
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
            int onHandCount = inventory.GetItemTotalCount(Item, minConditon, minRarity);


            if (itemImage != null)
            {
                itemImage.sprite = Item.icon;
            }

            if (countNeeded != null)
            {
                countNeeded.text = item.count.ToString();
            }

            if (countAvailable != null)
            {
                countAvailable.text = onHandCount.ToString();
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

            if (rarityColorIndicator != null) rarityColorIndicator.LoadItem(Item);


            Color useColor = onHandCount >= item.count ? availableColor : unavailableColor;
            ApplyColors(colorApplication, useColor);

            useColor = onHandCount >= item.count ? availableColor2 : unavailableColor2;
            ApplyColors(colorApplication2, useColor);
        }

        #endregion

        #region Private Methods

        private void ApplyColors(ComponentUIColor filter, Color useColor)
        {
            if (itemImage != null && (filter & ComponentUIColor.Icon) == ComponentUIColor.Icon)
            {
                itemImage.color = useColor;
            }

            if (countNeeded != null && (filter & ComponentUIColor.CountNeeded) == ComponentUIColor.CountNeeded)
            {
                countNeeded.color = useColor;
            }

            if (countAvailable != null && (filter & ComponentUIColor.CountAvailable) == ComponentUIColor.CountAvailable)
            {
                countAvailable.color = useColor;
            }

            if (displayName != null && (filter & ComponentUIColor.ComponentName) == ComponentUIColor.ComponentName)
            {
                displayName.color = useColor;
            }

            if (description != null && (filter & ComponentUIColor.Description) == ComponentUIColor.Description)
            {
                description.color = useColor;
            }

            if (subtext != null && (filter & ComponentUIColor.Description) == ComponentUIColor.Description)
            {
                subtext.color = useColor;
            }
        }

        #endregion

    }
}