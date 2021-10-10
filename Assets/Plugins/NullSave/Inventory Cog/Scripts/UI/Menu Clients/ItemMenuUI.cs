using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class ItemMenuUI : MonoBehaviour
    {

        #region Variables

        // Navigation
        public bool allowAutoWrap = false;
        public bool allowSelectByClick = false;
        public NavigationType navigationMode;
        public bool autoRepeat = true;
        public float repeatDelay = 0.5f;
        public bool invertInput = true;
        public string navButton = "Vertical";
        public KeyCode backKey = KeyCode.UpArrow;
        public KeyCode nextKey = KeyCode.DownArrow;

        public NavigationType selectionMode;
        public string buttonSubmit = "Submit";
        public KeyCode keySubmit = KeyCode.Return;

        private ItemMenuOptionUI[] options;
        private int selIndex;
        private float nextRepeat;
        private bool waitForZero;

        #endregion

        #region Properties

        public List<Category> Categories { get; set; }

        public ItemUI Item { get; set; }

        public ItemMenuClient Owner { get; set; }

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
                        value = options.Length - 1;
                    }
                    else
                    {
                        return;
                    }
                }
                if (value >= options.Length)
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

                if (selIndex >= 0 && selIndex < options.Length)
                {
                    options[selIndex].onUnSelected?.Invoke();
                }

                selIndex = value;
                if (selIndex >= 0 && selIndex < options.Length)
                {
                    options[selIndex].onSelected.Invoke();
                }
            }
        }

        public InventoryItemList Source { get; set; }

        #endregion

        #region Unity Methods

        private void Update()
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

            switch (selectionMode)
            {
                case NavigationType.ByButton:
#if GAME_COG
                    if (GameCog.Input.GetButtonDown(buttonSubmit))
#else
                    if (Input.GetButtonDown(buttonSubmit))
#endif
                    {
                        if (selIndex >= 0 && selIndex < options.Length)
                        {
                            options[selIndex].onSubmit?.Invoke();
                        }
                    }
                    break;
                case NavigationType.ByKey:
#if GAME_COG
                    if (GameCog.Input.GetKeyDown(keySubmit))
#else
                    if (Input.GetKeyDown(keySubmit))
#endif
                    {
                        if (selIndex >= 0 && selIndex < options.Length)
                        {
                            options[selIndex].onSubmit?.Invoke();
                        }
                    }
                    break;
            }

        }

        #endregion

        #region Public Methods

        public void CloseMenu()
        {
            Owner.CloseMenu();
        }

        public void ConsumeItem()
        {
            Source.Inventory.ConsumeItem(Item.Item, 1);
        }

        public void DropItem()
        {
            Source.DropSelected();
        }

        public void DropItemCount(int count)
        {
            Source.Inventory.DropItem(Item.Item, count);
        }

        public void EquipItem()
        {
            Item.Equip();
        }

        public void Initialize()
        {
            // Triggers first
            foreach (TriggerOnMenuItemFilter trigger in GetComponentsInChildren<TriggerOnMenuItemFilter>())
            {
                trigger.PerformMatch(Item.Item);
            }

            // Update options
            options = GetComponentsInChildren<ItemMenuOptionUI>();
            for (int i = 0; i < options.Length; i++)
            {
                if (i == 0)
                {
                    options[i].onSelected?.Invoke();
                }
                else
                {
                    options[i].onUnSelected?.Invoke();
                }
                if (allowSelectByClick)
                {
                    options[i].onClick.AddListener(SelectItem);
                }
            }

        }

        public void RequestAttachmentUI()
        {
            Source?.ShowAttachmentsUI();
        }

        public void UnequipItem()
        {
            Item.Unequip();
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

        private void SelectItem(ItemMenuOptionUI optionUI)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] = optionUI)
                {
                    SelectedIndex = i;
                    return;
                }
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