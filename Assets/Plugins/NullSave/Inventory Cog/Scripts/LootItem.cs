using UnityEngine;
using UnityEngine.Events;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("loot_item", false)]
    public class LootItem : MonoBehaviour
    {

        #region Variables

        public InventoryItem item;
        public bool autoPickup, autoEquip;
        public string autoConsumeWhen = "1 > 2";
        public int count = 1;
        public float currency = 0;

        public GenerationType rarityGen;
        [Range(0, 10)] public int rarity1;
        [Range(0, 10)] public int rarity2;

        public GenerationType conditionGen;
        [Range(0, 0)] public float condition1;
        [Range(0, 1)] public int condition2;

        public GenerationType valueGen;
        public float value1;
        public float value2;

        public bool mulByRarity;
        public bool mulByCondition;

        public UnityEvent onLoot, onPlayerEnter, onPlayerExit;

        #endregion

        #region Properties

        public InventoryCog PlayerInventory { get; private set; }

        #endregion

        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            InventoryCog inventoryCog = other.GetComponentInChildren<InventoryCog>();
            if (inventoryCog != null)
            {
                PlayerInventory = inventoryCog;
                onPlayerEnter?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            InventoryCog inventoryCog = other.GetComponentInChildren<InventoryCog>();
            if (inventoryCog != null)
            {
                PlayerInventory = null;
                onPlayerExit.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            InventoryCog inventoryCog = other.GetComponentInChildren<InventoryCog>();
            if (inventoryCog != null)
            {
                PlayerInventory = inventoryCog;
                onPlayerEnter?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            InventoryCog inventoryCog = other.GetComponentInChildren<InventoryCog>();
            if (inventoryCog != null)
            {
                PlayerInventory = null;
                onPlayerExit.Invoke();
            }
        }

        #endregion

        #region Public Methods

        public virtual void AddToInventory()
        {
            if (PlayerInventory == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                if (go != null)
                {
                    PlayerInventory = go.GetComponent<InventoryCog>();
                }
            }

            if (PlayerInventory == null)
            {
                Debug.LogError(name + ".LootItem.AddToInventory no Player present in trigger");
            }
            else
            {
                count = PlayerInventory.AddToInventory(this, autoEquip);
                if (count == 0)
                {
                    Destroy(gameObject);
                }
            }
        }

        public InventoryItem GenerateValues()
        {
            InventoryItem result = ScriptableObject.Instantiate(item);
            result.name = item.name;
            result.CurrentCount = count;
            switch (rarityGen)
            {
                case GenerationType.Constant:
                    item.rarity = rarity1;
                    break;
                case GenerationType.RandomBetweenConstants:
                    item.rarity = Random.Range(rarity1, rarity2);
                    break;
            }
            switch (conditionGen)
            {
                case GenerationType.Constant:
                    item.condition = condition1;
                    break;
                case GenerationType.RandomBetweenConstants:
                    item.condition = Random.Range(condition1, condition2);
                    break;
            }
            switch (valueGen)
            {
                case GenerationType.Constant:
                    item.value = value1;
                    break;
                case GenerationType.RandomBetweenConstants:
                    item.value = Random.Range(value1, value2);
                    break;
            }
            if (mulByCondition) item.value *= item.condition;
            if (mulByRarity) item.value *= item.rarity;

            return item;
        }

        #endregion

    }
}