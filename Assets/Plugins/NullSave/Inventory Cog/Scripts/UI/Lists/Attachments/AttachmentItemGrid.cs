using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    public class AttachmentItemGrid : AttachmentItemList
    {

        #region Variables

        // UI
        public ItemUI itemUIPrefab;
        public Vector2 gridSize = new Vector2(5, 4);
        public GridPageMode pageMode;

        // Navigation
        public string inputHorizontal = "Horizontal";
        public string inputVertical = "Vertical";
        public KeyCode keyLeft = KeyCode.A;
        public KeyCode keyUp = KeyCode.W;
        public KeyCode keyRight = KeyCode.D;
        public KeyCode keyDown = KeyCode.S;

        private List<ItemUI> loadedItems;
        private GridLayoutGroup prefabContainer;
        private int selIndex = -1;

        private float nextRepeat;
        private bool waitForZero;
        private bool unlockedThisFrame;

        #endregion

        #region Properties

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
                }

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

        private void Update()
        {
            if (!LockInput)
            {
                UpdateNavigation();

                if (unlockedThisFrame)
                {
                    unlockedThisFrame = false;
                    return;
                }

                switch (selectionMode)
                {
                    case NavigationType.ByButton:
#if GAME_COG
                        if (GameCog.Input.GetButtonDown(buttonSubmit))
#else
                        if (Input.GetButtonDown(buttonSubmit))
#endif
                        {
                            AttachToItem();
                        }
                        break;
                    case NavigationType.ByKey:
#if GAME_COG
                        if (GameCog.Input.GetKeyDown(keySubmit))
#else
                        if (Input.GetKeyDown(keySubmit))
#endif
                        {
                            AttachToItem();
                        }
                        break;
                }
            }
        }

        #endregion

        #region Public Methods

        public override void LoadAttachments(AttachmentSlot attachSlot)
        {
            ClearItems();
            Slot = attachSlot;
            Item = attachSlot.ParentItem;

            List<InventoryItem> items = Inventory.GetAvailableAttachments(attachSlot.ParentItem, attachSlot.AttachPoint.pointId);
            foreach (InventoryItem attachment in items)
            {
                ItemUI itemUI = Instantiate(itemUIPrefab, prefabContainer.transform);
                itemUI.LoadItem(Inventory, null, attachment);
                itemUI.SetSelected(loadedItems.Count == 0 && !LockInput);
                loadedItems.Add(itemUI);
            }

            SelectedIndex = 0;
        }

        #endregion

        #region Private Methods

        private void ClearItems()
        {
            if (loadedItems != null)
            {
                foreach (ItemUI ui in loadedItems)
                {
                    Destroy(ui.gameObject);
                }
            }

            selIndex = -1;
            onSelectionChanged?.Invoke(-1);

            loadedItems = new List<ItemUI>();
        }

        internal override void LockStateChanged()
        {
            if (hideSelectionWhenLocked && SelectedItem != null)
            {
                unlockedThisFrame = true;
                SelectedItem.SetSelected(!LockInput);
            }
        }

        #endregion

        #region Navigation Methods

        public Vector2 CellFromIndex(int index)
        {
            int row = Mathf.FloorToInt(index / gridSize.x);
            int col = index - (int)(row * gridSize.x);
            return new Vector2(col, row);
        }

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