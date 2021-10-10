using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [CreateAssetMenu(menuName = "TOCK/Inventory/Crafting Category", order = 0)]
    public class CraftingCategory : ScriptableObject
    {

        #region Variables

        public Sprite icon;
        public string displayName;
        [TextArea(2, 5)] public string description;
        public bool displayInList = true;

        #endregion

    }
}