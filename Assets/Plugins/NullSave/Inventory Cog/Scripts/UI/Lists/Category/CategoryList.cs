using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("tock-tag-list", false)]
    public class CategoryList : MonoBehaviour
    {

        #region Variables

        public bool showAllItems = false;
        public Sprite allItemsIcon;
        public string allItemsText = "All Items";
        public bool allowClickSelect = true;

        public CategoryUI categoryPrefab;
        public Transform prefabContainer;

        public NavigationType navMode = NavigationType.ByKey;
        public string navButton = "Horizontal";
        public KeyCode backKey = KeyCode.Z;
        public KeyCode nextKey = KeyCode.C;
        public bool allowAutoWrap = false;

        public ListCategoryFilter categoryFilter = ListCategoryFilter.All;
        public List<Category> categories;

        public InventoryItemList bindInventory, bindContainer;

        public SelectedIndexChanged onSelectionChanged;

        private List<CategoryUI> loadedItems = new List<CategoryUI>();
        private int selIndex = 0;

        #endregion

        #region Properties

        public Category SelectedItem
        {
            get
            {
                if (selIndex < 0 || selIndex >= loadedItems.Count) return null;
                return loadedItems[selIndex].Category;
            }
        }

        public int SelectedIndex
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
                loadedItems[selIndex].SetActive(true);
                onSelectionChanged?.Invoke(selIndex);
                UpdateList();
            }
        }

        #endregion

        #region Unity Methods

        private void OnDisable()
        {
            if (bindInventory != null)
            {
                bindInventory.onBindingUpdated.RemoveListener(UpdateList);
                bindInventory.onNeedNextCategory.RemoveListener(NextCategory);
                bindInventory.onNeedPreviousCategory.RemoveListener(PreviousCategory);
                bindInventory.onPageChanged.RemoveListener(UpdatePage);
            }

            if (bindContainer != null)
            {
                bindContainer.onBindingUpdated.RemoveListener(UpdateList);
                bindContainer.onNeedNextCategory.RemoveListener(NextCategory);
                bindContainer.onNeedPreviousCategory.RemoveListener(PreviousCategory);
                bindContainer.onPageChanged.RemoveListener(UpdatePage);
            }
        }

        private void OnEnable()
        {
            ClearItems();

            if (showAllItems)
            {
                Category fakeCategory = Instantiate(InventoryDB.Categories[0]);
                fakeCategory.displayName = allItemsText;
                fakeCategory.icon = allItemsIcon;
                fakeCategory.name = "__ic_allitems";
                CategoryUI instance = Instantiate(categoryPrefab, prefabContainer);
                if (allowClickSelect)
                {
                    instance.onClick.AddListener(() =>
                    {
                        for (int i = 0; i < loadedItems.Count; i++)
                        {
                            if (loadedItems[i] == instance)
                            {
                                SelectedIndex = i;
                                return;
                            }
                        }
                    });
                }
                instance.LoadCategory(fakeCategory);
                instance.SetActive(true);
                loadedItems.Add(instance);
            }

            foreach (Category category in InventoryDB.Categories)
            {
                if (categoryFilter == ListCategoryFilter.All ||
                    (categoryFilter == ListCategoryFilter.InList && categories.Contains(category)) ||
                    (categoryFilter == ListCategoryFilter.NotInList && !categories.Contains(category)))
                {
                    CategoryUI instance = Instantiate(categoryPrefab, prefabContainer);
                    if (allowClickSelect)
                    {
                        instance.onClick.AddListener(() =>
                        {
                            for (int i = 0; i < loadedItems.Count; i++)
                            {
                                if (loadedItems[i] == instance)
                                {
                                    SelectedIndex = i;
                                    return;
                                }
                            }
                        });
                    }
                    instance.LoadCategory(category);
                    instance.SetActive(loadedItems.Count == 0);
                    loadedItems.Add(instance);
                }
            }

            selIndex = 0;
            onSelectionChanged?.Invoke(0);

            if (bindInventory != null)
            {
                if (bindInventory.Inventory != null)
                {
                    UpdateList();
                }

                bindInventory.onBindingUpdated.AddListener(UpdateList);
                bindInventory.onNeedNextCategory.AddListener(NextCategory);
                bindInventory.onNeedPreviousCategory.AddListener(PreviousCategory);
                bindInventory.onPageChanged.AddListener(UpdatePage);
            }

            if (bindContainer != null)
            {
                if (bindContainer.Container != null)
                {
                    UpdateList();
                }

                bindContainer.onBindingUpdated.AddListener(UpdateList);
                bindContainer.onNeedNextCategory.AddListener(NextCategory);
                bindContainer.onNeedPreviousCategory.AddListener(PreviousCategory);
                bindContainer.onPageChanged.AddListener(UpdatePage);
            }

        }

        private void Update()
        {
            if (bindContainer != null && (bindContainer.Inventory == null || bindContainer.Inventory.IsPromptOpen)) return;
            if (bindInventory != null && (bindInventory.Inventory == null || bindInventory.Inventory.IsPromptOpen)) return;

            switch (navMode)
            {
                case NavigationType.ByButton:
#if GAME_COG
                    float axis = GameCog.Input.GetAxis(navButton);
#else
                    float axis = Input.GetAxis(navButton);
#endif
                    if(axis <= -0.1f)
                    {
                        SelectedIndex -= 1;
                    }
                    else if(axis >= 0.1f)
                    {
                        SelectedIndex += 1;
                    }
                    break;
                case NavigationType.ByKey:
#if GAME_COG
                    if (GameCog.Input.GetKeyDown(backKey))
#else
                    if (Input.GetKeyDown(backKey))
#endif
                    {
                        SelectedIndex -= 1;
                    }
#if GAME_COG
                    if (GameCog.Input.GetKeyDown(nextKey))
#else
                    if (Input.GetKeyDown(nextKey))
#endif
                    {
                        SelectedIndex += 1;
                    }
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void NextCategory()
        {
            SelectedIndex += 1;
        }

        public void PreviousCategory()
        {
            SelectedIndex -= 1;
        }

        #endregion

        #region Private Methods

        private void ClearItems()
        {
            foreach (CategoryUI item in loadedItems)
            {
                Destroy(item.gameObject);
            }

            selIndex = -1;
            loadedItems.Clear();
        }

        private void UnselectCurrent()
        {
            if (selIndex < 0 || selIndex >= loadedItems.Count) return;
            loadedItems[selIndex].CurrentPage = 0;
            loadedItems[selIndex].SetActive(false);
        }

        private void UpdateList()
        {
            if (bindContainer != null) bindContainer.LoadFromCategoyList(this);
            if (bindInventory != null) bindInventory.LoadFromCategoyList(this);
        }

        private void UpdatePage(int page)
        {
            if (selIndex < 0 || selIndex >= loadedItems.Count) return;
            loadedItems[selIndex].CurrentPage = page;
        }

        #endregion

    }
}