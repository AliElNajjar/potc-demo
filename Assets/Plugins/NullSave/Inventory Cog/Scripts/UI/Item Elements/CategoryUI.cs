using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("category", false)]
    public class CategoryUI : MonoBehaviour
    {

        #region Variables

        public TextMeshProUGUI categoryName;
        public Image categoryImage;
        public PageIndicatorUI pageIndicator;
        public Transform indicatorParent;
        public PageIndicatorMode indicatorMode;
        public bool includeLocked = true;
        public Color activeColor = Color.white;
        public Color inactiveColor = new Color(1, 1, 1, 0.4f);
        public Color activeTextColor = Color.white;
        public Color inactiveTextColor = new Color(1, 1, 1, 0.4f);
        public int slotsPerPage = 40;
        public bool onlyMoreThanOne = true;

        public UnityEvent onClick;

        private List<PageIndicatorUI> indicators;
        private int curPage;

        #endregion

        #region Properties

        public Category Category { get; set; }

        public int CurrentPage
        {
            get { return curPage; }
            set
            {
                if (value == curPage || value < 0 || value > PageCount - 1) return;
                indicators[curPage].SetActive(false);
                curPage = value;
                indicators[curPage].SetActive(true);
            }
        }

        public int PageCount { get; private set; }

        #endregion

        #region Public Methods

        public void Click()
        {
            onClick?.Invoke();
        }

        public void LoadCategory(Category category)
        {
            Category = category;

            if (categoryName != null) categoryName.text = category.displayName;
            if (categoryImage != null) categoryImage.sprite = category.icon;

            indicators = new List<PageIndicatorUI>();
            if (pageIndicator != null)
            {
                switch (indicatorMode)
                {
                    case PageIndicatorMode.All:
                        PageCount = category.GetUsedPagesUI(slotsPerPage, includeLocked);
                        if (PageCount > 1 || !onlyMoreThanOne)
                        {
                            for (int i = 0; i < PageCount; i++)
                            {
                                indicators.Add(Instantiate(pageIndicator, indicatorParent));
                                indicators[i].SetActive(i == CurrentPage);
                            }
                        }
                        break;
                    case PageIndicatorMode.AllUnlocked:
                        PageCount = category.GetUnlockedPages(slotsPerPage);
                        if (PageCount > 1 || !onlyMoreThanOne)
                        {
                            for (int i = 0; i < PageCount; i++)
                            {
                                indicators.Add(Instantiate(pageIndicator, indicatorParent));
                                indicators[i].SetActive(i == CurrentPage);
                            }
                        }
                        break;
                    case PageIndicatorMode.OnlyUsed:
                        PageCount = category.GetUsedPagesUI(slotsPerPage, includeLocked);
                        if (PageCount > 1 || !onlyMoreThanOne)
                        {
                            for (int i = 0; i < PageCount; i++)
                            {
                                indicators.Add(Instantiate(pageIndicator, indicatorParent));
                                indicators[i].SetActive(i == CurrentPage);
                            }
                        }
                        break;
                }
            }
        }

        public void SetActive(bool active)
        {
            if (categoryImage != null)
            {
                categoryImage.color = active ? activeColor : inactiveColor;
            }

            if (categoryName != null)
            {
                categoryName.color = active ? activeTextColor : inactiveTextColor;
            }

            foreach (PageIndicatorUI indicator in indicators)
            {
                indicator.SetInteractable(active);
            }
        }

        #endregion

    }
}