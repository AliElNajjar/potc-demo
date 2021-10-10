using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CreateAssetMenu(menuName = "TOCK/Inventory/Item Tag", order = 4)]
    public class InventoryItemUITag : ScriptableObject
    {

        #region Variables

        public Sprite icon;
        public Color iconColor = Color.white;

        public string tagText = "Tag";
        public Color textColor = Color.white;

        #endregion

    }
}