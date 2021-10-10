using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class InventoryComponentList : MonoBehaviour
    {

        #region Variables

        public ComponentListType listType;
        public InventoryItemList itemSource;
        public ComponentUI componentPrefab;
        public Transform componentContainer;

        private List<ComponentUI> loadedComponents;

        #endregion

        #region Unity Methods

        private void OnDisable()
        {
            itemSource.onSelectionChanged.RemoveListener(UpdateComponents);
        }

        private void OnEnable()
        {
            itemSource.onSelectionChanged.AddListener(UpdateComponents);
            UpdateComponents(0);
        }

        #endregion

        #region Private Methods

        private void ClearComponents()
        {
            if (loadedComponents == null) return;
            foreach (ComponentUI component in loadedComponents)
            {
                Destroy(component.gameObject);
            }
            loadedComponents.Clear();
        }

        private void UpdateComponents(int index)
        {
            ClearComponents();
            if (itemSource.SelectedItem == null) return;
            InventoryItem item = itemSource.SelectedItem.Item;
            loadedComponents = new List<ComponentUI>();

            switch (listType)
            {
                case ComponentListType.Breakdown:
                    if (item.CanBreakdown)
                    {
                        foreach (ItemReference component in item.breakdownResult)
                        {
                            ComponentUI ui = Instantiate(componentPrefab, componentContainer);
                            ui.LoadComponent(component, itemSource.Inventory, false, 0, 0);
                            loadedComponents.Add(ui);
                        }
                    }
                    break;
                case ComponentListType.Repair:
                    if (item.CanBreakdown)
                    {
                        foreach (ItemReference component in item.incrementComponents)
                        {
                            ComponentUI ui = Instantiate(componentPrefab, componentContainer);
                            ui.LoadComponent(component, itemSource.Inventory, true, 0, 0);
                            loadedComponents.Add(ui);
                        }
                    }
                    break;
            }
        }

        #endregion

    }
}