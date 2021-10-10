using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(ScrollRect))]
    [HierarchyIcon("tock-list", false)]
    public class RecipeScrollList : RecipeList
    {

        #region Variables

        // UI
        public RecipeUI itemUIPrefab;
        public float lineHeight = 32;
        public Padding padding;
        public RecipeComponentUI componentUIprefab;
        public Transform componentContainer;

        // Multi-craft
        public bool allowMulticraft = false;
        public CountSelectUI countSelectUI;
        public int minToShowCount = 5;
        public Transform countContainer;

        // Navigation
        public bool invertInput = false;
        public string navButton = "Vertical";
        public KeyCode backKey = KeyCode.UpArrow;
        public KeyCode nextKey = KeyCode.DownArrow;

        private List<RecipeUI> loadedItems;
        private List<RecipeComponentUI> loadedComponents;
        private ScrollRect scrollRect;
        private RectTransform viewPort, content;

        private int selIndex;
        private List<string> lastCategoryFilter;

        private CountSelectUI spawnedCount;

        private float nextRepeat;
        private bool waitForZero;

        #endregion

        #region Properties

        public override RecipeUI SelectedItem
        {
            get
            {
                if (selIndex < 0 || selIndex >= loadedItems.Count) return null;
                return loadedItems[selIndex];
            }
            set
            {
                for (int i = 0; i < loadedItems.Count; i++)
                {
                    if (loadedItems[i] == value)
                    {
                        SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        public override int SelectedIndex
        {
            get { return selIndex; }
            set
            {
                if (selIndex == value) return;
                if (value < 0)
                {
                    if (allowAutoWrap)
                    {
                        value = loadedItems.Count - 1;
                    }
                    else
                    {
                        return;
                    }
                }
                if (value >= loadedItems.Count)
                {
                    if (allowAutoWrap)
                    {
                        value = 0;
                    }
                    else
                    {
                        return;
                    }
                }
                UnselectCurrent();
                selIndex = value;
                if (selIndex >= 0 && selIndex < loadedItems.Count)
                {
                    loadedItems[selIndex].SetSelected(true);
                    UpdateComponents(loadedItems[selIndex].Recipe);
                }
                else
                {
                    UpdateComponents(null);
                }

                onSelectionChanged?.Invoke(selIndex);
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            viewPort = scrollRect.viewport;
            content = viewPort.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (Inventory != null & loadMode == ListLoadMode.OnEnable)
            {
                LoadRecipies(categories);
            }
        }

        private void Start()
        {
            if (loadMode == ListLoadMode.OnEnable)
            {
                LoadRecipies(categories);
            }
        }

        private void Update()
        {
            if (!LockInput)
            {
                UpdateNavigation();
            }
        }

        #endregion

        #region Public Methods

        public void CraftSelected()
        {
            if (allowMulticraft)
            {
                int maxCraft = inventoryCog.GetCraftableCount(SelectedItem.Recipe);
                if (maxCraft >= minToShowCount)
                {
                    Action<bool, int> callback = (bool craft, int count) =>
                    {
                        if (craft)
                        {
                            CraftSelectedCount(count);
                        }
                        if (spawnedCount != null) Destroy(spawnedCount.gameObject);
                    };

                    spawnedCount = Instantiate(countSelectUI, countContainer);
                    spawnedCount.SelectCount(SelectedItem.Recipe.displayName, 1, maxCraft, 1, callback);
                    return;
                }
            }
            
            inventoryCog.Craft(SelectedItem.Recipe, 1);
            ReloadLast();
        }

        public void CraftSelectedCount(int count)
        {
            inventoryCog.Craft(SelectedItem.Recipe, count);
            ReloadLast();
        }

        public override void LoadFromCategoyList(CraftingCategoryList source)
        {
            List<string> cats = new List<string>();
            CraftingCategory category = source.SelectedItem;

            if (category.name == "__ic_allitems")
            {
                switch (source.categoryFilter)
                {
                    case ListCategoryFilter.All:
                        LoadRecipies();
                        break;
                    default:
                        foreach (CraftingCategory cat in source.FilteredCategories)
                        {
                            cats.Add(cat.name);
                        }
                        LoadRecipies(cats);
                        break;
                }
            }
            else
            {
                cats.Add(category.name);
                LoadRecipies(cats);
            }
        }

        public override void LoadRecipies()
        {
            LoadRecipies(categories);
        }

        public override void LoadRecipies(List<string> categories)
        {
            if (Inventory == null) return;

            ClearItems();

            lastCategoryFilter = categories;
            foreach (CraftingRecipe recipe in InventoryDB.Recipes)
            {
                if (categories == null || ( recipe.craftingCategory != null && categories.Contains(recipe.craftingCategory.name) || categories.Count == 0))
                {
                    RecipeUI ui = Instantiate(itemUIPrefab, content);
                    ui.Inventory = Inventory;
                    ui.LoadRecipe(recipe, Inventory.GetRecipeCraftable(recipe));
                    ui.SetSelected(loadedItems.Count == 0 && (!LockInput || !hideSelectionWhenLocked));
                    if (allowSelectByClick)
                    {
                        ui.onClick.AddListener(SelectItem);
                    }
                    loadedItems.Add(ui);
                }
            }

            if (content != null)
            {
                content.sizeDelta = new Vector2(content.sizeDelta.x, loadedItems.Count * lineHeight + padding.Vertical);
            }

            SelectedIndex = 0;
        }

        public void LoadRecipiesForCategory(string category)
        {
            List<string> cats = new List<string>();
            cats.Add(category);
            LoadRecipies(cats);
        }

        public override void ReloadLast()
        {
            if (lastCategoryFilter == null) lastCategoryFilter = categories;
            LoadRecipies(lastCategoryFilter);
        }

        #endregion

        #region Private Methods

        private void ClearItems()
        {
            selIndex = -1;
            if (loadedItems != null)
            {
                foreach (RecipeUI ui in loadedItems)
                {
                    Destroy(ui.gameObject);
                }
            }
            loadedItems = new List<RecipeUI>();

            ClearComponents();
        }

        private void ClearComponents()
        {
            if (loadedComponents != null)
            {
                foreach (RecipeComponentUI ui in loadedComponents)
                {
                    Destroy(ui.gameObject);
                }
            }
            loadedComponents = new List<RecipeComponentUI>();
        }

        internal override void LockStateChanged()
        {
            if (hideSelectionWhenLocked)
            {
                if (LockInput)
                {
                    UnselectCurrent();
                }
                else
                {
                    SelectCurrent();
                }
            }
        }

        private void SelectCurrent()
        {
            if (selIndex < 0 || selIndex >= loadedItems.Count) return;
            loadedItems[selIndex].SetSelected(true);
        }

        private void UnselectCurrent()
        {
            if (selIndex < 0 || selIndex >= loadedItems.Count) return;
            loadedItems[selIndex].SetSelected(false);
        }

        private void UpdateComponents(CraftingRecipe recipe)
        {
            ClearComponents();

            if (componentUIprefab != null && componentContainer != null)
            {
                if (recipe.componentType == ComponentType.Standard)
                {
                    foreach (ItemReference item in recipe.components)
                    {
                        RecipeComponentUI component = Instantiate(componentUIprefab, componentContainer);
                        component.LoadComponent(item, Inventory, 0, 0);
                        loadedComponents.Add(component);
                    }
                }
                else
                {
                    foreach (AdvancedComponent item in recipe.advancedComponents)
                    {
                        RecipeComponentUI component = Instantiate(componentUIprefab, componentContainer);
                        component.LoadComponent(new ItemReference(item.item, item.count), Inventory, item.minCondition, item.minRarity);
                        loadedComponents.Add(component);
                    }
                }
            }
        }

        #endregion

        #region Navigation Methods

        private float GetInput()
        {
#if GAME_COG
            return GameCog.Input.GetAxis(navButton);
#else
            return Input.GetAxis(navButton);
#endif
        }

        private void NavigateByButton()
        {
            if (autoRepeat)
            {
                if (nextRepeat > 0) nextRepeat -= Time.deltaTime;
                if (nextRepeat > 0) return;
            }

            float input = GetInput();
            if (waitForZero && input != 0) return;

            if (invertInput)
            {
                input = -input;
            }

            if (input <= -0.1f)
            {
                NavigatePrevious();
            }
            else if (input >= 0.1f)
            {
                NavigateNext();
            }
            else if (input == 0)
            {
                nextRepeat = 0;
            }
        }

        private void NavigateByKey()
        {
#if GAME_COG
            if (GameCog.Input.GetKeyDown(backKey))
            {
                NavigatePrevious();
            }
            if (GameCog.Input.GetKeyDown(nextKey))
            {
                NavigateNext();
            }
#else
            if (Input.GetKeyDown(backKey))
            {
                NavigatePrevious();
            }
            if (Input.GetKeyDown(nextKey))
            {
                NavigateNext();
            }
#endif
        }

        private void NavigateNext()
        {
            SelectedIndex += 1;
            UpdateRepeat();
        }

        private void NavigatePrevious()
        {
            SelectedIndex -= 1;
            UpdateRepeat();
        }

        private void UpdateNavigation()
        {
            switch (navigationMode)
            {
                case NavigationType.ByButton:
                    NavigateByButton();
                    break;
                case NavigationType.ByKey:
                    NavigateByKey();
                    break;
            }
        }

        private void UpdateRepeat()
        {
            if (autoRepeat)
            {
                nextRepeat = repeatDelay;
            }
            else
            {
                waitForZero = true;
            }
        }

        #endregion

    }
}