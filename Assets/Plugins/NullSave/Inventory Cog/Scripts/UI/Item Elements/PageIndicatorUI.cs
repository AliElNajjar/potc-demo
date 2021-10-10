using UnityEngine;
using UnityEngine.UI;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("category_page", false)]
    [RequireComponent(typeof(Image))]
    public class PageIndicatorUI : MonoBehaviour
    {

        #region Variables

        public Color activeColor = Color.white;
        public Color inactiveColor = new Color(1, 1, 1, 0.4f);
        public Color disabledColor = new Color(1, 1, 1, 0);

        private Image image;
        private bool isActive = true;
        private bool isInteractable = true;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        #endregion

        #region Public Methods

        public void SetActive(bool active)
        {
            isActive = active;
            UpdateUI();
        }

        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
            UpdateUI();
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            if (isInteractable)
            {
                image.color = isActive ? activeColor : inactiveColor;
            }
            else
            {
                image.color = disabledColor;
            }
        }

        #endregion

    }
}