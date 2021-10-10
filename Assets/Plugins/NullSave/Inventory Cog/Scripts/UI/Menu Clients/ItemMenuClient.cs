using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class ItemMenuClient : MonoBehaviour
    {

        #region Variables

        public InventoryItemList itemSource;
        public Transform menuContainer;
        public bool useCallingPosition = true;

        public List<ItemMenuSelector> menus;

        private ItemMenuUI activeMenu;

        #endregion

        #region Unity Methods

        private void OnDisable()
        {
            if (itemSource != null)
            {
                itemSource.onItemSubmit.RemoveListener(ItemSelected);
            }
        }

        private void OnEnable()
        {
            if (itemSource != null)
            {
                itemSource.onItemSubmit.AddListener(ItemSelected);
            }
        }

        #endregion

        #region Public Methods

        public void CloseMenu()
        {
            if (activeMenu == null) return;
            activeMenu.Source.LockInput = false;
            Destroy(activeMenu.gameObject);
            activeMenu = null;
        }

        #endregion

        #region Private Methods

        private void ItemSelected(InventoryItemList list, ItemUI item, List<Category> categories)
        {
            if (item == null || item.Item == null) return;

            foreach (ItemMenuSelector menu in menus)
            {
                foreach (Category category in categories)
                {
                    if (category.name == menu.category.name)
                    {
                        ShowItemMenu(menu.menu, list, item, categories);
                        return;
                    }
                }
            }
        }

        private void ShowItemMenu(ItemMenuUI menuUI, InventoryItemList list, ItemUI item, List<Category> categories)
        {
            list.LockInput = true;
            activeMenu = Instantiate(menuUI, menuContainer);
            activeMenu.Source = list;
            activeMenu.Item = item;
            activeMenu.Categories = categories;
            activeMenu.Owner = this;
            activeMenu.Initialize();
            if (useCallingPosition)
            {
                activeMenu.transform.position = item.transform.position;
            }
        }

        #endregion

    }
}