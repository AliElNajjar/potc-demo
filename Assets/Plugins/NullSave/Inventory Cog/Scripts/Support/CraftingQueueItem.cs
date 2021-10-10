using System;
using System.Collections.Generic;
using System.IO;

namespace NullSave.TOCK.Inventory
{
    [Serializable]
    public class CraftingQueueItem
    {

        #region Variables

        public CraftingRecipe recipe;
        public DateTime timeStarted;
        public DateTime realWorldEnd;
        public float secondsRemaining;
        public int count;
        public List<ItemReference> usedComponents;

        #endregion

        #region Public Methods

        public void LoadState(Stream stream)
        {
            string recipeName = stream.ReadStringPacket();
            foreach (CraftingRecipe cr in InventoryDB.Recipes)
            {
                if (cr.name == recipeName)
                {
                    recipe = cr;
                    break;
                }
            }

            timeStarted = new DateTime(stream.ReadLong());
            realWorldEnd = new DateTime(stream.ReadLong());
            secondsRemaining = stream.ReadFloat();
        }

        public void SaveState(Stream stream)
        {
            stream.WriteStringPacket(recipe.name);
            stream.WriteLong(timeStarted.Ticks);
            stream.WriteLong(realWorldEnd.Ticks);
            stream.WriteFloat(secondsRemaining);
        }

        #endregion

    }
}
