using System;
using System.Collections.Generic;

namespace NullSave.TOCK.Inventory
{
    [Serializable]
    public class CraftingResult
    {

        #region Variables

        public InventoryItem item;
        public int count;
        public ResultType conditionResult;
        public ResultType rarityResult;
        public ResultType valueResult;

        #endregion

        #region Constructor

        public CraftingResult()
        {
            count = 1;
        }

        #endregion

        #region Public Methods

        public void SetResults(List<ItemReference> items, InventoryItem resultItem)
        {
            float condition;
            switch (conditionResult)
            {
                case ResultType.AverageOfComponents:
                    condition = 0;
                    foreach (ItemReference itemReference in items)
                    {
                        condition += itemReference.item.condition;
                    }
                    resultItem.condition = condition / items.Count;
                    break;
                case ResultType.HighestOfComponents:
                    condition = float.MinValue;
                    foreach (ItemReference itemReference in items)
                    {
                        if (itemReference.item.condition > condition)
                        {
                            condition = itemReference.item.condition;
                        }
                    }
                    resultItem.condition = condition;
                    break;
                case ResultType.LowestOfComponents:
                    condition = float.MaxValue;
                    foreach (ItemReference itemReference in items)
                    {
                        if (itemReference.item.condition < condition)
                        {
                            condition = itemReference.item.condition;
                        }
                    }
                    resultItem.condition = condition; break;
            }

            int rarity;
            switch (rarityResult)
            {
                case ResultType.AverageOfComponents:
                    rarity = 0;
                    foreach (ItemReference itemReference in items)
                    {
                        rarity += itemReference.item.rarity;
                    }
                    resultItem.rarity = rarity / items.Count;
                    break;
                case ResultType.HighestOfComponents:
                    rarity = int.MinValue;
                    foreach (ItemReference itemReference in items)
                    {
                        if (itemReference.item.rarity > rarity)
                        {
                            rarity = itemReference.item.rarity;
                        }
                    }
                    resultItem.rarity = rarity;
                    break;
                case ResultType.LowestOfComponents:
                    rarity = int.MaxValue;
                    foreach (ItemReference itemReference in items)
                    {
                        if (itemReference.item.rarity < rarity)
                        {
                            rarity = itemReference.item.rarity;
                        }
                    }
                    resultItem.rarity = rarity;
                    break;
            }

            float value;
            switch (valueResult)
            {
                case ResultType.AverageOfComponents:
                    value = 0;
                    foreach (ItemReference itemReference in items)
                    {
                        value += itemReference.item.value;
                    }
                    resultItem.value = value / items.Count;
                    break;
                case ResultType.HighestOfComponents:
                    value = float.MinValue;
                    foreach (ItemReference itemReference in items)
                    {
                        if (itemReference.item.value > value)
                        {
                            value = itemReference.item.value;
                        }
                    }
                    resultItem.value = value;
                    break;
                case ResultType.LowestOfComponents:
                    value = float.MaxValue;
                    foreach (ItemReference itemReference in items)
                    {
                        if (itemReference.item.value < value)
                        {
                            value = itemReference.item.value;
                        }
                    }
                    resultItem.value = value; break;
            }
        }

        #endregion

    }
}