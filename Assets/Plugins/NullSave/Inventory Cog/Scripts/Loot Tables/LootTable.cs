using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    public class LootTable : MonoBehaviour
    {

        #region Variables

        public List<LootDrop> dropList;

        #endregion

        #region Public Methods

        public void AddRandomLootToInventory(InventoryCog target)
        {
            List<LootItem> loot = GetRandomLoot();
            foreach(LootItem item in loot)
            {
                target.AddToInventory(item);
            }
        }

        public void DropRandomLoot()
        {
            List<LootItem> loot = GetRandomLoot();
            foreach (LootItem item in loot)
            {
                GameObject goItem = Instantiate(item).gameObject;
                goItem.transform.position = transform.position;
            }
        }

        public List<LootItem> GetRandomLoot()
        {
            // Sort items by weight
            dropList.Sort((p1, p2) => p1.weight.CompareTo(p2.weight));

            // Get total weight
            float totalWeight = 0;
            foreach(LootDrop dropItem in dropList)
            {
                totalWeight += dropItem.weight;
            }

            // Get roll
            float roll = Random.Range(0, totalWeight);

            // Get reward
            foreach (LootDrop dropItem in dropList)
            {
                if(roll <= dropItem.weight)
                {
                    return dropItem.items;
                }
                else
                {
                    roll -= dropItem.weight;
                }
            }

            return null;
        }

        #endregion

    }
}