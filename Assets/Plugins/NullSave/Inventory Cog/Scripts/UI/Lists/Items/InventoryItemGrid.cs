using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("item-grid", false)]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class InventoryItemGrid : InventoryItemList
    {

        #region Variables

        // UI
        public ItemUI itemUIPrefab;
        public Vector2 gridSize = new Vector2(5, 4);
        public GridPageMode pageMode;
        public bool alwaysFillPage;

        // Navigation
        public string inputHorizontal = "Horizontal";
        public string inputVertical = "Vertical";
        public KeyCode keyLeft = KeyCode.A;
        public KeyCode keyUp = KeyCode.W;
        public KeyCode keyRight = KeyCode.D;
        public KeyCode keyDown = KeyCode.S;

        private GridLayoutGroup prefabContainer;
        private List<ItemUI> loadedItems;
        private List<Category> lastCategoryFilter;
        private List<InventoryItem> itemCache;

        private int selIndex;

        private float nextRepeat;
        private bool waitForZero;
        private int selectedPage;
        private bool startFromLast;
        private bool reloadCalled;

        private CategoryList lastSource;

        #endregion

        #region Properties

        public virtual bool IsLockedToCategory { get; set; }

        public int ItemsPerPage
        {
            get
            {
                return (int)(gridSize.x * gridSize.y);
            }
        }

        public int SelectedPage
        {
            get
            {
                return selectedPage;
            }
            private set
            {
                selectedPage = value;
                onPageChanged?.Invoke(selectedPage);
            }
        }

        public Vector2 SelectedCell
        {
            get
            {
                int row = Mathf.FloorToInt(selIndex / gridSize.x);
                int col = selIndex - (int)(row * gridSize.x);
                return new Vector2(col, row);
            }
            set
            {
                SelectedIndex = (int)(value.y * gridSize.x + value.x);
            }
        }

        public override int SelectedIndex
        {
            get { return selIndex; }
            set
            {
                if (loadedItems == null) return;
                if (loadedItems.Count <= value) value = loadedItems.Count - 1;
                if (value < 0) value = 0;
                if (selIndex <= loadedItems.Count - 1 && selIndex >= 0) loadedItems[selIndex].SetSelected(false);
                selIndex = value;

                if (selIndex >= 0 && selIndex <= loadedItems.Count - 1)
                {
                    if (!LockInput || !hideSelectionWhenLocked) loadedItems[selIndex].SetSelected(true);
                    SetDetail(loadedItems[selIndex].Item);
                }
                else
                {
                    SetDetail(null);
                }

                UpdateCheckoutUI();
                onSelectionChanged?.Invoke(selIndex);
            }
        }

        public override ItemUI SelectedItem
        {
            get
            {
                if (loadedItems == null) return null;

                if (selIndex >= 0 && selIndex <= loadedItems.Count - 1)
                {
                    return loadedItems[selIndex];
                }

                return null;
            }
            set
            {
                for (int i = 0; i < loadedItems.Count; i++)
                {
                    if (loadedItems[i] == value)
                    {
                        SelectedIndex = i;
                        UpdateCheckoutUI();
                        return;
                    }
                }
            }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            prefabContainer = GetComponent<GridLayoutGroup>();
        }

        private void OnEnable()
        {
            if (loadMode == ListLoadMode.OnEnable && Inventory != null)
            {
                LoadItems();
            }
        }

        private void Start()
        {
            if (loadMode == ListLoadMode.OnEnable && Inventory != null)
            {
                LoadItems();
            }
        }

        private void Update()
        {
            reloadCalled = false;

            if (!LockInput)
            {
                UpdateNavigation();

                switch (selectionMode)
                {
                    case NavigationType.ByButton:
#if GAME_COG
                        if (GameCog.Input.GetButtonDown(buttonSubmit))
#else
                        if (Input.GetButtonDown(buttonSubmit))
#endif
                        {
                            onItemSubmit?.Invoke(this, SelectedItem, lastCategoryFilter);
                        }
                        break;
                    case NavigationType.ByKey:
#if GAME_COG
                        if (GameCog.Input.GetKeyDown(keySubmit))
#else
                        if (Input.GetKeyDown(keySubmit))
#endif
                        {
                            onItemSubmit?.Invoke(this, SelectedItem, lastCategoryFilter);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Public Methods

        public override void LoadFromCategoyList(CategoryList source)
        {
            LoadFromCategoyList(source, false);
        }

        public override void LoadFromCategoyList(CategoryList source, bool startFromLastPage)
        {
            startFromLast = startFromLastPage;
            Category category = source.SelectedItem;
            if (category == null)
            {
                LoadItems();
                return;
            }
            if (category.name == "__ic_allitems")
            {
                LoadItems();
            }
            else
            {
                List<Category> catFilter = new List<Category>();
                switch (listSource)
                {
                    case ListSource.InventoryCog:
                        catFilter.Add(Inventory.GetCategory(category.name));
                        LoadFromInventory(catFilter);
                        break;
                    case ListSource.InventoryContainer:
                        catFilter.Add(category);
                        LoadFromContainer(catFilter);
                        break;
                    case ListSource.InventoryMerchant:
                        catFilter.Add(category);
                        LoadFromMerchant(catFilter);
                        break;
                    case ListSource.ContainerItem:
                        catFilter.Add(category);
                        LoadFromContainerItem(catFilter);
                        break;
                }
            }

            lastSource = source;
        }

        public override void LoadItems()
        {
            lastSource = null;

            switch (listSource)
            {
                case ListSource.InventoryCog:
                    LoadFromInventory(GetCategoryFilter());
                    break;
                case ListSource.InventoryContainer:
                    LoadFromContainer(GetCategoryFilter());
                    break;
                case ListSource.InventoryMerchant:
                    LoadFromMerchant(GetCategoryFilter());
                    break;
                case ListSource.ContainerItem:
                    LoadFromContainerItem(GetCategoryFilter());
                    break;
            }
        }

        public override void LoadItems(List<InventoryItem> items)
        {
            itemCache = items;
            LoadPage(0);
        }

        public override void ReloadLast()
        {
            selIndex = -1;
            int curPage = SelectedPage;
            if (lastSource == null)
            {
                LoadItems();
            }
            else
            {
                LoadFromCategoyList(lastSource, false);
            }
            SelectedPage = curPage;
            SelectedIndex = 0;
        }

        #endregion

        #region Private Methods

        public Vector2 CellFromIndex(int index)
        {
            int row = Mathf.FloorToInt(index / gridSize.x);
            int col = index - (int)(row * gridSize.x);
            return new Vector2(col, row);
        }

        private void ClearItems()
        {
            if (loadedItems != null)
            {
                foreach (ItemUI ui in loadedItems)
                {
                    ui.onZeroCount.RemoveListener(ZeroCount);
                    Destroy(ui.gameObject);
                }
            }

            SetDetail(null);
            selIndex = -1;
            onSelectionChanged?.Invoke(-1);

            loadedItems = new List<ItemUI>();
        }

        private List<Category> GetCategoryFilter()
        {
            // Generate category list
            List<Category> catFilter = new List<Category>();
            switch (categoryFilter)
            {
                case ListCategoryFilter.All:
                    foreach (Category category in InventoryDB.Categories)
                    {
                        switch (listSource)
                        {
                            case ListSource.InventoryContainer:
                            case ListSource.InventoryMerchant:
                            case ListSource.ContainerItem:
                                catFilter.Add(category);
                                break;
                            case ListSource.InventoryCog:
                                Category catRef = Inventory.GetCategory(category.name);
                                if (catRef.catUnlocked)
                                {
                                    catFilter.Add(catRef);
                                }
                                break;
                        }
                    }
                    break;
                case ListCategoryFilter.InList:
                    foreach (Category category in categories)
                    {
                        switch (listSource)
                        {
                            case ListSource.InventoryContainer:
                            case ListSource.InventoryMerchant:
                            case ListSource.ContainerItem:
                                catFilter.Add(category);
                                break;
                            case ListSource.InventoryCog:
                                Category catRef = Inventory.GetCategory(category.name);
                                if (catRef.catUnlocked)
                                {
                                    catFilter.Add(catRef);
                                }
                                break;
                        }
                    }
                    break;
                case ListCategoryFilter.NotInList:
                    foreach (Category category in InventoryDB.Categories)
                    {
                        if (!categories.Contains(category))
                        {
                            switch (listSource)
                            {
                                case ListSource.InventoryContainer:
                                case ListSource.InventoryMerchant:
                                case ListSource.ContainerItem:
                                    catFilter.Add(category);
                                    break;
                                case ListSource.InventoryCog:
                                    Category catRef = Inventory.GetCategory(category.name);
                                    if (catRef.catUnlocked)
                                    {
                                        catFilter.Add(catRef);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }

            return catFilter;
        }

        private int GetLockedSlots(List<Category> catFilter)
        {
            int lockedSlots = 0;
            if (listSource == ListSource.InventoryCog)
            {
                foreach (Category category in catFilter)
                {
                    if (category.hasLockingSlots)
                    {
                        lockedSlots += category.MaximumSlots - category.UnlockedSlots;
                    }
                }
            }
            return lockedSlots;
        }

        private int GetMaxPages(List<Category> catFilter, out int usedSlots)
        {
            usedSlots = 0;

            float slotsPerPage = (int)(gridSize.x * gridSize.y);
            if (listSource == ListSource.InventoryContainer)
            {
                if (pageMode == GridPageMode.AllAvailable)
                {
                    if (container.hasMaxStoreSlots)
                    {
                        usedSlots = container.maxStoreSlots;
                        return Mathf.CeilToInt(Container.maxStoreSlots / slotsPerPage);
                    }
                    else
                    {
                        usedSlots = Mathf.CeilToInt(Mathf.Max(container.StoredItems.Count, slotsPerPage));
                        return Mathf.CeilToInt(Mathf.Max(container.StoredItems.Count, slotsPerPage) / slotsPerPage);
                    }
                }
                else
                {
                    usedSlots = container.StoredItems.Count;
                    return Mathf.CeilToInt(Container.StoredItems.Count / slotsPerPage);
                }
            }
            else if (listSource == ListSource.ContainerItem)
            {
                if (pageMode == GridPageMode.AllAvailable)
                {
                    if (ContainerItem.hasMaxStoreSlots)
                    {
                        usedSlots = ContainerItem.maxStoreSlots;
                        return Mathf.CeilToInt(ContainerItem.maxStoreSlots / slotsPerPage);
                    }
                    else
                    {
                        usedSlots = Mathf.CeilToInt(Mathf.Max(ContainerItem.StoredItems.Count, slotsPerPage));
                        return Mathf.CeilToInt(Mathf.Max(ContainerItem.StoredItems.Count, slotsPerPage) / slotsPerPage);
                    }
                }
                else
                {
                    usedSlots = ContainerItem.StoredItems.Count;
                    return Mathf.CeilToInt(ContainerItem.StoredItems.Count / slotsPerPage);
                }
            }
            else
            {
                int lockedSlots = 0;
                foreach (Category category in catFilter)
                {
                    if (pageMode == GridPageMode.AllAvailable)
                    {
                        if (category.hasLockingSlots)
                        {
                            usedSlots += category.UnlockedSlots;
                            lockedSlots += category.MaximumSlots - category.UnlockedSlots;
                        }
                        else if (category.hasMaxSlots)
                        {
                            usedSlots += category.MaximumSlots;
                        }
                        else
                        {
                            usedSlots += category.UsedSlots;
                        }
                    }
                    else
                    {
                        usedSlots += category.UsedSlots;
                    }
                }

                if (showLockedSlots)
                {
                    return Mathf.CeilToInt((usedSlots + lockedSlots) / slotsPerPage);
                }
                else
                {
                    return Mathf.CeilToInt(usedSlots / slotsPerPage);
                }
            }
        }

        private void LoadFromContainer(List<Category> catFilter)
        {
            if (Container == null) return;
            lastCategoryFilter = catFilter;

            // Load filtered items
            LoadItems(FilterItems(Container.GetStoredItems(catFilter)));
        }

        private void LoadFromContainerItem(List<Category> catFilter)
        {
            if (ContainerItem == null) return;
            lastCategoryFilter = catFilter;

            // Load filtered items
            LoadItems(FilterItems(ContainerItem.GetStoredItems(catFilter)));
        }

        private void LoadFromInventory(List<Category> catFilter)
        {
            if (Inventory == null) return;
            lastCategoryFilter = catFilter;

            // Load filtered items
            List<InventoryItem> catItems = new List<InventoryItem>();
            foreach (Category category in catFilter)
            {
                catItems.AddRange(category.AssignedItems);
            }
            LoadItems(FilterItems(catItems));
        }

        private void LoadFromMerchant(List<Category> catFilter)
        {
            if (Inventory == null) return;
            lastCategoryFilter = catFilter;

            // Load filtered items
            List<InventoryItem> catItems = new List<InventoryItem>();
            foreach (ItemReference item in Merchant.BasicStock)
            {
                if (catFilter.Contains(item.item.category))
                {
                    catItems.Add(item.item);
                }
            }

            LoadItems(FilterItems(catItems));
        }

        private void LoadPage(int pageIndex)
        {
            Unsubscribe();
            Subscribe();

            int usedSlots;
            int maxPerPage = (int)(gridSize.x * gridSize.y);
            int maxPages = GetMaxPages(lastCategoryFilter, out usedSlots);
            int lockedSlots = 0;

            selIndex = -1;

            if (showLockedSlots)
            {
                lockedSlots = GetLockedSlots(lastCategoryFilter);
            }

            if (startFromLast)
            {
                pageIndex = maxPages - 1;
                startFromLast = false;
            }

            // Check for page constraints
            if (pageIndex < 0)
            {
                if (!IsLockedToCategory)
                {
                    onNeedPreviousCategory?.Invoke();
                    SelectedIndex = 0;
                }
                return;
            }
            if (pageIndex > maxPages)
            {
                if (!IsLockedToCategory)
                {
                    onNeedNextCategory?.Invoke();
                    SelectedIndex = 0;
                }
                return;
            }

            ClearItems();
            SelectedPage = pageIndex;

            int maxItems = Mathf.Min(maxPerPage, usedSlots);
            int usedItems = 0;
            int skipItems = SelectedPage * maxItems;

            for (int i = skipItems; i < itemCache.Count; i++)
            {
                ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                ui.LoadItem(Inventory, Container, itemCache[i]);
                ui.ItemListParent = this;
                ui.SetSelected(loadedItems.Count == 0 && (!LockInput || !hideSelectionWhenLocked));
                ui.onZeroCount.AddListener(ZeroCount);
                if (allowSelectByClick)
                {
                    ui.onClick.AddListener(SelectItem);
                }
                loadedItems.Add(ui);
                maxItems -= 1;
                usedItems += 1;
                if (maxItems == 0)
                {
                    break;
                }
            }

            if (skipItems > itemCache.Count)
            {
                maxItems = 0;
            }

            while (maxItems > 0)
            {
                ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                ui.ItemListParent = this;
                ui.LoadItem(Inventory, Container, null);
                ui.SetSelected(false);

                loadedItems.Add(ui);
                if (allowSelectByClick)
                {
                    ui.onClick.AddListener(SelectItem);
                }
                maxItems -= 1;
                usedItems += 1;
            }

            if (showLockedSlots)
            {
                while (usedItems < maxPerPage && lockedSlots > 0)
                {
                    ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                    ui.LoadLockedSlot(Inventory);
                    ui.SetSelected(false);
                    loadedItems.Add(ui);
                    if (allowSelectByClick)
                    {
                        ui.onClick.AddListener(SelectItem);
                    }

                    lockedSlots -= 1;
                    usedItems += 1;
                }
            }

            if (pageMode == GridPageMode.AllAvailable && alwaysFillPage)
            {
                while (usedItems < maxPerPage)
                {
                    ItemUI ui = Instantiate(itemUIPrefab, prefabContainer.transform);
                    ui.ItemListParent = this;
                    ui.LoadItem(Inventory, Container, null);
                    ui.SetSelected(false);

                    loadedItems.Add(ui);
                    if (allowSelectByClick)
                    {
                        ui.onClick.AddListener(SelectItem);
                    }
                    usedItems += 1;
                }
            }

            SelectedIndex = 0;
            SelectedPage = pageIndex;
        }

        internal override void LockStateChanged()
        {
            if (hideSelectionWhenLocked && SelectedItem != null)
            {
                SelectedItem.SetSelected(!LockInput);
            }
        }

        #endregion

        #region Navigation Methods

        private Vector2 GetInput()
        {
#if GAME_COG
            return new Vector2(GameCog.Input.GetAxis(inputHorizontal), GameCog.Input.GetAxis(inputVertical));
#else
            return new Vector2(Input.GetAxis(inputHorizontal), Input.GetAxis(inputVertical));
#endif
        }

        private void NavigateByButton()
        {
            if (autoRepeat)
            {
                if (nextRepeat > 0) nextRepeat -= Time.deltaTime;
                if (nextRepeat > 0) return;
            }

            Vector2 input = GetInput();
            if (waitForZero)
            {
                if (input.x != 0 || input.y != 0) return;
                waitForZero = false;
            }

            if (input == Vector2.zero)
            {
                nextRepeat = 0;
            }
            if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                // Vertical
                if (input.y > 0.1f)
                {
                    NavigateUp();
                }
                else if (input.y < -0.1f)
                {
                    NavigateDown();
                }
            }
            else
            {
                // Horizontal
                if (input.x < -0.1f)
                {
                    NavigateLeft();
                }
                else if (input.x > 0.1f)
                {
                    NavigateRight();
                }
            }
        }

        private void NavigateByKey()
        {
#if GAME_COG
            if (GameCog.Input.GetKeyDown(keyUp)) NavigateUp();
            if (GameCog.Input.GetKeyDown(keyDown)) NavigateDown();
            if (GameCog.Input.GetKeyDown(keyLeft)) NavigateLeft();
            if (GameCog.Input.GetKeyDown(keyRight)) NavigateRight();
#else
            if (Input.GetKeyDown(keyUp)) NavigateUp();
            if (Input.GetKeyDown(keyDown)) NavigateDown();
            if (Input.GetKeyDown(keyLeft)) NavigateLeft();
            if (Input.GetKeyDown(keyRight)) NavigateRight();
#endif
        }

        private void NavigateDown()
        {
            if (loadedItems == null) return;

            // Down
            int newIndex = selIndex + (int)gridSize.x;
            Vector2 cell = CellFromIndex(newIndex);
            if (cell.x <= gridSize.x)
            {
                SelectedIndex = newIndex;
            }
            UpdateRepeat();
        }

        private void NavigateLeft()
        {
            if (loadedItems == null) return;

            // Left
            int newIndex = selIndex - 1;
            Vector2 cell = CellFromIndex(newIndex);
            if (cell.y != SelectedCell.y)
            {
                newIndex = -1;
            }
            if (newIndex >= 0)
            {
                SelectedIndex = newIndex;
            }
            else
            {
                if (SelectedPage > 0)
                {
                    Vector2 newSel = SelectedCell;
                    newSel.x = gridSize.x - 1;

                    LoadPage(SelectedPage - 1);
                    SelectedCell = newSel;
                }
                else
                {
                    if (!IsLockedToCategory)
                    {
                        onNeedPreviousCategory?.Invoke();
                    }
                }
            }
            UpdateRepeat();
        }

        private void NavigateRight()
        {
            if (loadedItems == null) return;

            // Right
            int newIndex = selIndex + 1;
            Vector2 cell = CellFromIndex(newIndex);
            if (cell.y != SelectedCell.y)
            {
                cell.x += gridSize.x;
            }

            if (cell.x < gridSize.x && newIndex < loadedItems.Count)
            {
                SelectedIndex = newIndex;
            }
            else
            {
                if (SelectedPage < GetMaxPages(lastCategoryFilter, out int used) - 1)
                {
                    Vector2 newSel = SelectedCell;
                    newSel.x = 0;

                    LoadPage(SelectedPage + 1);
                    SelectedCell = newSel;
                }
                else
                {
                    if (!IsLockedToCategory)
                    {
                        onNeedNextCategory?.Invoke();
                    }
                }
            }
            UpdateRepeat();
        }

        private void NavigateUp()
        {
            if (loadedItems == null) return;

            // Up
            int newIndex = selIndex - (int)gridSize.x;
            if (newIndex >= 0)
            {
                SelectedIndex = newIndex;
            }
            UpdateRepeat();
        }

        private void SetDetail(InventoryItem item)
        {
            if (detailClient != null)
            {
                detailClient.LoadItem(Inventory, item, item == null ? null : item.category);
                if (item == null && hideEmptyDetails)
                {
                    detailClient.gameObject.SetActive(false);
                }
                else
                {
                    detailClient.gameObject.SetActive(true);
                }
            }
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

        private void ZeroCount(ItemUI item)
        {
            if (reloadCalled) return;
            reloadCalled = true;
            ReloadLast();
        }

        #endregion

    }
}