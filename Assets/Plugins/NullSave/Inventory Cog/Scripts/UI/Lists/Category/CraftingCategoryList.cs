using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("tock-tag-list", false)]
    public class CraftingCategoryList : MonoBehaviour
    {

        #region Variables

        public bool showAllItems = false;
        public Sprite allItemsIcon;
        public string allItemsText = "All Items";

        public CraftingCategoryUI categoryPrefab;
        public Transform prefabContainer;

        public NavigationType navMode = NavigationType.ByKey;
        public string navButton = "Horizontal";
        public KeyCode backKey = KeyCode.Z;
        public KeyCode nextKey = KeyCode.C;
        public bool allowAutoWrap = false;

        public ListCategoryFilter categoryFilter = ListCategoryFilter.All;
        public List<CraftingCategory> categories;

        public RecipeList bindToList;

        public SelectedIndexChanged onSelectionChanged;

        private List<CraftingCategoryUI> loadedItems = new List<CraftingCategoryUI>();
        private int selIndex = 0;

        #endregion

        #region Properties

        public List<CraftingCategory> FilteredCategories { get { return categories; } }

        public CraftingCategory SelectedItem
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
                if (bindToList != null) bindToList.LoadFromCategoyList(this);
            }
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            ClearItems();

            if (showAllItems)
            {
                CraftingCategory fakeCategory = Instantiate(InventoryDB.CraftingCategories[0]);
                fakeCategory.displayName = allItemsText;
                fakeCategory.icon = allItemsIcon;
                fakeCategory.name = "__ic_allitems";
                CraftingCategoryUI instance = Instantiate(categoryPrefab, prefabContainer);
                instance.LoadCategory(fakeCategory);
                instance.SetActive(true);
                loadedItems.Add(instance);
            }

            foreach (CraftingCategory category in InventoryDB.CraftingCategories)
            {
                if (categoryFilter == ListCategoryFilter.All ||
                    (categoryFilter == ListCategoryFilter.InList && categories.Contains(category)) ||
                    (categoryFilter == ListCategoryFilter.NotInList && !categories.Contains(category)))
                {
                    CraftingCategoryUI instance = Instantiate(categoryPrefab, prefabContainer);
                    instance.LoadCategory(category);
                    instance.SetActive(loadedItems.Count == 0);
                    loadedItems.Add(instance);
                }
            }

            selIndex = 0;
            onSelectionChanged?.Invoke(0);

            if (bindToList != null)
            {
                if (bindToList.Inventory != null)
                {
                    UpdateList();
                }

                bindToList.onBindingUpdated.AddListener(UpdateList);
            }

        }

        private void Update()
        {
            switch (navMode)
            {
                case NavigationType.ByButton:
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
            int selIndex = SelectedIndex;
            SelectedIndex += 1;
            if (selIndex != SelectedIndex)
            {
                UpdateList();
            }
        }

        public void PreviousCategory()
        {
            int selIndex = SelectedIndex;
            SelectedIndex -= 1;
            if (selIndex != SelectedIndex)
            {
                if (bindToList != null) bindToList.LoadFromCategoyList(this);
            }
        }

        #endregion

        #region Private Methods

        private void ClearItems()
        {
            foreach (CraftingCategoryUI item in loadedItems)
            {
                Destroy(item.gameObject);
            }

            selIndex = -1;
            loadedItems.Clear();
        }

        private void UnselectCurrent()
        {
            if (selIndex < 0 || selIndex >= loadedItems.Count) return;
            loadedItems[selIndex].SetActive(false);
        }

        private void UpdateList()
        {
            if (bindToList != null) bindToList.LoadFromCategoyList(this);
        }

        #endregion

    }
}