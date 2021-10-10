using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("customization_point", false)]
    public class CustomizationPoint : MonoBehaviour
    {

        #region Variables

        public string pointId = "CustomizationPoint";
        public CustomizationPointLocation location;
        public bool isDefault;

        #endregion

    }
}