using System.Collections;
using TMPro;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [DefaultExecutionOrder(-10)]
    public class SortUI : MonoBehaviour
    {

        #region Variables

        public InventoryItemList targetList;
        public bool byName = true, byRarity = true, byValue = true, byWeight = true, byCondition = true, byType = true, byCount = true;
        public bool sortAsc;

        public bool persistSort = true;
        public InventorySortOrder defaultOrder = InventorySortOrder.DisplayNameDesc;

        public NavigationType triggerMode;
        public string button = "Fire1";
        public KeyCode key = KeyCode.Alpha0;

        public TextMeshProUGUI sortText;
        public string sortPrefix;
        public string sortSuffix;

        private int curSort = -1;
        private string localGuid = System.Guid.NewGuid().ToString();

        #endregion

        #region Unity Methods

        public void OnEnable()
        {
            if(persistSort)
            {
                curSort = PlayerPrefs.GetInt("inventory_sort_" + localGuid, (int)defaultOrder);
                Sort((InventorySortOrder)curSort);
            }
            else
            {
                curSort = (int)defaultOrder;
            }
        }

        private void Update()
        {
#if GAME_COG
            if ((triggerMode == NavigationType.ByButton && GameCog.Input.GetButtonDown(button)) ||
                (triggerMode == NavigationType.ByKey && GameCog.Input.GetKeyDown(key)))
#else
            if ((triggerMode == NavigationType.ByButton && Input.GetButtonDown(button)) ||
                (triggerMode == NavigationType.ByKey && Input.GetKeyDown(key)))
#endif
            {
                NextSort();
            }
        }

        #endregion

        #region Properties

        public InventoryCog Inventory { get; set; }

        #endregion

        #region Public Methods

        public void Sort(InventorySortOrder sortOrder, bool refresh = false)
        {
            if (persistSort)
            {
                PlayerPrefs.SetInt("inventory_sort", (int)sortOrder);
            }

            curSort = (int)sortOrder;

            if (targetList.Inventory == null)
            {
                if(Inventory == null)
                {
                    StartCoroutine("WaitForInventory");
                    return;
                }
                targetList.Inventory = Inventory;
            }

            targetList.Inventory.Sort(sortOrder);
            if (refresh)
            {
                targetList.ReloadLast();
            }

            if(sortText != null)
            {
                sortText.text = sortPrefix + GetSortText(GetNextSort()) + sortSuffix;
            }
        }

        #endregion

        #region Private Methods

        private string GetSortText(int sortType)
        {
            switch (sortType)
            {
                case 1:
                case 2:
                    return "Name";
                case 3:
                case 4:
                    return "Rarity";
                case 5:
                case 6:
                    return "Value";
                case 7:
                case 8:
                    return "Weight";
                case 9:
                case 10:
                    return "Condition";
                case 11:
                case 12:
                    return "Type";
                case 13:
                case 14:
                    return "Count";
            }

            return "Unknown";
        }

        private int GetNextSort()
        {
            int sort = curSort;

            while (true)
            {
                sort += 2;
                if (sort > 14) sort -= 14;

                switch (sort)
                {
                    case 1:
                    case 2:
                        return sort;
                    case 3:
                    case 4:
                        return sort;
                    case 5:
                    case 6:
                        return sort;
                    case 7:
                    case 8:
                        return sort;
                    case 9:
                    case 10:
                        return sort;
                    case 11:
                    case 12:
                        return sort;
                    case 13:
                    case 14:
                        return sort;
                }
            }
        }

        private void NextSort()
        {
            Sort((InventorySortOrder)GetNextSort(), true);
        }

        private IEnumerator WaitForInventory()
        {
            while (Inventory == null && targetList.Inventory == null) yield return new WaitForEndOfFrame();
            Sort((InventorySortOrder)curSort, true);
        }

        #endregion

    }
}