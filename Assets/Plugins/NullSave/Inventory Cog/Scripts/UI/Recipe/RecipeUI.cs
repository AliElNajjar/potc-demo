using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("tock-ui", false)]
    public class RecipeUI : MonoBehaviour
    {

        #region Variables

        // Recipe UI
        public Image icon;
        public TextMeshProUGUI displayName, description, categoryName, duration, queuedCount;
        public Color craftableColor = Color.white;
        public GameObject hideIfInstant;
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
        [Tooltip("Uses standard c# time formatting")] public string timeFormat = "m\\:ss";
        public Color uncraftableColor = new Color(1, 1, 1, 0.4f);
        public GameObject lockedIndicator, selectedIndicator, craftableIndicator;
        public CraftingUIColor colorApplication = (CraftingUIColor)63;
        public string countFormat = "({count})";
        public Slider queueProgress;
        public GameObject hideIfNoQueue;
        public bool monitorQueue = false;

        public RarityColorIndicator rarityColorIndicator;
        public Slider raritySlider;
        public bool hideIfRarityZero;

        // component listing
        public RecipeComponentUI componentUIprefab;
        public Transform componentContainer;

        public RecipeUIClick onClick;

        private List<RecipeComponentUI> loadedComponents = new List<RecipeComponentUI>();

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        public CraftingRecipe Recipe { get; set; }

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (!monitorQueue || queueProgress == null) return;
            queueProgress.minValue = 0;
            queueProgress.maxValue = 1;
            queueProgress.value = Inventory.GetQueuedFirstProgress(Recipe);
        }

        #endregion

        #region Public Methods

        public void ClearComponents()
        {
            foreach (RecipeComponentUI component in loadedComponents)
            {
                Destroy(component.gameObject);
            }
            loadedComponents.Clear();
        }

        public void Click()
        {
            onClick?.Invoke(this);
        }

        public void LoadRecipe(CraftingRecipe recipe, bool craftable)
        {
            Recipe = recipe;

            if (recipe == null)
            {
                Unsubscribe();
                return;
            }

            // Recipe UI
            if (icon != null)
            {
                icon.sprite = recipe.icon;
                if ((colorApplication & CraftingUIColor.Icon) == CraftingUIColor.Icon)
                {
                    icon.color = craftable ? craftableColor : uncraftableColor;
                }
            }

            if (displayName != null)
            {
                displayName.text = recipe.displayName;
                if ((colorApplication & CraftingUIColor.RecipeName) == CraftingUIColor.RecipeName)
                {
                    displayName.color = craftable ? craftableColor : uncraftableColor;
                }
            }

            if (description != null)
            {
                description.text = recipe.description;
                if ((colorApplication & CraftingUIColor.Description) == CraftingUIColor.Description)
                {
                    description.color = craftable ? craftableColor : uncraftableColor;
                }
            }

            if (categoryName != null)
            {
                categoryName.text = recipe.craftingCategory.displayName;
                if ((colorApplication & CraftingUIColor.Category) == CraftingUIColor.Category)
                {
                    categoryName.color = craftable ? craftableColor : uncraftableColor;
                }
            }

            if (lockedIndicator != null)
            {
                lockedIndicator.SetActive(!recipe.Unlocked);
            }

            if (craftableIndicator != null)
            {
                craftableIndicator.SetActive(craftable);
            }

            if (hideIfInstant != null & recipe.craftTime == CraftingTime.Instant)
            {
                hideIfInstant.SetActive(false);
            }

            if (duration != null)
            {
                switch (recipe.craftTime)
                {
                    case CraftingTime.Instant:
                        duration.text = "Instant";
                        break;
                    case CraftingTime.RealTime:
                    case CraftingTime.GameTime:
                        System.TimeSpan ts = System.TimeSpan.FromSeconds(recipe.craftSeconds);
                        duration.text = ts.ToString(timeFormat);
                        break;
                }
            }

            if (componentUIprefab != null)
            {
                ClearComponents();

                if (recipe.componentType == ComponentType.Standard)
                {
                    foreach (ItemReference component in recipe.components)
                    {
                        RecipeComponentUI goComponent = Instantiate(componentUIprefab, componentContainer);
                        goComponent.LoadComponent(new ItemReference(component.item, component.count), Inventory, 0, 0);
                        loadedComponents.Add(goComponent);
                    }
                }
                else
                {
                    foreach (AdvancedComponent component in recipe.advancedComponents)
                    {
                        RecipeComponentUI goComponent = Instantiate(componentUIprefab, componentContainer);
                        goComponent.LoadComponent(new ItemReference(component.item, component.count), Inventory, component.minCondition, component.minRarity);
                        loadedComponents.Add(goComponent);
                    }
                }
            }

            if (rarityColorIndicator != null)
            {
                rarityColorIndicator.SetRarity(recipe.rarity);
            }

            if (raritySlider != null)
            {
                raritySlider.minValue = 0;
                raritySlider.maxValue = 10;
                raritySlider.value = recipe.rarity;
                if (hideIfRarityZero)
                {
                    raritySlider.gameObject.SetActive(recipe.rarity > 0);
                }
            }

            // Subscriptions
            Unsubscribe();
            if (monitorQueue)
            {
                Subscribe();
                QueueUpdated(null, 0);
            }
        }

        public void SetSelected(bool selected)
        {
            if (selectedIndicator != null) selectedIndicator.SetActive(selected);
        }

        #endregion

        #region Private Methods

        private void QueueUpdated(CraftingRecipe recipe, int count)
        {
            int queued = Inventory.GetQueuedCount(Recipe);
            if (hideIfNoQueue)
            {
                hideIfNoQueue.SetActive(queued > 0);
            }
            if (queuedCount != null)
            {
                queuedCount.text = countFormat.Replace("{count}", queued.ToString());
            }

        }

        private void Subscribe()
        {
            if (Inventory != null)
            {
                Inventory.onCraftQueued.AddListener(QueueUpdated);
                Inventory.onQueuedCraftComplete.AddListener(QueueUpdated);
            }
        }

        private void Unsubscribe()
        {
            if (Inventory != null)
            {
                Inventory.onCraftQueued.RemoveListener(QueueUpdated);
                Inventory.onQueuedCraftComplete.RemoveListener(QueueUpdated);
            }
        }

        #endregion

    }
}